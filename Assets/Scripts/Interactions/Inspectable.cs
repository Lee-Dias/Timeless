using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

/// <summary>
/// Represents an object that can be inspected by the player and optionally added to the inventory.
/// </summary>
/// <remarks>
/// The `Inspectable` class integrates with the `InspectionsHandler` to allow objects to be examined 
/// in detail. Players can interact with inspectable objects, triggering an inspection process where 
/// the object can be rotated, zoomed, and added to their inventory if permitted.
/// 
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Initiates and manages the inspection process through the `InspectionsHandler`.</description>
/// </item>
/// <item>
/// <description>Supports optional integration with inventory management by allowing items to be added 
/// to the player's inventory upon inspection completion.</description>
/// </item>
/// <item>
/// <description>Includes debugging functionality to track the inspection process in the Console.</description>
/// </item>
/// </list>
/// 
/// <para><b>How It Works:</b></para>
/// <para>
/// When the `StartInspection` method is called, the object triggers an inspection in the associated 
/// `InspectionsHandler`. Event listeners are added to monitor the inspection process, providing hooks 
/// for external logic when the inspection starts or ends.
/// </para>
/// <para>
/// Debugging options allow for detailed tracking of inspection events, including the ability to identify 
/// specific objects in the logs.
/// </para>
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Requires an `InspectionsHandler` in the scene to manage the inspection process.</description>
/// </item>
/// <item>
/// <description>Works with an `Item` class for interaction and inventory logic.</description>
/// </item>
/// </list>
/// 
/// <para><b>Debugging Options:</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>showDebugMessages</c>: Enables or disables debug logging for this script.</description>
/// </item>
/// <item>
/// <description><c>identifyObject</c>: Includes the object's name in debug messages if enabled.</description>
/// </item>
/// </list>
/// 
/// <para><b>Unity Events:</b></para>
/// <list type="bullet">
/// <item>
/// <description><c>onInspectionEnded</c>: Triggered when the inspection ends, passing a boolean 
/// indicating whether the item was added to the inventory.</description>
/// </item>
/// </list>
/// 
/// </remarks>
public class Inspectable : MonoBehaviour
{
    public UnityEvent<bool> onInspectionEnded;
    [SerializeField] private bool canBeAddedToInv = true;
    InspectionsHandler inspectionsHandler;

    [Header("Debugging")]

    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    private CrosshairUI crosshairUI;

    [HideInInspector]public bool inspectingFromInv;

    private PauseManager pauseManager;


    private void Start()
    {
        inspectionsHandler = FindFirstObjectByType<InspectionsHandler>();
        if (inspectionsHandler == null) Debug.LogError($"{nameof(InspectionsHandler)} needs to be in the scene.");
        // Try to find the pauseManager object in the scene.
        pauseManager = FindFirstObjectByType<PauseManager>();
    }


    public void StartInspection(Item item)
    {
        if(pauseManager.IsPaused == false){
            if (inspectionsHandler == null) FindFirstObjectByType<InspectionsHandler>();
            inspectionsHandler.StartInspection(item, canBeAddedToInv, inspectingFromInv);
            inspectionsHandler.onInspectionStarted.AddListener(OnInspectionStarted);
            inspectionsHandler.onInspectionEnded.AddListener(OnInspectionEnded);
            crosshairUI = FindFirstObjectByType<CrosshairUI>();
            crosshairUI?.gameObject.SetActive(false);
        }else{
            this.gameObject.SetActive(true);
        }

    }

    private void OnInspectionStarted()
    {
        Log("Inspection started");
    }

    private void OnInspectionEnded(bool wasAddedToInventory)
    {  
        inspectingFromInv = false;
        crosshairUI?.gameObject.SetActive(true);
        inspectionsHandler.onInspectionStarted.RemoveListener(OnInspectionStarted);
        inspectionsHandler.onInspectionEnded.RemoveListener(OnInspectionEnded);
        onInspectionEnded.Invoke(!wasAddedToInventory);
        if (wasAddedToInventory) Log("Inspection ended with item was added to the inventory");
        else Log("Inspection ended");

    }
    public void OnInspectionEndedFromInv()
    {  
        inspectingFromInv = false;
        crosshairUI?.gameObject.SetActive(true);
        inspectionsHandler.onInspectionStarted.RemoveListener(OnInspectionStarted);
        inspectionsHandler.onInspectionEnded.RemoveListener(OnInspectionEnded);

    }

    /// <summary>
    /// Logs a debug message to the Console if debugging is enabled.
    /// Includes the object's name as an identifier if 'identifyObject' is true.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    private void Log(string message)
    {
        if (showDebugMessages)
        {
            if (identifyObject) Debug.Log(message, this); // Includes object's name in the Console
            else Debug.Log(message); // Logs message without object identifier
        }
    }
}
