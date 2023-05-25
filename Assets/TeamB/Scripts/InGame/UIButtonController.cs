using System;
using System.ComponentModel.Design;
using Asteroids.HostSimple;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using sabanogames.Common.UI;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.InGame
{
    public class UIButtonController : MonoBehaviour
    {
        [SerializeField] private CommonButton aimingTypeAButton;
        [SerializeField] private CommonButton aimingTypeBButton;
        [SerializeField] private CommonButton aimingTypeCButton;

        [SerializeField] private Image reCastImageA;
        [SerializeField] private Image reCastImageB;
        [SerializeField] private Image reCastImageC;
        private float _recastTime = 0;
        private LocalInputPoller _localInputPoller;

        public EnemyType.Enemy currentAttackType = EnemyType.Enemy.None;

        private void Start()
        {
            _recastTime = LevelDesignSingleton.Instance.GetCoolTime();
            _localInputPoller = FindAnyObjectByType<LocalInputPoller>();
            
            aimingTypeAButton.OnClick.TakeUntilDestroy(gameObject).Subscribe(_ =>
            {
                if (currentAttackType != EnemyType.Enemy.TypeA)
                {
                    _localInputPoller.Aiming(EnemyType.Enemy.TypeA);
                    currentAttackType = EnemyType.Enemy.TypeA;
                }
                else
                {
                    _localInputPoller.Attack();
                    currentAttackType = EnemyType.Enemy.None;
                }
            });
            
            aimingTypeBButton.OnClick.TakeUntilDestroy(gameObject).Subscribe(_ =>
            {
                if (currentAttackType != EnemyType.Enemy.TypeB)
                {
                    _localInputPoller.Aiming(EnemyType.Enemy.TypeB);
                    currentAttackType = EnemyType.Enemy.TypeB;
                }
                else
                {
                    _localInputPoller.Attack();
                    currentAttackType = EnemyType.Enemy.None;
                }
            });
            
            aimingTypeCButton.OnClick.TakeUntilDestroy(gameObject).Subscribe(_ =>
            {
                if (currentAttackType != EnemyType.Enemy.TypeC)
                {
                    _localInputPoller.Aiming(EnemyType.Enemy.TypeC);
                    currentAttackType = EnemyType.Enemy.TypeC;
                }
                else
                {
                    _localInputPoller.Attack();
                    currentAttackType = EnemyType.Enemy.None;
                }
            });
        }

        public void SetReCast()
        {
            Debug.Log("SetRecast");
            Recast(aimingTypeAButton, reCastImageA).Forget();
            Recast(aimingTypeBButton, reCastImageB).Forget();
            Recast(aimingTypeCButton, reCastImageC).Forget();
        }

        public async UniTask Recast(CommonButton button,Image image)
        {
            button.SetEnabled(false);
            image.fillAmount = 0;
            image.DOFillAmount(1, _recastTime);
            await UniTask.Delay(TimeSpan.FromSeconds(_recastTime));
            image.fillAmount = 1;
            button.SetEnabled(true);

        }
    }
}
