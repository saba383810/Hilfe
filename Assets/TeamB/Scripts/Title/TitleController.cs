using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sabanogaems.AudioManager;
using Sabanogames.AudioManager;
using sabanogames.Common.UI;
using TeamB.Scripts.Common.API;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour
{
    [SerializeField] private CommonButton titleButton;
    [SerializeField] private Image titleLogo;
    [SerializeField] private TMP_Text tapToStartText;
    
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 60;
        BGMManager.Instance.Play(BGMPath.TITLE_BGM);

        Color logoColor = titleLogo.color;
        logoColor.a = 0f;
        titleLogo.color = logoColor;
        
        titleButton.OnClickDefendChattering.TakeUntilDestroy(gameObject).Subscribe( async _ =>
        {
            SEManager.Instance.Play(SEPath.TITLE_TAP_SOUND);
            // var popup = await PopupManager.ShowPopupAsync(PopupKey.LOADING_POPUP);
            if (APIClient.IsLoggedIn())
            {
                var user = await APIClient.Users.GetUser(Preferences.GetPlayerId());
                Debug.Log(user);
                SceneManager.LoadScene("OutGame");
                // popup.Hide();
            }
            else
            {
                // popup.Hide();
                await UniTask.Delay(TimeSpan.FromSeconds(1f));
                SceneManager.LoadScene("ADV");
            }
        });

        var logoSequence = DOTween.Sequence();
        logoSequence
            .Append(titleLogo.DOFade(1f, 2f))
            .Join(titleLogo.transform.DOLocalMove(new Vector3(0, titleLogo.transform.localPosition.y + 100, 0), 2f));
        var textSequence = DOTween.Sequence();
        textSequence
            .Append(tapToStartText.DOFade(0.1f, 0.5f))
            .Append(tapToStartText.DOFade(1f, 0.5f))
            .AppendInterval(2f)
            .SetLoops(-1);
    }
}
