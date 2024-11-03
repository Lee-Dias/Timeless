using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    InteractionEventsHandler interactionEventsHandler;
    public Dictionary<string, Interactable> Inventory { get; private set; }

    private Interactable selectedItem;

    private void Awake()
    {
        Inventory = new Dictionary<string, Interactable>();
    }

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();
        interactionEventsHandler.ItemPickedUp += OnItemPickedUp;
    }

    public void OnItemPickedUp(Interactable interactable)
    {
        if (!Inventory.ContainsKey(interactable.Name))
        {
            Debug.Log("Item picked up: " + interactable.Name);
            Inventory.Add(interactable.Name, interactable);
            interactable.gameObject.SetActive(false);
        }
    }

    public Interactable GetSelected(){
        return selectedItem;
        
    }
}
