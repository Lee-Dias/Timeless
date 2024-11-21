using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private SoundCollection footsStepSounds;

    private void Start()
    {
        GetComponent<AudioSource>().playOnAwake = false;
    }
    public void PlayFootStepSound()
    {
        footsStepSounds.Play(true);
    }
}
