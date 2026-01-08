#if UNITY_EDITOR
using UnityEngine;
using System.Threading;
using MH.GameScene.Runtime;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime.Utilities;

namespace MM.MapEditors
{
    public class GridOutlineCom : ComponentBase, IUpdate
    {
        private float _camSize;
        private Vector3 _camPos;
        private Outline _outline;
        private MapEditorEntity _map;
        private bool _active;

        public override bool Active
        {
            get => _active;
            set
            {
                _active = value;
                RefreshShowState();
            }
        }

        public override void OnStart()
        {
            base.OnStart();
            _map = (MapEditorEntity)Entity;
            _map.RegisterInitFinish(InitHandler);
        }

        private void InitHandler()
        {
            UniTask.Create(LoadAsync, _destroyTokenSource.Token);
        }

        private async UniTask LoadAsync(CancellationToken token)
        {
            _outline = await Outline.Create(Entity, token);
            token.ThrowIfCancellationRequested();
            SceneViewCom sceneView = _map.GetCom<SceneViewCom>();
            IGameLayer gameLayer = sceneView.GetLayer(GameConst.SURFACE_LAYER);
            IObjectLayer objLayer = gameLayer.GetObjectLayer();
            objLayer.SetProp(_outline, Vector2Int.zero);
            _outline.SortingOrder++;
            RefreshShowState();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_outline != null)
            {
                _outline.Destroy();
                _outline = null;
            }

            _map = null;
        }

        void IUpdate.OnUpdate(float deltaTime)
        {
            if (_outline == null)
                return;

            Vector3 pos = Camera.main.transform.position;
            if (pos != _camPos)
            {
                _outline.SetIndex(MathUtility.GamePosToIndex(pos));
                _camPos = pos;
            }

            float size = Camera.main.orthographicSize;
            if (_camSize != size)
            {
                int resultHeight = Mathf.CeilToInt(size);
                int orginSize = resultHeight * 2;
                if (!Mathf.Approximately(size % orginSize, 0))
                {
                    resultHeight = (resultHeight + orginSize) / orginSize * (orginSize + 1);
                }
                int resultWidth = resultHeight * Screen.width / Screen.height;
                if (resultWidth % orginSize != 0)
                    resultWidth = (resultWidth + orginSize) / orginSize *  (orginSize + 1);
                _outline.SetSize(resultWidth, resultHeight);
                _camSize = size;
            }
        }

        private void RefreshShowState()
        {
            if (_outline != null)
                _outline.Active = _active;
        }

        private class Outline : IObjectView
        {
            private GameObject _inst;
            private SpriteRenderer _render;
            private IEntity _entity;

            public bool Active
            {
                get => _inst.activeSelf;
                set => _inst.SetActive(value);
            }

            public Color Color
            {
                get => _render.color;
                set => _render.color = value;
            }

            public string SortingLayer
            {
                get => _render.sortingLayerName;
                set => _render.sortingLayerName = value;
            }

            public int SortingOrder
            {
                get => _render.sortingOrder;
                set => _render.sortingOrder = value;
            }

            public static async UniTask<Outline> Create(IEntity bindEntity, CancellationToken token)
            {
                Outline outline = new Outline();
                outline._entity = bindEntity;
                await UniTask.Create(outline.InitAsync, token);
                token.ThrowIfCancellationRequested();
                return outline;
            }

            private async UniTask InitAsync(CancellationToken token)
            {
                _inst = await _entity.World.Resource.LoadObject("Outline");
                token.ThrowIfCancellationRequested();
                _render = _inst.GetComponentInChildren<SpriteRenderer>();
            }

            public void SetParent(Transform layerRoot)
            {

            }

            public void SetIndex(Vector2Int index)
            {
                _inst.transform.position = MathUtility.IndexToGamePos(index);
            }

            public void SetSize(int x, int y)
            {
                _render.size = new Vector2Int(x, y);
            }

            public void Destroy()
            {
                if (_inst)
                {
                    GameObject.Destroy(_inst);
                    _inst = null;
                }

                _render = null;
            }
        }
    }
}
#endif