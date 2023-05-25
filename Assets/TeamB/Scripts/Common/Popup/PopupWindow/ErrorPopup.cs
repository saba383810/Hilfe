using Common;
using TMPro;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class ErrorPopup : Popup
    {
        [SerializeField] private TMP_Text errorText;

        public void Setup(string errorMessage)
        {
            base.Setup();
            errorText.text = errorMessage;
        }
    }
}