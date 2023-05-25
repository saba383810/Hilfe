using Common;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class CancelPopup : Popup
    {
        [SerializeField] private CommonButton goHomeButton;
        public override void Setup()
        {
            base.Setup();
            goHomeButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ =>
            {
                FindAnyObjectByType<GameStateController>().Disconnect();
            });

        }
    }
}