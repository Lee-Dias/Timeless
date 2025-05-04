using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Handles the displaying and interaction with items in the game, including displaying an item on a display or taking it from the display.
/// Works with the player's inventory and invokes Unity events when actions occur with items.
/// </summary>
/// <remarks>
/// <b>Key Features:</b>
/// <list type="bullet">
/// <item><description><c>onItemDisplayed</c>: Event triggered when an item is displayed on the display.</description></item>
/// <item><description><c>onTakeItem</c>: Event triggered when the item is taken from the display by the player.</description></item>
/// <item><description><c>onHasItem</c>: Event triggered when there is an item present on the display.</description></item>
/// <item><description><c>displayPos</c>: Position in the game world where the item will be displayed.</description></item>
/// </list>
/// 
/// <b>Methods:</b>
/// <list type="bullet">
/// <item><description><c>HasItem()</c>: Checks if there is an item displayed. If so, it removes the item and invokes the <see cref="onTakeItem"/> event. If no item is displayed, it invokes the <see cref="onHasItem"/> event.</description></item>
/// <item><description><c>DisplayItem()</c>: Displays the currently selected item from the player's inventory on the display.</description></item>
/// </list>
/// 
/// <b>Inspector Configurations:</b>
/// <list type="bullet">
/// <item><description><c>displayPos</c>: Position where the item should be displayed in the game world.</description></item>
/// </list>
/// </remarks>
public class ItemDisplayer : MonoBehaviour
{
    //"Unity event invoked when an item is displayed on the item display.
    public UnityEvent<Item> onItemDisplayed;

    //"Unity event invoked when the item on the display is taken by the player.
    public UnityEvent<Item> onTakeItem;

    //"Unity event invoked when there is an item on the display.
    public UnityEvent<Item> onHasItem;

    [Tooltip("The position where the item should be displayed in the game world.")]
    [SerializeField] private Transform displayPos;

    // Reference to the player's inventory used to get the currently selected item.
    private PlayerInventory playerInventory;

    // The current game object representing the displayed item.
    private GameObject displayedObject;

    // The current item that is displayed.
    private Item displayedItem;

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
    /// Checks if there is an item currently displayed. If there is an item, it will be taken and removed from the display.
    /// If no item is displayed, it invokes the <see cref="onHasItem"/> event.
    /// </summary>
    public void HasItem()
    {
        // Checks if there is an item currently displayed on the display.
        if (displayedObject != null && displayedItem != null)
        {
            // Takes the item from the display and removes it.
            onTakeItem.Invoke(displayedItem);
            displayedItem = null;
            Destroy(displayedObject);
            displayedObject = null;
        }
        else
        {
            // If no item is displayed, invoke the onHasItem event.
            onHasItem.Invoke(displayedItem);
        }
    }

    /// <summary>
    /// Displays the currently selected item from the player's inventory on the item display.
    /// </summary>
    public void DisplayItem()
    {
        // Get the currently selected item from the player's inventory.
        Item item = playerInventory.GetSelectedItem();

        // Check if there is no item currently displayed.
        if (displayedObject == null && displayedItem == null)
        {
            // Instantiate the item prefab and place it at the display position.
            displayedObject = Instantiate(item.Prefab, displayPos.position, item.Prefab.transform.rotation);      
            displayedObject.transform.SetParent(displayPos);
            
        
            // Set the current item as the displayed item.
            displayedItem = item;
            // Set the Y position to 1, while keeping X and Z unchanged
            Vector3 currentPosition = displayedObject.transform.position;
            currentPosition.y += item.heightWhenPlacedY;
            displayedObject.transform.position = currentPosition;
            //makes so the object cant be interacted with only the container
            displayedObject.layer = 0;
            // Invoke the onItemDisplayed event, passing the displayed item.
            onItemDisplayed.Invoke(item);
        }
        else
        {
            // If an item is already displayed, do nothing.
            return;
        }
    }
}
