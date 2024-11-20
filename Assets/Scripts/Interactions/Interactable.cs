using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    private bool canInteract = true;
    public bool CanInteract => canInteract;

    public UnityEvent InteractEvent;

    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }

    public void Interact()
    {
        if (canInteract) InteractEvent?.Invoke();
    }
}

