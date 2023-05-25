using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;

public class BulletContainer : NetworkBehaviour
{
    [SerializeField] private GameObject normalAttackBullet;
    private readonly List<NormalAttackBullet> _activeBullets = new(1024);

    private int _bulletCnt = 0;

    public override void FixedUpdateNetwork()
    {
        if(_activeBullets.Count == 0) return;

        foreach (var bullet in _activeBullets.Where(bullet => !bullet.IsAlive))
        {
            _activeBullets.Remove(bullet);
            Destroy(bullet.gameObject);
            return;
        }
    }

    public override void Render()
    {
        foreach (var bullet in _activeBullets)
        {
            bullet.Render(Runner.DeltaTime);
        }
    }
    
    /// <summary>
    ///   Playerがたまを飛ばす命令を送る
    /// </summary>
    [Rpc(RpcSources.All, RpcTargets.All, TickAligned = true)]
    public void RPC_FireBullet(Vector2 initializePosition,float angle, int photonId)
    {
        var bulletInfo = new NormalAttackBullet.BulletInfo(photonId, initializePosition, angle);
        FireNormalAttackBullet(bulletInfo);
    }

    private void FireNormalAttackBullet(NormalAttackBullet.BulletInfo bulletInfo)
    {
        var obj = Instantiate(normalAttackBullet, bulletInfo.InitializePosition, Quaternion.identity, transform);
        var bullet = obj.GetComponent<NormalAttackBullet>();
        _bulletCnt++;
        bullet.Setup(bulletInfo,_bulletCnt);
        _activeBullets.Add(bullet);
    }

    public List<NormalAttackBullet> GetActiveBullets()
    {
        return _activeBullets;
    }

    [Rpc(RpcSources.All,RpcTargets.All)]
    public void RPC_SetIsAliveFalse(int id)
    {
        var bullet = _activeBullets.FirstOrDefault(bullet => bullet.BulletId == id);
        if(bullet != null) bullet.SetIsAliveFalse();
    }
}
