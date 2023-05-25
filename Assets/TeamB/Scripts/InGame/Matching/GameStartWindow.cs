using System;
using System.Globalization;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TeamB.Scripts.Common.API;
using TMPro;
using UnityEngine;

namespace TeamB.Scripts.InGame.Matching
{
    public enum CharacterType
    {
        Grimy = 0,
        Melogardia = 1,
        Fangqulite = 2
    }
    
    public class VotePercentData
    {
        public float Grimy;
        public float Melogardia;
        public float Fangqulite;

        public VotePercentData(float grimy, float melogardia, float fang)
        {
            Grimy = grimy;
            Melogardia = melogardia;
            Fangqulite = fang;
        }
    }
    
    public class GameStartWindow : MonoBehaviour
    {
        [SerializeField] private Transform img;
        [SerializeField] private TMP_Text grimyPercentText;
        [SerializeField] private TMP_Text fangcuriteText;
        [SerializeField] private TMP_Text melogardiaPercentText;
        
        [SerializeField] private GameObject[] characterGameObjects;
        [SerializeField] private CanvasGroup voteCanvasGroup;

        private GameObject showCharacterObject;
        
        private const string GrimyKey = "grimmy";
        private const string fangKey = "fangculite";
        private const string MellogardiaKey = "mellogardia";

        public async UniTask Setup()
        {
            var data = await APIClient.VoteRanking.GetVoteRankings();
            var allCnt = data.Sum(vote => (int)vote.vote_count);
            Models.VoteRanking maxData = data[0];
            VotePercentData votePercentData = new VotePercentData(0,0,0);
            // テキストのパーセンテージを表示
            foreach (var vote in data)
            {
                Debug.Log($"[API] id: {vote.id}, {vote.vote_count}, {vote.character_name}");
                var percent = (int)Math.Floor((float)vote.vote_count / allCnt * 100);
                switch (vote.id)
                {
                    case GrimyKey:
                        grimyPercentText.text = percent.ToString(CultureInfo.InvariantCulture);
                        votePercentData.Grimy = percent;
                        break;
                    case fangKey:
                        fangcuriteText.text = percent.ToString(CultureInfo.InvariantCulture);
                        votePercentData.Fangqulite = percent;
                        break;
                    case MellogardiaKey:
                        melogardiaPercentText.text = percent.ToString(CultureInfo.InvariantCulture);
                        votePercentData.Melogardia = percent;
                        break;
                }
                Debug.Log($"Grimy: {votePercentData.Grimy} Fang: {votePercentData.Fangqulite} Melo:{votePercentData.Melogardia}");

                if (maxData.vote_count < vote.vote_count) maxData = vote;
                
            }
            Preferences.SetVotePercentData(votePercentData);
            switch (maxData.id)
            {
                // 一番多いキャラクターの情報を表示
                case GrimyKey:
                    showCharacterObject = characterGameObjects[0];
                    break;
                case fangKey:
                    showCharacterObject = characterGameObjects[1];
                    break;
                case MellogardiaKey:
                    showCharacterObject = characterGameObjects[2];
                    break;
            }
            await Show();
        }

        private async UniTask Show()
        {
            var animSpeed = 0.5f;
            //アニメーション
            var tmpImgPos = img.transform.localPosition;
            DOTween.Sequence()
                .OnStart(() =>
                {
                    img.transform.localPosition = new Vector2(1000, 200);
                    showCharacterObject.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    voteCanvasGroup.alpha = 0;
                    voteCanvasGroup.gameObject.SetActive(true);
                    gameObject.SetActive(true);
                })
                .Append(img.transform.DOLocalMove(tmpImgPos, animSpeed))
                .AppendInterval(0.3f)
                .AppendCallback(()=> showCharacterObject.SetActive(true))
                .Append(showCharacterObject.transform.DOScale(Vector3.one, animSpeed * 2).SetEase(Ease.OutBounce))
                .AppendCallback(() => voteCanvasGroup.DOFade(1,animSpeed));

        }
    }
}
