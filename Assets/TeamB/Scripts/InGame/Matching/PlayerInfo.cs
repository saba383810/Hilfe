using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.InGame.Matching
{
    public class PlayerInfo : MonoBehaviour
    {
        [SerializeField] private TMP_Text nameText;
        [SerializeField] private Image image;   
        [SerializeField] private Sprite[] animationSprites;
        [SerializeField] private float animationSpeed;
        private CancellationTokenSource _cts;
        public void Setup(string userName)
        {
            _cts ??= new CancellationTokenSource();
            ShowPlayerAnimation(_cts.Token).Forget();
            nameText.text = userName;
            gameObject.SetActive(true);
        }

        public async UniTask ShowPlayerAnimation(CancellationToken token)
        {
            if(_cts == null) _cts = new CancellationTokenSource();
            while (true)
            {
                foreach (var sprite in animationSprites)
                {
                    image.sprite = sprite;
                    await UniTask.Delay(TimeSpan.FromSeconds(animationSpeed), cancellationToken: token);
                    if(token.IsCancellationRequested) return;
                }
                await UniTask.Yield(cancellationToken:token);
                if(token.IsCancellationRequested) return;
            }
        }

        public void Disable()
        {
            
            gameObject.SetActive(false);
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
        }
    }
}
