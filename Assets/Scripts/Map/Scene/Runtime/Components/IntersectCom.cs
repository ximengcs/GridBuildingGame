using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Entities
{
    public class IntersectCom : ComponentBase, IUpdate, ITriggerModule
    {
        private List<IScenentersect> _intersects;
        private bool _startTouch;
        private bool _checkMove;

        public bool CheckMove
        {
            get => _checkMove;
            set => _checkMove = value;
        }

        public override void OnInit(Entity entity, object data)
        {
            base.OnInit(entity, data);
            _checkMove = false;
            _intersects = new List<IScenentersect>();
            TriggerExtension.Initialize(this);
        }

        public void Register(IScenentersect subject)
        {
            _intersects.Add(subject);
        }

        public void Unregister(IScenentersect subject)
        {
            _intersects.Remove(subject);
        }

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        void IUpdate.OnUpdate(float deltaTime)
        {
            if (UnityUtility.IsPointerOverGameObject() && !_startTouch)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _startTouch = true;
                OnPointDown(Input.mousePosition);
            }
            else if (Input.GetMouseButton(0))
            {
                if (_startTouch)
                {
                    OnPointing(Input.mousePosition);
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                if (_startTouch)
                {
                    OnPointUp(Input.mousePosition);
                    _startTouch = false;
                }
            }

            if (_checkMove)
                OnMoving(Input.mousePosition);

            float scrollWheel = Input.GetAxis("Mouse ScrollWheel");
            if (scrollWheel != 0)
            {
                Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.2f, 0));
                float rate = Mathf.Max(1, Mathf.Abs(worldPos.x));
                OnScale(scrollWheel * rate);
            }
        }

#elif UNITY_IPHONE || UNITY_ANDROID
        private float _lastTouchDis;
        private Vector2 _lastTouch1;
        private Vector2 _lastTouch2;

        void IUpdate.OnUpdate(float deltaTime)
        {
            if (UnityUtility.IsPointerOverGameObject())
            {
                if (_startTouch && Input.touchCount > 0)
                    OnPointUp(Input.GetTouch(0).position);
                _startTouch = false;
                return;
            }

            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                if (touch1.phase == TouchPhase.Began || touch2.phase == TouchPhase.Began)
                {
                    float dis = Vector2.Distance(touch1.position, touch2.position);
                    _lastTouchDis = dis;
                }
                else if (touch1.phase == TouchPhase.Moved || touch2.phase == TouchPhase.Moved)
                {
                    Vector2 dir1 = touch1.position - _lastTouch1;
                    Vector2 dir2 = touch2.position - _lastTouch2;
                    float dot = Vector2.Dot(dir1, dir2);
                    float dis = Vector2.Distance(touch1.position, touch2.position);

                    if (dot < 0)
                    {
                        float value = dis - _lastTouchDis;
                        float disWorld = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Abs(value), 0)).x;
                        float scaleValue = disWorld * 0.2f * (value < 0 ? 1 : -1);
                        OnScale(scaleValue);

                        if (_startTouch)
                        {
                            _startTouch = false;
                            OnPointUp(touch1.position);
                        }
                    }
                    else
                    {
                        if (!_startTouch)
                        {
                            _startTouch = true;
                            OnPointDown(touch1.position);
                        }
                        else
                        {
                            OnPointing(touch1.position);
                        }
                    }

                    _lastTouchDis = dis;
                }
                else if (touch1.phase == TouchPhase.Ended || touch2.phase == TouchPhase.Ended)
                {
                    if (_startTouch)
                    {
                        _startTouch = false;
                        OnPointUp(touch1.position);
                    }
                }

                _lastTouch1 = touch1.position;
                _lastTouch2 = touch2.position;
            }

            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _startTouch = true;
                        OnPointDown(touch.position);
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (_startTouch)
                        {
                            OnPointing(touch.position);
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        if (_startTouch)
                        {
                            OnPointUp(touch.position);
                            _startTouch = false;
                        }
                        break;
                }
            }

            if (_checkMove)
                OnMoving(Input.mousePosition);

        }
#else
        void IUpdate.OnUpdate(float deltaTime) 
        {
            throw new System.NotImplementedException();
        }
#endif

        private IIntersect RaycastIntersect()
        {
            Vector3 screenPos = Input.mousePosition;
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit)
            {
                return hit.collider.GetComponent<IIntersect>();
            }
            else
            {
                return null;
            }
        }

        private void OnScale(float value)
        {
            // Scene Trigger
            for (int i = 0; i < _intersects.Count; i++)
            {
                IScenentersect sceneIntersect = _intersects[i];
                sceneIntersect?.OnScale(value);
            }
        }

        protected void OnMoving(Vector2 screenPoint)
        {
            // Collider Trigger
            IIntersect intersect = RaycastIntersect();
            intersect?.OnMoving(screenPoint);

            // Scene Trigger
            for (int i = 0; i < _intersects.Count; i++)
            {
                IScenentersect sceneIntersect = _intersects[i];
                sceneIntersect?.OnMoving(screenPoint);
            }
        }

        protected void OnPointDown(Vector2 screenPoint)
        {
            // Collider Trigger
            IIntersect intersect = RaycastIntersect();
            intersect?.OnPointDown(screenPoint);

            // Scene Trigger
            for (int i = 0; i < _intersects.Count; i++)
            {
                IScenentersect sceneIntersect = _intersects[i];
                sceneIntersect?.OnPointDown(screenPoint);
            }
        }

        protected void OnPointing(Vector2 screenPoint)
        {
            // Collider Trigger
            IIntersect intersect = RaycastIntersect();
            intersect?.OnPointing(screenPoint);

            // Scene Trigger
            for (int i = 0; i < _intersects.Count; i++)
            {
                IScenentersect sceneIntersect = _intersects[i];
                sceneIntersect?.OnPointing(screenPoint);
            }
        }

        protected void OnPointUp(Vector2 screenPoint)
        {
            // Collider Trigger
            IIntersect intersect = RaycastIntersect();
            intersect?.OnPointUp(screenPoint);

            // Scene Trigger
            for (int i = 0; i < _intersects.Count; i++)
            {
                IScenentersect sceneIntersect = _intersects[i];
                sceneIntersect?.OnPointUp(screenPoint);
            }
        }
    }
}
