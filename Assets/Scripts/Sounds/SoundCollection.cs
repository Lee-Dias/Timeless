using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public struct SoundCollection
{
    public AudioSource AudioSource;
    public List<AudioClip> Sounds;

    public void Play(bool randomPitch = false, float min = .8f, float max = 1.2f)
    {
        if (randomPitch) AudioSource.pitch = UnityEngine.Random.Range(min, max);
        else AudioSource.pitch = 1;

        int randomIndex = UnityEngine.Random.Range(0, Sounds.Count);
        AudioSource.clip = Sounds[randomIndex];
        AudioSource.Play();
    }
}
