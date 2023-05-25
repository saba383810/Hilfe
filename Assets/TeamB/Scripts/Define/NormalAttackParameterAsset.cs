using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 通常攻撃パラメータアセット
/// </summary>
[CreateAssetMenu(fileName = "NormalAttackParameter", menuName = "ScriptableObjects/CreateNormalAttackParameterAsset")]
public class NormalAttackParameterAsset : ScriptableObject
{
    [SerializeField, Header("通常攻撃の攻撃力")] private float _power = 0.334f;
    [SerializeField, Header("速度")] private float _velocity = 10f;
    [SerializeField, Header("攻撃間隔（秒）")] private float _attackIntervalTime = 0.5f;

    public float GetPower()
    {
        return _power;
    }
    
    public float GetAttackIntervalTime()
    {
        return _attackIntervalTime;
    }
    
    public float GetVelocity()
    {
        return _velocity;
    }
}
