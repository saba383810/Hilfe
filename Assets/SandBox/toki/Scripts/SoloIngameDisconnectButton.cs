using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

public class SoloIngameDisconnectButton : MonoBehaviour
{
    [SerializeField, Header("ホームに戻るボタン")] private CommonButton _disconnectButton;

    private void Start()
    {
        _disconnectButton.OnClick.TakeUntilDestroy(gameObject).Subscribe(_ => ShowPopup());
    }

    private void ShowPopup()
    {
        PopupManager.ShowPopupAsync(PopupKey.CANCEL_POPUP);
    }
}
