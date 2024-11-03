using UnityEngine;

public class PlayerInteractableSlot : MonoBehaviour
{
    PlayerInputs playerInputs;
    [SerializeField] LayerMask interactableSlotLayerMask;
    [SerializeField] private float interactionDistance = 1;

    private void Start()
    {
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    private void Update()
    {
        if (playerInputs.InteractButtonDown)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, interactionDistance, interactableSlotLayerMask))
            {
                InteractableSlot interactableSlot = hit.transform.GetComponent<InteractableSlot>();
                if (interactableSlot != null)
                {
                    interactableSlot.Interact();
                }
            }
            
        }
        
    }
}
