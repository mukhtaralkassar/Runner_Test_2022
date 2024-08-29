using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleEffect : EffectSound
{
    private int _DeathHash = Animator.StringToHash("Death");
    public Animation ObstacleAnimation;
    public AnimationClip AnimationHitClip;
    public Animator ObstacleAnimator;
    public RuntimeAnimatorController AnimationController;
    public void HitEffect()
    {
        if (ObstacleAnimation != null)
        {
            ObstacleAnimation.Stop();
            ObstacleAnimation.clip = AnimationHitClip;
            ObstacleAnimation.Play();
        }
        else if (ObstacleAnimator != null)
        {
            ObstacleAnimator.StopPlayback();
            ObstacleAnimator.runtimeAnimatorController = AnimationController;
            ObstacleAnimator.SetTrigger(_DeathHash);
            ObstacleAnimator.Play(_DeathHash);
        }
        HitSound();
    }
    private void OnDisable()
    {
        if (ObstacleAnimation != null)
            ObstacleAnimation.Stop();
    }
}
