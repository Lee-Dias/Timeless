using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class Pedestal : Interactable
{
    public UnityEvent Completed;
    [Header("Pedestal Setup")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform cristalSpawnPos;
    [SerializeField] private Item cristal;
    [SerializeField] private Camera pedestalCamera;
    [SerializeField] private Light glowLight;
    [SerializeField] private Canvas pedestalInstructions;
    [SerializeField] private Renderer sarcophagusRenderer;
    [SerializeField] private Material cristalMaterial;
    [SerializeField] private float maxRotationX = 45;
    private Material originalSarcophagusMaterial;

    private PlayerInputs playerInputs;
    private CrosshairUI crosshairUI;
    private PlayerUIInventory playerUIInventory;
    private bool puzzleDone = false;
    private bool coroutineEnded = true;
    private float timeOnCristal = 0;

    private void Awake()
    {
        // Attempt to find essential components once, for better performance and clarity.
        playerInputs = FindFirstObjectByType<PlayerInputs>();

        if (!playerInputs)
        {
            Debug.LogWarning("PlayerInputs not found.");
        }
        if (lineRenderer == null) Debug.LogError("LineRenderer is not assigned!");
        if (cristalSpawnPos == null) Debug.LogError("CristalSpawnPos is not assigned!");
        if (cristal == null) Debug.LogError("Cristal item is not assigned!");
        if (pedestalCamera == null) Debug.LogError("Pedestal Camera is not assigned!");
        if (glowLight == null) Debug.LogError("Glow Light is not assigned!");
        if (pedestalInstructions == null) Debug.LogError("Pedestal Instructions Canvas is not assigned!");
    }

    private void Start()
    {
        // Ensure the pedestal's camera is initially disabled.
        pedestalCamera.gameObject.SetActive(false);

        crosshairUI = FindFirstObjectByType<CrosshairUI>();
        playerUIInventory = FindFirstObjectByType<PlayerUIInventory>();

        originalSarcophagusMaterial = sarcophagusRenderer.material;
    }

    /// <summary>
    /// Spawns the cristal and prepares pedestal for interaction.
    /// </summary>
    public void HasCristal()
    {
        GameObject spawnedCristal = Instantiate(cristal.Prefab, cristalSpawnPos.position, Quaternion.identity);
        spawnedCristal.GetComponent<Interactable>().CanInteract = false;
        AudioSource aS = GetComponent<AudioSource>();
        if (aS != null) aS.Play();

        OnCristalInteracted();
        InteractEvent.AddListener(OnCristalInteracted);
        InteractEvent.RemoveListener(GetComponent<InventoryItemMatcher>().CheckItem);
        FindFirstObjectByType<PlayerInventory>().RemoveItemFromInventory(cristal);
    }

    /// <summary>
    /// Handles interactions after the cristal has been placed.
    /// </summary>
    private void OnCristalInteracted()
    {
        if (!coroutineEnded || puzzleDone) return;

        StartCoroutine(RotateCristalCoroutine());
        DisablePlayerControls();
    }

    /// <summary>
    /// Starts rotating the cristal and laser system.
    /// </summary>
    private IEnumerator RotateCristalCoroutine()
    {
        ActivatePedestal();

        float rotationSpeed = 25f;
        float smoothingFactor = 0.1f;
        Quaternion targetRotation = cristalSpawnPos.rotation;
        Quaternion lineRendererTargetRotation = lineRenderer.transform.rotation;

        coroutineEnded = false;
        lineRendererTargetRotation.y -= 100;
        lineRenderer.transform.localRotation = lineRendererTargetRotation;

        timeOnCristal = 0;
        while (!coroutineEnded)
        {
            HandlePlayerInput(rotationSpeed, smoothingFactor, ref targetRotation, ref lineRendererTargetRotation);
            HandleLaserBeam();
            yield return null;
        }

        DeactivatePedestal();
    }

    private void HandlePlayerInput(float rotationSpeed, float smoothingFactor, ref Quaternion targetRotation, ref Quaternion lineRendererTargetRotation)
    {
        // Rotation handling for cristal and laser
        if (Mathf.Abs(playerInputs.MoveInput.x) > 1e-5f)
        {
            float yRotation = playerInputs.MoveInput.x * rotationSpeed * Time.deltaTime;
            targetRotation *= Quaternion.Euler(0, yRotation, 0);
            cristalSpawnPos.localRotation = Quaternion.Slerp(cristalSpawnPos.localRotation, targetRotation, smoothingFactor);
        }

        if (Mathf.Abs(playerInputs.MoveInput.y) > 1e-5f)
        {
            // Calculate rotation increment from input
            float xRotation = playerInputs.MoveInput.y * rotationSpeed * Time.deltaTime;

            // Apply the rotation increment
            lineRendererTargetRotation *= Quaternion.Euler(xRotation, 0, 0);

            // Convert target rotation to Euler angles
            Vector3 targetEulerAngles = lineRendererTargetRotation.eulerAngles;

            // Normalize X rotation to handle values over 180 degrees
            if (targetEulerAngles.x > 180)
                targetEulerAngles.x -= 360;

            // Clamp the X rotation
            targetEulerAngles.x = Mathf.Clamp(targetEulerAngles.x, -maxRotationX, maxRotationX);

            // Reconstruct the quaternion with clamped values
            lineRendererTargetRotation = Quaternion.Euler(targetEulerAngles.x, 180, 0);

            // Smoothly interpolate to the new rotation
            lineRenderer.transform.localRotation = Quaternion.Slerp(lineRenderer.transform.localRotation, lineRendererTargetRotation, smoothingFactor);

        }

        // Stop the coroutine if the return button is pressed
        if (playerInputs.ReturnButtonDown)
        {
            coroutineEnded = true;
        }
    }

    private void HandleLaserBeam()
    {
        // Cast a ray from the laser and update the laser's endpoint
        RaycastHit hit;
        Vector3 direction = lineRenderer.transform.forward * -1;

        int layerMask = ~LayerMask.GetMask("Player");

        if (Physics.Raycast(lineRenderer.transform.position, direction, out hit, 50, layerMask))
        {
            SarcophagusCristal sarcophagusCristal = hit.collider.GetComponent<SarcophagusCristal>();
            if (sarcophagusCristal)
            {
                HandleSarcophagusHit(sarcophagusCristal);
                sarcophagusRenderer.material = cristalMaterial;
            }
            else
            {
                timeOnCristal = 0;
                sarcophagusRenderer.material = originalSarcophagusMaterial;
            }
            UpdateLaserPosition(hit.point);
        }
        else
        {
            UpdateLaserPosition(lineRenderer.transform.position + direction * 50);
        }
    }

    private void HandleSarcophagusHit(SarcophagusCristal sarcophagusCristal)
    {
        timeOnCristal += Time.deltaTime;
        if (timeOnCristal > .5f)
        {
            lineRenderer.gameObject.SetActive(false);
            sarcophagusCristal.Hit();
            InteractEvent.RemoveListener(OnCristalInteracted);           
            Completed?.Invoke();
            puzzleDone = true;
            CanInteract = false;
            coroutineEnded = true;
        }
    }

    private void UpdateLaserPosition(Vector3 hitPoint)
    {
        lineRenderer.SetPosition(0, lineRenderer.transform.position);
        lineRenderer.SetPosition(1, hitPoint);
        glowLight.transform.position = hitPoint;
    }

    private void ActivatePedestal()
    {
        pedestalCamera.gameObject.SetActive(true);
        lineRenderer.gameObject.SetActive(true);
        glowLight.gameObject.SetActive(true);
        pedestalInstructions.gameObject.SetActive(true);

        crosshairUI.gameObject.SetActive(false);
        playerUIInventory.SetActive(false);
    }

    private void DeactivatePedestal()
    {
        pedestalCamera.gameObject.SetActive(false);
        glowLight.gameObject.SetActive(false);
        pedestalInstructions.gameObject.SetActive(false);
        playerUIInventory.SetActive(true);
        crosshairUI.gameObject.SetActive(true);

        EnablePlayerControls();
    }

    private void DisablePlayerControls()
    {
        FindFirstObjectByType<PlayerMovement>().SetCanMove(false);
        FindFirstObjectByType<PlayerCameraRotation>().SetCanLookAround(false);
        FindFirstObjectByType<CameraSway>().SetEnabled(false);
    }

    private void EnablePlayerControls()
    {
        FindFirstObjectByType<PlayerMovement>().SetCanMove(true);
        FindFirstObjectByType<PlayerCameraRotation>().SetCanLookAround(true);
        FindFirstObjectByType<CameraSway>().SetEnabled(true);
    }
}
