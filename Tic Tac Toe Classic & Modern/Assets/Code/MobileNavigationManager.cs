using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class MobileNavigationManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameSetupPanel;
    [SerializeField] private GameObject gameplayPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject quitConfirmationPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Game Manager Reference")]
    [SerializeField] private GameManager gameManager;

    private void Update()
    {
        bool backPressed = false;

#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            backPressed = true;
        }
#else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            backPressed = true;
        }
#endif

        if (backPressed)
        {
            HandleBackButtonPressed();
        }
    }

    public void HandleBackButtonPressed()
    {
        // 1. If quit confirmation is open -> close it
        if (quitConfirmationPanel != null && quitConfirmationPanel.activeSelf)
        {
            CloseQuitConfirmation();
        }
        // 2. If settings panel is open -> close it
        else if (settingsPanel != null && settingsPanel.activeSelf)
        {
            settingsPanel.SetActive(false);
        }
        // 3. If pause panel is open -> resume game
        else if (pausePanel != null && pausePanel.activeSelf)
        {
            ResumeGame();
        }
        // 4. If we are in gameplay -> open pause menu
        else if (gameplayPanel != null && gameplayPanel.activeSelf)
        {
            PauseGame();
        }
        // 5. If we are in setup or game over -> return to main menu
        else if ((gameSetupPanel != null && gameSetupPanel.activeSelf) || (gameOverPanel != null && gameOverPanel.activeSelf))
        {
            OpenMainMenu();
        }
        // 6. If we are in main menu -> open quit confirmation
        else if (mainMenuPanel != null && mainMenuPanel.activeSelf)
        {
            OpenQuitConfirmation();
        }
    }

    // --- Pause Menu Functions ---
    public void PauseGame()
    {
        if (pausePanel != null) pausePanel.SetActive(true);
        Time.timeScale = 0f; // Stop game time
    }

    public void ResumeGame()
    {
        if (pausePanel != null) pausePanel.SetActive(false);
        Time.timeScale = 1f; // Resume game time
    }

    // Opens Settings Panel from Pause Menu
    public void OnPauseSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnPauseMainMenuClicked()
    {
        Time.timeScale = 1f;
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameplayPanel != null) gameplayPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    // --- Quit Functions ---
    public void OpenQuitConfirmation()
    {
        if (quitConfirmationPanel != null) quitConfirmationPanel.SetActive(true);
    }

    public void CloseQuitConfirmation()
    {
        if (quitConfirmationPanel != null) quitConfirmationPanel.SetActive(false);
    }

    public void ConfirmQuit()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }

    private void OpenMainMenu()
    {
        if (gameSetupPanel != null) gameSetupPanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }
}