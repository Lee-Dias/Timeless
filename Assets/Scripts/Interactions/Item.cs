using UnityEngine;

[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    public int ID;
    public Sprite Icon;
    public string Name;
    public GameObject Prefab;
}
