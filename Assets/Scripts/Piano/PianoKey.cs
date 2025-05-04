using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator), typeof(AudioSource))]
public class PianoKey : Interactable
{
    // AudioClips for functional keys
    [SerializeField] private AudioClip cNote;
    [SerializeField] private AudioClip csNote; // C#
    [SerializeField] private AudioClip dNote;
    [SerializeField] private AudioClip dsNote; // D#
    [SerializeField] private AudioClip eNote;
    [SerializeField] private AudioClip fNote;
    [SerializeField] private AudioClip fsNote; // F#
    [SerializeField] private AudioClip gNote;
    [SerializeField] private AudioClip gsNote; // G#
    [SerializeField] private AudioClip aNote;
    [SerializeField] private AudioClip asNote; // A#
    [SerializeField] private AudioClip bNote;

    [SerializeField] private List<AudioClip> nonFunctionalWhiteList;
    [SerializeField] private List<AudioClip> nonFunctionalBlackList;

    private Animator anim;
    private AudioSource audioSource;

    public Key ThisKey;

    public UnityEvent<Key> keyPressed;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        InteractEvent.AddListener(Press);
        InteractEvent.AddListener(PlayKeySound);
    }

    private void OnDisable()
    {
        InteractEvent.RemoveListener(Press);
        InteractEvent.RemoveListener(PlayKeySound);
    }

    private void Press()
    {
        anim.SetTrigger("Press");
        keyPressed?.Invoke(ThisKey);
    }
    private AudioClip GetRandomClipFromList(List<AudioClip> list)
    {
        if (list == null) return null;
        return list[Random.Range(0, list.Count)];
    }

    private void PlayKeySound()
    {
        switch (ThisKey)
        {
            case Key.C:
                PlaySound(cNote);
                break;
            case Key.Cs:
                PlaySound(csNote);
                break;
            case Key.D:
                PlaySound(dNote);
                break;
            case Key.Ds:
                PlaySound(dsNote);
                break;
            case Key.E:
                PlaySound(eNote);
                break;
            case Key.F:
                PlaySound(fNote);
                break;
            case Key.Fs:
                PlaySound(fsNote);
                break;
            case Key.G:
                PlaySound(gNote);
                break;
            case Key.Gs:
                PlaySound(gsNote);
                break;
            case Key.A:
                PlaySound(aNote);
                break;
            case Key.As:
                PlaySound(asNote);
                break;
            case Key.B:
                PlaySound(bNote);
                break;
            case Key.NonFunctionalWhite:
                PlaySound(GetRandomClipFromList(nonFunctionalWhiteList));
                break;
            case Key.NonFunctionalBlack:
                PlaySound(GetRandomClipFromList(nonFunctionalBlackList));
                break;
            default:
                PlaySound(GetRandomClipFromList(nonFunctionalWhiteList));
                Debug.LogWarning($"No sound mapped for key: {ThisKey}");
                break;
        }
    }
    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioClip is null and cannot be played.");
        }
    }
}