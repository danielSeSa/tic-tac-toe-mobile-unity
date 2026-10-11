using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gameSetupPanel;
    [SerializeField] private GameObject settingsPanel; 

    public void OnPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        if (gameSetupPanel != null)
        {
            gameSetupPanel.SetActive(true);
        }
    }

    public void OnSettingsButtonClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
        }
    }

    public void OnCloseSettingsClicked()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    public void OnQuitButtonClicked()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}