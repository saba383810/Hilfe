using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Fusion;
using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TeamB.Scripts.InGame.Enemy
{
    public struct EnemyIndexId
    {
        public int Index { get; set; }
        public int PhotonId { get; set; }

        public EnemyIndexId(int index, int id)
        {
            Index = index;
            PhotonId = id;
        }
    }

    public class EnemyContainer : NetworkBehaviour
    {
        [SerializeField] private GameObject enemyPrefab;
        [SerializeField] public float stageScale = 7;
        
        private static int EnemyMax => EnemyConfig.EnemyCount;
        private static int SyncMagnification => EnemyConfig.SyncMagnification;

        private readonly List<EnemyController> _activeEnemyControllers = new(EnemyConfig.EnemyCount);
        private EnemyContainerData _enemyContainerData;
        private byte _additionalNum;
        private Subject<EnemyIndexId> _setEnemyPhotonId;
        private List<EnemyType.Enemy> _generateEnemyTypeList;
        private Vector2 _enemyGenerateRange;
        private Vector2 _enemyGenerateStartPos;
        
        public Vector2 enemyMoveRangeMax;
        public Vector2 enemyMoveRangeMin;
        
        public override void Spawned()
        {
            _enemyContainerData = GetComponent<EnemyContainerData>();
            _enemyGenerateRange = LevelDesignSingleton.Instance.GetEnemyGenerateRange();
            _enemyGenerateStartPos = LevelDesignSingleton.Instance.GetEnemyGenerateStartPos();
            enemyMoveRangeMax = LevelDesignSingleton.Instance.GetEnemyStageForbiddenAreaMax();
            enemyMoveRangeMin = LevelDesignSingleton.Instance.GetEnemyStageForbiddenAreaMin();
            GenerateEnemy().Forget();
        }

        private async UniTask GenerateEnemy()
        {
            _setEnemyPhotonId?.Dispose();
            _setEnemyPhotonId = new Subject<EnemyIndexId>();
            _setEnemyPhotonId.TakeUntilDestroy(gameObject).Subscribe(val => SetEnemyPhotonId(val.Index,val.PhotonId));
            if (Object.HasStateAuthority)
            {
                var enemyTypes = GetEnemyTypes();
                var enemyTypesCastIntList = enemyTypes.ConvertAll(x => (byte)x);
                var index = 0;
                foreach (var enemyTypeByte in enemyTypesCastIntList)
                {
                    _enemyContainerData.EnemyTypeArray.Set(index, enemyTypeByte);
                    index++;
                }
            }
            else
            {
                if (!Object.HasStateAuthority) await UniTask.Delay(TimeSpan.FromSeconds(1f));
            }
            for (var i = 0; i < EnemyMax; i++)
            {
                var obj = Instantiate(enemyPrefab, GetGeneratePosition(i), Quaternion.identity);
                obj.transform.SetParent(transform);
                var enemyController = obj.GetComponent<EnemyController>();
                enemyController.Setup(i, EnemyType.Enemy.TypeA, _setEnemyPhotonId, this);
                _enemyContainerData.EnemyArray.Set(i, obj.transform.position);
                _activeEnemyControllers.Add(enemyController);
            
                if (Object.HasStateAuthority) _enemyContainerData.PlayerIdArray.Set(i,-1);
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority) return;
            // 座標の保持
            var index = 0;
            var cnt = 0;
            while (_activeEnemyControllers.Count > cnt)
            {
                index++;
                _enemyContainerData.EnemyArray.Set(cnt, _activeEnemyControllers[cnt].transform.position);
                cnt = index * SyncMagnification - _additionalNum;
            }
            _additionalNum++;
            if (_additionalNum == SyncMagnification) _additionalNum = 0;
        }
    
        public override void Render()
        {
            // Hostだけ実行
            if (!Object.HasStateAuthority) return;
            for (var i = 0; i < _activeEnemyControllers.Count; i++)
            {
                _activeEnemyControllers[i].FollowTarget(Runner.DeltaTime * SyncMagnification);
            }
        }

        private void Update()
        {
            // ローカルで見た目を反映
            for (var i = 0; i < _activeEnemyControllers.Count; i++)
            {
                // networkに保持されている座標に移動
                var pos = _enemyContainerData.EnemyArray[i];
                var playerId = _enemyContainerData.PlayerIdArray[i];
                var enemyTypeByte = _enemyContainerData.EnemyTypeArray[i];
                _activeEnemyControllers[i].CheckAnimation(playerId);
                _activeEnemyControllers[i].Render(pos,Time.deltaTime);
                _activeEnemyControllers[i].CheckEnemyType((EnemyType.Enemy)enemyTypeByte);
            }
        }

        private Vector3 GetGeneratePosition(int index)
        {
            var xIndex = index % 10;
            var yIndex = index / 10;
            var pivotX = _enemyGenerateRange.x;
            var pivotY = _enemyGenerateRange.y;
            var startPosX = _enemyGenerateStartPos.x;
            var startPosY = _enemyGenerateStartPos.y;
            return new Vector3((xIndex * pivotX) + startPosX,(yIndex * pivotY) + startPosY,0);
        }

        private void SetEnemyPhotonId(int enemyId, int photonId)
        {
            _enemyContainerData.PlayerIdArray.Set(enemyId, (sbyte)photonId);
        }

        private List<EnemyType.Enemy> GetEnemyTypes()
        {
            var votePercentData = Preferences.GetVotePercentData();
            Debug.Log($"Grimy: {votePercentData.Grimy} Fang: {votePercentData.Fangqulite} Melo:{votePercentData.Melogardia}");

            var typeARange = votePercentData.Grimy;
            var typeBRange = votePercentData.Grimy + votePercentData.Fangqulite;

            var enemies = new List<EnemyType.Enemy>();
            for (var i = 0; i < EnemyMax; i++)
            {
                var rand = Random.Range(0, 101);
                if (rand <= typeARange)
                {
                    enemies.Add(EnemyType.Enemy.TypeA);
                }
                else if (rand <= typeBRange)
                {
                    enemies.Add(EnemyType.Enemy.TypeB);
                }
                else
                {
                    enemies.Add(EnemyType.Enemy.TypeC);
                }
            }
            return enemies;
        }

        public void SetEnemyPosition(int index,Vector2 pos)
        {
            RPC_SetEnemyPosition(index,pos);
        }

        [Rpc(RpcSources.All, RpcTargets.All)]
        public void RPC_SetEnemyPosition(int index, Vector3 pos)
        {
            if (!Object.HasStateAuthority) return;
            _enemyContainerData.EnemyArray.Set(index, pos);
        }

        [Rpc(RpcSources.All,RpcTargets.All)]
        public void RPC_ResetPositionEnemy(int index)
        {
            _activeEnemyControllers[index].ResetEnemy().Forget();
        }
    }
}