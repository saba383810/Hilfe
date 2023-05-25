using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// プレイヤーのアニメーター内パラメータの変更, 取得を行うクラス
/// </summary>
public class PlayerAnimatorController : MonoBehaviour
{
    [SerializeField, Header("プレイヤのアニメータ")]
    private Animator _playerAnimator;

    private const string playerTypeParameterName = "PlayerType";
    private const string playerStatusParameterName = "PlayerStatus";
    
    private int _playerTypeInt;
    private int _playerStatus;
    
    // 初期化
    public void Initialization(Player.PlayerType playerTypeType, Player.PlayerStatus playerStatus)
    {
        _playerTypeInt = _playerAnimator.GetInteger(playerTypeParameterName);
        _playerStatus = _playerAnimator.GetInteger(playerStatusParameterName);
        SetPlayerType(playerTypeType);
        SetPlayerStatus(playerStatus);
    }
    
    // PlayerTypeの変更
    public void SetPlayerType(Player.PlayerType playerTypeType)
    {
        _playerAnimator.SetInteger(playerTypeParameterName, (int) playerTypeType);
    }
    
    // PlayerTypeの取得
    public Player.PlayerType GetPlayerType()
    {
        return (Player.PlayerType)_playerTypeInt;
    }
    
    // PlayerStatus をセット
    public void SetPlayerStatus(Player.PlayerStatus playerStatus)
    {
        _playerStatus = (int) playerStatus;
        _playerAnimator.SetInteger(playerStatusParameterName, (int) playerStatus);
    }
    
    //  を取得
    public Player.PlayerStatus GetPlayerStatus()
    {
        return (Player.PlayerStatus) _playerStatus;
    }
}
