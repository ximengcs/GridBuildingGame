using Cinemachine;
using UnityEngine;
using DG.Tweening;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime.Utilities;
using System;

namespace MH.GameScene.Core.Entites
{
    public class WorldCamera : Entity, IWorldCamera
    {
        private CinemachineVirtualCamera _camera;
        private Transform _followObject;

        private Vector3 _camPos;
        private float _maxDistance;
        private Rect _camRect;
        private Rect _viewRect;
        private float _rectCamSize;
        private Tween _tween;
        private Tween _sizeTween;

        private float _targetSize = 0;
        private int _minSize = 3;
        private int _maxSize = 11;

        private Action _posChangeEvent;

        public event Action PosChangeEvent
        {
            add { _posChangeEvent += value; }
            remove { _posChangeEvent -= value; }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);

            WorldView worldView = World.FindEntity<WorldView>();
            CinemachineBrain camBrain = Camera.main.GetComponent<CinemachineBrain>();
            _camera = (CinemachineVirtualCamera)camBrain.ActiveVirtualCamera;
            _followObject = new GameObject("CameraFollow").transform;
            _camera.Follow = _followObject;
            worldView.AddChild(_followObject);

            GameObject rectObj = new GameObject("ViewRect");
            rectObj.AddComponent<PolygonCollider2D>();
            worldView.AddChild(rectObj);
            _maxDistance = 2;
            _targetSize = _camera.m_Lens.OrthographicSize;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            Vector3 pos = Camera.main.transform.position;
            if (pos != _camPos)
            {
                _camPos = pos;
                _posChangeEvent?.Invoke();
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _tween?.Kill();
            _tween = null;
            _sizeTween?.Kill();
            _sizeTween = null;
        }

        public void SetRect(Vector2 min, Vector2 max)
        {
            _viewRect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            RefreshCamLimit();
        }

        public void SetSize(float value)
        {
            if (value == 0)
                return;

            _sizeTween?.Kill();
            _targetSize = Mathf.Clamp(_targetSize - value, _minSize, _maxSize);
            _sizeTween = DOTween.To(
                () => _camera.m_Lens.OrthographicSize,
                (v) => _camera.m_Lens.OrthographicSize = v,
                _targetSize, 0.2f).SetEase(Ease.Linear)
                .OnComplete(() => _sizeTween = null);
        }

        public void SetPos(Vector3 pos)
        {
            if (Camera.main.orthographicSize != _rectCamSize)
                RefreshCamLimit();

            pos = _followObject.position - pos;
            _followObject.position = _camRect.Clamp(pos, _maxDistance);
        }

        public void SetPosEnd(Vector3 pos)
        {
            pos = _followObject.position - pos;
            if (!_camRect.Contains(pos))
            {
                pos = _camRect.Clamp(pos);
                _tween?.Kill();
                _tween = _followObject.DOMove(pos, 0.2f).OnComplete(() => _tween = null);
            }
        }

        private void RefreshCamLimit()
        {
            // 计算最小和最大边界
            Vector2 minBounds = _viewRect.min;
            Vector2 maxBounds = _viewRect.max;

            // 计算相机视口大小
            Camera mainCamera = Camera.main;
            _rectCamSize = mainCamera.orthographicSize;
            float aspectRatio = mainCamera.aspect;

            float cameraHeight = _rectCamSize * 2;
            float cameraWidth = cameraHeight * aspectRatio;

            Vector2 cameraExtents = new Vector2(cameraWidth / 2, cameraHeight / 2);

            // 计算相机的移动范围
            Vector2 cameraMinBounds = minBounds + cameraExtents;
            Vector2 cameraMaxBounds = maxBounds - cameraExtents;
            _camRect = Rect.MinMaxRect(cameraMinBounds.x, cameraMinBounds.y, cameraMaxBounds.x, cameraMaxBounds.y);
        }
    }
}
