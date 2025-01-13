using UnityEngine;

/// <summary>
/// This class is responsible for oscillating the intensity and range of a Light component.
/// </summary>
[ExecuteInEditMode]
public class OscillateLight : MonoBehaviour
{
    // The Light component to be controlled.
    [SerializeField]
    private Light oscillateLight;

    
    [SerializeField]
    private float intensityMin = .5f; // Minimum intensity value.
    
    [SerializeField]
    private float intensityMax = 1f; // Maximum intensity value.

    [SerializeField]
    private float rangeMin = 3.5f; // Minimum range value.

    [SerializeField]
    private float rangeMax = 4.5f; // Maximum range value.

    // Speed of the transition for intensity and range.
    [SerializeField]
    private float changeSpeed = 1f;

    // Random offset for the oscillation time
    private float intensityOffset;
    private float rangeOffset;

    private void Awake()
    {
        // Initialize random offsets
        intensityOffset = Random.Range(0f, 2f * Mathf.PI);
        rangeOffset = Random.Range(0f, 2f * Mathf.PI);
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if (oscillateLight == null) return;

        // Oscillate the light's intensity and range every frame
        OscillateLightIntensity();
        OscillateLightRange();
    }

    /// <summary>
    /// Oscillates the light's intensity between intensityMin and intensityMax.
    /// </summary>
    private void OscillateLightIntensity()
    {
        // Calculate new intensity with offset
        float intensity = Mathf.Lerp(intensityMin, intensityMax, (Mathf.Sin(Time.time * changeSpeed + intensityOffset) + 1f) / 2f);
        oscillateLight.intensity = intensity;
    }

    /// <summary>
    /// Oscillates the light's range between rangeMin and rangeMax.
    /// </summary>
    private void OscillateLightRange()
    {
        // Calculate new range with offset
        float range = Mathf.Lerp(rangeMin, rangeMax, (Mathf.Sin(Time.time * changeSpeed + rangeOffset) + 1f) / 2f);
        oscillateLight.range = range;
    }

    private void OnValidate()
    {
        if (oscillateLight == null)
        {
            oscillateLight = GetComponent<Light>();
        }
    }
}
