using System;
using System.Collections.Generic;
using Common;
using Cysharp.Threading.Tasks;
using Fusion;
using SandBox.saba.Scripts;
using TeamB.Scripts.InGame;
using TeamB.Scripts.InGame.Stage;
using UniRx;
using UniRx.Triggers;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// プレイヤの攻撃を制御する
/// </summary>
public class PlayerAttackController : NetworkBehaviour
{
    [SerializeField] private Transform typeAAimingTransform;
    [SerializeField] private Transform typeBAimingTransform;
    [SerializeField] private Transform typeCAimingTransform;
    [SerializeField] private Transform[] typeCAimingTransforms;
    [SerializeField] private GameObject typeAEffectPrefab;
    [SerializeField] private GameObject typeBEffectPrefab;
    [SerializeField] private GameObject typeCEffectPrefab;
    [SerializeField] private Collider2D collider;
    [Networked] private NetworkButtons ButtonsPrevious { get; set; }
    
    private INormalAttack _normalAttack;
    private TickTimer _normalAttackCooldown;
    private BulletContainer _bulletContainer;
    private PlayerController _playerController;
    private PlayerDataNetworked _playerDataNetworked;
    private bool _isAiming;
    private EnemyType.Enemy _enemyType = EnemyType.Enemy.None;
    private const string ENEMY_TAG = "Enemy";
    public List<EnemyController> attackList = new ();
    
    public UIButtonController uiButtonController;
    private IngameUIController _inGameUiController;
    private BackgroundStage _backgroundStage;

    // シングルトン参照
    private int usableEnemyCnt;
    //[SerializeField, Header("繰り返すまでの時間")]
    private float delayBetweenNormalAttack;


    private void Awake()
    {
        usableEnemyCnt = LevelDesignSingleton.Instance.GetPikminConsumption();
        delayBetweenNormalAttack = LevelDesignSingleton.Instance.GetIntervalNormalAttack();
        uiButtonController = FindObjectOfType<UIButtonController>();
        _inGameUiController = FindAnyObjectByType<IngameUIController>();
    }

    private void Start()
    {
        _normalAttackCooldown = TickTimer.CreateFromSeconds(Runner, delayBetweenNormalAttack);
        _playerController = GetComponent<PlayerController>();
        _playerDataNetworked = GetComponent<PlayerDataNetworked>();
        
        //collider.OnTriggerEnter2DAsObservable().TakeUntilDestroy(gameObject).Subscribe(EnterEnemy);
        collider.OnTriggerStay2DAsObservable().TakeUntilDestroy(gameObject).Subscribe(StayEnemy);
        collider.OnTriggerExit2DAsObservable().TakeUntilDestroy(gameObject).Subscribe(ExitEnemy);
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasInputAuthority) return;
        if (Runner.TryGetInputForPlayer<PlayerInput>(Object.InputAuthority, out var input))
        {
            NormalAttack();
            PikminAttack(input.AttackActions);
            ButtonsPrevious = input.AttackActions;
        }
    }
    
    /// <summary>
    ///   通常攻撃
    /// </summary>
    private void NormalAttack()
    {
        if(attackList.Count == 0) return;
        if (_normalAttackCooldown.ExpiredOrNotRunning(Runner) == false) return;
        _normalAttackCooldown = TickTimer.CreateFromSeconds(Runner, delayBetweenNormalAttack);
        var enemyController = attackList[0];

        if (_bulletContainer == null)
        {
            _bulletContainer = FindAnyObjectByType<BulletContainer>();
            if(_bulletContainer == null) return;
        }
        
        var playerId = _playerDataNetworked.PlayerId;
        var direction = GetEnemyAngle(enemyController.transform);
        attackList.Remove(enemyController);
        _bulletContainer.RPC_FireBullet(transform.position, direction, playerId);
    }

    /// <summary>
    ///   ピクミンアタック
    /// </summary>
    private void PikminAttack(NetworkButtons inputAttackActions)
    {
        if (_backgroundStage == null) _backgroundStage = FindAnyObjectByType<BackgroundStage>();

        if(inputAttackActions.WasPressed(ButtonsPrevious,PikuminAttackType.AimingTypeA))
        {
            Debug.Log("Aiming TypeA");
            if (GetCurrentEngelCnt(EnemyType.Enemy.TypeA) < usableEnemyCnt)
            {
                PopupManager.ShowFooterErrorPopupAsync("天使の数が足りないため、発射できません");
                return;
            }
            SetActiveSkillButtonView(EnemyType.Enemy.TypeA);
            _backgroundStage.SetStageGrayOut(true);
            RPC_Aiming((int)EnemyType.Enemy.TypeA);
            _enemyType = EnemyType.Enemy.TypeA;
        }
        if(inputAttackActions.WasPressed(ButtonsPrevious,PikuminAttackType.AimingTypeB))
        {
            Debug.Log("Aiming TypeB");
            if (GetCurrentEngelCnt(EnemyType.Enemy.TypeB) < usableEnemyCnt)
            {
                PopupManager.ShowFooterErrorPopupAsync("天使の数が足りないため、発射できません");
                return;
            }
            SetActiveSkillButtonView(EnemyType.Enemy.TypeB);
            _backgroundStage.SetStageGrayOut(true);
            RPC_Aiming((int)EnemyType.Enemy.TypeB);
            _enemyType = EnemyType.Enemy.TypeB;
            
        }
        if(inputAttackActions.WasPressed(ButtonsPrevious,PikuminAttackType.AimingTypeC))
        {
            Debug.Log("Aiming TypeC");
            if (GetCurrentEngelCnt(EnemyType.Enemy.TypeC) < usableEnemyCnt)
            {
                PopupManager.ShowFooterErrorPopupAsync("天使の数が足りないため、発射できません");
                return;
            }
            SetActiveSkillButtonView(EnemyType.Enemy.TypeC);
            _backgroundStage.SetStageGrayOut(true);
            RPC_Aiming((int)EnemyType.Enemy.TypeC);
            _enemyType = EnemyType.Enemy.TypeC;
        }

        if (inputAttackActions.WasPressed(ButtonsPrevious, PikuminAttackType.Attack))
        {
            if(_enemyType == EnemyType.Enemy.None) return;
            Debug.Log("Attack");
            SetActiveSkillButtonView(EnemyType.Enemy.None);
            _backgroundStage.SetStageGrayOut(false);
            Attack(_enemyType);
        }
    }

    public void SetActiveSkillButtonView(EnemyType.Enemy enemyType)
    {
        _inGameUiController.typeASkillButtonView.SetActiveShot(enemyType == EnemyType.Enemy.TypeA);
        _inGameUiController.typeBSkillButtonView.SetActiveShot(enemyType == EnemyType.Enemy.TypeB);
        _inGameUiController.typeCSkillButtonView.SetActiveShot(enemyType == EnemyType.Enemy.TypeC);
    }
    
    public void Attack(EnemyType.Enemy enemyType)
    {
        Debug.Log($"Attack! enemyType : {enemyType}");
        RPC_AllAimingDisable();
        if (GetCurrentEngelCnt(enemyType) < usableEnemyCnt)
        {
            PopupManager.ShowFooterErrorPopupAsync("天使の数が足りないため、発射できません");
            Debug.Log("所持数が足りないためスキルが発動できませんでした。");
            return;
        }
        uiButtonController.SetReCast();
        _playerController.UseEngel(enemyType, usableEnemyCnt);
        RPC_Attack((int)enemyType);
        _enemyType = EnemyType.Enemy.None;
    }

    private void EnterEnemy(Collider2D other)
    {
        if (!other.CompareTag(ENEMY_TAG)) return;
        if (!other.TryGetComponent(out EnemyController enemyController)) return;

        if(!attackList.Contains(enemyController)) attackList.Add(enemyController);
    }

    private void StayEnemy(Collider2D other)
    {
        if (!other.CompareTag(ENEMY_TAG)) return;
        if (!other.TryGetComponent(out EnemyController enemyController)) return;

        if (enemyController.playerController != null)
        {
            if(attackList.Contains(enemyController)) attackList.Remove(enemyController);
            return;
        }

        if(!attackList.Contains(enemyController)) attackList.Add(enemyController);
    }

    private void ExitEnemy(Collider2D other)
    {
        if (!other.CompareTag(ENEMY_TAG)) return;
        if (!other.TryGetComponent(out EnemyController enemyController)) return;
        if(attackList.Contains(enemyController)) attackList.Remove(enemyController);
        
    }

    private float GetEnemyAngle(Transform target)
    {
        var delta = target.position - transform.position;
        var angle = Vector2Extensions.Vector2ToAngle(delta);
        return angle;
    }
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_Aiming(int enemyTypeCastInt)
    {
        Debug.Log($"RPC_Aiming{enemyTypeCastInt}");
        var enemyType = (EnemyType.Enemy) enemyTypeCastInt;
        SetActiveAimingPosition(enemyType, true);
    }
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    private void RPC_AllAimingDisable()
    {
        
        AllSetActiveAimingPosition(false);
    }
    
    
    [Rpc(RpcSources.All,RpcTargets.All)]
    public async void RPC_Attack(int enemyTypeCastInt)
    {
        
        var enemyType = (EnemyType.Enemy) enemyTypeCastInt;
        Debug.Log($"RPC_Attack:{enemyType}");

        var effectPrefab = GetEffectPrefab(enemyType);
        
        switch (enemyType)
        {
            //前に発射
            case EnemyType.Enemy.TypeA:
            {
                var aimingPositionTransform = GetAimingTransform(enemyType);
                var obj = Instantiate(effectPrefab, aimingPositionTransform.position, quaternion.identity);
                var pikminAttack = obj.GetComponent<PikminAttackBullet>();
                pikminAttack.Setup(_playerController, enemyType);
                break;
            }
            // PlayerのParentにする
            case EnemyType.Enemy.TypeB:
            {
                var aimingPositionTransform = GetAimingTransform(enemyType);
                var obj = Instantiate(effectPrefab, aimingPositionTransform.position, quaternion.identity,transform);
                var pikminAttack = obj.GetComponent<PikminAttackBullet>();
                pikminAttack.Setup(_playerController, enemyType);
                break;
            }
            // 3点に発射する
            case EnemyType.Enemy.TypeC:
            {
                foreach (var typeCTransform in typeCAimingTransforms)
                {
                    var obj = Instantiate(effectPrefab, typeCTransform.position, quaternion.identity);
                    var pikminAttack = obj.GetComponent<PikminAttackBullet>();
                    pikminAttack.Setup(_playerController, enemyType);
                    await UniTask.Delay(TimeSpan.FromSeconds(0.2f));
                }
                break;
            }
        }
    }

    public int GetCurrentEngelCnt(EnemyType.Enemy enemyType)
    {
        return enemyType switch
        {
            EnemyType.Enemy.TypeA => _playerDataNetworked.EnemyTypeACnt,
            EnemyType.Enemy.TypeB => _playerDataNetworked.EnemyTypeBCnt,
            EnemyType.Enemy.TypeC => _playerDataNetworked.EnemyTypeCCnt,
            _ => 100
        };
    }

    public GameObject GetEffectPrefab(EnemyType.Enemy enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                return typeAEffectPrefab;
            case EnemyType.Enemy.TypeB:
                return typeBEffectPrefab;
            case EnemyType.Enemy.TypeC:
                return typeCEffectPrefab;
            default:
                Debug.LogError($"正しいEnemyTypeを入力してください{enemyType}");
                return null;
        }
    }

    public Transform GetAimingTransform(EnemyType.Enemy enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                return typeAAimingTransform;
            case EnemyType.Enemy.TypeB:
                return typeBAimingTransform; ;
            case EnemyType.Enemy.TypeC:
                return typeCAimingTransform;
            default:
                Debug.LogError($"正しいEnemyTypeを入力してください{enemyType}");
                return null;
        }
    }

    public void SetActiveAimingPosition(EnemyType.Enemy enemyType,bool isActive)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                typeAAimingTransform.gameObject.SetActive(isActive);
                typeBAimingTransform.gameObject.SetActive(!isActive);
                typeCAimingTransform.gameObject.SetActive(!isActive);
                break;
            case EnemyType.Enemy.TypeB:
                typeAAimingTransform.gameObject.SetActive(!isActive);
                typeBAimingTransform.gameObject.SetActive(isActive);
                typeCAimingTransform.gameObject.SetActive(!isActive);
                break;
            case EnemyType.Enemy.TypeC:
                typeAAimingTransform.gameObject.SetActive(!isActive);
                typeBAimingTransform.gameObject.SetActive(!isActive);
                typeCAimingTransform.gameObject.SetActive(isActive);
                break;
        }
    }

    public void AllSetActiveAimingPosition(bool isActive)
    {
        typeAAimingTransform.gameObject.SetActive(isActive);
        typeBAimingTransform.gameObject.SetActive(isActive);
        typeCAimingTransform.gameObject.SetActive(isActive);
    }
}
