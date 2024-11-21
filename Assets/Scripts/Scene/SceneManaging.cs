using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagemening : Singleton<SceneManagemening>
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("PrototypeScene");
        }
    }
}