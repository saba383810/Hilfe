using System;
using Common;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using sabanogames.Common.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.Common
{
    public class CreatePrivateMatchPopup : Popup
    {
        [SerializeField] private CommonButton joinPrivateMatchButton;
        [SerializeField] private TMP_Text selectMemberText;
        [SerializeField] private CommonButton[] selectMemberButtons;
        [SerializeField] private Image[] selectMemberButtonImages;
        [SerializeField] private TMP_Text secretText;
        [SerializeField] private TMP_InputField secretInputField;
        [SerializeField] private CommonButton createMatchButton;
        private readonly AsyncReactiveProperty<int> _state = new(0);
        private int selectMemberCount = 0;
        private IDisposable joinButtonDisposable;
        private void Start()
        {
            Setup();
        }

        public override void Setup()
        {
            base.Setup();
            Subscriber();
            _state.Value = 1;
        }

        private void Subscriber()
        {
            _state.ToObservable().TakeUntilDestroy(gameObject).Subscribe(state =>
            {
                if (state == 1)
                {
                    selectMemberText.color = Color.white;
                    ChangeSelectButtonsColor(10);
                    secretText.color = Color.gray;
                    secretInputField.interactable = false;
                    createMatchButton.SetInteractable(false);
                }
                else if (state == 2)
                {
                    selectMemberText.color = Color.white;
                    secretText.color = Color.gray;
                    secretInputField.interactable = true;
                    createMatchButton.SetInteractable(false);
                }
            });
            secretInputField.onValueChanged.AddListener(text => 
                createMatchButton.SetInteractable(text.Length is >= 4 and <= 12));
            selectMemberButtons[0].OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ =>
                {
                    selectMemberCount = 2;
                    ChangeSelectButtonsColor(0);
                    _state.Value = 2;
                });
            selectMemberButtons[1].OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ =>
                {
                    selectMemberCount = 3;
                    ChangeSelectButtonsColor(1);
                    _state.Value = 2;
                });
            selectMemberButtons[2].OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(_ =>
                {
                    selectMemberCount = 4;
                    ChangeSelectButtonsColor(2);
                    _state.Value = 2;
                });
            
            joinButtonDisposable?.Dispose();
            joinButtonDisposable = joinPrivateMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject)
                .Subscribe(async _ =>
                {
                    await PopupManager.ShowPopupAsync(PopupKey.JOIN_PRIVATE_MATCH_POPUP);
                    Hide();
                });
        }

        private void ChangeSelectButtonsColor(int number)
        {
            Debug.Log(number);
            for (int i = 0; i < selectMemberButtonImages.Length; i++)
            {
                selectMemberButtonImages[i].gameObject.SetActive(i == number);
            }
        }
    }
}