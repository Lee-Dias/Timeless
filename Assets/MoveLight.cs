using UnityEngine;


public class MoveLight : MonoBehaviour
{
    [SerializeField]
    private new Light light;

    [SerializeField]
    private float intensityMin = 2f;   // Minimum intensity value
    [SerializeField]
    private float intensityMax = 3f;  // Maximum intensity value
    [SerializeField]
    private float rangeMin = 3.5f;    // Minimum range value
    [SerializeField]
    private float rangeMax = 4.5f;    // Maximum range value
    [SerializeField]
    private float changeSpeed = 1f;   // Speed of the transition for intensity and range

    private bool increasingIntensity = true; // Determines whether intensity is increasing
    private bool increasingRange = true;     // Determines whether range is increasing

    private void Update()
    {
        OscillateLightIntensity();
        OscillateLightRange();
    }

    private void OscillateLightIntensity()
    {
        // Check if the intensity is increasing
        if (increasingIntensity)
        {
            // Gradually increase intensity
            light.intensity += changeSpeed * Time.deltaTime;

            // If maximum intensity is reached, start decreasing
            if (light.intensity >= intensityMax)
            {
                light.intensity = intensityMax; // Clamp to maximum
                increasingIntensity = false;
            }
        }
        else
        {
            // Gradually decrease intensity
            light.intensity -= changeSpeed * Time.deltaTime;

            // If minimum intensity is reached, start increasing
            if (light.intensity <= intensityMin)
            {
                light.intensity = intensityMin; // Clamp to minimum
                increasingIntensity = true;
            }
        }
    }

    private void OscillateLightRange()
    {
        // Check if the range is increasing
        if (increasingRange)
        {
            // Gradually increase range
            light.range += changeSpeed * Time.deltaTime;

            // If maximum range is reached, start decreasing
            if (light.range >= rangeMax)
            {
                light.range = rangeMax; // Clamp to maximum
                increasingRange = false;
            }
        }
        else
        {
            // Gradually decrease range
            light.range -= changeSpeed * Time.deltaTime;

            // If minimum range is reached, start increasing
            if (light.range <= rangeMin)
            {
                light.range = rangeMin; // Clamp to minimum
                increasingRange = true;
            }
        }
    }
}
