using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class CheckRotations : MonoBehaviour
{

    public UnityEvent Completedpuzzle;

    [SerializeField]
    private Animator anim;

    public void CheckChilds()
    {
        int totalItems = 0; 
        int totalCorrectItems = 0; 
        foreach (Transform child in transform)
        {
            totalItems += 1;
            RotateObject rotatedObject = child.GetComponent<RotateObject>();

            if(rotatedObject.hasCorrectRotation){
                totalCorrectItems += 1;
            }
        }
        if(totalCorrectItems == totalItems){
            foreach (Transform child in transform)
            {
                Interactable interactable = child.GetComponent<Interactable>();
                if (interactable != null)
                {
                    interactable.CanInteract = false;
                }
            }
            Completedpuzzle?.Invoke();
            if (anim != null)
                anim.SetTrigger("fall");
        }
       

    }
}
