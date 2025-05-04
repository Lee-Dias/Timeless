using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

/// <summary>
/// Manages a puzzle mechanism involving a set of buttons on a tablet.
/// The user must activate the correct combination of buttons to solve the puzzle.
/// Once the puzzle is solved, a crystal is spawned at a specified position.
/// </summary>
public class Tablet : MonoBehaviour
{
    public UnityEvent puzzleEnded;  // Action to trigger when the puzzle is completed.
    private TabletButton[] buttons;  // Array to hold all button references on the tablet.

    [SerializeField, Tooltip("Correct button indices for solving the puzzle.")]
    private int[] rightButtons;  // Correct button indices for solving the puzzle.

    private List<int> activeButtons;  // List of currently activated buttons.

    [SerializeField] private Interactable cristal;  // The crystal prefab to spawn when the puzzle is solved.

    [Header("Debugging")]
    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;

    private void Awake()
    {
        activeButtons = new List<int>();
    }

    /// <summary>
    /// Initializes button references and checks for necessary child components.
    /// </summary>
    private void Start()
    {
        buttons = GetComponentsInChildren<TabletButton>();
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("Tablet needs buttons as child objects.");
        }
        else
        {
            Log("Tablet buttons initialized successfully.");
        }
        cristal.CanInteract = false;
    }

    /// <summary>
    /// Activates button interaction, subscribing to button activation/deactivation events.
    /// </summary>
    private void OnEnable()
    {
        buttons = GetComponentsInChildren<TabletButton>();
        if (buttons == null || buttons.Length == 0)
        {
            Debug.LogError("Tablet needs buttons as child objects.");
            return;
        }

        foreach (TabletButton button in buttons)
        {
            button.activateButton += OnActivateButton;
            button.deactivateButton += OnDeActivateButton;
        }

        Log($"Tablet activated with {buttons.Length} buttons.");
    }

    /// <summary>
    /// Deactivates button interaction, unsubscribing from button events.
    /// </summary>
    private void OnDisable()
    {
        foreach (TabletButton button in buttons)
        {
            button.activateButton -= OnActivateButton;
            button.deactivateButton -= OnDeActivateButton;
        }

        Log("Tablet deactivated.");
    }

    /// <summary>
    /// Handles button activation. Adds the button to the list of active buttons
    /// and checks if the correct combination of buttons is pressed to solve the puzzle.
    /// </summary>
    private void OnActivateButton(int buttonIndex)
    {
        if (!activeButtons.Contains(buttonIndex))
        {
            activeButtons.Add(buttonIndex);
            Log($"Button {buttonIndex} activated. Active buttons: {string.Join(", ", activeButtons)}");
        }

        if (!rightButtons.Contains(buttonIndex))
        {
            Log($"Button {buttonIndex} is not part of the solution.");
            return;
        }

        // Check if all correct buttons have been activated.
        if (activeButtons.Count == rightButtons.Length &&
            rightButtons.All(activeButtons.Contains))
        {
            Log("All correct buttons activated. Puzzle solved!");
            EndPuzzle();
        }
    }

    /// <summary>
    /// Handles button deactivation. Removes the button from the list of active buttons.
    /// </summary>
    private void OnDeActivateButton(int buttonIndex)
    {
        if (activeButtons.Contains(buttonIndex))
        {
            activeButtons.Remove(buttonIndex);
            Log($"Button {buttonIndex} deactivated. Active buttons: {string.Join(", ", activeButtons)}");
        }

        // Check if all correct buttons have been activated.
        if (activeButtons.Count == rightButtons.Length &&
            rightButtons.All(activeButtons.Contains))
        {
            Log("All correct buttons activated. Puzzle solved!");
            EndPuzzle();
        }
    }

    /// <summary>
    /// Ends the puzzle, triggering the puzzle's end logic and spawning the crystal.
    /// </summary>
    private void EndPuzzle()
    {
        foreach (TabletButton button in buttons)
        {
            button.OnPuzzleEnd();
        }
        cristal.CanInteract = true;
        puzzleEnded?.Invoke();
        Log("Puzzle ended.");
    }

    /// <summary>
    /// Logs a debug message to the Console if debugging is enabled.
    /// Includes the object's name as an identifier if 'identifyObject' is true.
    /// </summary>
    /// <param name="message">The debug message to log.</param>
    private void Log(string message)
    {
        if (showDebugMessages)
        {
            if (identifyObject)
                Debug.Log(message, this);  // Logs message with object name.
            else
                Debug.Log(message);  // Logs message without object name.
        }
    }
}
