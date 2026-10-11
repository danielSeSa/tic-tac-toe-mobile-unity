using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI Panels")]
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Gameplay UI")]
    [SerializeField] private TMP_Text turnText;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private Cell[] cells;

    [Header("GameOver UI")]
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private TMP_Text finalScoreText;

    [Header("Network Settings")]
    public bool isNetworkGame = false;

    private string[] board = new string[9];
    private string currentTurn = "X";
    private string humanSymbol = "X";
    private string aiSymbol = "O";
    private bool isGameActive = true;
    private bool isAiProcessing = false;

    private int scoreX = 0;
    private int scoreO = 0;
    private int currentRound = 1;

    private Queue<int> xMoves = new Queue<int>();
    private Queue<int> oMoves = new Queue<int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        ResetFullMatch();
    }

    public void ResetFullMatch()
    {
        scoreX = 0;
        scoreO = 0;
        currentRound = 1;
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        StartRound();
    }

    private void StartRound()
    {
        board = new string[9];
        xMoves.Clear();
        oMoves.Clear();
        isGameActive = true;
        isAiProcessing = false;

        humanSymbol = (GameData.SelectedSymbol == PlayerSymbol.X) ? "X" : "O";
        aiSymbol = (humanSymbol == "X") ? "O" : "X";
        currentTurn = "X";

        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Setup(i, this);
            cells[i].ResetCell();
        }

        UpdateUI();

        if (!isNetworkGame && GameData.SelectedOpponent == OpponentType.VsAI && currentTurn == aiSymbol)
        {
            Invoke(nameof(TriggerAIMove), 0.5f);
        }
    }

    public void OnCellClicked(int index)
    {
        if (!isGameActive || isAiProcessing || !string.IsNullOrEmpty(board[index])) return;

        if (isNetworkGame && LocalNetworkManager.Instance != null && LocalNetworkManager.Instance.IsConnected)
        {
            string mySymbol = LocalNetworkManager.Instance.MySymbol;
            if (currentTurn != mySymbol)
            {
                return;
            }

            LocalNetworkManager.Instance.SendMessageToNetwork("MOVE|" + index);
        }

        MakeMove(index);

        if (!isNetworkGame && isGameActive && GameData.SelectedOpponent == OpponentType.VsAI && currentTurn == aiSymbol)
        {
            isAiProcessing = true;
            Invoke(nameof(TriggerAIMove), 0.5f);
        }
    }

    private void TriggerAIMove()
    {
        if (!isGameActive) return;

        int aiIndex = AIOpponent.GetBestMove(board, aiSymbol, humanSymbol, GameData.SelectedDifficulty);
        isAiProcessing = false;

        if (aiIndex != -1)
        {
            MakeMove(aiIndex);
        }
    }

    private void MakeMove(int index)
    {
        if (currentTurn == "X")
        {
            AudioManager.Instance?.PlayXMove();
        }
        else
        {
            AudioManager.Instance?.PlayOMove();
        }

        if (GameData.SelectedMode == GameMode.Modern)
        {
            Queue<int> currentQueue = (currentTurn == "X") ? xMoves : oMoves;

            if (currentQueue.Count == 3)
            {
                int oldestIndex = currentQueue.Dequeue();
                board[oldestIndex] = "";
                cells[oldestIndex].ResetCell();

                AudioManager.Instance?.PlayDisappear();
            }

            board[index] = currentTurn;
            cells[index].SetSymbol(currentTurn);
            currentQueue.Enqueue(index);

            if (currentQueue.Count == 3)
            {
                int oldestIndex = currentQueue.Peek();
                cells[oldestIndex].SetFaded(true);
            }
        }
        else
        {
            board[index] = currentTurn;
            cells[index].SetSymbol(currentTurn);
        }

        if (CheckWin())
        {
            isGameActive = false;
            if (currentTurn == "X") scoreX++; else scoreO++;
            turnText.text = $"Player {currentTurn} Wins Round {currentRound}!";
            UpdateScoreUI();

            AudioManager.Instance?.PlayWinRound();

            Invoke(nameof(HandleRoundEnd), 1.5f);
            return;
        }

        if (GameData.SelectedMode == GameMode.Classic && CheckDraw())
        {
            isGameActive = false;
            turnText.text = "Round Draw!";

            AudioManager.Instance?.PlayDrawLoss();

            Invoke(nameof(HandleRoundEnd), 1.5f);
            return;
        }

        currentTurn = (currentTurn == "X") ? "O" : "X";
        UpdateUI();
    }

    private void HandleRoundEnd()
    {
        if (currentRound >= GameData.TotalRounds)
        {
            ShowGameOver();
        }
        else
        {
            currentRound++;
            StartRound();
        }
    }

    private void ShowGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);

            if (scoreX > scoreO)
            {
                winnerText.text = "PLAYER X WINS THE MATCH!";
                AudioManager.Instance?.PlayWinMatch();
            }
            else if (scoreO > scoreX)
            {
                winnerText.text = "PLAYER O WINS THE MATCH!";
                AudioManager.Instance?.PlayWinMatch();
            }
            else
            {
                winnerText.text = "MATCH DRAW!";
                AudioManager.Instance?.PlayDrawLoss();
            }

            finalScoreText.text = $"Final Score: X ({scoreX}) - O ({scoreO})";
        }
    }

    public void ReceiveNetworkMove(int index)
    {
        MakeMove(index);
    }

    public void ReceiveNetworkReset()
    {
        ResetFullMatch();
    }

    public void RequestNetworkReset()
    {
        if (isNetworkGame && LocalNetworkManager.Instance != null)
        {
            LocalNetworkManager.Instance.SendMessageToNetwork("RESET");
        }
        ResetFullMatch();
    }

    public void OnPlayAgainClicked()
    {
        if (isNetworkGame)
        {
            RequestNetworkReset();
        }
        else
        {
            ResetFullMatch();
        }
    }

    public void OnMainMenuClicked()
    {
        if (isNetworkGame && LocalNetworkManager.Instance != null)
        {
            LocalNetworkManager.Instance.Disconnect();
        }

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void OnRestartClicked()
    {
        if (isNetworkGame)
        {
            RequestNetworkReset();
        }
        else
        {
            ResetFullMatch();
        }
    }

    private bool CheckWin()
    {
        int[][] winConditions = new int[][]
        {
            new int[] {0, 1, 2}, new int[] {3, 4, 5}, new int[] {6, 7, 8},
            new int[] {0, 3, 6}, new int[] {1, 4, 7}, new int[] {2, 5, 8},
            new int[] {0, 4, 8}, new int[] {2, 4, 6}
        };

        foreach (var condition in winConditions)
        {
            if (!string.IsNullOrEmpty(board[condition[0]]) &&
                board[condition[0]] == board[condition[1]] &&
                board[condition[1]] == board[condition[2]])
            {
                return true;
            }
        }
        return false;
    }

    private bool CheckDraw()
    {
        foreach (var cell in board)
        {
            if (string.IsNullOrEmpty(cell)) return false;
        }
        return true;
    }

    private void UpdateUI()
    {
        turnText.text = $"Turn: Player {currentTurn}";
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        scoreText.text = $"Round {currentRound}/{GameData.TotalRounds}  |  X: {scoreX} - O: {scoreO}";
    }
}