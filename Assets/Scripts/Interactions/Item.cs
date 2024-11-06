using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public enum Era { PreHistoric, WildWest, Medieval, Egypt};

    public int ID;
    public Sprite Icon;
    public string Name;
    public Era era;
    public GameObject Prefab;
}
