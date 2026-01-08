using UnityEngine;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Views
{
    public class ObjectLayer : LayerBase, IObjectLayer
    {
        private GameObject _root;
        private GameObject _layerObj;
        private string _objLayerName;
        private string _sortingLayer;
        private int _soringOrder;
        private HashSet<IObjectView> _views;

        public Transform Root => _layerObj.transform;

        public override string SortingLayer => _sortingLayer;

        public override int SortingOrder => _soringOrder;

        public override string Name => _objLayerName;

        public ObjectLayer(GameObject root, string layerName, string objLayerName, int order)
        {
            _root = root;
            _objLayerName = objLayerName;
            _views = new HashSet<IObjectView>();
            _layerObj = new GameObject($"{nameof(ObjectLayer)}_{objLayerName}");
            _layerObj.transform.parent = root.transform;
            _sortingLayer = layerName;
            _soringOrder = order;
        }

        public void SetProp(IObjectView view, Vector2Int index)
        {
            view.SetIndex(index);
        }

        public void Add(IObjectView view)
        {
            _views.Add(view);
            view.SortingLayer = SortingLayer;
             view.SortingOrder = SortingOrder;
            view.SetParent(_layerObj.transform);
        }

        public void Remove(IObjectView view)
        {
            _views.Remove(view);
        }

        public override void OnDestroy()
        {
            GameObject.Destroy(_layerObj);
            _root = null;
            _layerObj = null;
            _views = null;
        }
    }
}
