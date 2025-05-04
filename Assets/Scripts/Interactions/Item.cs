using UnityEngine;

/// <summary>
/// Represents an item in the game, encapsulating its properties, icon the UI and prefab for Inspections.
/// </summary>
/// <remarks>
/// <b>Key Features:</b>
/// <list type="bullet">
/// <item><description><c>ID</c>: A unique identifier for the item.</description></item>
/// <item><description><c>Icon</c>: The icon representing the item in the UI.</description></item>
/// <item><description><c>Name</c>: The name of the item displayed in the game.</description></item>
/// <item><description><c>Era</c>: The historical era to which the item belongs, using an enumeration for categorization.</description></item>
/// <item><description><c>Prefab</c>: A 3D representation of the item used for instantiating it in the game world (Inspections mainly).</description></item>
/// </list>
/// </remarks>
[CreateAssetMenu(fileName = "Item", menuName = "Scriptable Objects/Item")]
public class Item : ScriptableObject
{
    /// <summary>
    /// The different eras in which the item can belong. 
    /// This enum is used to categorize items based on their historical era.
    /// </summary>
    public enum Era { PreHistoric, WildWest, Medieval, Egypt , Present}

    /// <summary>
    /// <br>The unique identifier for the item.</br>
    /// <br>Make sure this ID is UNIQUE.</br>
    /// <br>Item matching are based on this.</br>
    /// </summary>
    public int ID;

    /// <summary>
    /// This icon will be used in the UI
    /// </summary>
    public Sprite Icon;

    /// <summary>
    /// The name of the item, for description purposes
    /// </summary>
    public string Name;

    /// <summary>
    /// The era to which the item belongs, used for categorization and matching purposes.
    /// </summary>
    public Era era;

    /// <summary>
    /// The prefab for inspection purposes.
    /// </summary>
    public GameObject Prefab;

    /// <summary>
    /// in case the item is being misspositioned when placed.
    /// </summary>
    public float heightWhenPlacedY = 0f;

    public int Puzzlenumber = 0;
}

