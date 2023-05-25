using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadingSceneController : MonoBehaviour
{
    [SerializeField] private Transform[] transforms = new Transform[3];
    [SerializeField] private TMP_Text text;
    [SerializeField] private CanvasGroup canvasGroup;
    private CancellationTokenSource _cts;
    private Sequence _enemySequence;
    private Sequence _bubbleSequence;
    const float AnimSpeed = 0.3f;

    private void Awake()
    {
        canvasGroup.alpha = 0;
    }
    
    public void Show()
    {
        _cts = new CancellationTokenSource();
        canvasGroup.DOFade(1, 1);
        LoadingTextAnimation(_cts.Token).Forget();
        EnemyAnimation().Forget();
    }

    public void Hide()
    {
        canvasGroup.DOFade(0, 1);
    }

    public async UniTask LoadingTextAnimation(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            text.text = "Now Loading";
            for (var i = 0; i < 4; i++)
            {
                await UniTask.Delay(TimeSpan.FromSeconds(0.3f), cancellationToken: token);
                text.text +=".";
                if(token.IsCancellationRequested) return;
            }
        }
    }

    public async UniTask EnemyAnimation()
    {
        const int upMax = 30;
       
        var enemy1Pos = transforms[0].localPosition;
        var enemy2Pos = transforms[1].localPosition;
        var enemy3Pos = transforms[2].localPosition;
        _enemySequence = DOTween.Sequence()
            .OnStart(() =>
            {
                transforms[0].localPosition = enemy1Pos;
                transforms[1].localPosition = enemy2Pos;
                transforms[2].localPosition = enemy3Pos;
            })
            .Append(transforms[0].DOLocalMoveY(enemy1Pos.y + upMax, AnimSpeed))
            .Join(transforms[0].DOScaleX(0.8f,AnimSpeed))
            .Append(transforms[0].DOLocalMoveY(enemy1Pos.y, AnimSpeed))
            .Join(transforms[0].DOScaleX(1f,AnimSpeed))
            .Join(transforms[1].DOLocalMoveY(enemy2Pos.y + upMax, AnimSpeed))
            .Join(transforms[1].DOScaleX(0.8f,AnimSpeed))
            .Append(transforms[1].DOLocalMoveY(enemy2Pos.y, AnimSpeed))
            .Join(transforms[1].DOScaleX(1f,AnimSpeed))
            .Join(transforms[2].DOLocalMoveY(enemy3Pos.y + upMax, AnimSpeed))
            .Join(transforms[2].DOScaleX(0.8f,AnimSpeed))
            .Append(transforms[2].DOLocalMoveY(enemy3Pos.y, AnimSpeed))
            .Join(transforms[2].DOScaleX(1f,AnimSpeed))
            .SetLoops(-1);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _enemySequence.Kill();
        _bubbleSequence.Kill();
    }
}
