using UnityEngine;

public class InspectionsHandler : MonoBehaviour
{
    InteractionEventsHandler interactionEventsHandler;
    [SerializeField] private Transform objectContainer;
    private GameObject inspectingObject;
    [SerializeField] private LayerMask inspectLayer;
    private Interactable currenctInteractable;

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();

        interactionEventsHandler.InspectObject += InspectObject;
    }

    private void Update()
    {
        if (currenctInteractable != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                interactionEventsHandler.TriggerItemPickedUp(currenctInteractable);
                currenctInteractable = null;
                Destroy(inspectingObject);

                interactionEventsHandler.TriggerFinishInspect();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                currenctInteractable = null;
                Destroy(inspectingObject);

                interactionEventsHandler.TriggerFinishInspect();
            }
        }
    }

    private void InspectObject(GameObject gameObject)
    {
        if (inspectingObject == null && currenctInteractable == null)
        {
            currenctInteractable = gameObject.GetComponent<Interactable>();
            if (currenctInteractable != null)
            {
                inspectingObject = Instantiate(gameObject, objectContainer.position, Quaternion.Euler(0, 90, 0));

                SetLayerRecursively(inspectingObject, Mathf.RoundToInt(Mathf.Log(inspectLayer.value, 2)));

            }
        }
    }

    // Recursively set the layer on the GameObject and all its children
    void SetLayerRecursively(GameObject obj, int newLayer)
    {
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}
