using Common;
using TMPro;
using UnityEngine;

namespace Fresh23_N.Scripts.Common
{
    public class MessagePopup : Popup
    {
        [SerializeField] private TextMeshProUGUI captionText;
        [SerializeField] private TextMeshProUGUI bodyText;

        public void Setup(string caption, string body)
        {
            base.Setup();
            captionText.text = caption;
            bodyText.text = body;
        }
    }
}