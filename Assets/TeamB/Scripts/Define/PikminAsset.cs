using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ピクミンのステータスアセット
/// </summary>
[CreateAssetMenu(fileName = "PikminStatus", menuName = "ScriptableObjects/CreatePikminParameterAsset")]
public class PikminAsset : ScriptableObject
{
    [SerializeField, Header("ピクミンタイプ")]
    private EnemyType.Enemy _pikminType;
    [SerializeField, Header("速度")]
    private float _velocity = 10f;

    public EnemyType.Enemy GetEnemyType()
    {
        return _pikminType;
    }
    
    public float GetVelocity()
    {
        return _velocity;
    }
}
