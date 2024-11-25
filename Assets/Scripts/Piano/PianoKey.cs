using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class PianoKey : Interactable
{
    private Animator anim;

    public Key ThisKey;

    public UnityEvent<Key> keyPressed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        InteractEvent.AddListener(Press);
    }

    private void OnDisable()
    {
        InteractEvent.RemoveListener(Press);
    }

    private void Press()
    {
        anim.SetTrigger("Press");
        keyPressed?.Invoke(ThisKey);
    }
}