using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private InteractionEventsHandler interactionEventsHandler;

    public enum InteractType { Pickable, InteractOnce, InteractMulti }
    public enum InspectType { BringToCamera, CameraToInteractable, None }
    public enum ItemType { Key, Fuse }
    public enum ItemEra{ PreHistoric, Medieval, Egypt, WildWest, None}


    [SerializeField] private InteractType type;
    [SerializeField] private InspectType inspectType = InspectType.None;
    [SerializeField] private ItemType itemType;
    [SerializeField] private ItemEra itemEra;

    public string Name => interactableName;
    public Sprite Icon => icon;

    // Expose ItemType and ItemEra as public read-only properties
    public ItemType Type => itemType;
    public ItemEra Era => itemEra;

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