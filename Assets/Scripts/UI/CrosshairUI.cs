using UnityEngine;

/// <summary>
/// Controls the behavior of the crosshair UI element, specifically its growth and shrinkage
/// through animation triggers. This class assumes that the crosshair UI is set up correctly 
/// with an Animator attached to a child object of the GameObject this script is attached to.
/// </summary>
public class CrosshairUI : MonoBehaviour
{
    private Animator crosshairAnim;  // Reference to the Animator component controlling the crosshair animation.

    /// <summary>
    /// Initializes the crosshair animator. If the animator is not found, an error message is logged.
    /// </summary>
    private void Start()
    {
        // Try to get the Animator component from the children of this object
        crosshairAnim = GetComponentInChildren<Animator>();

        // This is most likely not gonna happen, the prefab should be set up correctly.
        // If you got this error, add the correct animator in the child of this obj.
        if (crosshairAnim == null)
        {
            Debug.LogError("Crosshair requires an Animator component in its children. Please check your setup.", this);
        }
    }

    /// <summary>
    /// Triggers the animation to make the crosshair grow.
    /// </summary>
    public void Grow()
    {
        crosshairAnim.SetBool("Grow", true);
    }

    /// <summary>
    /// Triggers the animation to make the crosshair shrink.
    /// </summary>
    public void Shrink()
    {
        crosshairAnim.SetBool("Grow", false);
    }
}
