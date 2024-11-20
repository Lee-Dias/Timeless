using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    [SerializeField] private bool addToInv = false;
    [SerializeField, ShowIf(nameof(addToInv))] private Item item;
    [SerializeField] private bool interactOne = false;

    public bool CanInteract = true;

    public UnityEvent InteractEvent;


    public void Interact()
    {
        if (CanInteract)
        {
            InteractEvent?.Invoke();
            if (addToInv)
            {
                FindFirstObjectByType<PlayerInventory>()?.AddItemToInventory(item);
                gameObject.SetActive(false);
            }
            if (interactOne) CanInteract = false;
        }
    }
}

