using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SingleSetupManager : MonoBehaviour
{
    [Header("Setup Mode Configuration")]
    [SerializeField] private GameMode panelGameMode; // Set this to Classic or Modern in the Inspector

    [Header("Panels Reference")]
    [SerializeField] private GameObject currentSetupPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameplayPanel;

    [Header("Setup UI Elements")]
    [SerializeField] private TMP_Dropdown difficultyDropdown;
    [SerializeField] private TMP_Dropdown symbolDropdown;
    [SerializeField] private TMP_Dropdown roundsDropdown;

    // Triggered when clicking the Start Game button
    public void OnStartGameClicked()
    {
        // Set the game mode based on this panel's inspector configuration
        GameData.SelectedMode = panelGameMode;
        GameData.SelectedOpponent = OpponentType.VsAI; // Singleplayer AI mode

        // Assign AI difficulty
        if (difficultyDropdown != null)
        {
            GameData.SelectedDifficulty = (AIDifficulty)difficultyDropdown.value;
        }

        // Assign player symbol (X or O)
        if (symbolDropdown != null)
        {
            GameData.SelectedSymbol = (PlayerSymbol)symbolDropdown.value;
        }

        // Assign total rounds (0: 1 Round, 1: 3 Rounds, 2: 5 Rounds)
        if (roundsDropdown != null)
        {
            switch (roundsDropdown.value)
            {
                case 0: GameData.TotalRounds = 1; break;
                case 1: GameData.TotalRounds = 3; break;
                case 2: GameData.TotalRounds = 5; break;
            }
        }

        // Ensure network mode is disabled for singleplayer AI games
        if (GameManager.Instance != null)
        {
            GameManager.Instance.isNetworkGame = false;
        }

        // Transition to gameplay panel
        if (currentSetupPanel != null) currentSetupPanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(true);
    }

    // Triggered when clicking the Back button
    public void OnBackClicked()
    {
        if (currentSetupPanel != null) currentSetupPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}