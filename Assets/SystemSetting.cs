
using System;
using UnityEngine;
using UnityEngine.UI;

public class SystemSetting : MonoBehaviour
{
    public SoundManager soundManager;
    
    public Toggle BGM_Toggle;
    public Toggle SFX_Toggle;
    
    public Slider slider_BGM;
    public Slider slider_SFX;

    private void Start()
    {
        if (PlayerPrefs.HasKey("BGMVolume"))
        {
            float _volume = PlayerPrefs.GetFloat("BGMVolume");
            soundManager.bgm_audio.volume = _volume;
            slider_BGM.value = _volume;
        }

        if (PlayerPrefs.HasKey("SFXVolume"))
        {
            float _volume = PlayerPrefs.GetFloat(("SFXVolume"));
            soundManager.se_audio.volume = _volume;
            soundManager.se_audio2.volume = _volume;
            slider_SFX.value = _volume;
        }

        if (PlayerPrefs.HasKey("BGMMute"))
        {
            if (bool.TryParse(PlayerPrefs.GetString("BGMMute"), out bool _bgmMute))
            {
                soundManager.bgm_audio.mute = _bgmMute;
                BGM_Toggle.isOn = !_bgmMute;
            }
        }
        
        if (PlayerPrefs.HasKey("SFXMute"))
        {
            if (bool.TryParse(PlayerPrefs.GetString("SFXMute"), out bool _sfxMute))
            {
                soundManager.se_audio.mute = _sfxMute;
                soundManager.se_audio2.mute = _sfxMute;
                soundManager.count_audio.mute = _sfxMute;
                SFX_Toggle.isOn = !_sfxMute;
            }
        }
        
        slider_BGM.onValueChanged.AddListener(BGM_SliderChanged);
        slider_SFX.onValueChanged.AddListener(SFX_SliderChanged);
        
        BGM_Toggle.onValueChanged.AddListener(BGMMute);
        SFX_Toggle.onValueChanged.AddListener(SFXMute);
    }

    private void OnEnable()
    {
        slider_BGM.value = soundManager.bgm_audio.volume;
        slider_SFX.value = soundManager.se_audio.volume;
        BGM_Toggle.isOn = !soundManager.bgm_audio.mute;
        SFX_Toggle.isOn = !soundManager.se_audio.mute;
    }

    private void BGMMute(bool value)
    {
        soundManager.bgm_audio.mute = !value;
        soundManager.SEAudioPlay(0);
    }

    private void SFXMute(bool value)
    {
        soundManager.se_audio.mute = !value;
        soundManager.se_audio2.mute = !value;
        soundManager.count_audio.mute = !value;
        if (value == false)
        {
            soundManager.SEAudioPlay(0);
        }
    }

    public void _Okay()
    {
        PlayerPrefs.SetString("BGMMute", soundManager.bgm_audio.mute.ToString());
        PlayerPrefs.SetString("SFXMute", soundManager.se_audio.mute.ToString());
        PlayerPrefs.SetFloat("BGMVolume", slider_BGM.value);
        PlayerPrefs.SetFloat("SFXVolume", slider_SFX.value);
    }

    private void BGM_SliderChanged(float value){
        soundManager.bgm_audio.volume = value;
    }

    private void SFX_SliderChanged(float value){ 
        soundManager.se_audio.volume = slider_SFX.value;
        soundManager.se_audio2.volume = slider_SFX.value;
        soundManager.SEAudioPlay(0);
    }
}
