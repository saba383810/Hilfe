using System;
using Common;
using Cysharp.Threading.Tasks;
using sabanogames.Common.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using Unity.VisualScripting;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class JoinPrivateMatchPopup : Popup
    {
        [SerializeField] private CommonButton createPrivateMatchButton;
        [SerializeField] private TMP_InputField secretInputField;
        [SerializeField] private CommonButton joinMatchButton;

        private IDisposable _iDisposable;

        private void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            base.Setup();
            Subscriber();
        }
        
        private void Subscriber()
        {
            joinMatchButton.SetInteractable(false);
            secretInputField.onValueChanged.AddListener(text => 
                joinMatchButton.SetInteractable(text.Length > 0));
            _iDisposable?.Dispose();
            _iDisposable = createPrivateMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(async _ =>
                {
                    await PopupManager.ShowPopupAsync(PopupKey.CREATE_PRIVATE_MATCH_POPUP);
                    Hide();
                });
        }
    }
}