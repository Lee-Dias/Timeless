using System;
using System.Collections.Generic;
using UnityEditor.Purchasing;
using UnityEngine;

public class TabletButton : Interactable
{
    public Action<int> activateButton;
    public Action<int> deactivateButton;
    [SerializeField] private int buttonIndex;
    [SerializeField] private Color glowColor = Color.blue;
    public bool IsActive { get; private set; } = false;
    private Animator anim;
    private Light glowLight;

    private Material overlayMaterial;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        Renderer renderer = GetComponent<Renderer>();
        overlayMaterial = renderer.materials[1];

        glowLight = GetComponentInChildren<Light>(true);
        glowLight.color = glowColor;
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
            overlayMaterial.EnableKeyword("_EMISSION");
            overlayMaterial.SetColor("_EmissionColor", Color.black);
            glowLight.gameObject.SetActive(false);
        }
        else
        {
            activateButton?.Invoke(buttonIndex);
            overlayMaterial.EnableKeyword("_EMISSION");
            overlayMaterial.SetColor("_EmissionColor", glowColor * 2.0f);
            glowLight.gameObject.SetActive(true);
        }

        IsActive = !IsActive;
        anim.SetTrigger("Press");
    }


    public void OnPuzzleEnd()
    {
        SetCanInteract(false);
    }
}
