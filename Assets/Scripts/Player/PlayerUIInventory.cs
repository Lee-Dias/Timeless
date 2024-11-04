using System.Collections.Generic;
using UnityEngine;

public class PlayerUIInventory : MonoBehaviour
{
    private Animator anim;
    [SerializeField] private GameObject inventorySlotPrefab;
    private List<GameObject> icons;

    private void Awake()
    {
        icons = new List<GameObject>();

        anim = GetComponent<Animator>();

        if (inventorySlotPrefab == null)
        {
            Debug.LogError("Inventory slot prefab is not assigned!");
            return;
        }
    }

    public void AddSlotToUI(Item item)
    {
        if (item == null)
        {
            Debug.LogError("Item is null!");
            return;
        }

        if (icons.Count != 0)
        {
            foreach (GameObject icon in icons)
            {
                if (icon.GetComponent<InventorySlotUI>().ID == item.ID) return;
            }
        }

        // This is stupid, if someone knows a better way, feel free to do it!
        int i = icons.Count;
        icons.Add(Instantiate(inventorySlotPrefab));
        icons[i].GetComponent<InventorySlotUI>().ItemName = item.Name;
        icons[i].GetComponent<InventorySlotUI>().Icon.sprite = item.Icon;
        icons[i].GetComponent<InventorySlotUI>().ID = item.ID;
        icons[i].transform.SetParent(transform);
        anim.SetTrigger("In");
    }
}