using System;
using Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TMPro;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class TutorialPopup : Popup
    {
        private const uint MinPage = 1;
        private const uint MaxPage = 4;
        [SerializeField] private TMP_Text pageText;
        [SerializeField] private TMP_Text descriptionText;
        [SerializeField] private CommonButton leftButton;
        [SerializeField] private CommonButton rightButton;
        [SerializeField] private CommonButton closeButton;
        [SerializeField] private Image descriptionImage;
        [SerializeField] private Image grayButton;
        [SerializeField] private Sprite[] sourceSprites;

        private readonly string[] descriptions =
        {
            "勝利条件：\n４人対戦で、天使を１番多く集めた人の勝利（制限時間は９０秒）。\n\n天使を集める方法には自動収集とスキル攻撃がある。",
            "基本操作：\n左手でプレイヤーを移動させ、右手でスキル選択・発動できる。",
            "自動収集：\n浄化光線が自動で悪魔に向けて発射される。\n\n悪魔に当たると天使に変わり、\nスキルゲージが溜まっていく。",
            "天使スキル：\n他のプレイヤーの天使にスキルを当てて横取りできる。\n\nただし、天使を５体消費するので使い所には気をつけよう。",
        };
        private readonly AsyncReactiveProperty<uint> _currentPage = new(MinPage);
        private bool isCloseButtonInteractable;

        private IObservable<uint> CurrentPageObservable =>
            _currentPage.ToObservable().Where(page => page is >= MinPage and <= MaxPage);

        public override void Setup()
        {
            base.Setup();
            BGMManager.Instance.Play(BGMPath.INGAME_BGM_TUTORIAL);
            leftButton.OnClick.TakeUntilDestroy(gameObject)
                .Where(_ => _currentPage.Value > MinPage)
                .Subscribe(_ => _currentPage.Value--);
            rightButton.OnClick.TakeUntilDestroy(gameObject)
                .Where(_ => _currentPage.Value < MaxPage)
                .Subscribe(_ => _currentPage.Value++);
            CurrentPageObservable.Subscribe(page =>
            {
                descriptionText.text = descriptions[page - 1];
                descriptionImage.sprite = sourceSprites[page - 1];
                leftButton.gameObject.SetActive(page != MinPage);
                rightButton.gameObject.SetActive(page != MaxPage);
                if (page == MaxPage)
                {
                    isCloseButtonInteractable = true;
                }
                closeButton.SetInteractable(isCloseButtonInteractable);
                grayButton.gameObject.SetActive(!isCloseButtonInteractable);
                pageText.text = $"{_currentPage.Value} / {MaxPage}";
            });
        }

        private void OnDestroy()
        {
            Preferences.SetTutorial(true);
        }
    }
}