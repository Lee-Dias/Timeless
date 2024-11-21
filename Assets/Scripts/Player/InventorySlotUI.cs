using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles the UI representation of an inventory slot, including the item icon, outline, and item details.
/// </summary>
/// <remarks>
/// <b>Key Features:</b>
/// <list type="bullet">
/// <item><description><c>Icon</c>: The image component that displays the item's icon.</description></item>
/// <item><description><c>Outline</c>: The image component that represents the outline for the slot.</description></item>
/// <item><description><c>ID</c>: The unique identifier for the item stored in this slot.</description></item>
/// <item><description><c>ItemName</c>: The name of the item displayed in this slot.</description></item>
/// <item><description><c>SetItemUI()</c>: Method that updates the UI elements to reflect the details of the given item.</description></item>
/// </list>
/// </remarks>
public class InventorySlotUI : MonoBehaviour
{
    [Tooltip("The image component that displays the icon of the item in this inventory slot.")]
    public Image Icon;

    [Tooltip("The image component that represents the outline of the inventory slot.")]
    public Image Outline;

    // The unique identifier for the item stored in this slot.
    [HideInInspector]
    public int ID;

    // The name of the item displayed in this slot.
    [HideInInspector]
    public string ItemName;

    [SerializeField]
    public Image Background;

    /// <summary>
    /// Sets the UI elements for the item in this inventory slot based on the given item.
    /// </summary>
    /// <param name="item">The item to display in the inventory slot.</param>
    public void SetItemUI(Item item)
    {
        // Set the icon sprite to the item's icon.
        Icon.sprite = item.Icon;

        // Set the ID of the item in the slot.
        ID = item.ID;

        // Set the name of the item in the slot.
        ItemName = item.name;
    }

    public void SetActive(bool b)
    {
        Outline.gameObject.SetActive(b);
        Icon.gameObject.SetActive(b);
        Background.gameObject.SetActive(b);
    }
}
