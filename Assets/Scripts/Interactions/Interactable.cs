using UnityEngine;

public class Interactable : MonoBehaviour
{
    private InteractionEventsHandler interactionEventsHandler;

    public enum InteractType { Pickable, InteractOnce, InteractMulti }
    public enum InspectType { BringToCamera, CameraToInteractable, None }

    [SerializeField] private InteractType type;
    [SerializeField] private InspectType inspectType = InspectType.None;

    public string Name => interactableName;

    public Sprite Icon => icon;

    [SerializeField] private string interactableName;
    [SerializeField] private Sprite icon;

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();

        if (interactionEventsHandler == null)
        {
            Debug.LogError("InteractionEventsHandler not found in the scene.");
        }
    }

    public void Interact()
    {
        if (type == InteractType.Pickable && inspectType == InspectType.None)
        {
            interactionEventsHandler.TriggerItemPickedUp(this);
        }

        if (type == InteractType.Pickable && inspectType == InspectType.BringToCamera)
        {
            interactionEventsHandler.TriggerInspectObject(gameObject);
        }
    }
}