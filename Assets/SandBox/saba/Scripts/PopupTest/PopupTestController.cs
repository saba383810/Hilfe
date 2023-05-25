using System.Collections.Generic;
using Common;
using sabanogames.Common.UI;
using TeamB.Scripts.Common;
using UniRx;
using UnityEngine;

public class PopupTestController : MonoBehaviour
{
    [SerializeField] private CommonButton showMessagePopupButton;
    [SerializeField] private CommonButton errorPopupButton;
    [SerializeField] private CommonButton tutorialPopupButton;
    [SerializeField] private CommonButton rankingPopupButton;
    [SerializeField] private CommonButton resultPopupButton;

    [SerializeField] private Sprite testSprite;
    
    private void Start()
    {
        showMessagePopupButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_=> PopupManager.ShowMessagePopupAsync("Caption", "メッセージが入ります"));

        errorPopupButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => PopupManager.ShowErrorPopupAsync("Errorが発生しました"));

        tutorialPopupButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => PopupManager.ShowPopupAsync(PopupKey.TUTORIAL_POPUP));
        
        rankingPopupButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ => PopupManager.ShowRankingPopupAsync());
        
        resultPopupButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
            .Subscribe(_ =>
            {
                var list = new List<(string username, uint score, Sprite sprite, bool isSelf)>
                {
                    ("player1", 1, testSprite, false),
                    ("player2", 10, testSprite, false),
                    ("player3", 100, testSprite, true),
                    ("player4", 999, testSprite, false)
                };
                //PopupManager.ShowResultPopupAsync(list);
            });
    }
}
