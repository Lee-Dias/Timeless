using UnityEngine;

public class FlipFlipBoolAnimationPlayer : MonoBehaviour
{
    public bool value = true;
    private Animator anim;

    private void Awake()
    {
        anim = FindFirstObjectByType<Animator>();    
    }

    public void PlayAnimation(string animation)
    {
        anim.SetBool(animation, value);
        value = !value;
    }
}
