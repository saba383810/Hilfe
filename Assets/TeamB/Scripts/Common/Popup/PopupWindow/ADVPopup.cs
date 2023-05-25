using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class ADVPopup : Popup
    {
        [SerializeField] private CommonButton nextButton;
        [SerializeField] private GameObject[] faces;
        [SerializeField] private TMP_Text speechText;
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image blackImage;
        [SerializeField] private Image arrowImage;

        [SerializeField] private GameObject _speechBubbleGameObject;
        
        public AsyncReactiveProperty<uint> pageCount = new(1);

        public void Setup(List<ADVData> advDates, bool isMain = false)
        {
            if (isMain) BGMManager.Instance.Play(BGMPath.ADV_BGM_MAIN_THEME);
            blackImage.gameObject.SetActive(false);
            ChangeFace(advDates[0].face);
            speechText.text = advDates[0].advText;
            nameText.text = advDates[0].isOpenName ? "フィリー" : "？？？？";
            SEManager.Instance.Play(advDates[0].voice);
            nextButton.OnClickDefendChattering
                .TakeUntilDestroy(gameObject)
                .WithLatestFrom(pageCount.ToObservable(), (_, page) => page)
                .Subscribe(async page =>
                {
                    int index = (int)page;
                    SEManager.Instance.Stop(advDates[index-1].voice);
                    if (page >= advDates.Count)
                    {
                        blackImage.gameObject.SetActive(true);
                        var sequence = DOTween.Sequence();
                        sequence
                            .Append(blackImage.DOFade(1f, 2f))
                            .OnComplete(() =>
                            {
                                Hide();
                            });
                    }
                    else
                    {
                        ChangeFace(advDates[index].face);
                        speechText.text = advDates[index].advText;
                        nameText.text = advDates[index].isOpenName ? "フィリー" : "？？？？";
                        pageCount.Value++;
                        if (index == 7)
                        {
                            _speechBubbleGameObject.SetActive(false);
                            SEManager.Instance.Play(SEPath.INGAME_ADV_ALERT);
                            await UniTask.Delay(TimeSpan.FromSeconds(4f));
                            _speechBubbleGameObject.SetActive(true);
                            SEManager.Instance.Play(advDates[index].voice);
                        }
                        else
                        {
                            SEManager.Instance.Play(advDates[index].voice);
                        }
                    }
                });
            var sequence = DOTween.Sequence();
            sequence
                .Append(arrowImage.transform.DOLocalMove(new Vector3(arrowImage.transform.localPosition.x, arrowImage.transform.localPosition.y + 10, arrowImage.transform.localPosition.z), 1f))
                .Append(arrowImage.transform.DOLocalMove(new Vector3(arrowImage.transform.localPosition.x, arrowImage.transform.localPosition.y, arrowImage.transform.localPosition.z), 1f))
                .AppendInterval(0.5f)
                .SetLoops(-1);
        }

        private void ChangeFace(ADVFace face)
        {
            faces[0].SetActive(face.Equals(ADVFace.Talk));
            faces[1].SetActive(face.Equals(ADVFace.Smile));
            faces[2].SetActive(face.Equals(ADVFace.MouthShout));
        }
    }

    public struct ADVData
    {
        public ADVFace face;
        public string advText;
        public bool isOpenName;
        public string voice;

        public ADVData(string voice, ADVFace face, string advText, bool isOpenName)
        {
            this.face = face;
            this.advText = advText;
            this.isOpenName = isOpenName;
            this.voice = voice;
        }
    }

    public enum ADVFace
    {
        Talk,
        Smile,
        MouthShout
    }
}