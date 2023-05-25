using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class RankingCell : MonoBehaviour
    {
        [SerializeField] private Image rankingImage;
        [SerializeField] private Image rankingImageBg;
        [SerializeField] private Image arrowImage;
        [SerializeField] private TMP_Text rankingText;
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text scoreText;

        public void Initialize(uint ranking, string username, uint score, Sprite rankImage, Sprite rankImageBg, bool isMine)
        {
            var isImage = ranking is >= 1 and <= 3;
            rankingText.gameObject.SetActive(!isImage);
            arrowImage.gameObject.SetActive(isMine);
            rankingImageBg.sprite = rankImageBg;
            rankingImage.sprite = rankImage;
            if (isImage)
            {
                rankingImage.sprite = rankImage;
            }
            else
            {
                rankingText.text = ranking.ToString();
            }
            usernameText.text = username;
            scoreText.text = score.ToString();
        }

        public void InitializeOutOfRange(string username, uint score)
        {
            rankingText.gameObject.SetActive(true);
            rankingImage.gameObject.SetActive(false);
            rankingText.text = score == 0 ? "-" : "圏外";
            usernameText.text = username;
            scoreText.text = score.ToString();
        }
    }
}