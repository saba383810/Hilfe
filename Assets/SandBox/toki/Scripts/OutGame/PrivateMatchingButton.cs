using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;

/// <summary>
/// プライベートマッチ選択ボタン
/// </summary>
public class PrivateMatchingButton : MonoBehaviour
{
    [SerializeField] private CommonButton privateMatchButton;

    private void Start()
    {
        privateMatchButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe(_ => InstantiatePopup());
    }
    
    private void InstantiatePopup()
    {
        PopupManager.ShowPopupAsync(PopupKey.CREATE_PRIVATE_MATCH_POPUP);
    }
}

