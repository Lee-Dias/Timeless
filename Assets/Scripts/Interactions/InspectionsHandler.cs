using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class InspectionsHandler : MonoBehaviour
{
    public UnityEvent onInspectionStarted;
    public UnityEvent onInspectionEnded;

    PlayerInputs playerInputs;
    [SerializeField] private Transform objectContainer;
    private GameObject inspectingObject;
    private Item currentItem;

    private float maxDistance = 0f;
    private float minDistance = -.25f;

    private void Start()
    {
        GetComponent<Camera>().enabled = false;
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    public void StartInspection(Item item)
    {
        if (inspectingObject == null && currentItem == null)
        {
            currentItem = item;
            if (currentItem != null)
            {
                inspectingObject = Instantiate(item.Prefab, objectContainer.position, Quaternion.identity);
                inspectingObject.transform.SetParent(objectContainer);
                StartCoroutine(InspectionCoroutine());
                onInspectionStarted.Invoke();
            }
        }
    }

    private IEnumerator InspectionCoroutine()
    {
        yield return null;

        GetComponent<Camera>().enabled = true;

        float pitch = 0;
        float yaw = 0;

        while (currentItem != null)
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

                inspectingObject.transform.localRotation = Quaternion.Euler(0, yaw, pitch);
            }

            if (playerInputs.GrabButtonDown)
            {
                PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
                playerInventory.AddItemToInventory(currentItem);

                currentItem = null;
                Destroy(inspectingObject);

                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke();
                yield break;
            }
            if (playerInputs.ReturnButtonDown)
            {
                currentItem = null;
                Destroy(inspectingObject);

                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke();
                yield break;
            }
            yield return null;
        }
        onInspectionEnded.Invoke();
    }
}