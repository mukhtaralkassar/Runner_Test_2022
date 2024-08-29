using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : EffectSound
{
    public AudioClip JumpClip;
    public AudioClip SkiingClip;
    public AudioClip MoveClip;
    public void JumpEffect()
    {
        AudioSource.Stop();
        AudioSource.clip = JumpClip;
        AudioSource.loop = false;
        AudioSource.Play();
    }
    public void SKiingEffect()
    {
        AudioSource.Stop();
        AudioSource.clip = SkiingClip;
        AudioSource.loop = false;
        AudioSource.Play();
    }
    public void MoveEffect()
    {
        AudioSource.Stop();
        AudioSource.clip = MoveClip;
        AudioSource.loop = true;
        AudioSource.Play();
    }
}
