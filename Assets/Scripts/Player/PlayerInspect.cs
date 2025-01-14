using UnityEngine;

public class PlayerInspect : MonoBehaviour
{
    private PlayerInputs playerInputs; // Reference to the PlayerInputs script to get player input.
    private PlayerInventory playerInventory; // Reference to the PlayerInventory script to get player inventory.
    private Inspectable inspectable; // Reference to the Inspectable script to get inspectable.
    private InspectionsHandler inspectionsHandler; // Reference to the InspectionsHandler script to get inspectionsHandler.

    private bool canInspect = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Find the PlayerInputs component.
        playerInputs = FindFirstObjectByType<PlayerInputs>();
        // Find the PlayerInventory component.
        playerInventory = FindFirstObjectByType<PlayerInventory>();
        // Find the inspectable component.
        inspectable = FindFirstObjectByType<Inspectable>();
        // Find the inspectionsHandler component.
        inspectionsHandler = FindFirstObjectByType<InspectionsHandler>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!canInspect) return;

        //check if player click inspect button and goes trhough if he isnt inspecting anything
        if (playerInputs.InspectButtonDown && !inspectionsHandler.inspecting)
        {
            Inspect();
        }
    }
    //if there is a item selected and if there is then stratsInspection and notifies it is inspecting from the inventory
    public void Inspect()
    {
        Item item = playerInventory.GetSelectedItem() ?? null;
        if (item != null)
        {
            inspectable.inspectingFromInv = true;
            inspectable.StartInspection(item);
        }
    }

    public void SetCanInspect(bool value)
    {
        canInspect = value;
    }
}
