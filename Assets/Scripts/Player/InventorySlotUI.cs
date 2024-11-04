using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{
    public Image Icon;
    public Image Outline;
    [HideInInspector] public int ID;
    [HideInInspector] public string ItemName;
}