using UnityEngine;
using UnityEngine.Events;

public class Inspectable : MonoBehaviour
{
    public UnityEvent<bool> onInspectionEnded;

    InspectionsHandler inspectionsHandler;

    private void Start()
    {
        inspectionsHandler = FindFirstObjectByType<InspectionsHandler>();
    }


    public void StartInspection(Item item)
    {
        inspectionsHandler.StartInspection(item);
        inspectionsHandler.onInspectionStarted.AddListener(OnInspectionStarted);
        inspectionsHandler.onInspectionEnded.AddListener(OnInspectionEnded);
    }


    private void OnInspectionStarted()
    {

    }

    private void OnInspectionEnded(bool wasAddedToInventory)
    {
        inspectionsHandler.onInspectionStarted.RemoveListener(OnInspectionStarted);
        inspectionsHandler.onInspectionEnded.RemoveListener(OnInspectionEnded);
        onInspectionEnded.Invoke(!wasAddedToInventory);
    }
}
