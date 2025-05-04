using UnityEngine;

public class SarcophagusCristal : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void Hit()
    {
        if (anim != null) anim.SetTrigger("Open");
    }
}
