using sabanogames.Common.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TeamB.Scripts.InGame.UI
{
    public class SkillButtonView : MonoBehaviour
    {
        [SerializeField] private Image gage;
        [SerializeField] private GameObject undo;
        [SerializeField] private TMP_Text skillCntText;
        [SerializeField] private CommonButton button;
        [SerializeField] private GameObject shot;
        
        public void Setup(int enemyCnt, int usableEnemyCnt)
        {
            gage.fillAmount = (float)enemyCnt / usableEnemyCnt;
            undo.SetActive(enemyCnt < usableEnemyCnt);
            skillCntText.text = (enemyCnt / usableEnemyCnt).ToString();
        }

        public void SetActiveShot(bool isActive)
        {
            shot.SetActive(isActive);
        }
    }
}
