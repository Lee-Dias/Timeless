using UnityEngine;

[CreateAssetMenu(fileName = "PlayerPrefs", menuName = "Scriptable Objects/PlayerPrefs")]
public class PlayerPrefs : ScriptableObject
{
    public float mainVolume = 1;
    public float effectsVolume = 1;
    public float musicVolume = 1;

    public bool invertZoom;
    public bool invertMouseY;

    public float sense = 1;

    public void SetMainVolume(float v)
    {
        mainVolume = v;
    }

    public void SetSense(float v)
    {
        sense = v;
    }
}
