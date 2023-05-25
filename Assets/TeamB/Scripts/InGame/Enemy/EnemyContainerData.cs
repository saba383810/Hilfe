using Fusion;
using TeamB.Scripts.InGame.Enemy;
using UnityEngine;

public class EnemyContainerData : NetworkBehaviour
{
    [Networked, Capacity(EnemyConfig.EnemyCount)] public NetworkArray<Vector2> EnemyArray => default;

    [Networked,Capacity(EnemyConfig.EnemyCount)] public NetworkArray<sbyte> PlayerIdArray => default;
    [Networked,Capacity(EnemyConfig.EnemyCount)] public NetworkArray<byte> EnemyTypeArray => default;

}
