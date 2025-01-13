using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;
using System.Linq;

public class PianoPuzzle : MonoBehaviour
{

    public UnityEvent onCompleted;
    [SerializeField] private Animator safe;

    [Header("Debugging")]
    [SerializeField, Tooltip("Enable this to display debug messages from this script in the Console.")]
    private bool showDebugMessages = false;

    [SerializeField, Tooltip("If enabled, debug messages will include the object's name as an identifier."), ShowIf(nameof(showDebugMessages))]
    private bool identifyObject = true;
    public List<Key> rightKeysInOrder;

    private int rightKeysPressed = 0;
    private PianoKey[] pianoKeys;

    private void Awake()
    {
        pianoKeys = GetComponentsInChildren<PianoKey>();
        foreach (PianoKey pianoKey in pianoKeys)
        {
            Log($"Registering key: {pianoKey.ThisKey}");
            pianoKey.keyPressed.AddListener(OnKeyPressed);
        }
    }

    private void OnKeyPressed(Key key)
    {
        Log($"Key pressed: {key}");
        if (rightKeysInOrder[rightKeysPressed] == key)
        {
            rightKeysPressed++;
            Log($"Correct key! Progress: {rightKeysPressed}/{rightKeysInOrder.Count}");
        }
        else
        {
            Log($"Wrong key! Resetting progress.");
            rightKeysPressed = 0;
        }

        if (rightKeysPressed >= rightKeysInOrder.Count)
        {
            PuzzleEnded();
        }
    }

    private void PuzzleEnded()
    {
        Log($"Puzzle completed! All keys pressed in order.");

        foreach (PianoKey pianoKey in pianoKeys)
        {
            pianoKey.CanInteract = false;
        }
        onCompleted?.Invoke();
        if (safe != null)
        {
            safe.SetTrigger("Open");
            safe.GetComponent<AudioSource>()?.Play();
        }

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
            if (identifyObject) Debug.Log($"[{name}] {message}", this); // Includes object's name in the Console
            else Debug.Log(message); // Logs message without object identifier
        }
    }
}
