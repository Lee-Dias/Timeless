using System;
using UnityEditor.Purchasing;
using UnityEngine;

public class TabletButton : Interactable
{
    public Action<int> activateButton;
    public Action<int> deactivateButton;
    [SerializeField] private int buttonIndex;
    public bool IsActive { get; private set; } = false;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void OnEnable()
    {
        InteractEvent.AddListener(TFlipFlop);
    }
    private void OnDisable()
    {
        InteractEvent.RemoveListener(TFlipFlop);
    }

    public void TFlipFlop()
    {
        if (IsActive)
        {
            deactivateButton?.Invoke(buttonIndex);
        }
        else activateButton?.Invoke(buttonIndex);
        IsActive = !IsActive;
        anim.SetTrigger("Press");
    }

    public void OnPuzzleEnd()
    {
        SetCanInteract(false);
    }
}
