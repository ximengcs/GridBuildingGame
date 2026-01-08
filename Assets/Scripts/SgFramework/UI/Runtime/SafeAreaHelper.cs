using System;
using UnityEngine;
using UnityEngine.UI;

namespace SgFramework.UI
{
    public class SafeAreaHelper : MonoBehaviour
    {
        [SerializeField] private RectTransform safeArea;

        private void Awake()
        {
            CalculateSafeArea();
        }

        private void CalculateSafeArea()
        {
            var c = GetComponent<CanvasScaler>();
            var res = c.referenceResolution;
            var height = Screen.height;
            var top = Screen.safeArea.yMin;
            var bottom = height - Screen.safeArea.yMax;

            var screenRate = (float)Screen.width / Screen.height;
            var baseRate = 750f / 1334f;
            if (screenRate > baseRate)
            {
                var rate = 1f + (top + bottom) / height;
                res.y = 1334f * rate;
            }

            c.referenceResolution = res;
            var safeRect = Screen.safeArea;
            var anchorMin = safeRect.position;
            var anchorMax = safeRect.position + safeRect.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            safeArea.anchorMin = anchorMin;
            safeArea.anchorMax = anchorMax;
        }
    }
}