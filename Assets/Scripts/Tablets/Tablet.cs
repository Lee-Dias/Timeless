using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tablet : MonoBehaviour
{
    private Action puzzleDone;
    private TabletButton[] buttons;

    [SerializeField] private int[] rightButtons;
    [SerializeField] private List<int> activeButtons;

    [SerializeField] private GameObject cristalPrefab;
    [SerializeField] private Transform cristalSpawnPosition;

    private void Start()
    {
        buttons = GetComponentsInChildren<TabletButton>();
        if (buttons == null) Debug.LogError("Tablet need the buttons on child", this);
    }

    private void OnEnable()
    {
        buttons = GetComponentsInChildren<TabletButton>();
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("Tablet needs the buttons on child objects", this);
            return;
        }

        foreach (TabletButton button in buttons)
        {
            button.activateButton += OnActivateButton;
            button.deactivateButton += OnDeActivateButton;
        }
    }

    private void OnDinable()
    {
        foreach (TabletButton button in buttons)
        {
            button.activateButton -= OnActivateButton;
            button.deactivateButton -= OnDeActivateButton;
        }
    }


    private void OnActivateButton(int buttonIndex)
    {
        // Add the buttonIndex to activeButtons if it's not already in the list
        if (!activeButtons.Contains(buttonIndex))
            activeButtons.Add(buttonIndex);

        if (!rightButtons.Contains(buttonIndex))
            return;

        // Check if all rightButtons are activated
        if (activeButtons.Count == rightButtons.Length &&
            rightButtons.All(activeButtons.Contains))
        {
            EndPuzzle();
        }
    }

    private void EndPuzzle()
    {
        foreach (TabletButton button in buttons)
        {
            button.OnPuzzleEnd();
        }
        Debug.Log("Tablet puzzle ended!");
        Instantiate(cristalPrefab, cristalSpawnPosition.position, Quaternion.identity);
    }

    private void OnDeActivateButton(int buttonIndex)
    {
        if (activeButtons.Contains(buttonIndex)) activeButtons.Remove(buttonIndex);

        // Check if all rightButtons are activated
        if (activeButtons.Count == rightButtons.Length &&
            rightButtons.All(activeButtons.Contains))
        {
            EndPuzzle();
        }
    }
}