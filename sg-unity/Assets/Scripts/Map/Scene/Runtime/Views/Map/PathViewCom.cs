using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Core.PathFinding;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Views
{
    public class PathViewCom : ComponentBase
    {
        private GameObject _inst;
        private LineRenderer _lineRender;
        private Vector3[] _points;

        public override void OnInit(Entity entity, object data)
        {
            base.OnInit(entity, data);
            InitAsync();
        }

        private async void InitAsync()
        {
            _inst = await Entity.World.Resource.LoadObject("PathView");
            _lineRender = _inst.GetComponent<LineRenderer>();
            Color color = Color.red;
            color.a = 0.8f;
            _lineRender.startColor = color;
            _lineRender.endColor = color;
            _lineRender.startWidth = 0.2f;
            _lineRender.endWidth = 0.2f;
            _lineRender.sortingLayerName = GameConst.SURFACE_LAYER;
            _lineRender.sortingOrder = GameConst.MAX_ORDER;
            RefreshPath();
        }

        public void SetPath(IPath<IGridEntity> path)
        {
            if (path != null)
            {
                _points = new Vector3[path.Count];
                for (int i = 0; i < path.Count; i++)
                    _points[i] = MathUtility.IndexToGamePos(path[i].Index);

                RefreshPath();
            }
            else
            {
                _lineRender.positionCount = 0;
            }
        }

        private void RefreshPath()
        {
            if (_lineRender != null && _points != null)
            {
                _lineRender.positionCount = _points.Length;
                for (int i = 0; i < _points.Length; i++)
                    _lineRender.SetPosition(i, _points[i]);
                _points = null;
            }
        }
    }
}
