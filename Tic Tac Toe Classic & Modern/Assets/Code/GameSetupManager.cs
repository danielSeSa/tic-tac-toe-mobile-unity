using UnityEngine;
using UnityEngine.UI;

public class GameSetupMenuManager : MonoBehaviour
{
    [Header("Main Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameSetupPanel;

    [Header("Sub-Panels Reference")]
    [SerializeField] private GameObject classicSetupPanel;
    [SerializeField] private GameObject modernSetupPanel;
    [SerializeField] private GameObject multiplayerMenuPanel;

    private void OnEnable()
    {
        // Hide all sub-panels initially when entering GameSetupPanel so the user only sees the 3 mode buttons
        if (classicSetupPanel != null) classicSetupPanel.SetActive(false);
        if (modernSetupPanel != null) classicSetupPanel.SetActive(false);
        if (multiplayerMenuPanel != null) multiplayerMenuPanel.SetActive(false);
    }

    // Opens Classic Setup Panel and updates GameData
    public void ShowClassicSetup()
    {
        if (classicSetupPanel != null) classicSetupPanel.SetActive(true);
        if (modernSetupPanel != null) modernSetupPanel.SetActive(false);
        if (multiplayerMenuPanel != null) multiplayerMenuPanel.SetActive(false);

        GameData.SelectedMode = GameMode.Classic;
    }

    // Opens Modern Setup Panel and updates GameData
    public void ShowModernSetup()
    {
        if (classicSetupPanel != null) classicSetupPanel.SetActive(false);
        if (modernSetupPanel != null) modernSetupPanel.SetActive(true);
        if (multiplayerMenuPanel != null) multiplayerMenuPanel.SetActive(false);

        GameData.SelectedMode = GameMode.Modern;
    }

    // Opens Multiplayer Menu Panel
    public void ShowMultiplayerMenu()
    {
        if (classicSetupPanel != null) classicSetupPanel.SetActive(false);
        if (modernSetupPanel != null) classicSetupPanel.SetActive(false);
        if (multiplayerMenuPanel != null) multiplayerMenuPanel.SetActive(true);
    }

    // Returns from GameSetupPanel back to MainMenuPanel
    public void OnBackToMainMenuClicked()
    {
        if (gameSetupPanel != null) gameSetupPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}