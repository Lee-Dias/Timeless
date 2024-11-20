using UnityEngine;
using UnityEngine.Events;

public class Interactable : MonoBehaviour
{
    public UnityEvent InteractEvent;

    public void Interact()
    {
        InteractEvent?.Invoke();
    }
}
