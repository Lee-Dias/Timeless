using UnityEngine;
using UnityEngine.Events;

public class TFlipFlopEvent : MonoBehaviour
{
    public UnityEvent Flip;
    public UnityEvent Flop;

    private bool value = true;

    public void TFlipFlop()
    {
        value = !value;
        if (value) Flip?.Invoke();
        else Flop?.Invoke();
    }
}
