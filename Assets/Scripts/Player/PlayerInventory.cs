using UnityEngine;

public class PlayerInventory : MonoBehaviour
{

    private void Start()
    {
        Interactable[] interactables = FindObjectsByType<Interactable>(FindObjectsSortMode.InstanceID);

        foreach (var interactable in interactables)
        {
            interactable.ItemPickedUp += OnItemPickedUp;
        }
    }

    public void OnItemPickedUp(Interactable interactable)
    {
        Debug.Log("Item picked up: " + interactable.name);
    }
}
