using UnityEngine;
using UnityEngine.SceneManagement;

public class Buttons : MonoBehaviour
{
    public void StartGame(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    public void ExitGame(string scene)
    {
        Application.Quit();
    }
    public void Settings(GameObject Settings){
        Settings.SetActive(!Settings.activeSelf);

    }
}
