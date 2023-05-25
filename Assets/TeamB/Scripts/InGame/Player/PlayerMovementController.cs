using System;
using Cinemachine;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Fusion;
using SandBox.saba.Scripts;
using UnityEngine;
using UnityEngine.Video;

public class PlayerMovementController : NetworkBehaviour
{
    [SerializeField] private float movementSpeed = 400;
    [SerializeField] private Transform playerImageTransform;
    [SerializeField] private PlayerAnimatorController animatorController;

    // シングルトン参照
    private float maxSpeed = 20;
    private float _speedItemEffectTime = 20;
    
    private PlayerController _playerController = null;
    private Rigidbody2D rb2D;
    private CinemachineVirtualCamera _virtualCamera;
    private int _currentRotation = 0;
    private Sequence _rotateAnimation;

    // タグ定義
    private const string SPEED_ITEM_TAG = "SpeedItem";
    
    // アイテム等で加速した際にmaxSpeedに掛かる値
    private float _speedMultiplier = 1f;
    private float _currentMaxSpeed;

    public void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
        _virtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();
        maxSpeed = LevelDesignSingleton.Instance.GetPlayerMaxSpeed();
        _speedItemEffectTime = LevelDesignSingleton.Instance.GetSpeedUpItemEffectTime();
    }

    public override void Spawned()
    {
        _playerController = GetComponent<PlayerController>();
        if(!_playerController.Object.HasInputAuthority) return;
        if(_virtualCamera.Follow == null) _virtualCamera.Follow = transform;
    }

    public void SetAnimationType(int playerIndex)
    {
        animatorController.Initialization((Player.PlayerType)playerIndex,Player.PlayerStatus.Attack);
    }

    public override void FixedUpdateNetwork()
    {
        if (Runner.TryGetInputForPlayer<PlayerInput>(Object.InputAuthority, out var input))
        {
            Move(input);
            if(_virtualCamera.Follow == null) _virtualCamera.Follow = transform;
        }
    }

    public void SetSpeedMultiplier(float speedMultiplier)
    {
        _speedMultiplier = speedMultiplier;
    }
    
    private void OnTriggerEnter2D(Collider2D enterCollider)
    {
        var isSpeedItem = enterCollider.CompareTag(SPEED_ITEM_TAG);

        if (isSpeedItem)
        {
            var speedMultiplier = enterCollider.GetComponent<SpeedUpItem>().GetSpeedMultiplier();
            
            RPC_SetPlayerAndPikminSpeedMultiplier(speedMultiplier);
        }
    }
    [Rpc(RpcSources.All,RpcTargets.All)]
    private async void RPC_SetPlayerAndPikminSpeedMultiplier(float speedMultiplier)
    {
        SetSpeedMultiplier(speedMultiplier);
        SetEnemyMultiplier(speedMultiplier);
        await UniTask.Delay(TimeSpan.FromSeconds(_speedItemEffectTime));
        SetSpeedMultiplier(1);
        SetEnemyMultiplier(1);
    }

    private void SetEnemyMultiplier(float speedMultiplier)
    {
        for (int intEnemyType = 0; intEnemyType < 3; intEnemyType++) 
        {
            foreach (var engelList in _playerController.GetEngelList((EnemyType.Enemy) intEnemyType)) 
            { 
                engelList.SetSpeedMultiplier(speedMultiplier); 
            }
        }
    }
    
    // Moves the spaceship RB using the input for the client with InputAuthority over the object
    private void Move(PlayerInput input)
    {
        if (input.Direction.sqrMagnitude == 0)
        {
            rb2D.velocity = Vector2.zero;
            animatorController.SetPlayerStatus(Player.PlayerStatus.StandBy);
            return;
        }
        animatorController.SetPlayerStatus(Player.PlayerStatus.Attack);
        var direction = input.Direction * movementSpeed * Runner.DeltaTime;
        
        rb2D.velocity += direction;

        _currentMaxSpeed = maxSpeed * _speedMultiplier;
        
        if (rb2D.velocity.magnitude > _currentMaxSpeed)
        {
            rb2D.velocity = rb2D.velocity.normalized * _currentMaxSpeed;
        }
        MoveAnimation(direction);
    }

    private void MoveAnimation(Vector2 direction)
    {
        var yAngle = GetMoveRotation(direction);
        playerImageTransform.rotation = Quaternion.Euler(0, yAngle, 0);
    }

    private int GetMoveRotation(Vector2 direction)
    {
        return direction.x > 0 ? 180 : 0;
    }
    
}
