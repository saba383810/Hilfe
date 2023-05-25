using System;
using System.Collections.Generic;
using Asteroids.HostSimple;
using Fusion;
using UnityEngine;

public class PlayerController : NetworkBehaviour
{
    [SerializeField] private Transform aiming;
    [SerializeField] private ChangeAimingView changeAimingView;
    [SerializeField] private PlayerDataNetworked playerDataNetworked;
    [NonSerialized] public int PlayerId;
    [NonSerialized] public int PlayerIndex;

    private IngameUIController _inGameUIController;
    private Vector3 _prevPosition;
    private List<EnemyController> _engelTypeAList = new ();
    private List<EnemyController> _engelTypeBList = new ();
    private List<EnemyController> _engelTypeCList = new ();

    public void Setup(string userName, int playerId, int playerIndex)
    {
        playerDataNetworked.NickName = userName;
        playerDataNetworked.PlayerId = playerId;
        playerDataNetworked.PlayerIndex = playerIndex;
        gameObject.name = $"Player{playerId}";
        RPC_SetGameObjectName(playerId,playerIndex);
    }
    
    public override void Spawned()
    {
        _engelTypeAList = new List<EnemyController>();
        _inGameUIController = FindAnyObjectByType<IngameUIController>();
        var localInputPoller = FindAnyObjectByType<LocalInputPoller>();
        localInputPoller.enabled = true;
    }

    private void FixedUpdate()
    {
        ChangeAimingDirection();
    }

    public List<EnemyController> GetEngelList(EnemyType.Enemy enemyType)
    {
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                return _engelTypeAList;
            case EnemyType.Enemy.TypeB:
                return _engelTypeBList;
            case EnemyType.Enemy.TypeC:
                return _engelTypeCList;
        }
        Debug.LogError("エネミータイプが不正値だった");
        return _engelTypeAList;
    }

    private void ChangeAimingDirection()
    {
        var position = transform.position;
        var delta = position - _prevPosition;
        if (delta.sqrMagnitude == 0) return;
        
        var angle = Vector2Extensions.Vector2ToAngle(delta);
        _prevPosition = position;
   
        aiming.rotation = Quaternion.Euler(0,0,angle);
    }

    public void AddEngel(EnemyController enemyController)
    {
        switch (enemyController.enemyType)
        {
            case EnemyType.Enemy.TypeA:
                _engelTypeAList.Add(enemyController);
                playerDataNetworked.EnemyTypeACnt++;
                break;
            case EnemyType.Enemy.TypeB:
                _engelTypeBList.Add(enemyController);
                playerDataNetworked.EnemyTypeBCnt++;
                break;
            case EnemyType.Enemy.TypeC:
                _engelTypeCList.Add(enemyController);
                playerDataNetworked.EnemyTypeCCnt++;
                break;
        }
    }
    
    public void RemoveEngel(EnemyController[] enemyControllers, bool isUseEngel)
    {
        foreach (var enemyController in enemyControllers)
        {
            if(isUseEngel) enemyController.GetEnemyContainer().RPC_ResetPositionEnemy(enemyController.GetIndex());
            
            switch (enemyController.enemyType)
            {
                case EnemyType.Enemy.TypeA:
                    _engelTypeAList.Remove(enemyController);
                    playerDataNetworked.EnemyTypeACnt--;
                    break;
                case EnemyType.Enemy.TypeB:
                    _engelTypeBList.Remove(enemyController);
                    playerDataNetworked.EnemyTypeBCnt--;
                    break;
                case EnemyType.Enemy.TypeC:
                    _engelTypeCList.Remove(enemyController);
                    playerDataNetworked.EnemyTypeCCnt--;
                    break;
            }
        }
    }

    public void UseEngel(EnemyType.Enemy enemyType,int useCnt)
    {
        EnemyController[] enemyControllers;
        switch (enemyType)
        {
            case EnemyType.Enemy.TypeA:
                if(_engelTypeAList.Count == 0) return;
                enemyControllers = _engelTypeAList.GetRange(0,useCnt).ToArray();
                break;
            case EnemyType.Enemy.TypeB:
                if(_engelTypeBList.Count == 0) return;
                enemyControllers = _engelTypeBList.GetRange(0, useCnt).ToArray();
                break;
            case EnemyType.Enemy.TypeC:
                if(_engelTypeCList.Count == 0) return;
                enemyControllers = _engelTypeCList.GetRange(0, useCnt).ToArray();
                break;
            default:
                return;
        }
        _inGameUIController.ShowUseEngelTextAnimation();
        RemoveEngel(enemyControllers,true);
    }

    public int GetPhotonID()
    {
        return playerDataNetworked.PlayerId;
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RPC_SetGameObjectName(int playerId, int playerIndex)
    {
        gameObject.name = $"Player{playerId}";
        this.PlayerId = playerId;
        PlayerIndex = playerIndex;
        changeAimingView.Setup((Player.PlayerType)playerIndex);
        GetComponent<PlayerMovementController>().SetAnimationType(playerIndex);
    }
}
