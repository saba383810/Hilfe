using System.Collections;
using System.Collections.Generic;
using Common;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelDesignSingleton : SingletonMonoBehaviour<LevelDesignSingleton>
{
    [SerializeField, Header("制限時間（秒）")] private float _limitTime = 90f;
    [SerializeField, Header("プレイヤーの最大速度")] private float _playerMaxSpeed = 20f;
    [SerializeField, Header("ピクミンの速度")] private float _pikminSpeed = 400f;
    [SerializeField, Header("エネミーの速度")] private float _enemySpeed = 200f;
    [SerializeField, Header("エネミーとピクミンの最大速度")] private float _enemyAndMaxVelocity = 100; 
    [SerializeField, Header("ピクミン攻撃でピクミンを消費する量")]
    private int _pikminConsumption = 5;

    [SerializeField, Header("ピクミン攻撃TypeA（方向指定）の範囲")]
    private float _pikminAttackRangeTypeA = 9f;

    [SerializeField, Header("ピクミン攻撃TypeB（自分中心）の範囲")]
    private float _pikminAttackRangeTypeB = 1f;

    [SerializeField, Header("ピクミン攻撃TypeC（3箇所ランダムに出る）の範囲")]
    private float _pikminAttackRangeTypeC = 7f;

    [SerializeField, Header("ピクミン攻撃TypeA（方向指定）の当たり判定がなくなるまでの時間（秒）")]
    private float _pikminAttackEffectTimeTypeA = 1f;

    [SerializeField, Header("ピクミン攻撃TypeB（自分中心）の当たり判定がなくなるまでの時間（秒）")]
    private float _pikminAttackEffectTimeTypeB = 3f;

    [SerializeField, Header("ピクミン攻撃TypeC（3箇所ランダムに出る）の当たり判定がなくなるまでの時間（秒）")]
    private float _pikminAttackEffectTimeTypeC = 0.5f;

    [SerializeField, Header("アイテムの速度アップの倍率")]
    private float _speedUpItemUpMultiplier = 2f;

    [SerializeField, Header("アイテムの速度アップの効果時間")]
    private float _speedUpItemEffectTime = 20f;

    // プレイヤが移動できる範囲はステージ背景（子階層にステージのコライダを持つ）のスケールとEnemyContainerのstageScaleに代入
    [SerializeField, Header("プレイヤーが移動できる範囲")]
    private float _stageScale = 7f;

    [SerializeField, Header("エネミーのランダム行動範囲")] private float _enemyRandomRangeScale = 10f;

    [SerializeField, Header("エネミー生成 幅 (x,y)")] private Vector2 enemyGenerateRange = new (20,20); 
    [SerializeField, Header("エネミー生成 初期位置")] private Vector2 enemyGenerateStartPos = new Vector2(-100, -100);

    [SerializeField, Header("エネミーステージ移動可能範囲　最大値(x,y)")] private Vector2 enemyStageForbiddenAreaMax = new (100,100);
    [SerializeField, Header("エネミーステージ移動可能範囲　最小値(x,y)")] private Vector2 enemyStageForbiddenAreaMin = new (100,100);

    [SerializeField, Header("スタンの秒数（実装前）")] private float _stanEffectTime;

    [SerializeField, Header("リキャスト")]
    private float _coolTime;

    [SerializeField, Header("エネミーHP")] private int _enemyHP = 300;
    [SerializeField, Header("通常攻撃の攻撃力")] private int _normalAttackPower = 100;
    [SerializeField, Header("ピクミンの移動範囲1")] private float _pikminMoveStopRange = 10f;

    [SerializeField, Header("ピクミンの移動範囲2")]
    private float _pikminMoveRange = 24f;

    [SerializeField, Header("通常弾が撃たれる時間間隔")]
    private float _delayBetweenNormalAttack = 0.5f;
    
    [SerializeField, Header("通常弾の生存時間")] private float normalBulletDestroyTime = 5f;

    protected override void Init()
    {
        base.Init();
        DontDestroyOnLoad(gameObject);
    }

    public float GetLimitTime()
    {
        return _limitTime;
    }

    public float GetPlayerMaxSpeed()
    {
        return _playerMaxSpeed;
    }

    public float GetPikminSpeed()
    {
        return _pikminSpeed;
    }

    public float GetEnemySpeed()
    {
        return _enemySpeed;
    }

    public float GetMaxEnemyVelocity()
    {
        return _enemyAndMaxVelocity;
    }
    public int GetPikminConsumption()
    {
        return _pikminConsumption;
    }

    public float GetPikminAttackRange(EnemyType.Enemy enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                return _pikminAttackRangeTypeA;
            case EnemyType.Enemy.TypeB:
                return _pikminAttackRangeTypeB;
            case EnemyType.Enemy.TypeC:
                return _pikminAttackRangeTypeC;
        }

        Debug.LogError("不正なタイプが入力された");
        return _pikminAttackRangeTypeA;
    }

    public float GetPikminAttackEffectTime(EnemyType.Enemy enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                return _pikminAttackEffectTimeTypeA;
            case EnemyType.Enemy.TypeB:
                return _pikminAttackEffectTimeTypeB;
            case EnemyType.Enemy.TypeC:
                return _pikminAttackEffectTimeTypeC;
        }

        Debug.LogError("不正なタイプが入力された");
        return _pikminAttackEffectTimeTypeA;
    }

    // TODO: oka: スタン実装後
    public float GetStanEffectTime()
    {
        return _stanEffectTime;
    }
    
    public float GetSpeedUpItemMultiplier()
    {
        return _speedUpItemUpMultiplier;
    }
    
    public float GetSpeedUpItemEffectTime()
    {
        return _speedUpItemEffectTime;
    }

    public float GetStageScale()
    {
        return _stageScale;
    }

    public float GetEnemyRandomRangeScale()
    {
        return _enemyRandomRangeScale;
    }
    
    
    public int GetEnemyHP()
    {
        return _enemyHP;
    }

    public int GetNormalAttackPower()
    {
        return _normalAttackPower;
    }

    public float GetPikminMoveStopRange()
    {
        return _pikminMoveStopRange;
    }

    public float GetPikminMoveRange()
    {
        return _pikminMoveRange;
    }

    public Vector2 GetEnemyGenerateRange()
    {
        return enemyGenerateRange;
    }

    public Vector2 GetEnemyStageForbiddenAreaMax()
    {
        return enemyStageForbiddenAreaMax;
    }

    public Vector2 GetEnemyStageForbiddenAreaMin()
    {
        return enemyStageForbiddenAreaMin;
    }

    public Vector2 GetEnemyGenerateStartPos()
    {
        return enemyGenerateStartPos;
    }
    
    public float GetIntervalNormalAttack()
    {
        return _delayBetweenNormalAttack;
    }

    public float GetNormalBulletDestroyTime()
    {
        return normalBulletDestroyTime;
    }

    public float GetCoolTime()
    {
        return _coolTime;
    }
}
