using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    InteractionEventsHandler interactionEventsHandler;
    public Dictionary<int, Interactable> Inventory { get; private set; }

    private void Awake()
    {
        Inventory = new Dictionary<int, Interactable>();
    }

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();
        interactionEventsHandler.ItemPickedUp += OnItemPickedUp;
    }

    public void OnItemPickedUp(Interactable interactable)
    {
        Debug.Log("Item picked up: " + interactable.name);
        if (!Inventory.ContainsValue(interactable))
        {
            Inventory.Add(Inventory.Count, interactable);
            interactable.gameObject.SetActive(false);
        }
    }
}
