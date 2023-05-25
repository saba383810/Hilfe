using System;
using DG.Tweening;
using UnityEngine;

namespace TeamB.Scripts.InGame.Stage
{
    public class BackgroundStage : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer backgroundSpriteRenderer;
        [SerializeField] private float animSpeed = 0.3f;
        private Sequence _sequence;
        
        // シングルトン参照
        private float _stageScale;

        private void Awake()
        {
            _stageScale = LevelDesignSingleton.Instance.GetStageScale();
        }

        private void Start()
        {
            gameObject.transform.localScale = new Vector3(_stageScale, _stageScale, 1f);
        }

        public void SetStageGrayOut(bool isActive)
        {
            if (isActive)
            {
                _sequence = DOTween.Sequence()
                    .Append(backgroundSpriteRenderer
                        .DOColor(new Color(0.7f, 0.7f, 0.7f), animSpeed));
            }
            else
            {
                _sequence = DOTween.Sequence()
                    .Append(backgroundSpriteRenderer
                        .DOColor(Color.white, animSpeed));
            }
        }
    }
}
