using System.Collections;
using UnityEngine;

public class InspectionsHandler : MonoBehaviour
{
    PlayerInputs playerInputs;
    InteractionEventsHandler interactionEventsHandler;
    [SerializeField] private Transform objectContainer;
    private GameObject inspectingObject;
    [SerializeField] private LayerMask inspectLayer;
    private Interactable currenctInteractable;

    private float maxDistance = 0f;
    private float minDistance = -.25f;

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();
        playerInputs = FindFirstObjectByType<PlayerInputs>();

        interactionEventsHandler.InspectObject += InspectObject;
    }

    private void InspectObject(GameObject gameObject)
    {
        if (inspectingObject == null && currenctInteractable == null)
        {
            currenctInteractable = gameObject.GetComponent<Interactable>();
            if (currenctInteractable != null)
            {
                inspectingObject = Instantiate(gameObject, objectContainer.position, Quaternion.Euler(0, 90, 0));
                inspectingObject.transform.SetParent(objectContainer);
                gameObject.SetActive(false);
                StartCoroutine(InspectionCoroutine());
            }
        }
    }

    private IEnumerator InspectionCoroutine()
    {
        yield return null;

        float pitch = inspectingObject.transform.rotation.eulerAngles.x;
        float yaw = inspectingObject.transform.rotation.eulerAngles.y;

        while (currenctInteractable != null)
        {

            if (playerInputs.ZoomInput != Vector2.zero)
            {
                Vector3 pos = inspectingObject.transform.localPosition;

                pos.z += playerInputs.ZoomInput.y / 100;
                pos.z = Mathf.Clamp(pos.z, minDistance, maxDistance);

                inspectingObject.transform.localPosition = pos;
            }

            if (playerInputs.RotateButton)
            {
                yaw -= playerInputs.LookInput.x;
                pitch += playerInputs.LookInput.y;

                inspectingObject.transform.rotation = Quaternion.Euler(pitch, yaw, 0);
            }

            if (playerInputs.GrabButtonDown)
            {
                interactionEventsHandler.TriggerItemPickedUp(currenctInteractable);
                
                currenctInteractable = null;
                Destroy(inspectingObject);

                interactionEventsHandler.TriggerFinishInspect();

                yield break;
            }
            if (playerInputs.ReturnButtonDown)
            {
                currenctInteractable.gameObject.SetActive(true);
                currenctInteractable = null;
                Destroy(inspectingObject);

                interactionEventsHandler.TriggerFinishInspect();

                yield break;
            }
            yield return null;
        }
    }
}
