using System.Collections.Generic;
using Cinemachine;
using Cysharp.Threading.Tasks;
using Fusion;
using UnityEngine;

public class PlayerSpawner : SimulationBehaviour, IPlayerJoined, IPlayerLeft, ISpawned
{
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab;
    [SerializeField] private Transform playerSpawnParent;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
    private bool _gameIsReady = false;
    private GameStateController _stateController;

    public async UniTask StartPlayerSpawner(GameStateController stateController)
    {
        _gameIsReady = true;
        _stateController = stateController;
        
        if(!Object.HasStateAuthority) return;
        foreach (var player in Runner.ActivePlayers)
        {
            Debug.Log($"[PLayerSpawner] player id {player.PlayerId}");
            var playerIndex = _stateController.GetPlayerIndex(player.PlayerId);
            SpawnPlayer(player,playerIndex);
            await UniTask.Yield();
        }
    }
    
    public void Spawned()
    {
        if (!Object.HasInputAuthority) return;
        
    }
    public void PlayerJoined(PlayerRef player)
    {
        if (Object.HasStateAuthority == false) return;
        if (_gameIsReady == false) return;
       //  SpawnPlayer(player);
        
    }
    
    private void SpawnPlayer(PlayerRef player,int index)
    {
        var playerId = player.PlayerId;
        var spawnPosition = spawnPoints[index].transform.position;
        var userName = _stateController.UserNameDictionary[playerId];
        
        var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPosition, Quaternion.identity, player);
        playerObject.gameObject.transform.SetParent(playerSpawnParent);
        
        var playerController = playerObject.GetComponent<PlayerController>();
        playerController.Setup(userName,player.PlayerId,index);
        
        Runner.SetPlayerObject(player, playerObject);
        _stateController.TrackNewPlayer(playerId,playerObject.transform);
    }

    public void PlayerLeft(PlayerRef player)
    {
        DespawnPlayer(player);
    }
    
    private void DespawnPlayer(PlayerRef player)
    {
        if (Runner.TryGetPlayerObject(player, out var spaceshipNetworkObject))
        {
            Runner.Despawn(spaceshipNetworkObject);
        }

        // Reset Player Object
        Runner.SetPlayerObject(player, null);
    }
    
}
