using UnityEngine;

public class PlayerInteractor : MonoBehaviour
{
    PlayerInputs playerInputs;
    [SerializeField] LayerMask interactablesLayerMask;

    private void Start()
    {
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    private void Update()
    {
        if (playerInputs.InteractButtonDown)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, 50, interactablesLayerMask))
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
