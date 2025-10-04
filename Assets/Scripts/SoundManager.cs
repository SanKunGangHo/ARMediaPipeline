using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [Header("BGM")] public AudioClip[] bgm_clip;

    [Header("SE")] public AudioClip[] se_clip;

    [Header("Count")] public AudioClip[] count_clip;
    
    public AudioSource bgm_audio;
    public AudioSource se_audio;
    public AudioSource se_audio2;
    public AudioSource count_audio;

    private SoundManager _instance;
    public static SoundManager instance { get; private set;}

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            float _volume = PlayerPrefs.GetFloat("BGMVolume");
            Debug.Log(_volume);
            bgm_audio.volume = _volume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float _volume = PlayerPrefs.GetFloat(("SFXVolume"));
            Debug.Log(_volume);
            se_audio.volume = _volume;
            se_audio2.volume = _volume;
        }

        if (PlayerPrefs.HasKey("BGMMute"))
        {
            if (bool.TryParse(PlayerPrefs.GetString("BGMMute"), out bool _bgmMute))
            {
                Debug.Log(_bgmMute);
                bgm_audio.mute = _bgmMute;
            }
        }

        if (PlayerPrefs.HasKey("SFXMute"))
        {
            if (bool.TryParse(PlayerPrefs.GetString("SFXMute"), out bool _sfxMute))
            {
                Debug.Log(_sfxMute);
                se_audio.mute = _sfxMute;
                se_audio2.mute = _sfxMute;
                count_audio.mute = _sfxMute;
            }
        }
    }

    public void BGMAudioPlay(int i)
    {
        bgm_audio.clip = bgm_clip[i];
        bgm_audio.Play();
    }
    
    public void SEAudioPlay(int i)
    {
        se_audio.clip = se_clip[i];
        se_audio.Play();
    }
    
    public void SEAudioPlay2(int i)
    {
        se_audio2.clip = se_clip[i];
        se_audio2.Play();
    }
    
    public void CountAudioPlay(int i)
    {
        count_audio.clip = count_clip[i];
        count_audio.Play();
    }
    
    public void AudioStart(string type)
    {
        switch (type)
        {
            case "BGM" :
                bgm_audio.Play();
                break;
            case "SE1" :
                se_audio.Play();
                break;
            case "SE2" :
                se_audio.Play();
                break;
            case "Count" :
                count_audio.Play();
                break;
        }
    }

    public void AudioStop(string type)
    {
        switch (type)
        {
            case "BGM" :
                bgm_audio.Stop();
                break;
            case "SE1" :
                se_audio.Stop();
                break;
            case "SE2" :
                se_audio.Stop();
                break;
        }
    }
}
