using System;
using Sabanogaems.AudioManager;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace sabanogames.Common.UI
{
    public enum ButtonSeType
    {
        Ok,
        Cancel,
        Tap,
        None
    }

    public class CommonButton : MonoBehaviour, IButtonInput
    {
        [SerializeField, Header("クリックイベントに使用したいButton")]
        private Button button;

        [SerializeField, Header("ButtonのSE")] private ButtonSeType buttonSe = ButtonSeType.Ok;

        private string SeKey => GetSe(buttonSe);

        private readonly TimeSpan _defendChatteringReInputSpan = TimeSpan.FromMilliseconds(1000);
        private readonly TimeSpan _onClickReInputSpan = TimeSpan.FromMilliseconds(150);

        private void Awake()
        {
            if (button == null)
            {
                button = gameObject.GetComponent<UnityEngine.UI.Button>();
            }
        }

        /// <summary>シンプルなボタン</summary>
        public IObservable<Unit> OnClick =>
            button.OnClickAsObservable().ThrottleFirst(_onClickReInputSpan).DoSePlayShot(SeKey);

        /// <summary>連打防止用のbuttonの実装(連打感覚は _reInputSpanの期間)</summary>
        public IObservable<Unit> OnClickDefendChattering => button.OnClickAsObservable()
            .ThrottleFirst(_defendChatteringReInputSpan).DoSePlayShot(SeKey);

        /// <summary>一回だけ押せるボタンの実装</summary>
        public IObservable<Unit> OnClickOnce => button.OnClickAsObservable().First().DoSePlayShot(SeKey);

        /// <summary>
        /// Buttonを有効または無効にする (Disabledステータスになるので見た目が変わる)
        /// </summary>
        /// <param name="isActive"></param>
        public void SetInteractable(bool isActive)
        {
            button.interactable = isActive;
        }

        /// <summary>
        /// Buttonを有効または無効にする(色は変わらない)
        /// </summary>
        /// <param name="isActive"></param>
        public void SetEnabled(bool isActive)
        {
            button.enabled = isActive;
        }

        private string GetSe(ButtonSeType seType)
        {
            switch (seType)
            {
                case ButtonSeType.Ok:
                    return SEPath.INGAME_SELECT;
                case ButtonSeType.Cancel:
                    return SEPath.INGAME_CANCEL_SOUND;
                case ButtonSeType.Tap:
                    return SEPath.INGAME_BASIC_TAP;
                case ButtonSeType.None:
                    return null;
                default:
                    Debug.LogError($"想定していないSEが入力されました seType:{seType}");
                    throw new Exception();
            }
        }

        private void Reset()
        {
            button = GetComponentInChildren<Button>();
        }
    }
}