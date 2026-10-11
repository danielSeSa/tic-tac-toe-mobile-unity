using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Music Switcher UI")]
    [SerializeField] private TMP_Text trackNameText;
    [SerializeField] private Button prevTrackButton;
    [SerializeField] private Button nextTrackButton;
    [SerializeField] private Button repeatButton;
    [SerializeField] private TMP_Text repeatButtonText;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider sfxVolumeSlider;

    private void OnEnable()
    {
        InitializeUIValues();
    }

    private void InitializeUIValues()
    {
        if (AudioManager.Instance == null) return;

        if (musicVolumeSlider) musicVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.MusicVolume);
        if (sfxVolumeSlider) sfxVolumeSlider.SetValueWithoutNotify(AudioManager.Instance.SFXVolume);

        UpdateMusicUI();
    }

    public void OnPrevTrackClicked()
    {
        AudioManager.Instance.PreviousTrack();
        UpdateMusicUI();
    }

    public void OnNextTrackClicked()
    {
        AudioManager.Instance.NextTrack();
        UpdateMusicUI();
    }

    public void OnRepeatToggleClicked()
    {
        AudioManager.Instance.ToggleRepeatTrack();
        UpdateMusicUI();
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetMusicVolume(value);
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.SetSFXVolume(value);
    }

    private void UpdateMusicUI()
    {
        if (AudioManager.Instance == null) return;

        if (trackNameText)
        {
            trackNameText.text = AudioManager.Instance.GetCurrentTrackName();
        }

        if (repeatButtonText)
        {
            repeatButtonText.text = AudioManager.Instance.IsRepeatTrack ? "Repeat: ON" : "Repeat: OFF";
        }
    }
}