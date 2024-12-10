using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private SoundCollection footsStepSounds;
    [SerializeField] private Transform cameraBoober;
    private Vector3 previousCameraBobberPos;
    private bool hasPlayedFootStepSound = false;

    private void Awake()
    {
        GetComponent<AudioSource>().playOnAwake = false;
    }

    private void Update()
    {
        if (previousCameraBobberPos.y > cameraBoober.localPosition.y
            && !hasPlayedFootStepSound)
        {
            PlayFootStepSound();
            hasPlayedFootStepSound = true;
        }

        if (previousCameraBobberPos.y < cameraBoober.localPosition.y)
            hasPlayedFootStepSound = false;

        previousCameraBobberPos = cameraBoober.localPosition;
    }

    public void PlayFootStepSound()
    {
        footsStepSounds.Play(true);
    }
}
