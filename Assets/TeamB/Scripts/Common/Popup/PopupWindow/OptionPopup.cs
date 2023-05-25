using Common;
using DG.Tweening;
using Fusion.StatsInternal;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TeamB.Scripts.OutGame;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class OptionPopup : Popup
    {
        [SerializeField] private CommonButton prologueButton;
        [SerializeField] private CommonButton tutorialButton;
        [SerializeField] private Slider bgmSlider;
        [SerializeField] private Slider seSlider;
        [SerializeField] private Image seHandle;
        [SerializeField] private TMP_Text prologueText;
        [SerializeField] private TMP_Text tutorialText;
        [SerializeField] private RectTransform backGroundObj;
        [SerializeField] private RectTransform bodyObj;

        public override void Setup()
        {
            base.Setup();
            bgmSlider.value = Preferences.GetBgmBaseVolume();
            seSlider.value = Preferences.GetSeBaseVolume();
            var mul = backGroundObj.rect.size.x / bodyObj.rect.size.x;
            bodyObj.localScale = new Vector3(mul, mul, mul);

            prologueButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(async _ =>
                {
                    var popup = await PopupManager.ShowADVPopupAsync(ADVDataList.GetList());
                    popup.OnDestroyAsObservable().TakeUntilDestroy(gameObject).Subscribe(_ =>
                    {
                        BGMManager.Instance.Play(BGMPath.HOME_BGM);
                    });
                });

            tutorialButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(async _ =>
                {
                    var popup = await PopupManager.ShowPopupAsync(PopupKey.TUTORIAL_POPUP);
                    popup.OnDestroyAsObservable().TakeUntilDestroy(gameObject).Subscribe(_ =>
                    {
                        BGMManager.Instance.Play(BGMPath.HOME_BGM);
                    });
                });
            
            bgmSlider.onValueChanged.AddListener(volume =>
            {
                BGMManager.Instance.ChangeBaseVolume(volume);
                Preferences.SetBgmBaseVolume(volume);
            });
            
            seSlider.onValueChanged.AddListener(volume =>
            {
                var textSize = tutorialText.fontSize;
                Debug.Log(textSize);
                SEManager.Instance.ChangeBaseVolume(volume);
                Preferences.SetSeBaseVolume(volume);
            });

            seHandle.OnPointerUpAsObservable().TakeUntilDestroy(gameObject)
                .Subscribe(_ => SEManager.Instance.Play(SEPath.INGAME_SELECT));
        }
    }
}