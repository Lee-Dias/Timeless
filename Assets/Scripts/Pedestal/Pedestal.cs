using System.Collections;
using UnityEngine;

public class Pedestal : Interactable
{
    [Header("Pedestal Setup")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Transform cristalSpawnPos;
    [SerializeField] private Item cristal;
    [SerializeField] private Camera pedestalCamera;
    [SerializeField] private Light glowLight;
    [SerializeField] private Canvas pedestalInstructions;

    private PlayerInputs playerInputs;
    private CrosshairUI crosshairUI;
    private bool puzzleDone = false;
    private bool coroutineEnded = true;
    private float timeOnCristal = 0;

    private void Awake()
    {
        // Attempt to find essential components once, for better performance and clarity.
        playerInputs = FindFirstObjectByType<PlayerInputs>();
        crosshairUI = FindFirstObjectByType<CrosshairUI>();

        if (!playerInputs)
        {
            Debug.LogWarning("PlayerInputs not found, instantiating new one.");
            playerInputs = Instantiate(new PlayerInputs());
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
    }

    /// <summary>
    /// Spawns the cristal and prepares pedestal for interaction.
    /// </summary>
    public void HasCristal()
    {
        GameObject spawnedCristal = Instantiate(cristal.Prefab, cristalSpawnPos.position, Quaternion.identity, cristalSpawnPos);
        spawnedCristal.GetComponent<Interactable>().CanInteract = false;
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
            float xRotation = playerInputs.MoveInput.x * rotationSpeed * Time.deltaTime;
            targetRotation *= Quaternion.Euler(0, xRotation, 0);
            cristalSpawnPos.localRotation = Quaternion.Slerp(cristalSpawnPos.localRotation, targetRotation, smoothingFactor);
        }

        if (Mathf.Abs(playerInputs.MoveInput.y) > 1e-5f)
        {
            float yRotation = playerInputs.MoveInput.y * rotationSpeed * Time.deltaTime;
            lineRendererTargetRotation *= Quaternion.Euler(yRotation, 0, 0);
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

        if (Physics.Raycast(lineRenderer.transform.position, direction, out hit, 50))
        {
            SarcophagusCristal sarcophagusCristal = hit.collider.GetComponent<SarcophagusCristal>();
            if (sarcophagusCristal)
            {
                HandleSarcophagusHit(sarcophagusCristal);
            }
            else timeOnCristal = 0;
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
            puzzleDone = true;
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
        crosshairUI.gameObject.SetActive(false);
        glowLight.gameObject.SetActive(true);
        pedestalInstructions.gameObject.SetActive(true);
    }

    private void DeactivatePedestal()
    {
        pedestalCamera.gameObject.SetActive(false);
        crosshairUI.gameObject.SetActive(true);
        glowLight.gameObject.SetActive(false);
        pedestalInstructions.gameObject.SetActive(false);

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