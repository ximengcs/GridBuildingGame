#if UNITY_EDITOR
using System;
using UnityEngine;
using MM.MapEditors;
using System.Collections.Generic;
using MH.GameScene.Core.Entites;

namespace MH.GameScene.UIs.MapEdit
{
    public partial class MapEditorUI : MonoBehaviour
    {
        [SerializeField]
        private GameObject menusNode;

        [SerializeField]
        private GameObject panelNode;

        [SerializeField]
        private GridInfoUI gridInfo;

        [SerializeField]
        private AssistFeatureBar assistFeatureBar;

        private IEditFeature _current;
        private IFeaturePanel _currentPanel;
        private Dictionary<Type, IEditFeature> _features;
        private Dictionary<Type, IFeaturePanel> _panels;

        public World World { get; set; }

        public MapEditorEntity Scene { get; private set; }

        private void Awake()
        {
            gridInfo.OnInit(this);
            gridInfo.Hide();
            _features = new Dictionary<Type, IEditFeature>();
            _panels = new Dictionary<Type, IFeaturePanel>();
            foreach (Transform inst in menusNode.transform)
            {
                IEditFeature feat = inst.GetComponent<IEditFeature>();
                if (feat == null) continue;
                feat.OnInit(this);
                feat.OnExit();
                feat.SetEnable(false);
                _features.Add(feat.GetType(), feat);
            }

            foreach (IFeaturePanel panel in panelNode.GetComponentsInChildren<IFeaturePanel>(true))
            {
                panel.OnHide();
                panel.OnInit(this);
                _panels.Add(panel.GetType(), panel);
            }

            SettingFeature setting = GetFeature<SettingFeature>();
            setting.SetEnable(true);
            setting.SelectFeature();
        }

        public void UpdateInfo(string layer, Vector2Int index)
        {
            gridInfo.UpdateInfo(layer, index);
        }

        public void SetScene(MapEditorEntity scene)
        {
            this.Scene = scene;

            if (scene != null)
            {
                foreach (var featureEntry in _features)
                    featureEntry.Value.OnSceneLoad(scene);

                GetFeature<PaintFeature>().SetEnable(true);
                GetFeature<ScanLayerFeature>().SetEnable(true);
                GetFeature<MoveFeature>().SetEnable(true);
                GetFeature<NpcFeature>().SetEnable(true);
                GetFeature<AreaFeature>().SetEnable(true);
                assistFeatureBar.Register(new AssistLineFeature(scene.GetCom<GridOutlineCom>()));
                assistFeatureBar.Register(new ToCameraZeroFeature());
            }
            else
            {
                foreach (var featureEntry in _features)
                    featureEntry.Value.OnSceneExit();

                GetFeature<PaintFeature>().SetEnable(false);
                GetFeature<ScanLayerFeature>().SetEnable(false);
                GetFeature<MoveFeature>().SetEnable(false);
                GetFeature<NpcFeature>().SetEnable(false);
                GetFeature<AreaFeature>().SetEnable(false);
                assistFeatureBar.Clear();
            }
        }

        public void OnSelectFeature(IEditFeature feature)
        {
            _current?.OnExit();
            _current = feature;
            _current.OnEnter();
        }

        public T GetPanel<T>() where T : IFeaturePanel
        {
            if (_panels.TryGetValue(typeof(T), out IFeaturePanel panel))
                return (T)panel;
            return default;
        }

        public T ShowPanel<T>() where T : IFeaturePanel
        {
            _currentPanel?.OnHide();
            if (_panels.TryGetValue(typeof(T), out IFeaturePanel panel))
            {
                _currentPanel = panel;
                _currentPanel.OnShow();
            }
            return (T)_currentPanel;
        }

        public T GetFeature<T>() where T : IEditFeature
        {
            if (_features.TryGetValue(typeof(T), out IEditFeature feature))
            {
                return (T)feature;
            }
            return default;
        }
    }
}
#endif