using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using sabanogames.Common.UI;
using SandBox.saba.Scripts;
using TeamB.Scripts.InGame.Stage;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Asteroids.HostSimple
{
    public class LocalInputPoller : MonoBehaviour, INetworkRunnerCallbacks
    {
        private const string JOY_STICK_KEY = "Movement";
        private CommonButton _attackButton;
        private CommonButton _aimingButton;
        private bool _isAimingAButtonEnabled = false;
        private bool _isAimingBButtonEnabled = false;
        private bool _isAimingCButtonEnabled = false;
        private bool _isAttackButtonEnabled = false;
        public bool InputEnabled { get; set; }

        private void Awake()
        {
            InputEnabled = false;
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            if(!InputEnabled) return;
            var localInput = new PlayerInput();
            var horizontal = UltimateJoystick.GetHorizontalAxis(JOY_STICK_KEY);
            var vertical = UltimateJoystick.GetVerticalAxis(JOY_STICK_KEY);
            localInput.Direction = new Vector2(horizontal,vertical);

            if (_isAimingAButtonEnabled)
            {
                localInput.AttackActions.Set(PikuminAttackType.AimingTypeA, true);
                _isAimingAButtonEnabled = false;
            }
            else if (_isAimingBButtonEnabled)
            {
                localInput.AttackActions.Set(PikuminAttackType.AimingTypeB,true);
                _isAimingBButtonEnabled = false;
            }
            else if (_isAimingCButtonEnabled)
            {
                localInput.AttackActions.Set(PikuminAttackType.AimingTypeC,true);
                _isAimingCButtonEnabled = false;
            }
            else if (_isAttackButtonEnabled)
            {
                localInput.AttackActions.Set(PikuminAttackType.Attack,true);
                _isAttackButtonEnabled = false;
            }

            input.Set(localInput);
        }
        
        public void Aiming(EnemyType.Enemy enemyType)
        {
            switch (enemyType)
            {
                case EnemyType.Enemy.TypeA:
                    _isAimingAButtonEnabled = true;
                    break;
                case EnemyType.Enemy.TypeB:
                    _isAimingBButtonEnabled = true;
                    break;
                case EnemyType.Enemy.TypeC:
                    _isAimingCButtonEnabled = true;
                    break;
                case EnemyType.Enemy.None:
                    return;
            }
            
        }

        public void Attack()
        {
            _isAttackButtonEnabled = true;
        }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
        {
        }

        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
        {
        }

        public void OnSceneLoadDone(NetworkRunner runner)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}
