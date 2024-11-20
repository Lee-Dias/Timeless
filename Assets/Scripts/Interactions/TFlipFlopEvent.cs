using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class that alternates between two Unity events (Flip and Flop) based on a boolean value.
/// </summary>
/// <remarks>
/// <b>Key Features:</b>
/// <list type="bullet">
/// <item><description><c>Flip</c>: Event triggered when the value is <c>true</c>.</description></item>
/// <item><description><c>Flop</c>: Event triggered when the value is <c>false</c>.</description></item>
/// <item><description><c>TFlipFlop()</c>: Method that toggles the boolean value and invokes the appropriate event.</description></item>
/// </list>
/// </remarks>
public class FlipFlopEvent : MonoBehaviour
{
    //Unity event triggered when the boolean value is true.
    public UnityEvent Flip;

    // Unity event triggered when the boolean value is false.
    public UnityEvent Flop;

    // The current boolean value that determines which event will be triggered.
    private bool isFlipped = true;

    /// <summary>
    /// Toggles the boolean value and invokes the corresponding event based on its state.
    /// If <c>isFlipped</c> is <c>true</c>, the <c>Flip</c> event is invoked.
    /// If <c>isFlipped</c> is <c>false</c>, the <c>Flop</c> event is invoked.
    /// </summary>
    public void TFlipFlop()
    {
        // Toggle the boolean value.
        isFlipped = !isFlipped;

        // Invoke the appropriate event based on the current state of isFlipped.
        if (isFlipped)
        {
            Flip.Invoke();
        }
        else
        {
            Flop.Invoke();
        }
    }
}
