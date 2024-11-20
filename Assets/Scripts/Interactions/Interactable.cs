using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool addToInv = false;
    [SerializeField, ShowIf(nameof(addToInv))] private Item item;
    [SerializeField] private bool interactOne = false;

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
            if (interactOne) canInteract = false;
        }
    }
}

