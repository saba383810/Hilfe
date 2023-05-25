using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.InGame.UI
{
    public enum NumberEffectType
    {
        Three  = 0,
        Two    = 1,
        One    = 2,
        Start  = 3,
        Finish = 4,
        Amount30 = 5,
    }

    public class NumberEffectView : MonoBehaviour
    {
        [Header("123")]
        [SerializeField] private Image image;
        [SerializeField] private CanvasGroup canvasGroup;
        
        [Header("Start")]
        [SerializeField] private Transform startTransform;
        [SerializeField] private CanvasGroup startCanvasGroup;
        
        [Header("Finish")]
        [SerializeField] private Transform finishTransform;
        [SerializeField] private CanvasGroup finishCanvasGroup;

        [Header("Amount30")] 
        [SerializeField] private Transform amount30Transform;
        [SerializeField] private CanvasGroup amount30CanvasGroup;
        

        
        [SerializeField] private Sprite[] sprites;
        private const float AnimSpeed = 0.3f;
        
        public void Show(NumberEffectType numberEffectType)
        {
            if (numberEffectType != NumberEffectType.Finish && numberEffectType != NumberEffectType.Start && numberEffectType != NumberEffectType.Amount30)
            {
                var spriteIndex = (int)numberEffectType;
                image.sprite = sprites[spriteIndex];
                ShowAnimation();
            }
            
            if(numberEffectType == NumberEffectType.Start) Show(startCanvasGroup, startTransform);
            if(numberEffectType == NumberEffectType.Finish) Show(finishCanvasGroup,finishTransform);
            if(numberEffectType == NumberEffectType.Amount30) Show(amount30CanvasGroup,amount30Transform);
            
        }

        private void ShowAnimation()
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    canvasGroup.alpha = 0;
                    image.transform.localScale = Vector3.zero;
                    gameObject.SetActive(true);
                })
                .Append(canvasGroup.DOFade(1, AnimSpeed))
                .Join(image.transform.DOScale(Vector3.one, AnimSpeed))
                .AppendInterval(AnimSpeed)
                .Append(canvasGroup.DOFade(0, AnimSpeed))
                .OnComplete(()=> image.gameObject.SetActive(false));
        }

        private void Show(CanvasGroup canvasGroup,Transform targetTransform)
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    canvasGroup.alpha = 0;
                    targetTransform.localScale = Vector3.zero;
                    targetTransform.gameObject.SetActive(true);
                })
                .Append(canvasGroup.DOFade(1, AnimSpeed))
                .Join(targetTransform.DOScale(Vector3.one, AnimSpeed))
                .AppendInterval(1)
                .Append(canvasGroup.DOFade(0, AnimSpeed))
                .OnComplete(()=> targetTransform.gameObject.SetActive(false));
        }
    }
}