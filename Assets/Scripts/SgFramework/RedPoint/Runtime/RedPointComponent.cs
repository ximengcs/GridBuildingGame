using System;
using TMPro;
using UnityEngine;

namespace SgFramework.RedPoint
{
    public class RedPointComponent : MonoBehaviour
    {
        public string path;
        [SerializeField] private GameObject objRedPoint;
        [SerializeField] private TextMeshProUGUI txtRedCount;

        private IDisposable _disposable;

        private void Awake()
        {
            SetPath(path);
        }

        public void SetPath(string newPath)
        {
            _disposable?.Dispose();
            _disposable = null;

            path = newPath;
            _disposable = RedPointManager.Instance.Subscribe(this);
        }

        public void SetData(LogicNode node)
        {
            objRedPoint.SetActive(node.Valid);
            if (txtRedCount == null)
            {
                return;
            }

            txtRedCount.SetText($"{node.Value}");
        }
    }
}