using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private bool addToInv = false;
    private bool canInteract = true;
    public bool CanInteract => canInteract;

    public UnityEvent InteractEvent;

    public void SetCanInteract(bool value)
    {
        canInteract = value;
    }

    public void Interact()
    {
        if (canInteract)
        {
            InteractEvent?.Invoke();
            if (addToInv)
            {
                FindFirstObjectByType<PlayerInventory>()?.AddItemToInventory(item);
                gameObject.SetActive(false);
            }
        }
    }
}

