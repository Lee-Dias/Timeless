using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractType { Pickable, InteractOnce, InteractMulti }
    public enum InspectType { BringToCamera, CameraToInteractable, None }
    [SerializeField] private InteractType type;
    [SerializeField] private InspectType inspectType = InspectType.None;

    public delegate void ItemPickedUpHandler(Interactable interactable);
    public event ItemPickedUpHandler ItemPickedUp;

    public void Interact()
    {
        if (type == InteractType.Pickable && inspectType == InspectType.None)
        {
            ItemPickedUp?.Invoke(this);
        }
    }
}
