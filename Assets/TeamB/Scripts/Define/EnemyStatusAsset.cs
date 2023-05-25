using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// エネミーのステータスアセット
/// </summary>
[CreateAssetMenu(fileName = "EnemyStatus", menuName = "ScriptableObjects/CreateEnemyParameterAsset")]
public class EnemyStatusAsset : ScriptableObject
{
    [SerializeField, Header("エネミータイプ")]
    private EnemyType.Enemy _enemyType;
    [SerializeField, Header("最大HP")]
    private float _maxHP = 1f;
    [SerializeField, Header("速度")]
    private float _velocity = 5f;

    public EnemyType.Enemy GetEnemyType()
    {
        return _enemyType;
    }
    
    public float GetMaxHP()
    {
        return _maxHP;
    }
    
    public float GetVelocity()
    {
        return _velocity;
    }
}