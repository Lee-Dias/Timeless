using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class PlayerSettingsLoader : MonoBehaviour
{
    [SerializeField] private PlayerPrefs playerPrefs;

    [SerializeField] private Slider volumeSlider;

    [SerializeField] private Slider senseSlider;

    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        LoadPrefs();
    }

    public void LoadPrefs()
    {
        LoadSoundSettings("mainVol", playerPrefs.mainVolume);
        LoadSoundSettings("musicVol", playerPrefs.musicVolume);
        LoadSoundSettings("effectsVol", playerPrefs.effectsVolume);

        if (volumeSlider != null)
            volumeSlider.value = playerPrefs.mainVolume;

        if (senseSlider != null)
            senseSlider.value = playerPrefs.sense;
    }

    private void LoadSoundSettings(string mixerGroup, float v)
    {
        // Ensure the volume is not too small to avoid invalid calculations
        // 1e-5f is used as a practical lower bound to prevent issues like taking the logarithm of zero.
        if (v <= 1e-5f)
            v = 1e-5f; // Clamp to the lower bound

        // Convert the linear volume (0.0 to 1.0) to a decibel scale using a logarithmic function
        // Unity’s audio mixer expects decibel values. `20 * Mathf.Log10(v)` converts:
        // - Linear input of 1.0 to 0 dB (no attenuation).
        // - Linear input < 1.0 to negative decibels (attenuated volume).
        // - Values near 0 are clamped to approximately -80 dB.
        audioMixer.SetFloat(mixerGroup, Mathf.Log10(v) * 20);
    }
}
