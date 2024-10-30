using UnityEngine;

public class InteractionEventsHandler : MonoBehaviour
{
    public delegate void ItemPickedUpHandler(Interactable interactable);
    public event ItemPickedUpHandler ItemPickedUp;
    public delegate void InspectObjectHandler(GameObject gameObject);
    public event InspectObjectHandler InspectObject;
    public delegate void FinishInspectHandler();
    public event FinishInspectHandler FinishInspect;
    public void TriggerItemPickedUp(Interactable interactable)
    {
        ItemPickedUp?.Invoke(interactable);
    }

    public void TriggerInspectObject(GameObject gameObject)
    {
        InspectObject?.Invoke(gameObject);
    }

    public void TriggerFinishInspect()
    {
        FinishInspect?.Invoke();
    }
}
