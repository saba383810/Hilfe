using Common;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

namespace TeamB.Scripts.OutGame
{
    public class HomeController : MonoBehaviour
    {
        [SerializeField] private CommonButton createPrivateMatchButton;

        private void Start()
        {
            createPrivateMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ => PopupManager.ShowPopupAsync(PopupKey.CREATE_PRIVATE_MATCH_POPUP));
        }
    }
}