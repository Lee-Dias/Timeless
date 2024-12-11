using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagemening : Singleton<SceneManagemening>
{

    public void ChangeScene(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
