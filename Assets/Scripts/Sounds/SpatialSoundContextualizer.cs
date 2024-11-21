using UnityEngine;
using System.Collections.Generic;

public class SpatialSoundContextualizer : MonoBehaviour
{
    [Header("Raycast Settings")]
    public float maxRaycastDistance = 30.0f;
    public float updateFrequencySeconds = 0.5f;
    public int numberOfRaycasts = 10;
    public int maxBounceCount = 3;  // Number of allowed bounces for each raycast

    [Header("Audio Effects Settings")]
    public float maxReverbWetness = 0.5f;
    public int wallLowpassCutoffAmount = 600;

    private float[] distanceArray;
    private float lastUpdateTime = 0.0f;
    private bool updateDistances = true;

    private AudioReverbFilter reverbFilter;
    private AudioLowPassFilter lowPassFilter;

    private float targetLowpassCutoff = 20000.0f;
    private float targetReverbRoomSize = 0.0f;
    private float targetReverbWetness = 0.0f;
    private float targetVolumeDb = 0.0f;

    private Collider player;

    // List to store random directions for the raycasts
    private Vector3[] raycastDirections;

    // Gizmos visualization of raycast bounces
    private List<List<Vector3>> raycastPaths;

    void Start()
    {
        // Audio effects setup
        reverbFilter = gameObject.AddComponent<AudioReverbFilter>();
        lowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();

        targetVolumeDb = AudioListener.volume;
        AudioListener.volume = 0.0f;

        // Initialize distance array and raycast directions array
        distanceArray = new float[numberOfRaycasts];
        raycastDirections = new Vector3[numberOfRaycasts];
        raycastPaths = new List<List<Vector3>>(numberOfRaycasts);

        player = FindFirstObjectByType<PlayerMovement>().GetComponent<Collider>();

        // Initialize random ray directions for the first update
        GenerateRandomRaycastDirections();
    }

    // Generates random normalized directions and stores them in raycastDirections array
    void GenerateRandomRaycastDirections()
    {
        for (int i = 0; i < numberOfRaycasts; i++)
        {
            raycastDirections[i] = Random.onUnitSphere; // Random point on the surface of a sphere
            raycastPaths.Add(new List<Vector3>());  // Initialize path list for Gizmo visualization
        }
    }

    // Update the raycast distance and handle bounces
    void UpdateRaycastDistance(int raycastIndex)
    {
        Vector3 origin = transform.position;
        Vector3 direction = raycastDirections[raycastIndex];
        float totalDistance = 0.0f;

        // Clear previous path for Gizmo visualization
        raycastPaths[raycastIndex].Clear();
        raycastPaths[raycastIndex].Add(origin);  // Add initial position for visualization

        for (int bounces = 0; bounces <= maxBounceCount; bounces++)
        {
            Ray ray = new Ray(origin, direction);
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
            {
                float hitDistance = Vector3.Distance(origin, hit.point);
                totalDistance += hitDistance;

                raycastPaths[raycastIndex].Add(hit.point);  // Add hit point for Gizmo visualization

                // Check if we've exceeded the max distance for this ray
                if (totalDistance > maxRaycastDistance)
                {
                    distanceArray[raycastIndex] = maxRaycastDistance;
                    break;
                }

                // Reflect the ray direction based on the surface normal
                direction = Vector3.Reflect(direction, hit.normal);
                origin = hit.point;  // Move the origin to the hit point for the next bounce
            }
            else
            {
                // No hit, so set remaining distance to maxRaycastDistance
                distanceArray[raycastIndex] = Mathf.Min(totalDistance, maxRaycastDistance);
                break;
            }
        }

        // If no bounces happened, mark the distance as -1 (nothing was hit)
        if (totalDistance == 0.0f)
        {
            distanceArray[raycastIndex] = -1;
        }
    }

    void UpdateSpatialAudio()
    {
        UpdateReverb();
        UpdateLowpassFilter();
    }

    void UpdateReverb()
    {
        if (reverbFilter != null)
        {
            float roomSize = 0.0f;
            float wetness = 1.0f;
            foreach (float dist in distanceArray)
            {
                if (dist >= 0)
                {
                    roomSize += (dist / maxRaycastDistance) / distanceArray.Length;
                    roomSize = Mathf.Min(roomSize, 1.0f);
                }
                else
                {
                    wetness -= 1 / distanceArray.Length;
                    wetness = Mathf.Max(wetness, 0.0f);
                }
            }
            targetReverbWetness = wetness;
            targetReverbRoomSize = roomSize;
        }
    }

    void UpdateLowpassFilter()
    {
        if (lowPassFilter != null && player != null)
        {
            Vector3 directionToPlayer = (player.transform.position - transform.position).normalized;
            Ray ray = new Ray(transform.position, directionToPlayer);

            float lowpassCutoff = 20000.0f;
            if (Physics.Raycast(ray, out RaycastHit hit, maxRaycastDistance))
            {
                float rayDistance = Vector3.Distance(transform.position, hit.point);
                float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
                float wallToPlayerRatio = rayDistance / Mathf.Max(distanceToPlayer, 0.001f);

                if (hit.collider.GetComponent<PlayerMovement>() != null)
                {
                    wallToPlayerRatio = distanceToPlayer;
                }
                else if (rayDistance < distanceToPlayer)
                {
                    lowpassCutoff = wallLowpassCutoffAmount * wallToPlayerRatio;
                }
            }
            targetLowpassCutoff = lowpassCutoff;

        }
    }

    void LerpParameters()
    {
        AudioListener.volume = Mathf.Lerp(AudioListener.volume, targetVolumeDb, Time.deltaTime);
        lowPassFilter.cutoffFrequency = Mathf.Lerp(lowPassFilter.cutoffFrequency, targetLowpassCutoff, Time.deltaTime);
        reverbFilter.reverbLevel = Mathf.Lerp(reverbFilter.reverbLevel, targetReverbRoomSize, Time.deltaTime);
        reverbFilter.dryLevel = Mathf.Lerp(reverbFilter.dryLevel, (targetReverbWetness * maxReverbWetness) * -1, Time.deltaTime);
    }

    void Update()
    {
        lastUpdateTime += Time.deltaTime;

        if (lastUpdateTime > updateFrequencySeconds)
        {
            // Generate new random directions for raycasts on each update
            GenerateRandomRaycastDirections();

            // Update all raycast distances using the new random directions
            for (int i = 0; i < numberOfRaycasts; i++)
            {
                UpdateRaycastDistance(i);
            }

            // Update spatial audio effects after raycast updates
            UpdateSpatialAudio();

            // Reset the update timer
            lastUpdateTime = 0.0f;
        }

        // Smoothly interpolate audio effects
        LerpParameters();
    }

    // Use Gizmos to visualize the raycast paths (including bounces) in the scene
    private void OnDrawGizmos()
    {
        if (raycastPaths != null)
        {
            Gizmos.color = Color.red;

            foreach (List<Vector3> path in raycastPaths)
            {
                for (int i = 0; i < path.Count - 1; i++)
                {
                    Gizmos.DrawLine(path[i], path[i + 1]);
                }
            }
        }
    }
}
