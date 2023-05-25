using Common;
using UnityEngine;

namespace TeamB.Scripts.Common
{
    public class AfterTutorialPopup : Popup
    {
        [SerializeField] private RectTransform backGround;
        [SerializeField] private RectTransform body;

        public override void Setup()
        {
            base.Setup();
            var mul = backGround.rect.size.x / body.rect.size.x;
            body.localScale = new Vector3(mul, mul, mul);
        }
    }
}