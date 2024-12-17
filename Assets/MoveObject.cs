using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System.Collections.Generic;

public class MoveObject : MonoBehaviour
{
    [Dropdown("axisValues")]
    [SerializeField]
    private string axisToMove;

    [SerializeReference]
    private float maxDistance = 5f; // Gets total distance to move
    [SerializeField]
    private float moveStep = 0.1f; // Distance to move per step
    [SerializeField]
    private float timeStep = 0.1f; // Time between each step in seconds
    [SerializeField]
    private float correctPosition = 10f; // Correct Position to have

    public bool hasCorrectPosition { get { return Mathf.Abs(correctPosition - transform.position.x) < 1f; } private set { } }

    private List<string> axisValues { get { return new List<string>() { "X", "Y", "Z" }; } }

    public IEnumerator MoveOverTime()
    {
        float totalDistance = 0f; // Tracks the total distance moved

        while (totalDistance < maxDistance)
        {
            // Calculate the movement step for this iteration
            float remainingDistance = maxDistance - totalDistance;
            float moveThisStep = Mathf.Clamp(moveStep, -Mathf.Abs(remainingDistance), Mathf.Abs(remainingDistance));

            // Apply the movement
            if (axisToMove == "X")
                transform.Translate(moveThisStep, 0f, 0f);
            else if (axisToMove == "Y")
                transform.Translate(0f, moveThisStep, 0f);
            else if (axisToMove == "Z")
                transform.Translate(0f, 0f, moveThisStep);

            // Update the total distance moved
            totalDistance += (moveThisStep * 100);

            // Wait for the specified time step
            yield return new WaitForSeconds(timeStep);
        }
    }

    // Example: Start the movement when the game starts
    public void Move()
    {
        StartCoroutine(MoveOverTime());
    }
}
