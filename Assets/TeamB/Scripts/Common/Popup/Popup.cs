using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

namespace Common
{
    public enum ShowPopupAnimationType
    {
        ScaleIn,
        SlideIn,
        None
    }

    public enum HidePopupAnimationType
    {
        ScaleOut,
        SlideOut,
        None
    }

    public class Popup : MonoBehaviour
    {
        public Subject<Unit> OnCloseButtonClicked { get; } = new();
        public CanvasGroup canvasGroup;
        public Transform window;
        public const float AnimSpeed = 0.2f;

        [SerializeField, Header("Exit Button")]
        private CommonButton[] closeButtons;

        public virtual void Setup()
        {
            // buttonイベントの登録
            foreach (var button in closeButtons)
                button.OnClickOnce.TakeUntilDestroy(gameObject)
                    .Subscribe(_ => Hide());
        }

        public virtual void Show(ShowPopupAnimationType animationType = ShowPopupAnimationType.ScaleIn)
        {
            SEManager.Instance.Play(SEPath.OUTGAME_POPUP);
            switch (animationType)
            {
                case ShowPopupAnimationType.ScaleIn:
                    ScaleIn();
                    break;
                case ShowPopupAnimationType.SlideIn:
                    SlideIn();
                    break;
                default:
                    NoneIn();
                    break;
            }
            if (animationType == ShowPopupAnimationType.ScaleIn)
            {
                ScaleIn();
            }
            else
            {
                NoneIn();
            }
        }

        public virtual void Hide(HidePopupAnimationType animationType = HidePopupAnimationType.ScaleOut)
        {
            if (animationType==HidePopupAnimationType.ScaleOut)
            {
                OnCloseButtonClicked.OnNext(Unit.Default);
                ScaleOut();
            }
            else
            {
                SlideOut();
            }
        }

        private void ScaleIn()
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    canvasGroup.alpha = 0;
                    window.localScale = Vector3.zero;
                })
                .Append(canvasGroup.DOFade(1, AnimSpeed))
                .Join(window.DOScale(Vector3.one, AnimSpeed));
        }

        private void ScaleOut()
        {
            DOTween.Sequence()
                .Append(canvasGroup.DOFade(0, AnimSpeed))
                .Join(window.DOScale(Vector3.zero, AnimSpeed))
                .OnComplete(() =>
                {
                    var myObj = gameObject;
                    Destroy(myObj);
                });
        }

        private void NoneIn()
        {
            DOTween.Sequence()
                .OnStart(() =>
                {
                    canvasGroup.alpha = 0;
                    window.gameObject.SetActive(true);
                })
                .Append(canvasGroup.DOFade(1, AnimSpeed));
        }

        private void SlideIn()
        {
            window.DOLocalMoveY(850, 1f).SetEase(Ease.OutQuart);
        }

        private void SlideOut()
        {
            window.DOLocalMoveY(1350, 1f).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                var myObj = gameObject;
                Destroy(myObj);
            });
        }
    }
}