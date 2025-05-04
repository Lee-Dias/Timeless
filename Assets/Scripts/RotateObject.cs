using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class RotateObject : MonoBehaviour
{

    [Dropdown("axisValues")]
    [SerializeField]
    private string axisToRotate;

    [SerializeReference]
    private int maxRotation = 25; // Gets total degrees to rotate
    [SerializeField]
    private float rotationStep = 0.1f; // Degrees to rotate per step 
    [SerializeField]
    private float timeStep = 0.1f; // Time between each step in seconds
    [SerializeField]
    private float correctRotation = 180f; // Correct Rotation to have

    public bool hasCorrectRotation {get {return Mathf.Abs(correctRotation - transform.rotation.eulerAngles.z) < 1f;} private set {}}

    private List<string> axisValues { get { return new List<string>() { "X", "Y", "Z" }; } }

    public IEnumerator RotateOverTime()
    {
        float totalRotation = 0f; // Tracks the total rotation
        
        while (Mathf.Abs(totalRotation) < Mathf.Abs(maxRotation))
        {
            // Calculate the rotation step for this iteration
            float remainingRotation = maxRotation - totalRotation;
            float rotationThisStep = Mathf.Clamp(rotationStep, -Mathf.Abs(remainingRotation), Mathf.Abs(remainingRotation));

            // Apply the rotation
            if (axisToRotate == "X")
                transform.Rotate(rotationThisStep, 0f, 0f);
            else if(axisToRotate == "Y")
                transform.Rotate(0f, rotationThisStep, 0f);
            else if(axisToRotate == "Z")
                transform.Rotate(0f, 0f, rotationThisStep);
            
                

            // Update the total rotation
            totalRotation += rotationThisStep;

            // Wait for the specified time step
            yield return new WaitForSeconds(timeStep);
        }

        CheckRotations checkRotations = transform.parent.GetComponent<CheckRotations>();   
        if (checkRotations != null)
            checkRotations.CheckChilds();
    }
    // Example: Start the rotation when the game starts
    public void Rotate()
    {
        StartCoroutine(RotateOverTime());
    }
}
