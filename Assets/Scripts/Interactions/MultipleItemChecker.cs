using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

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
    [Header("Debugging")]
    public UnityEvent Completedpuzzle;

    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    private int totalPlacableItems;
    private int totalcorrect;

    public void CheckChildItems()
    {
        totalPlacableItems = 0;
        totalcorrect = 0;

        Log("Starting the item check...");

        foreach (Transform child in transform)
        {
           
            InventoryItemMatcher inventoryItemMatcher = child.GetComponent<InventoryItemMatcher>();

            if (inventoryItemMatcher != null)
            {
                totalPlacableItems += 1;
                if (inventoryItemMatcher.HasRightItem())
                {
                    totalcorrect += 1;
                    Log($"Child {child.name} is correct.");
                }
                else
                {
                    Log($"Child {child.name} is incorrect.");
                }
            }
            else
            {
                Debug.Log($"Child {child.name} does not have an InventoryItemMatcher.");
            }
        }
        Log($"Total correct items: {totalcorrect}");

        if (totalcorrect == totalPlacableItems)
        {
            Log("All items are correct. Disabling interactions...");
            foreach (Transform child in transform)
            {
                Interactable interactable = child.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.CanInteract = false;
                    Log($"Interaction disabled for {child.name}.");
                }
                else
                {
                    Log($"Child {child.name} does not have an Interactable component.");
                }
            }
            Completedpuzzle?.Invoke();
        }
        else
        {
            Log("Not all items are correct. Interaction remains enabled.");
        }
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
