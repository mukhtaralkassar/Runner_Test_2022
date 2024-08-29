using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectSound : MonoBehaviour
{
    public AudioSource AudioSource;
    public AudioClip HitAudioClip;
    public void HitSound()
    {
        AudioSource.Stop();
        AudioSource.clip = HitAudioClip;
        AudioSource.loop = false;
        AudioSource.Play();
    }
}
