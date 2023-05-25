using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ArrowAnimation : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    private const float AnimSpeed = 1f;
    
    private void Start()
    {
        DOTween.Sequence()
            .OnStart(() =>
            {
                canvasGroup.alpha = 1;
            })
            .Append(canvasGroup.DOFade(0.3f, AnimSpeed))
            .Append(canvasGroup.DOFade(1f, AnimSpeed))
            .AppendInterval(AnimSpeed/2)
            .SetLoops(-1);
    }
}
