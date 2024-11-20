using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class CrosshairUI : MonoBehaviour
{
    private Animator crosshairAnim;

    private void Start()
    {
        try
        {
            crosshairAnim = GetComponentInChildren<Animator>();
        }
        catch
        {
            // This is most likely not gonna happen, the prefab should be set up correctly
            // If you got this error, add the correct animator in the child of this obj.

            Debug.LogError("Crosshair needs an animator!", this);
        }
    }

    public void Grow()
    {
        crosshairAnim.SetBool("Grow", true);
    }

    public void Shrink()
    {
        crosshairAnim.SetBool("Grow", false);
    }
}
