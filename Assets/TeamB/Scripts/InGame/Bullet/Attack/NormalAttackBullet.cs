using System;
using DG.Tweening;
using Fusion;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 通常攻撃弾
/// </summary>
public class NormalAttackBullet : MonoBehaviour
{
    public class BulletInfo
    {
        public int PhotonId { get; set; }
        public Vector2 InitializePosition { get; set; }
        public float BulletAngle { get;  set; }
        public float Speed { get; set; }
        public float DestroyTime { get; set; }
        public BulletInfo(int photonId,Vector2 initializePosition, float bulletAngle)
        {
            PhotonId = photonId;
            InitializePosition = initializePosition;
            BulletAngle = bulletAngle;
        }
    }

    [SerializeField] private BoxCollider2D boxCollider2D;
    [SerializeField] private float speed = 2f;
    [NonSerialized] public bool IsAlive = true;
    [NonSerialized] public BulletInfo BulletData;
    [NonSerialized] public int BulletId;
    private Vector2 _bulletDirection;

    // シングルトン参照
    //[SerializeField] private float destroyTime = 5f;
    private float destroyTime;

    private void Awake()
    {
        destroyTime = LevelDesignSingleton.Instance.GetNormalBulletDestroyTime();
    }

    public void Setup(BulletInfo bulletInfo ,int bulletID)
    {
        BulletData = bulletInfo;
        BulletId = bulletID;

        bulletInfo.Speed = speed;
        bulletInfo.DestroyTime = destroyTime;
        transform.position = bulletInfo.InitializePosition;
        _bulletDirection = Vector2Extensions.AngleToVector2(bulletInfo.BulletAngle);
        Invoke(nameof(SetIsAliveFalse),destroyTime);
    }

    public void Render(float deltaTime)
    {
        Vector2 currentPos = transform.position;
        Vector2 targetPos = currentPos + _bulletDirection * BulletData.Speed * deltaTime;
        
        transform.position = Vector2.MoveTowards(currentPos, targetPos,100);
    }

    public void SetIsAliveFalse()
    {
        SetIsAlive(false);
    }
    private void SetIsAlive(bool isActive)
    {
        IsAlive = isActive;
        gameObject.SetActive(false);
    }
}
