#if UNITY_EDITOR
using MM.MapEditors;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static MH.GameScene.UIs.MapEdit.AssistFeatureBar;

namespace MH.GameScene.UIs.MapEdit
{
    public class AssistFeatureBar : MonoBehaviour
    {
        public class EntryInfo
        {
            private GameObject _inst;
            private Button _btn;
            private Image _icon;
            private IAssistFeature _feature;

            public bool State { get; set; }

            public Color Color
            {
                get => _icon.color;
                set => _icon.color = value;
            }

            public EntryInfo(Transform tf)
            {
                _inst = tf.gameObject;
                _btn = tf.GetComponent<Button>();
                _icon = tf.GetComponent<Image>();
            }

            public void Register(IAssistFeature feature)
            {
                _feature = feature;
                _btn.onClick.AddListener(ClickHandler);
                feature.OnInit(this);
                TriggerState();
            }

            public void Reset()
            {
                Hide();
                _btn.onClick.RemoveAllListeners();
               _feature = null;
                State = false;
            }

            public void Show()
            {
                _inst.SetActive(true);
            }

            public void Hide()
            {
                _inst.SetActive(false);
            }

            private void ClickHandler()
            {
                State = !State;
                _feature.OnClick(this);
                TriggerState();
            }

            private void TriggerState()
            {
                if (State)
                    _feature.OnSelect(this);
                else
                    _feature.OnUnselect(this);
            }
        }

        private List<EntryInfo> features;
        private int addIndex;

        private void Start()
        {
            features = new List<EntryInfo>();
            foreach (Transform tf in transform)
            {
                EntryInfo info = new EntryInfo(tf);
                info.Hide();
                features.Add(info);
            }
            gameObject.SetActive(false);
        }

        public void Register(IAssistFeature feature)
        {
            if (addIndex < features.Count)
            {
                EntryInfo entry = features[addIndex];
                entry.Show();
                entry.Register(feature);
                gameObject.SetActive(true);
            }

            addIndex++;
        }

        public void Clear()
        {
            addIndex = 0;
            foreach (EntryInfo info in features)
                info.Reset();
            gameObject.SetActive(false);
        }
    }

    public interface IAssistFeature
    {
        void OnInit(EntryInfo info);
        void OnClick(EntryInfo info);
        void OnSelect(EntryInfo info);
        void OnUnselect(EntryInfo info);
    }

    public class AssistLineFeature : IAssistFeature
    {
        private GridOutlineCom outline;

        public AssistLineFeature(GridOutlineCom outlineCom)
        {
            outline = outlineCom;
        }

        public void OnInit(EntryInfo info)
        {
            info.State = true;
        }

        public void OnClick(EntryInfo info)
        {

        }

        public void OnSelect(EntryInfo info)
        {
            info.Color = Color.cyan;
            outline.Active = true;
        }

        public void OnUnselect(EntryInfo info)
        {
            info.Color = Color.white;
            outline.Active = false;
        }
    }

    public class ToCameraZeroFeature : IAssistFeature
    {
        public void OnInit(EntryInfo info)
        {

        }

        public void OnSelect(EntryInfo info)
        {

        }

        public void OnClick(EntryInfo info)
        {
            Vector3 pos = Camera.main.transform.position;
            pos.x = 0;
            pos.y = 0;
            Camera.main.transform.position = pos;
        }

        public void OnUnselect(EntryInfo info)
        {

        }
    }
}
#endif