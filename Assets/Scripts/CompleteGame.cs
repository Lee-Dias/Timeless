using UnityEngine;
using UnityEngine.UI;  
using UnityEngine.SceneManagement;
using System.Collections; 

public class CompleteGame : MonoBehaviour
{
    [SerializeField]private string nextSceneName;  // Name of the next scene to load
    [SerializeField]private GameObject winTextUI;  // Reference to the "You Win" UI Text

    void Start()
    {
        // Ensure the "You Win" text is hidden at the start
        if (winTextUI != null)
            winTextUI.SetActive(false);
    }

    public void TriggerGameWin()
    {
        // Show the "You Win" message
        if (winTextUI != null)
            winTextUI.SetActive(true);

        // Stop the game logic (optional)
        Time.timeScale = 0f;

        // Start the coroutine to load the next scene after 5 seconds
        StartCoroutine(LoadNextSceneAfterDelay(5f));
    }

    IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        // Wait for the specified time (5 seconds)
        yield return new WaitForSecondsRealtime(delay);

        // Resume time scale in case you froze the game
        Time.timeScale = 1f;

        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
