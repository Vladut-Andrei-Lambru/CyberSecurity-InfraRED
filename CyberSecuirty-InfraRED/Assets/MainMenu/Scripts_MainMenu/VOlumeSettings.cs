using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class VolumeSettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Mixer")]
    public AudioMixer mixer;

    [Header("Exposed Params (must exist)")]
    public string musicParam = "MusicVol";
    public string sfxParam = "SfxVol";

    private const string KMaster = "Volume";        // same key as VolumeController
    private const string KMusic  = "MusicVolume";
    private const string KSfx    = "SFXVolume";

    private void Start()
    {
        float masterVolume = PlayerPrefs.GetFloat(KMaster, 1f);
        float musicVolume  = PlayerPrefs.GetFloat(KMusic, 1f);
        float sfxVolume    = PlayerPrefs.GetFloat(KSfx, 1f);

        if (masterSlider) masterSlider.SetValueWithoutNotify(masterVolume);
        if (musicSlider)  musicSlider.SetValueWithoutNotify(musicVolume);
        if (sfxSlider)    sfxSlider.SetValueWithoutNotify(sfxVolume);

        // Apply immediately
        ApplyMaster(masterVolume);
        ApplyMusic(musicVolume);
        ApplySfx(sfxVolume);

        if (masterSlider) masterSlider.onValueChanged.AddListener(SetMasterVolume);
        if (musicSlider)  musicSlider.onValueChanged.AddListener(SetMusicVolume);
        if (sfxSlider)    sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMasterVolume(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KMaster, value);
        PlayerPrefs.Save();
        ApplyMaster(value);
    }

    public void SetMusicVolume(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KMusic, value);
        PlayerPrefs.Save();
        ApplyMusic(value);
    }

    public void SetSFXVolume(float value)
    {
        value = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KSfx, value);
        PlayerPrefs.Save();
        ApplySfx(value);
    }

    private void ApplyMaster(float v)
    {
        // Master is global clamp (matches your VolumeController)
        AudioListener.volume = v;
    }

    private void ApplyMusic(float v)
    {
        if (mixer == null) return;
        mixer.SetFloat(musicParam, Linear01ToDb(v));
    }

    private void ApplySfx(float v)
    {
        if (mixer == null) return;
        mixer.SetFloat(sfxParam, Linear01ToDb(v));
    }

    private static float Linear01ToDb(float v)
    {
        v = Mathf.Clamp(v, 0.0001f, 1f);
        return Mathf.Log10(v) * 20f; // 1 -> 0dB, 0.5 -> -6dB, 0 -> ~-80dB via clamp
    }
}