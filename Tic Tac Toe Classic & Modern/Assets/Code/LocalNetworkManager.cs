using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using UnityEngine;

public enum NetworkRole
{
    None,
    Host,
    Client
}

public class LocalNetworkManager : MonoBehaviour
{
    public static LocalNetworkManager Instance { get; private set; }

    [Header("Port Settings")]
    [SerializeField] private int port = 7777;

    // Network Objects
    private TcpListener serverListener;
    private TcpClient socketConnection;
    private NetworkStream stream;
    private Thread networkThread;

    // State Properties
    public NetworkRole CurrentRole { get; private set; } = NetworkRole.None;
    public string MySymbol { get; private set; } = "X"; // Host = X, Client = O
    public bool IsConnected { get; private set; } = false;

    // Thread-safe Message Queue
    private readonly Queue<Action> mainThreadActions = new Queue<Action>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        // Execute network actions on the main Unity thread
        lock (mainThreadActions)
        {
            while (mainThreadActions.Count > 0)
            {
                mainThreadActions.Dequeue()?.Invoke();
            }
        }
    }

    // --- 1. Get Local IP Address ---
    public string GetLocalIPAddress()
    {
        try
        {
            foreach (NetworkInterface item in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (item.OperationalStatus == OperationalStatus.Up)
                {
                    foreach (UnicastIPAddressInformation ip in item.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip.Address))
                        {
                            return ip.Address.ToString();
                        }
                    }
                }
            }

            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
                {
                    return ip.ToString();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("[Network] Error getting IP Address: " + ex.Message);
        }

        return "127.0.0.1";
    }

    // --- 2. Start Host ---
    public void StartHost()
    {
        Disconnect();
        CurrentRole = NetworkRole.Host;
        MySymbol = "X";

        networkThread = new Thread(HostListenLoop)
        {
            IsBackground = true
        };
        networkThread.Start();
    }

    private void HostListenLoop()
    {
        try
        {
            serverListener = new TcpListener(IPAddress.Any, port);
            serverListener.Start();

            socketConnection = serverListener.AcceptTcpClient();
            stream = socketConnection.GetStream();
            IsConnected = true;

            EnqueueMainThreadAction(() =>
            {
                Debug.Log("[Network] Client Connected!");
                SendMessageToNetwork("INIT|O");
                OnConnectionEstablished();
            });

            ReadNetworkStream();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[Network] Host stopped: " + ex.Message);
        }
    }

    // --- 3. Connect to Host (Client) ---
    public void ConnectToHost(string hostIp)
    {
        Disconnect();
        CurrentRole = NetworkRole.Client;
        MySymbol = "O";

        networkThread = new Thread(() => ClientConnectLoop(hostIp))
        {
            IsBackground = true
        };
        networkThread.Start();
    }

    private void ClientConnectLoop(string hostIp)
    {
        try
        {
            socketConnection = new TcpClient();
            socketConnection.Connect(IPAddress.Parse(hostIp), port);
            stream = socketConnection.GetStream();
            IsConnected = true;

            EnqueueMainThreadAction(() =>
            {
                Debug.Log("[Network] Connected to Host!");
                OnConnectionEstablished();
            });

            ReadNetworkStream();
        }
        catch (Exception ex)
        {
            EnqueueMainThreadAction(() =>
            {
                Debug.LogError("[Network] Connection Failed: " + ex.Message);
                Disconnect();
            });
        }
    }

    // --- 4. Read Network Stream ---
    private void ReadNetworkStream()
    {
        byte[] buffer = new byte[1024];
        while (IsConnected && socketConnection != null && socketConnection.Connected)
        {
            try
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string rawMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                string[] messages = rawMessage.Split('\n');

                foreach (var msg in messages)
                {
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        string trimmed = msg.Trim();
                        EnqueueMainThreadAction(() => ProcessIncomingMessage(trimmed));
                    }
                }
            }
            catch
            {
                break;
            }
        }

        EnqueueMainThreadAction(OnDisconnected);
    }

    // --- 5. Send Message ---
    public void SendMessageToNetwork(string message)
    {
        if (!IsConnected || stream == null) return;

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Debug.LogError("[Network] Send Error: " + ex.Message);
        }
    }

    // --- 6. Process Incoming Messages ---
    private void ProcessIncomingMessage(string message)
    {
        string[] parts = message.Split('|');
        string command = parts[0];

        switch (command)
        {
            case "INIT":
                if (parts.Length > 1) MySymbol = parts[1];
                break;

            case "MOVE":
                if (parts.Length > 1 && int.TryParse(parts[1], out int cellIndex))
                {
                    GameManager.Instance?.ReceiveNetworkMove(cellIndex);
                }
                break;

            case "RESET":
                GameManager.Instance?.ReceiveNetworkReset();
                break;

            case "DISCONNECT":
                Disconnect();
                break;
        }
    }

    private void OnConnectionEstablished()
    {
        GameData.SelectedOpponent = OpponentType.LANNetwork;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNetworkGame = true;
        }

        // Automatically start the game UI via MultiplayerMenuManager using modern API
        var multiplayerManager = FindFirstObjectByType<MultiplayerMenuManager>();
        if (multiplayerManager != null)
        {
            multiplayerManager.StartNetworkGame();
        }
    }

    private void OnDisconnected()
    {
        IsConnected = false;
        CurrentRole = NetworkRole.None;
        Debug.Log("[Network] Disconnected from peer.");
    }

    public void Disconnect()
    {
        IsConnected = false;
        SendMessageToNetwork("DISCONNECT");

        try
        {
            stream?.Close();
            socketConnection?.Close();
            serverListener?.Stop();
            networkThread?.Abort();
        }
        catch { }

        CurrentRole = NetworkRole.None;
    }

    private void EnqueueMainThreadAction(Action action)
    {
        lock (mainThreadActions)
        {
            mainThreadActions.Enqueue(action);
        }
    }

    private void OnApplicationQuit()
    {
        Disconnect();
    }
}