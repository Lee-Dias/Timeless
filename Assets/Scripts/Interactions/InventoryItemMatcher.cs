using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

/// <summary>
/// Handles the matching of items in the player's inventory with a specified correct item, 
/// based on the item's era and identity.
/// </summary>
/// <remarks>
/// This script checks if the player's currently selected item matches predefined criteria, 
/// such as the correct era and item reference. It supports debugging functionality to log 
/// messages for troubleshooting and invokes Unity events when an item is selected.
///
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item><description>Checks if the selected item matches the required era and item.</description></item>
/// <item><description>Notifies via Unity events when an item is displayed.</description></item>
/// <item><description>Tracks if the correct item is on display using the <c>isRightItem</c> flag.</description></item>
/// </list>
/// 
/// <para><b>Inspector Configurations:</b></para>
/// <list type="bullet">
/// <item><description><c>era</c>: Specifies the required era of the item.</description></item>
/// <item><description><c>rightItem</c>: The exact item that is considered correct.</description></item>
/// <item><description><c>showDebugMessages</c>: Enables or disables debug logging.</description></item>
/// <item><description><c>identifyObject</c>: Includes the object's name in debug messages when enabled.</description></item>
/// </list>
/// 
/// <para><b>Unity Events:</b></para>
/// <list type="bullet">
/// <item><description><c>selectedItem</c>: Invoked when an item is selected, passing the selected item as a parameter.</description></item>
/// </list>
/// </remarks>
public class InventoryItemMatcher : MonoBehaviour
{
    [SerializeField, Tooltip("Specifies the required era that the selected item should belong to.")]
    private Item.Era era;

    [SerializeField, Tooltip("The exact item that is considered correct for this matching scenario.")]
    private Item rightItem;

    [Header("Debugging")]

    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    [SerializeField]
    private bool acceptEra = true;

    [SerializeField, ShowIf(nameof(acceptEra))]
    private bool acceptOnlyRightPuzzle = true;

    //"The player's inventory, used to access the selected item.
    private PlayerInventory playerInventory;

    //"Unity event that is invoked when an item is selected."
    public UnityEvent<Item> selectedItem;

    /// <summary>
    /// A flag indicating whether the selected item is the correct one.
    /// </summary>
    private bool isRightItem = false;

    private void Start()
    {
        // Get the player's inventory object.
        playerInventory = FindFirstObjectByType<PlayerInventory>();

        // Check if the PlayerInventory was found.
        if (playerInventory == null)
        {
            // Log a warning if PlayerInventory is not found.
            Debug.LogWarning($"{nameof(InventoryItemMatcher)} could not find a {nameof(PlayerInventory)} in the scene. Please ensure it is set up.");
        }
    }


    /// <summary>
    /// Checks the currently selected item in the player's inventory.
    /// Validates if the selected item matches the required era and the correct item.
    /// </summary>
    public void CheckItem()
    {
        // Retrieve the currently selected item.
        Item selected = playerInventory.GetSelectedItem();

        // Confirm that an item is selected.
        if (selected != null)
        {
            if (acceptEra == true){
                // Check if the item's era matches the required era.
                if (era == selected.era)
                {
                    //checks if its only supposed to accepth the right puzzle
                    if(acceptOnlyRightPuzzle ){
                        //checks if the item is the right puzzlenumber
                        if(rightItem.Puzzlenumber == selected.Puzzlenumber){
                            // Check if the selected item is the correct one.
                            if (rightItem == selected)
                            {
                                Log("Right item checked!");
                                // Set the flag to true if it's the correct item.
                                isRightItem = true; 
                            }
                            // Invoke the event to notify that the item has been selected, passing the selected item.
                            selectedItem.Invoke(selected);
                        }else{
                            Log("Item checked is from the right era but wrong puzzle.");
                        }
                    }else{
                        // Check if the selected item is the correct one.
                        if (rightItem == selected)
                        {
                            Log("Right item checked!");
                            // Set the flag to true if it's the correct item.
                            isRightItem = true; 
                        }
                        // Invoke the event to notify that the item has been selected, passing the selected item.
                        selectedItem.Invoke(selected);
                    }

                }
                else
                {
                    Log("Item checked is from the wrong era.");
                }
            }
            else{
                    // Check if the selected item is the correct one.
                    if (rightItem == selected)
                    {    
                        // Invoke the event to notify that the item has been selected, passing the selected item.
                        selectedItem.Invoke(selected);
                        Log("Right item checked!");
                        isRightItem = true; // Set the flag to true if it's the correct item.
                    }
                    else
                    {
                    Log("Item checked is wrong.");
                    }
            }
        }
    }

    /// <summary>
    /// Returns a boolean value indicating whether the correct item is currently selected.
    /// </summary>
    /// <returns>True if the correct item is selected, false otherwise.</returns>
    public bool HasRightItem()
    {
        return isRightItem;
    }

    /// <summary>
    /// Resets the <see cref="isRightItem"/> flag to false, indicating that there is no correct item displayed.
    /// </summary>
    public void NoItemOnDisplayer()
    {
        isRightItem = false;
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
            if (identifyObject)
            {
                // Log the message with the object's name in the console.
                Debug.Log(message, this);
            }
            else
            {
                // Log the message without the object's name.
                Debug.Log(message);
            }
        }
    }
}
