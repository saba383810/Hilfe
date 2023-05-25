using System;
using System.Collections.Generic;
using Common;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TeamB.Scripts.Common;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace TeamB.Scripts.OutGame
{
    public class ADVController : MonoBehaviour
    {
        [SerializeField] private CommonButton advButton;
        [SerializeField] private TMP_Text[] advTexts;
        [SerializeField] private Image blackImage;
        [SerializeField] private Image blackImagePopupUI;
        [SerializeField] private ADVPopup advPopup;

        private void Awake()
        {
            Color blackImageColor = blackImage.color;
            blackImageColor.a = 1f;
            blackImage.color = blackImageColor;
            foreach (var advText in advTexts)
            {
                Color advTextColor = advText.color;
                advTextColor.a = 0f;
                advText.color = advTextColor;
            }

            blackImagePopupUI.gameObject.SetActive(false);
            advPopup.gameObject.SetActive(false);
        }

        private void Start()
        {
            SignUp();
        }

        private async void SignUp()
        {
            var popup = await PopupManager.ShowPopupAsync(PopupKey.USER_NAME_DECISION_POPUP);
            popup.OnDestroyAsObservable().TakeUntilDestroy(gameObject).Subscribe(_ => AdvVideo());
        }

        private void AdvVideo()
        {
            Color blackImageColor = blackImage.color;
            advButton.SetInteractable(false);
            blackImageColor.a = 1f;
            blackImage.color = blackImageColor;

            BGMManager.Instance.Play(BGMPath.ADV_WELCOM_TO_HELL_PLUSCLOCK);

            var sequence = DOTween.Sequence();
            sequence
                .Append(blackImage.DOFade(0f, 2f))
                .Append(advTexts[0].DOFade(1f, 3f))
                .Append(blackImage.DOFade(0f, 0f))
                .Join(blackImage.DOFade(1f, 2f))
                .AppendCallback(() => SEManager.Instance.Play(SEPath.ADV_VOICE_TAP_B))
                .Append(advTexts[1].DOFade(1f, 1f))
                .Append(advTexts[2].DOFade(1f, 1f))
                .AppendInterval(1f)
                .Append(advTexts[1].DOFade(0f, 1f))
                .Join(advTexts[2].DOFade(0f, 1f))
                .AppendCallback(() =>
                {
                    blackImagePopupUI.gameObject.SetActive(true);
                    advPopup.gameObject.SetActive(true);
                    advPopup.Setup(ADVDataList.GetList());
                    BGMManager.Instance.Play(BGMPath.ADV_BGM_MAIN_THEME);
                })
                .Append(blackImagePopupUI.DOFade(0f, 2f))
                .OnComplete(() =>
                {
                    blackImagePopupUI.gameObject.SetActive(false);
                    AdvPopup();
                });
        }

        private void AdvPopup()
        {
            advPopup.OnDestroyAsObservable().Subscribe(async _ => { SceneManager.LoadScene("OutGame"); });
            foreach (var advText in advTexts)
            {
                Color advTextColor = advText.color;
                advTextColor.a = 0f;
                advText.color = advTextColor;
            }
        }
    }
    
    public static class ADVDataList
    {
        private static readonly List<ADVData> _list = new();

        public static List<ADVData> GetList()
        {
            if (_list.Count <= 0)
            {
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP1, ADVFace.Talk, "おはようございます。天界から舞い降りた天使さん……転げ落ちてしまった、の方が正しいですかね。", false));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP2, ADVFace.Smile, "私はフィデル・カプティヴァーム。悪魔更生委員会『Hilfe』の支配人をしております。", false));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP3, ADVFace.Talk, "お気軽にフィリーとお呼びくださいませ。", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP4, ADVFace.Smile, "この度は弊社隊員として悪魔浄化に貢献してくださるということで……その勇気に脱帽致します。", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP5_, ADVFace.MouthShout, "……？　何を困惑しているのです？天界に帰るには、悪魔浄化部隊の上層へと上り詰める他ありませんよ。", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP6, ADVFace.Talk, "貴方は故郷に帰るための権利を得られる。悪魔は浄化され、地獄の飽和は収まる。まぁ〜いいことづくめ！", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP7, ADVFace.Smile, "要は誰よりも多くの天使を集めたという業績があればようございます。他の隊員から奪ってでも、ね？", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP8_, ADVFace.MouthShout, "おや、いけません！出動の時間に遅れてしまいますよ。\nさぁ……", true));
                _list.Add(new ADVData(SEPath.ADV_VOICE_TAP9, ADVFace.Talk, "行ってらっしゃいませ。", true));
            }

            return _list;
        }
    }
}