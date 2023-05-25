using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ピクミン攻撃のパラメータアセット
/// </summary>
[CreateAssetMenu(fileName = "PikminAttackStatus", menuName = "ScriptableObjects/CreatePikminAttackParameterAsset")]
public class PikminAttackParameterAsset : ScriptableObject
{
    [SerializeField, Header("ピクミンタイプ")] private EnemyType.Enemy _pikminType;
    [SerializeField, Header("円形の範囲（直径）")] private float _circleSize = 100f;
    [SerializeField, Header("2回目のボタン押してから攻撃判定が出るまでの時間")] private float _arrivalIntervalTime = 2f;

    public EnemyType.Enemy GetEnemyType()
    {
        return _pikminType;
    }
    
    public float GetCircleSize()
    {
        return _circleSize;
    }
    
    public float GetArrivalIntervalTime()
    {
        return _arrivalIntervalTime;
    }
}
