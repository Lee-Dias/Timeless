using NUnit.Framework.Constraints;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableSlot : MonoBehaviour
{
    [SerializeField, Tooltip("Items from what era does this accept?(None means any era)")] private Interactable.ItemEra era;
    [SerializeField, Tooltip("What type of item does this accept?")] private Interactable.ItemType type;

    private PlayerInventory playerInventory;
    private PlayerInputs playerInputs;

    private Interactable selectedItem;

    private void Start()
    {
        //Gets Players Inventory and Inputs
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        playerInputs = FindFirstObjectByType<PlayerInputs>();
    }
    private void Update(){
    }
    public void Interact()
    {
        // Get the selected item in the Inventory
        selectedItem = playerInventory.GetSelected();
        
        // See if the selected item is the required 
        if(selectedItem != null){
            if (selectedItem.Type == type && (era == Interactable.ItemEra.None || selectedItem.Era == era)){
                Debug.Log("The item was placed");
            }
        }else{
            Debug.Log("Need another item");
        }
    }

}
