using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MultiplayerMenuManager : MonoBehaviour
{
    [Header("Panels Reference")]
    [SerializeField] private GameObject multiplayerPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject lanSetupPanel; // Optional if LAN is on a separate sub-panel, or managed inside this panel

    [Header("Multiplayer UI Elements")]
    [SerializeField] private TMP_Dropdown opponentDropdown;
    [SerializeField] private Button startLocalGameButton;

    [Header("LAN Network UI Elements")]
    [SerializeField] private GameObject lanOptionsGroup; // Parent object containing LAN fields, or control them individually
    [SerializeField] private TMP_Text localIpText;
    [SerializeField] private TMP_InputField hostIpInputField;
    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinButton;

    private void OnEnable()
    {
        // Initialize UI states when the panel is opened
        RefreshMultiplayerUI();
    }

    // Called when the opponent dropdown value changes
    public void OnOpponentDropdownChanged(int index)
    {
        // index 0: Local 2 Players, index 1: LAN Network (Two Phones)
        bool isLAN = (index == 1);

        // Control UI interactability and visibility based on selection
        if (startLocalGameButton != null)
        {
            startLocalGameButton.gameObject.SetActive(!isLAN);
        }

        if (lanOptionsGroup != null)
        {
            lanOptionsGroup.SetActive(isLAN);
        }
        else
        {
            // If no group object exists, control elements individually and gray them out
            if (hostIpInputField != null) hostIpInputField.interactable = isLAN;
            if (hostButton != null) hostButton.interactable = isLAN;
            if (joinButton != null) joinButton.interactable = isLAN;
        }

        // If LAN is selected, fetch and display the local device IP address
        if (isLAN && localIpText != null && LocalNetworkManager.Instance != null)
        {
            localIpText.text = "Your IP: " + LocalNetworkManager.Instance.GetLocalIPAddress();
        }
    }

    private void RefreshMultiplayerUI()
    {
        if (opponentDropdown != null)
        {
            OnOpponentDropdownChanged(opponentDropdown.value);
        }
    }

    // Triggered when starting a local 2-player game on the same device
    public void OnStartLocalGameClicked()
    {
        GameData.SelectedOpponent = OpponentType.LocalTwoPlayers;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNetworkGame = false;
        }

        if (multiplayerPanel != null) multiplayerPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }

    // Triggered when the host creates a room
    public void OnHostButtonClicked()
    {
        GameData.SelectedOpponent = OpponentType.LANNetwork;

        if (LocalNetworkManager.Instance != null)
        {
            LocalNetworkManager.Instance.StartHost();
        }
    }

    // Triggered when the client joins a room using the host's IP
    public void OnJoinButtonClicked()
    {
        GameData.SelectedOpponent = OpponentType.LANNetwork;

        if (LocalNetworkManager.Instance != null && hostIpInputField != null)
        {
            string targetIp = hostIpInputField.text.Trim();
            if (!string.IsNullOrEmpty(targetIp))
            {
                LocalNetworkManager.Instance.ConnectToHost(targetIp);
            }
        }
    }

    // Called automatically by LocalNetworkManager when connection is successful
    public void StartNetworkGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNetworkGame = true;
        }

        if (multiplayerPanel != null) multiplayerPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }

    // Return to the main menu
    public void OnBackClicked()
    {
        if (multiplayerPanel != null) multiplayerPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}