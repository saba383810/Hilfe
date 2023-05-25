using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーとピクミンのアニメーター内パラメータの変更, 取得を行うクラス
/// </summary>
public class EnemyAndPikminAnimatorController : MonoBehaviour
{
    [SerializeField, Header("エネミー（ピクミン）のアニメータ")]
    private Animator _enemyAndPikminAnimator;

    private const string enemyTypeParameterName = "EnemyType";
    private const string playerTypeParameterName = "PlayerType";
    private const string isPikminParameterName = "IsPikmin";
    
    private int _enemyTypeInt;
    private int _playerTypeInt;
    private bool _isPikmin;
    
    // 初期化
    public void Initialization(EnemyType.Enemy enemyType)
    {
        _enemyTypeInt = _enemyAndPikminAnimator.GetInteger(enemyTypeParameterName);
        _playerTypeInt = _enemyAndPikminAnimator.GetInteger(playerTypeParameterName);
        _isPikmin = _enemyAndPikminAnimator.GetBool(isPikminParameterName);
        SetEnemyType(enemyType);
        SetPlayerType(Player.PlayerType.TypeA);
        SetIsPikmin(false);
    }

    // EnemyType変更
    public void SetEnemyType(EnemyType.Enemy enemyType)
    {
        _enemyAndPikminAnimator.SetInteger(enemyTypeParameterName, (int) enemyType);
    }
    
    // EnemyTypeの取得
    public EnemyType.Enemy GetEnemyType()
    {
        return (EnemyType.Enemy)_enemyTypeInt;
    }
    
    // PlayerTypeの変更
    public void SetPlayerType(Player.PlayerType playerTypeType)
    {
        _enemyAndPikminAnimator.SetInteger(playerTypeParameterName, (int) playerTypeType);
    }
    
    // PlayerTypeの取得
    public Player.PlayerType GetPlayerType()
    {
        return (Player.PlayerType)_playerTypeInt;
    }
    
    // isPikmin をセット
    public void SetIsPikmin(bool isPikmin)
    {
        _isPikmin = isPikmin;
        _enemyAndPikminAnimator.SetBool(isPikminParameterName, isPikmin);
    }
    
    // isPikmin を取得
    public bool GetIsPikmin()
    {
        return _isPikmin;
    }
}
