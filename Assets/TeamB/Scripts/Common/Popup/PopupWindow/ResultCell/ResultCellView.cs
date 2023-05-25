using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class ResultCellData
    {
        public int PlayerIndex { get; set; }
        public string UserName { get; set; }
        public int Score { get; set; }
        public bool IsLocalPlayer { get; set; }

        public ResultCellData(int playerIndex, string userName, int score,bool isLocalPlayer)
        {
            PlayerIndex = playerIndex;
            UserName = userName;
            Score = score;
            IsLocalPlayer = isLocalPlayer;
        }
    }
    public class ResultCellView : MonoBehaviour
    {
        [SerializeField] private TMP_Text userNameText;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private GameObject isLocalPlayerObject;
        [SerializeField] private Image playerImage;
        [SerializeField] private Sprite[] sprites;

        public void Setup(ResultCellData data)
        {
            userNameText.text = data.UserName;
            scoreText.text = data.Score.ToString();
            if (data.IsLocalPlayer) isLocalPlayerObject.SetActive(true);
            playerImage.sprite = sprites[data.PlayerIndex];
            gameObject.SetActive(true);
        }
    }
}
