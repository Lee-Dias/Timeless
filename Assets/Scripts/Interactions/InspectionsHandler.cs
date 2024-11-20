using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class InspectionsHandler : MonoBehaviour
{
    public UnityEvent onInspectionStarted;

    // Returns true if the item was added to inventory.
    public UnityEvent<bool> onInspectionEnded;
    public UnityEvent<Item> onItemAddedToInventory;

    PlayerInputs playerInputs;
    [SerializeField] private Transform objectContainer;
    private GameObject inspectingObject;
    private Item currentItem;

    private float maxDistance = 0f;
    private float minDistance = -.25f;

    private bool canBeAddedToInv;

    private void Start()
    {
        GetComponent<Camera>().enabled = false;
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }

    public void StartInspection(Item item, bool canBeAddedToInv)
    {
        if (inspectingObject == null && currentItem == null)
        {
            if (objectContainer == null)
            {
                Debug.LogError("Inspection Handler is missing an object container!", this);
                return;
            }

            currentItem = item;
            if (currentItem != null)
            {
                this.canBeAddedToInv = canBeAddedToInv;
                inspectingObject = Instantiate(item.Prefab, objectContainer.position, Quaternion.identity, objectContainer);
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
                // Calculate the rotation values based on player input
                yaw = -playerInputs.LookInput.x;
                pitch = -playerInputs.LookInput.y;

                // Apply rotation to the object in global space
                inspectingObject.transform.Rotate(new Vector3(0, yaw, pitch), Space.World);
            }


            if (playerInputs.GrabButtonDown && canBeAddedToInv)
            {
                PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
                playerInventory.AddItemToInventory(currentItem);

                onItemAddedToInventory.Invoke(currentItem);
                currentItem = null;
                Destroy(inspectingObject);

                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke(true);
                yield break;
            }
            if (playerInputs.ReturnButtonDown)
            {
                currentItem = null;
                Destroy(inspectingObject);

                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke(false);
                yield break;
            }
            yield return null;
        }
        onInspectionEnded.Invoke(false);
    }
}