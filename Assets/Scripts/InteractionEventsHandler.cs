using UnityEngine;

public class InteractionEventsHandler : MonoBehaviour
{
    public delegate void ItemPickedUpHandler(Interactable interactable);
    public event ItemPickedUpHandler ItemPickedUp;

    public void TriggerItemPickedUp(Interactable interactable)
    {
        ItemPickedUp?.Invoke(interactable);
    }
}
