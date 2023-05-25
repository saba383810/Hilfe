using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class ResultCell : MonoBehaviour
    {
        [SerializeField] private TMP_Text usernameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private Image rankingImage;
        [SerializeField] private Image selfImage;
        [SerializeField] private GameObject selfArrow;

        public void Initialize(Sprite rankingSprite, string username, uint score, Sprite selfSprite, bool isSelf)
        {
            rankingImage.sprite = rankingSprite;
            usernameText.text = username;
            scoreText.text = score.ToString();
            selfImage.sprite = selfSprite;
            selfArrow.SetActive(isSelf);
        }
    }
}