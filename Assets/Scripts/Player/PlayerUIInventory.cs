using System.Collections.Generic;
using UnityEngine;

public class PlayerUIInventory : MonoBehaviour
{
    private Animator anim;
    InteractionEventsHandler interactionEventsHandler;

    [SerializeField] private GameObject inventorySlotPrefab;

    private List<GameObject> icons;

    private void Awake()
    {
        icons = new List<GameObject>();

        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        interactionEventsHandler = FindFirstObjectByType<InteractionEventsHandler>();
        interactionEventsHandler.ItemPickedUp += OnItemPickedUp;
    }

    public void OnItemPickedUp(Interactable interactable)
    {
        if (icons.Count != 0)
        {
            foreach (GameObject icon in icons)
            {
                if (icon.GetComponent<InventorySlotUI>().ItemName == interactable.Name) return;
            }
        }

        int i = icons.Count;
        icons.Add(Instantiate(inventorySlotPrefab));
        icons[i].GetComponent<InventorySlotUI>().ItemName = interactable.Name;
        icons[i].GetComponent<InventorySlotUI>().Icon.sprite = interactable.Icon;
        icons[i].transform.SetParent(transform);
        anim.SetTrigger("In");
    }
}
