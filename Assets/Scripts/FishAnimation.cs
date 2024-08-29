using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishAnimation : MonoBehaviour
{
    private Sequence _Seq;
    public float Duration = 0.5f;
    public float WaitForNextStep = 0.25f;
    public void Show()
    {
        transform.eulerAngles = Vector3.zero;
        StartAnimation();
    }
    private void Hide()
    {
        StopAnimatiom();
    }
    private void StartAnimation()
    {
        Sequence _Seq = DOTween.Sequence();
        _Seq.Append(transform.DORotate(new Vector3(0, 360, 0), Duration, RotateMode.FastBeyond360))
            .AppendInterval(WaitForNextStep).SetRelative(true).SetEase(Ease.Linear).SetLoops(-1);
    }
    private void StopAnimatiom()
    {
        if (_Seq != null)
        {
            _Seq.Pause();
            _Seq.Kill();
        }
    }
    private void OnEnable()
    {
        Show();
    }
    private void OnDisable()
    {
        Hide();
    }
}
