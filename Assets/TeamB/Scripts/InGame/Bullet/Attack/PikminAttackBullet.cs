using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PikminAttackBullet : MonoBehaviour
{
    [SerializeField] private CircleCollider2D circleCollider2D;
    
    // シングルトン参照
    private float destroyTime;
    private float _pikminAttackScale;
    
    public PlayerController PlayerController { get; set; }
    private EnemyType.Enemy EnemyType { get; set; }
    private CancellationTokenSource _cts;

    public async void Setup(PlayerController playerController,EnemyType.Enemy enemyType)
    {
        PlayerController = playerController;
        EnemyType = enemyType;
        
        destroyTime = LevelDesignSingleton.Instance.GetPikminAttackEffectTime(enemyType);
        _pikminAttackScale = LevelDesignSingleton.Instance.GetPikminAttackRange(enemyType);
        gameObject.transform.localScale = new Vector3(_pikminAttackScale, _pikminAttackScale, 1f);
        
        circleCollider2D.enabled = true;
        _cts = new CancellationTokenSource();
        
        await UniTask.Delay(TimeSpan.FromSeconds(destroyTime),cancellationToken:_cts.Token);
        if(_cts.IsCancellationRequested) return;
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
    }
}
