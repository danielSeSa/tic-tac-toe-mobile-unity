using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public struct BgmTrack
{
    public string trackName;
    public AudioClip clip;
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("BGM Playlist")]
    [SerializeField] private List<BgmTrack> bgmTracks = new List<BgmTrack>();

    [Header("SFX Clips")]
    [SerializeField] private AudioClip defaultClickSFX;
    [SerializeField] private AudioClip xMoveSFX;
    [SerializeField] private AudioClip oMoveSFX;
    [SerializeField] private AudioClip disappearSFX;
    [SerializeField] private AudioClip winRoundSFX;
    [SerializeField] private AudioClip winMatchSFX;
    [SerializeField] private AudioClip drawLossSFX;

    private int currentTrackIndex = 0;
    private bool isRepeatTrack = false;

    private float musicVolume = 0.5f; 
    private float sfxVolume = 0.5f;  

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (bgmTracks != null && bgmTracks.Count > 0)
        {
            PlayBGM(currentTrackIndex);
        }
    }

    private void Update()
    {
        if (bgmSource != null && !bgmSource.isPlaying && bgmTracks.Count > 0)
        {
            if (isRepeatTrack)
            {
                bgmSource.Play();
            }
            else
            {
                NextTrack();
            }
        }
    }

    public void PlayBGM(int index)
    {
        if (bgmTracks == null || bgmTracks.Count == 0 || bgmSource == null) return;

        currentTrackIndex = Mathf.Clamp(index, 0, bgmTracks.Count - 1);
        bgmSource.clip = bgmTracks[currentTrackIndex].clip;
        bgmSource.loop = isRepeatTrack;

        bgmSource.volume = musicVolume;
        if (!bgmSource.isPlaying) bgmSource.Play();
    }

    public void NextTrack()
    {
        if (bgmTracks.Count == 0) return;
        currentTrackIndex = (currentTrackIndex + 1) % bgmTracks.Count;
        PlayBGM(currentTrackIndex);
    }

    public void PreviousTrack()
    {
        if (bgmTracks.Count == 0) return;
        currentTrackIndex--;
        if (currentTrackIndex < 0) currentTrackIndex = bgmTracks.Count - 1;
        PlayBGM(currentTrackIndex);
    }

    public void ToggleRepeatTrack()
    {
        isRepeatTrack = !isRepeatTrack;
        if (bgmSource != null)
        {
            bgmSource.loop = isRepeatTrack;
        }
        PlayerPrefs.SetInt("BGM_Repeat", isRepeatTrack ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume;
        }
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayDefaultClick() => PlaySFX(defaultClickSFX);
    public void PlayXMove() => PlaySFX(xMoveSFX);
    public void PlayOMove() => PlaySFX(oMoveSFX);
    public void PlayDisappear() => PlaySFX(disappearSFX);
    public void PlayWinRound() => PlaySFX(winRoundSFX);
    public void PlayWinMatch() => PlaySFX(winMatchSFX);
    public void PlayDrawLoss() => PlaySFX(drawLossSFX);

    private void LoadSettings()
    {
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
        isRepeatTrack = PlayerPrefs.GetInt("BGM_Repeat", 0) == 1;

        if (bgmSource != null)
        {
            bgmSource.volume = musicVolume;
        }

        if (sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
        }
    }

    public string GetCurrentTrackName()
    {
        if (bgmTracks != null && bgmTracks.Count > currentTrackIndex)
            return bgmTracks[currentTrackIndex].trackName;
        return "No Music";
    }

    public bool IsRepeatTrack => isRepeatTrack;
    public float MusicVolume => musicVolume;
    public float SFXVolume => sfxVolume;
}