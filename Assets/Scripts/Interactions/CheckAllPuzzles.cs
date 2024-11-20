using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Manages and validates a collection of items to ensure all required conditions are met.
/// </summary>
/// <remarks>
/// This class is designed to check if all child objects under a parent container meet specific criteria. 
/// It works by iterating through the child objects, verifying their states using attached components, 
/// and applying additional logic based on the results.
/// 
/// <para><b>Key Features:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Counts all child objects marked as "placable" within the parent container.</description>
/// </item>
/// <item>
/// <description>Checks if each child object satisfies a specific condition using the <c>InventoryItemMatcher</c> component.</description>
/// </item>
/// <item>
/// <description>Disables interaction for all child objects when all conditions are satisfied.</description>
/// </item>
/// </list>
/// 
/// <para><b>How It Works:</b></para>
/// The script iterates through all child objects of the GameObject it is attached to. 
/// For each child, it uses an <c>InventoryItemMatcher</c> to determine if the item is in the correct state. 
/// If all items meet their conditions, the script disables interaction for all child objects 
/// by setting the <c>CanInteract</c> property of their <c>Interactable</c> component to <c>false</c>.
/// 
/// <para><b>Dependencies:</b></para>
/// <list type="bullet">
/// <item>
/// <description>Requires child objects to have an <c>InventoryItemMatcher</c> component to check conditions.</description>
/// </item>
/// <item>
/// <description>Requires child objects to have an <c>Interactable</c> component to toggle interaction.</description>
/// </item>
/// </list>
/// 
/// </remarks>
public class MultipleItemChecker : MonoBehaviour
{
    private int totalPlacableItems;
    private int totalcorrect;

    public void CheckChildItems()
    {
        totalPlacableItems = 0;
        totalcorrect = 0;
        foreach (Transform child in transform)
        {
            totalPlacableItems += 1;
            InventoryItemMatcher inventoryItemMatcher = child.GetComponent<InventoryItemMatcher>();

            if (inventoryItemMatcher.HasRightItem() == true)
            {
                totalcorrect += 1;
            }
        }
        if (totalcorrect == totalPlacableItems)
        {

            foreach (Transform child in transform)
            {
                Interactable interactable = child.GetComponent<Interactable>();
                interactable.CanInteract = false;
            }
        }
    }
}
