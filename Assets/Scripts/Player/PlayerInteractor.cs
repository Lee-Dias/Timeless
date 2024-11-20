using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    PlayerInputs playerInputs;
    [SerializeField] LayerMask interactablesLayerMask;
    [SerializeField] private float interactionDistance = 2;
    [SerializeField] private GameObject crosshairPrefab;
    private CrosshairUI crosshair;

    private void Start()
    {
        try
        {
            crosshair = FindAnyObjectByType<CrosshairUI>();

        }
        catch
        { // For organization, add the prefab manually to the scene.
            Debug.LogWarning(
                "Player Interactor didn't find a crosshair and is creating one,I recomend adding one to the scene",
                this);

            // This will add the prefab to the scene and get the component.
            crosshair = Instantiate(crosshairPrefab).GetComponent<CrosshairUI>();
        }


        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    private void Update()
    {
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionDistance, interactablesLayerMask))
        {
            Interactable interactable = hit.transform.GetComponent<Interactable>();

            if (interactable != null)
            {
                crosshair.Grow();
                if (playerInputs.InteractButtonDown)
                {
                    interactable.Interact();
                    crosshair.Shrink();
                }
            }
            else crosshair.Shrink();
        }
        else crosshair.Shrink();

    }

}