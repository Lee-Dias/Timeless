using UnityEngine;

public class SarcophagusCristal : MonoBehaviour
{
    [SerializeField] private Animator anim;

    public void Hit()
    {
        anim.SetTrigger("Open");
    }
}
