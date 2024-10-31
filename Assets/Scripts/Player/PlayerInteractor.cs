using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    PlayerInputs playerInputs;
    [SerializeField] LayerMask interactablesLayerMask;
    [SerializeField] private float interactionDistance = 1;

    private void Start()
    {
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    private void Update()
    {
        if (playerInputs.InteractButtonDown)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionDistance, interactablesLayerMask))
            {
                Interactable interactable = hit.transform.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }
            }
        }
    }
}
