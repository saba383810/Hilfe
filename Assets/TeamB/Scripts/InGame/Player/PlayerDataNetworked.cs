using System;
using Fusion;
using TeamB.Scripts.Common.Event;
using UnityEngine;
using UnityEngine.Serialization;

public class PlayerDataNetworked : NetworkBehaviour
{
    [Networked] public int PlayerId { get; set; }
    [Networked] public int PlayerIndex { get; set; }
    [Networked(OnChanged = nameof(OnNickNameChanged))] public NetworkString<_16> NickName { get; set; }
    [Networked(OnChanged = nameof(OnEngelCntChanged))] public short EnemyTypeACnt { get; set; }
    [Networked(OnChanged = nameof(OnEngelCntChanged))] public short EnemyTypeBCnt { get; set; }
    [Networked(OnChanged = nameof(OnEngelCntChanged))] public short EnemyTypeCCnt { get; set; }

    private static IngameUIController _inGameUIController;
    

    public static void OnNickNameChanged(Changed<PlayerDataNetworked> playerInfo)
    {
        var nickName = playerInfo.Behaviour.NickName.ToString();
        var playerIndex = playerInfo.Behaviour.PlayerIndex;
        var playerTransform = playerInfo.Behaviour.transform;
        var photonId = playerInfo.Behaviour.PlayerId;
        var isLocalPlayer = photonId == playerInfo.Behaviour.Object.Runner.LocalPlayer.PlayerId;
        Debug.Log($"OnNickNameChanged NickName: {nickName}, playerIndex: {playerIndex}, photonId: {photonId}, isLocalPlayer: {isLocalPlayer}");
        if (_inGameUIController == null) _inGameUIController = FindAnyObjectByType<IngameUIController>();
        if(isLocalPlayer) return;
        _inGameUIController.InitializePlayerInfo(nickName,playerTransform,playerIndex);
    }

    public static void OnEngelCntChanged(Changed<PlayerDataNetworked> playerInfo)
    {
        var playerIdx = playerInfo.Behaviour.PlayerIndex;
        var photonId = playerInfo.Behaviour.PlayerId;
        var typeACnt = playerInfo.Behaviour.EnemyTypeACnt;
        var typeBCnt = playerInfo.Behaviour.EnemyTypeBCnt;
        var typeCCnt = playerInfo.Behaviour.EnemyTypeCCnt;
        
        var playerScore = typeACnt + typeBCnt + typeCCnt;
        var isLocalPlayer = photonId == playerInfo.Behaviour.Object.Runner.LocalPlayer.PlayerId;
        if (_inGameUIController == null) _inGameUIController = FindAnyObjectByType<IngameUIController>();
        if (isLocalPlayer)
        {
            //_inGameUIController.SetPlayerIndex(playerIdx);
            _inGameUIController.SetSelfScore(playerScore);
            _inGameUIController.SetPikminCnt(typeACnt, typeBCnt, typeCCnt);
        }
        else
            _inGameUIController.SetPlayerScore(playerScore,playerIdx);
        
    }
}
