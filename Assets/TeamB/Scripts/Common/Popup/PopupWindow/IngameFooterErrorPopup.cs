using Common;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class IngameFooterErrorPopup : Popup
    {
        [SerializeField] private TMP_Text text;

        public void Setup(string val)
        {
            text.text = val;
        }
        public override void Show(ShowPopupAnimationType animationType = ShowPopupAnimationType.ScaleIn)
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    canvasGroup.alpha = 0;
                    gameObject.SetActive(true);
                })
                .Append(canvasGroup.DOFade(1, 0.5f))
                .AppendInterval(2)
                .Append(canvasGroup.DOFade(0, 0.5f))
                .OnComplete(() =>
                {
                    Destroy(gameObject);
                });
        }
    }
}
