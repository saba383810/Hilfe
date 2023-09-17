using System;
using Common;
using Cysharp.Threading.Tasks;
using sabanogames.Common.UI;
using TeamB.Scripts.Common.API;
using TMPro;
using UniRx;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class UserNameDecisionPopup : Popup
    {
        [SerializeField] private TMP_InputField inputText;
        [SerializeField] private CommonButton sendButton;
        public override void Setup()
        {
            base.Setup();
            sendButton.SetInteractable(false);
            sendButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(async _ =>
                {
                    if (string.IsNullOrEmpty(inputText.text) || inputText.text.Length > 5) return;
                    
                    sendButton.SetInteractable(false);
                    Preferences.SetPlayerName(inputText.text);
                    Hide();
                });

            inputText.onValueChanged.AddListener(val =>
            {
                sendButton.SetInteractable(!(string.IsNullOrEmpty(val) || val.Length > 5));
            });
        }
    }
}