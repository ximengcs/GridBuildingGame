using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Views
{
    public class GameLayer : IGameLayer
    {
        private GameObject _root;
        private GameObject _layerRoot;
        private string _mainLayerName;

        private Dictionary<string, LayerBase> _layers;

        public GameLayer(GameObject root, string layerName)
        {
            _root = root;
            _layerRoot = new GameObject(layerName);
            _layerRoot.transform.parent = root.transform;
            _mainLayerName = layerName;
            _layers = new Dictionary<string, LayerBase>();
        }

        public void Destroy()
        {
            foreach (LayerBase layer in _layers.Values)
                layer.OnDestroy();

            GameObject.Destroy(_layerRoot);
            _root = null;
            _layerRoot = null;
            _layers = null;
        }

        public void Hide()
        {
            _layerRoot.gameObject.SetActive(false);
        }

        public void Show()
        {
            _layerRoot.gameObject.SetActive(true);
        }

        public IObjectLayer GetObjectLayer(string name)
        {
            if (_layers.TryGetValue(name, out LayerBase layer))
            {
                return layer as IObjectLayer;
            }
            return null;
        }

        public IObjectLayer AddObjectLayer(string name, int sortingOrder)
        {
            if (!_layers.TryGetValue(name, out LayerBase layer))
            {
                layer = new ObjectLayer(_layerRoot, _mainLayerName, name, sortingOrder);
                _layers.Add(name, layer);
            }

            return layer as IObjectLayer;
        }

        public ITilemapLayer GetTilemapLayer(string name)
        {
            if (_layers.TryGetValue(name, out LayerBase layer))
            {
                return layer as ITilemapLayer;
            }
            return null;
        }

        public ITilemapLayer AddTilemapLayer(string name, TilemapRenderer.Mode mode, int sortingOrder)
        {
            if (!_layers.TryGetValue(name, out LayerBase layer))
            {
                layer = new TilemapLayer(_layerRoot, _mainLayerName, name, sortingOrder, mode);
                _layers.Add(name, layer);
            }

            return layer as ITilemapLayer;
        }

        public void RemoveLayer(string name)
        {
            if (_layers.ContainsKey(name))
                _layers.Remove(name);
        }
    }
}
