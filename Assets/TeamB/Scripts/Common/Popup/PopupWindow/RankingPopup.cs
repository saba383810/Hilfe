using System.Collections.Generic;
using System.Linq;
using Common;
using TeamB.Scripts.Common.API;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class RankingPopup : Popup
    {
        [SerializeField] private RankingCell selfRankingCell;
        [SerializeField] private RankingCell cellPrefab;
        [SerializeField] private GameObject content;

        [SerializeField] private Sprite[] rankingImages;
        [SerializeField] private Sprite[] rankingBgImages;

        public void Setup(List<(string userId, string username, uint score)> t, Models.User user)
        {
            base.Setup();
            var newList = t.OrderByDescending(t => t.score).ToList();
            uint rank = 1;
            var prevScore = newList[0].score;
            var isDisplaySelfScore = false;
            for (var i = 0; i < newList.Count; i++)
            {
                if (newList[i].score != prevScore)
                {
                    rank = (uint)i + 1;
                    prevScore = newList[i].score;
                }

                var cell = Instantiate(cellPrefab, content.transform);
                
                int rankIndex = rank <= 3 ? (int)rank - 1 : 3;
                
                cell.Initialize(rank, newList[i].username, newList[i].score, rankingImages[rankIndex], rankingBgImages[rankIndex], newList[i].userId != "");
                
                if (newList[i].userId != "" && !isDisplaySelfScore)
                {
                    selfRankingCell.Initialize(rank, newList[i].username, newList[i].score, rankingImages[rankIndex], rankingBgImages[rankIndex], false);
                    isDisplaySelfScore = true;
                }
            }

            if (!isDisplaySelfScore)
            {
                selfRankingCell.InitializeOutOfRange(user.user_name, user.score);
            }
            Preferences.SetRanking(false);
        }
    }
}