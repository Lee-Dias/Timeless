using UnityEngine;

[RequireComponent(typeof(Animator))]
public class FlipFlipBoolAnimationPlayer : MonoBehaviour
{
    private bool value = true;
    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    public void PlayAnimation(string animation)
    {
        value = !anim.GetBool(animation);
        anim.SetBool(animation, value);
    }
}
