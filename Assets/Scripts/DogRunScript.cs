using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogRunScript : MonoBehaviour
{
    public Animator DogAnimator;
    public Rigidbody DogRigidbody;
    public float Speed = 10;
    public float DistansteBetweenPlayerAndDog = 20;
    private int _DogRun = Animator.StringToHash("Dog_Run");
    private int _DogIdle = Animator.StringToHash("Dog_Idle");
    private bool IsEnableAnimation = false;

    public RuntimeAnimatorController RunAnimationController;
    public AudioClip RunAudioClip;
    public AudioSource AudioSource;
    public float CurrentSpeed
    {
        get
        {
            if (PlayerController.Instance)
                return (1 + PlayerController.Instance.SpeedRatio) * Speed;
            return Speed;
        }
    }
    private bool _IsPlayerRun
    {
        get
        {
            if (PlayerController.Instance)
                return PlayerController.Instance.IsStartRun;
            return false;
        }
    }
    private bool _IsPlayerClose
    {
        get
        {
            if (PlayerController.Instance != null)
            {
                if (!IsEnableAnimation)
                {
                    IsEnableAnimation = true;
                    DogAnimator.SetTrigger(_DogRun);
                }
                return Mathf.Abs(PlayerController.Instance.transform.position.z - transform.position.z) <= DistansteBetweenPlayerAndDog && PlayerController.Instance.IsStartRun;
            }
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (_IsPlayerClose && _IsPlayerRun)
        {
            DogRigidbody.velocity = transform.forward * CurrentSpeed;
        }
    }
    private void OnEnable()
    {
        IsEnableAnimation = false;
        AudioSource.Stop();
        AudioSource.clip = RunAudioClip;
        AudioSource.loop = true;
        AudioSource.Play();
        DogAnimator.runtimeAnimatorController = RunAnimationController;
        DogAnimator.SetTrigger(_DogIdle);
    }
}
