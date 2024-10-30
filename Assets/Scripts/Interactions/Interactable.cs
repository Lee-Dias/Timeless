using UnityEngine;

public class Interactable : MonoBehaviour
{
    public enum InteractType { Pickable, InteractOnce }
    public enum InspectType { BringToCamera, CameraToInteractable, None }
    [SerializeField] private InteractType type;
    [SerializeField] private InspectType inspectType = InspectType.None;
}
