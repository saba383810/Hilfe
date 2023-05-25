using Fusion;
using UnityEngine;

namespace SandBox.saba.Scripts
{
    enum PikuminAttackType
    {
        None,
        AimingTypeA,
        AimingTypeB,
        AimingTypeC,
        Attack
    }

    public struct PlayerInput : INetworkInput
    {
        public Vector2 Direction;
        public NetworkButtons AttackActions;
    }
}