using System;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Fusion;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using TeamB.Scripts.InGame.Enemy;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class EnemyController : MonoBehaviour
{
   [SerializeField] private Rigidbody2D rb2d;
   [SerializeField] private EnemyAndPikminAnimatorController enemyAndPikminAnimatorController;
   [SerializeField] private SpriteRenderer spriteRenderer;
   [SerializeField] private Transform enemyImage;
   [SerializeField] private float enemyStopMagnitude = 20;
   [SerializeField] private float hitVolume = 0.3f;

   // シングルトン参照
   //[SerializeField] private float pikminSpeed = 400;
   //[SerializeField] private float enemySpeed = 200;
   //[SerializeField] private float enemyRandomRangeScale = 10;
   //[SerializeField] private int maxHp = 3;
   //[SerializeField] private float pikminStopMagnitude = 10;
   private float pikminSpeed;
   private float enemySpeed;
   private float maxVelocity;
   private float enemyRandomRangeScale;
   private int maxHp;
   private float pikminStopMagnitude;
   private float _moveRange;
   
   private void Awake()
   { 
      pikminSpeed = LevelDesignSingleton.Instance.GetPikminSpeed(); 
      enemySpeed = LevelDesignSingleton.Instance.GetEnemySpeed();
      maxVelocity = LevelDesignSingleton.Instance.GetMaxEnemyVelocity(); 
      enemyRandomRangeScale = LevelDesignSingleton.Instance.GetEnemyRandomRangeScale();
      pikminStopMagnitude = LevelDesignSingleton.Instance.GetPikminMoveStopRange();
      _moveRange = LevelDesignSingleton.Instance.GetPikminMoveRange();
   }

   // 攻撃識別タグ定義
   private const string NORMAL_ATTACK_TAG = "NormalAttack";
   private const string PIKMIN_ATTACK_TAG = "PikminAttack";

   private const int PIKMIN_RESET_MOVE_TARGET_FRAME = 20;

   private BulletContainer _bulletContainer;
   private EnemyContainer _enemyContainer;
   private EnemyTakeDamage _enemyTakeDamage;
   private Sequence _randomMoveSequence;

   private int _hp;
   private int _id;
   
   public PlayerController playerController = null;

   private sbyte _playerId;
   private Vector2 _randomVector2;
   private CancellationTokenSource _cts;
   private Subject<EnemyIndexId> _setEnemyIndexId;
   public EnemyType.Enemy enemyType;
   private Vector2 _targetPos;

   private int _currentFrame;

   // アイテムとった時等に速度に掛かる値
   private float _speedMultiplier = 1f;

   private void Start()
   {
      _bulletContainer = FindAnyObjectByType<BulletContainer>();
      _enemyTakeDamage = GetComponent<EnemyTakeDamage>();
      _cts = new CancellationTokenSource();
      _targetPos = transform.position;
   }

   /// <summary>
   ///   Enemy生成時に初期化をする
   /// </summary>
   public void Setup(int id, EnemyType.Enemy enemyType, Subject<EnemyIndexId> setEnemyIndexId, EnemyContainer enemyContainer)
   {
      _id = id;
      gameObject.name = $"Enemy{_id}";
      maxHp = LevelDesignSingleton.Instance.GetEnemyHP();
      _hp = maxHp;
      _setEnemyIndexId = setEnemyIndexId;
      this.enemyType = enemyType;
      _enemyContainer = enemyContainer;
      spriteRenderer.sortingOrder = id;
      enemyAndPikminAnimatorController.Initialization(enemyType);
   }

   public void Render(Vector2 pos, float deltaTime)
   {
      float speed;
      bool isEnemy;
      if (playerController == null)
      {
         speed = enemySpeed;
         _speedMultiplier = 1;
         isEnemy = true;
      }
      else
      {
         speed = pikminSpeed * _speedMultiplier;
         isEnemy = false;
      }

      MoveOnFollowTarget(deltaTime, pos, speed, isEnemy);
   }

   public void SetSpeedMultiplier(float speedMultiplier)
   {
      _speedMultiplier = speedMultiplier;
   }

   public async void FollowTarget(float deltaTime)
   {
      Vector2 target = Vector2.zero;
      float speed;
      bool isEnemy;
      if (playerController == null)
      {
         speed = enemySpeed;
         _speedMultiplier = 1;
         try
         {
            target = await GetEnemyRandomMovePosition();
         }
         catch (OperationCanceledException e) { }
         
         isEnemy = true;
      }
      else
      {
         speed = pikminSpeed * _speedMultiplier;
         target = GetPikminRandomMovePosition();
         isEnemy = false;
      }

      MoveOnFollowTarget(deltaTime, target, speed, isEnemy);
   }


   private void MoveOnFollowTarget(float deltaTime, Vector2 targetPos, float speed, bool isEnemy)
   {
      var delta = targetPos - (Vector2) transform.position;
      var direction = Vector2Extensions.DeltaToDirection(delta);
      
      // Clientだけ少し移動を少し遅く同期する
      if(_bulletContainer != null) if (!_bulletContainer.Object.HasStateAuthority) direction /= 2;
      
      if (isEnemy)
      {
         // エネミー
         if (delta.sqrMagnitude > enemyStopMagnitude)
            rb2d.AddForce(direction * (speed * deltaTime), ForceMode2D.Impulse);
         else
            rb2d.velocity = Vector2.zero;
      }
      else
      {
         // ピクミン
         if (delta.sqrMagnitude > pikminStopMagnitude)
            rb2d.AddForce(direction * (speed * deltaTime), ForceMode2D.Impulse);
         else
            rb2d.velocity = Vector2.zero;
      }

      if (rb2d.velocity.magnitude > maxVelocity) rb2d.velocity = rb2d.velocity.normalized * maxVelocity;

      var angleY = GetMoveRotation(direction);
      enemyImage.rotation = Quaternion.Euler(0, angleY, 0);
   }

   public void CheckAnimation(sbyte playerId)
   {
      if (_playerId == playerId) return;

      if (playerId == EnemyConfig.EnemySbyte)
      {
         _playerId = playerId;
         enemyAndPikminAnimatorController.SetIsPikmin(false);
         return;
      }

      if (playerController == null || playerController.GetPhotonID() != playerId)
      {
         playerController = GameObject.Find($"Player{playerId}").GetComponent<PlayerController>();
      }
      enemyAndPikminAnimatorController.SetPlayerType((Player.PlayerType) playerController.PlayerIndex);
      enemyAndPikminAnimatorController.SetIsPikmin(true);
      _playerId = playerId;
   }

   public void CheckEnemyType(EnemyType.Enemy enemyType)
   {
      if(this.enemyType == enemyType) return;
      this.enemyType = enemyType;
      enemyAndPikminAnimatorController.SetEnemyType(enemyType);
      
   }

   private async void OnTriggerEnter2D(Collider2D enterCollider)
   {

      var isNormalAttack = enterCollider.CompareTag(NORMAL_ATTACK_TAG);
      var isPikminAttack = enterCollider.CompareTag(PIKMIN_ATTACK_TAG);

      var damageNum = 0;
      if (isNormalAttack && playerController == null)
      {
         // ダメージテキスト表示
         damageNum = GetDamageNum();
         var normalAttackBullet = enterCollider.GetComponent<NormalAttackBullet>();
         var photonId = normalAttackBullet.BulletData.PhotonId;
         if (_enemyContainer.Object.HasStateAuthority)
         {
            Damage(damageNum, photonId);
         }

         // クライアント実行
         await _enemyTakeDamage.CallDamageText();
         
         var bullet = _bulletContainer.GetActiveBullets().FirstOrDefault(bullet => bullet.BulletId == normalAttackBullet.BulletId);
         if(bullet != null) bullet.SetIsAliveFalse();
         if(_enemyContainer.Runner.LocalPlayer.PlayerId == photonId) SEManager.Instance.Play(SEPath.INGAME_PURGE_SOUND, hitVolume);
         // _bulletContainer.RPC_SetIsAliveFalse(normalAttackBullet.BulletId);
      }
      else if (isPikminAttack)
      {
         var pikminAttackBullet = enterCollider.GetComponent<PikminAttackBullet>();

         // nullじゃないなくて自分のピクミンだったらreturn
         if (playerController != null)
         {
            if (playerController.PlayerId == pikminAttackBullet.PlayerController.GetPhotonID()) return;
         }

         damageNum = maxHp * 3;
         if (_enemyContainer.Object.HasStateAuthority) Damage(damageNum, pikminAttackBullet.PlayerController.GetPhotonID());
         await _enemyTakeDamage.CallDamageText();
         SEManager.Instance.Play(SEPath.INGAME_PURGE_SOUND, hitVolume);
      }
   }

   public void Damage(int val, int photonId)
   {
      Debug.Log($"[Damage] _hp {_hp}: _hp ");
      
      _hp -= val; 
      if (_hp > 0) return;
      
      if (playerController != null) playerController.RemoveEngel(new []{this},false);
      
      // PhotonIdに紐づいたPlayerを取得
      GameObject player = null; 
      foreach (var playerRef in _enemyContainer.Runner.ActivePlayers)
      {
         if (playerRef.PlayerId == photonId)
         {
            player = _enemyContainer.Runner.GetPlayerObject(playerRef).gameObject;
         }
      }

      if (player == null) player = GameObject.Find($"Player{photonId}");

      if (player.TryGetComponent(out playerController))
      {
         _setEnemyIndexId.OnNext(new EnemyIndexId(_id, photonId));
         playerController.AddEngel(this);
      }
   }

   public int GetDamageNum()
   {
      var damageNum = LevelDesignSingleton.Instance.GetNormalAttackPower();
      return damageNum;
   }

   private int GetMoveRotation(Vector2 direction)
   {
      return direction.x > 0 ? 180 : 0;
   }

   private Vector2 GetPikminRandomMovePosition()
   {
      var delta = (Vector2) playerController.transform.position - (Vector2) transform.position;
      if (delta.sqrMagnitude > pikminStopMagnitude && _currentFrame > PIKMIN_RESET_MOVE_TARGET_FRAME)
      {
         _randomVector2 = new Vector2(Random.Range(-_moveRange, _moveRange), Random.Range(-_moveRange, _moveRange));
         _targetPos = (Vector2) playerController.transform.position + _randomVector2;
         _currentFrame = 0;
      }

      _currentFrame++;
      return _targetPos;
   }

   private async UniTask<Vector2> GetEnemyRandomMovePosition()
   {
      var delta = _targetPos - (Vector2) transform.position;
      if (delta.sqrMagnitude < enemyStopMagnitude)
      {
         var maxX = _enemyContainer.enemyMoveRangeMax.x;
         var maxY = _enemyContainer.enemyMoveRangeMax.y;
         var minX = _enemyContainer.enemyMoveRangeMin.x;
         var minY = _enemyContainer.enemyMoveRangeMin.y;

         _targetPos = (Vector2) transform.position + (new Vector2(Random.Range(-3, 4), Random.Range(-3, 4)) * enemyRandomRangeScale);
         while (_targetPos.x > maxX || _targetPos.y > maxY || _targetPos.x < minX || _targetPos.y < minY)
         {
            _targetPos = (Vector2) transform.position + (new Vector2(Random.Range(-3, 4), Random.Range(-3, 4)) * enemyRandomRangeScale);
            await UniTask.DelayFrame(10, cancellationToken: _cts.Token);
            if (_cts.IsCancellationRequested) return Vector2.zero;
         }
      }

      return _targetPos;
   }

   public async UniTask ResetEnemy()
   {
      playerController = null;
      GetComponent<Collider2D>().enabled = false;
      _setEnemyIndexId.OnNext(new EnemyIndexId(_id, EnemyConfig.EnemySbyte));
      await UniTask.Delay(TimeSpan.FromSeconds(3f));
      GetComponent<Collider2D>().enabled = true;
   }

   public EnemyContainer GetEnemyContainer()
   {
      return _enemyContainer;
   }

   public int GetIndex()
   {
      return _id;
   }
   
   private void OnDestroy()
   {
      _cts?.Cancel();
   }
}
