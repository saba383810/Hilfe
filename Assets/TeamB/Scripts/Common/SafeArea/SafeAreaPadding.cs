using UnityEngine;

namespace Fresh23_N.Scripts.Common
{
    [RequireComponent(typeof(RectTransform))]
    [ExecuteAlways]
    public class SafeAreaPadding : MonoBehaviour
    {
        private DeviceOrientation _postOrientation;
        private RectTransform _rectTransform;

        private void Start()
        {
            _rectTransform = GetComponent<RectTransform>();
        }


        private void Update()
        {
            if (Input.deviceOrientation != DeviceOrientation.Unknown && _postOrientation == Input.deviceOrientation)
            {
                return;
            }

            _postOrientation = Input.deviceOrientation;

            var area = Screen.safeArea;

            _rectTransform.sizeDelta = Vector2.zero;
            _rectTransform.anchorMax = new Vector2(area.xMax / Screen.width, area.yMax / Screen.height);
            _rectTransform.anchorMin = new Vector2(area.xMin / Screen.width, area.yMin / Screen.height);
        }
    }
}