using DG.Tweening;
using TeamB.Scripts.InGame.Matching;
using TMPro;
using UnityEngine;

public class MatchingUI : MonoBehaviour
{
    [SerializeField] private PlayerInfo[] playerInfoArray = new PlayerInfo[4];
    [SerializeField] private TMP_Text minuteText;
    [SerializeField] private TMP_Text secondText;
    [SerializeField] private TMP_Text currenPlayersText;
    private const float AnimSpeed = 0.5f;
    
    public void Setup(int index, string userName)
    {
        playerInfoArray[index].Setup(userName);
    }

    public void DisablePlayerInfo(int index)
    {
        playerInfoArray[index].Disable();
    }

    public void SetTimer(int timer)
    {
        minuteText.text = $"{(timer / 60):00}";
        secondText.text = $"{(timer % 60):00}";
    }

    public void SetCurrentPlayer(int playersCnt, int maxPlayers)
    {
        currenPlayersText.text = $"{playersCnt}/{maxPlayers}";
    }
}
