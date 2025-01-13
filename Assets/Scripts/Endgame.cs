using UnityEngine;
using UnityEngine.SceneManagement; // Required to manage scenes
public class Endgame : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object has the "Player" tag
        if (other.gameObject.layer == 6)
        {
            // Load the Main Menu scene
            SceneManager.LoadScene("MainMenu");
        }
    }
}
