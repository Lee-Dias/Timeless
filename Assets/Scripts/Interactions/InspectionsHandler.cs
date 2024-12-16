using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages item inspection, allowing players to examine objects up close and interact with them.
/// </summary>
/// <remarks>
/// The `InspectionsHandler` class provides functionality to handle object inspection in a 3D space. 
/// Players can rotate, zoom, and optionally add inspected items to their inventory. This system 
/// is commonly used in adventure or puzzle games to provide a closer look at objects of interest.
/// 
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Spawns an inspectable object at a defined position for detailed examination.</description>
/// </item>
/// <item>
/// <description>Allows the player to rotate and zoom in/out on the object using input controls.</description>
/// </item>
/// <item>
/// <description>Handles interaction logic for adding items to the inventory or exiting inspection.</description>
/// </item>
/// <item>
/// <description>Supports event-driven architecture to notify external systems when inspections start or end.</description>
/// </item>
/// </list>
/// 
/// <para><b>How It Works:</b></para>
/// <para>
/// When an inspection begins via <c>StartInspection</c>, the script instantiates the item prefab 
/// in the defined <c>objectContainer</c>. The item can then be manipulated (rotated and zoomed) 
/// based on player inputs.
/// </para>
/// <para>
/// Players can either add the inspected item to their inventory or exit the inspection without 
/// adding the item. Events are triggered to signal the start and end of the inspection process, 
/// as well as when an item is successfully added to the inventory.
/// </para>
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Requires a <c>PlayerInputs</c> script to capture player actions (e.g., rotation, zoom, grab).</description>
/// </item>
/// <item>
/// <description>Requires an <c>Item</c> class with a reference to the item’s prefab.</description>
/// </item>
/// <item>
/// <description>Requires a <c>PlayerInventory</c> to manage inventory logic.</description>
/// </item>
/// </list>
/// 
/// <para><b>Unity Events:</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>onInspectionStarted</c>: Triggered when an inspection begins.</description>
/// </item>
/// <item>
/// <description><c>onInspectionEnded</c>: Triggered when an inspection ends, with a boolean indicating success.</description>
/// </item>
/// <item>
/// <description><c>onItemAddedToInventory</c>: Triggered when an item is successfully added to the inventory.</description>
/// </item>
/// </list>
/// 
/// </remarks>
public class InspectionsHandler : MonoBehaviour
{
    //Event triggered when an inspection starts.
    public UnityEvent onInspectionStarted;

    //Event triggered when an inspection ends.\nThe boolean parameter indicates whether the item was added to the inventory (true) or not (false).
    public UnityEvent<bool> onInspectionEnded;
    public UnityEvent onInspectionEndedFromInv;

    // Event triggered when an item is added to the player's inventory.\nProvides the added Item as a parameter.
    public UnityEvent<Item> onItemAddedToInventory;

    [SerializeField] private Transform objectContainer; // The container to hold the inspected object.
    [SerializeField] private GameObject inspectionInstructions;
    private PlayerInputs playerInputs; // Reference to player input handling.
    private GameObject inspectingObject; // The currently inspected object.
    private Item currentItem; // The item being inspected.

    // Maximum and minimum allowable distances for zooming during inspection.
    private float maxDistance = 0f;
    private float minDistance = -0.25f;

    private bool canBeAddedToInv; // Determines if the item can be added to the inventory.

    private bool inspectingFromInv = false;

    private CrosshairUI crosshairUI;

    [HideInInspector] public bool inspecting;
    private Inspectable inspectable;


    /// <summary>
    /// Initializes the InspectionsHandler, ensuring the camera is disabled initially.
    /// Also locates the <see cref="PlayerInputs"/> component.
    /// </summary>
    private void Start()
    {
        inspecting = false;
        crosshairUI = FindFirstObjectByType<CrosshairUI>();
        crosshairUI?.gameObject.SetActive(true);
        // Check for the Camera component and disable it if present.
        Camera camera = GetComponent<Camera>();
        if (camera != null)
        {
            camera.enabled = false;  // Disable the camera at the start.
        }
        else
        {
            // Log an error if the camera component is missing.
            Debug.LogError($"{nameof(InspectionsHandler)} needs a Camera component. Please add one to the GameObject.");
        }

        // Try to find the PlayerInputs object in the scene.
        playerInputs = FindFirstObjectByType<PlayerInputs>();


        // If PlayerInputs is missing, instantiate it and log a warning.
        if (playerInputs == null)
        {
            Debug.LogWarning($"{nameof(InspectionsHandler)} needs a {nameof(PlayerInputs)} object in the scene. Instantiating a new one.");
            playerInputs = Instantiate(new PlayerInputs());
        }
    }


    /// <summary>
    /// Starts the inspection of an item.
    /// </summary>
    /// <param name="item">The item to be inspected.</param>
    /// <param name="canBeAddedToInv">Indicates if the item can be added to the inventory.</param>
    public void StartInspection(Item item, bool canBeAddedToInv, bool isInspectingFromInv)
    {

        // Ensure no other object is currently being inspected.
        if ((inspectingObject == null) && (currentItem == null))
        {
            inspecting = true;
            inspectingFromInv = isInspectingFromInv;
            // Check if the object container is set.
            if (objectContainer == null)
            {
                Debug.LogError("Inspection Handler is missing an object container!", this);
                return;
            }

            currentItem = item;
            if (currentItem != null)
            {
                this.canBeAddedToInv = canBeAddedToInv;
                // Instantiate the item's prefab for inspection.
                inspectingObject = Instantiate(item.Prefab, objectContainer.position, Quaternion.identity, objectContainer);
                // Begin the inspection coroutine.
                StartCoroutine(InspectionCoroutine());
                // Trigger the inspection started event.
                onInspectionStarted.Invoke();
            }
        }
    }

    /// <summary>
    /// Coroutine that manages the entire inspection process, including the following actions:
    /// <list type="bullet">
    /// <item><description>Zooming in/out of the item based on player input.</description></item>
    /// <item><description>Rotating the item in 3D space using player input (pitch and yaw).</description></item>
    /// <item><description>Adding the item to the player's inventory when requested.</description></item>
    /// <item><description>Allowing the player to return the item without adding it to the inventory.</description></item>
    /// </list>
    /// The coroutine runs continuously while the player inspects the item and processes the player input accordingly.
    /// It ends either when the player adds the item to the inventory or decides to return the item.
    /// </summary>
    /// <remarks>
    /// <b>Key Features:</b>
    /// <list type="bullet">
    /// <item><description>Handles zoom by adjusting the item's position in local space along the Z-axis.</description></item>
    /// <item><description>Supports item rotation by adjusting the pitch (X-axis) and yaw (Y-axis) based on player input.</description></item>
    /// <item><description>Manages item addition to the inventory using the <see cref="GrabButtonDown"/>.</description></item>
    /// <item><description>Ends the inspection and invokes the corresponding event when the player either adds or returns the item.</description></item>
    /// </list>
    /// </remarks>
    private IEnumerator InspectionCoroutine()
    {
        // Wait one frame to ensure initialization is complete.
        yield return null;

        inspectingObject.transform.localRotation = Quaternion.identity;

        // Enable the inspection camera.
        GetComponent<Camera>().enabled = true;
        inspectionInstructions.SetActive(true);

        float pitch = 0; // Rotation around the X-axis.
        float yaw = 0; // Rotation around the Y-axis.

        // Loop while the current item is being inspected.
        while (currentItem != null)
        {
            // Handle zoom input.
            if (playerInputs.ZoomInput != Vector2.zero)
            {
                Vector3 pos = inspectingObject.transform.localPosition;
                pos.z += playerInputs.ZoomInput.y / 100; // Adjust zoom level based on input.
                pos.z = Mathf.Clamp(pos.z, minDistance, maxDistance); // Clamp zoom within limits.
                inspectingObject.transform.localPosition = pos;
            }

            // Handle rotation input if the rotate button is pressed.
            if (playerInputs.RotateButton)
            {
                yaw = -playerInputs.LookInput.x; // Horizontal rotation input.
                pitch = -playerInputs.LookInput.y; // Vertical rotation input.

                // Apply rotation in world space.
                inspectingObject.transform.Rotate(new Vector3(0, yaw, pitch), Space.World);
            }

            // Handle adding the item to the inventory.
            if (playerInputs.GrabButtonDown && canBeAddedToInv && !inspectingFromInv)
            {
                // Add the item to the player's inventory.
                PlayerInventory playerInventory = FindFirstObjectByType<PlayerInventory>();
                playerInventory.AddItemToInventory(currentItem);

                // Trigger the item added event and end inspection.
                onItemAddedToInventory.Invoke(currentItem);
                currentItem = null;
                Destroy(inspectingObject);
                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke(true);
                inspecting = false;
                inspectionInstructions.SetActive(false);
                yield break;
            }

            // Handle returning the item without adding it to the inventory.
            if (playerInputs.ReturnButtonDown && !inspectingFromInv)
            {
                currentItem = null;
                Destroy(inspectingObject);
                GetComponent<Camera>().enabled = false;
                onInspectionEnded.Invoke(false);
                inspectionInstructions.SetActive(false);
                inspecting = false;
                yield break;
            }

            if (playerInputs.GrabButtonDown && inspectingFromInv)
            {
                currentItem = null;
                Destroy(inspectingObject);
                GetComponent<Camera>().enabled = false;
                inspectionInstructions.SetActive(false);
                onInspectionEndedFromInv.Invoke();
                crosshairUI?.gameObject.SetActive(true);
                inspecting = false;
                yield break;
            }



            // Wait for the next frame.
            yield return null;
        }

        // If inspection ends without adding the item, trigger the event with false.
        onInspectionEnded.Invoke(false);
        inspectionInstructions.SetActive(false);
        onInspectionEndedFromInv.Invoke();
    }
}
