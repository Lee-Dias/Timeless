using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image Icon;
    public Image Outline;
    [HideInInspector] public int ID;
    [HideInInspector] public string ItemName;

    public void SetItemUI(Item item)
    {
        Icon.sprite = item.Icon;
        ID = item.ID;
        ItemName = item.name;
    }
}