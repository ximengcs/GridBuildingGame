using R3;
using UnityEngine;

namespace MH.GameScene.Runtime.Utilities
{
    public class ObservableCommonTrigger
    {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN
        private const int _longPressPx = 0;
#elif UNITY_IPHONE || UNITY_ANDROID
        private const int _longPressPx = 10;
#endif

        private float _clickGap = 0.25f;
        private float _longPressGap = 0.5f;
        private float _pressTime;
        private bool _shortPress;
        private bool _longPress;

        private Vector2 _startPos;
        private Vector2 _lastPos;

        public float LongPressTime
        {
            set => _longPressGap = value;
        }

        #region Point Down
        private Subject<Vector2> _onTouchDown;

        public void OnPointDown(Vector2 screenPoint)
        {
            _pressTime = 0;
            if (_onTouchDown != null) _onTouchDown.OnNext(screenPoint);

            _startPos = screenPoint;
            _lastPos = screenPoint;
            _longPress = false;
            _shortPress = false;
        }

        public Observable<Vector2> OnPointDownAsObservable()
        {
            return _onTouchDown ?? (_onTouchDown = new Subject<Vector2>());
        }
        #endregion

        #region Pointing
        private Subject<Vector2> _onTouching;

        public void OnPointing(Vector2 pos)
        {
            _pressTime += Time.deltaTime;
            if (_onTouching != null) _onTouching.OnNext(pos);

            if (!_shortPress)
            {
                if (!_longPress)
                {
                    if (_pressTime >= _longPressGap)
                    {
                        _longPress = true;
                        if (_onLongPressStart != null)
                            _onLongPressStart.OnNext(pos);
                    }
                    else
                    {
                        float dis = Vector2.Distance(pos, _lastPos);
                        if (dis > _longPressPx && !Mathf.Approximately(dis, _longPressPx))
                        {
                            _shortPress = true;
                            if (_onShortPressStart != null)
                                _onShortPressStart.OnNext(pos);
                        }
                    }
                }
                else
                {
                    if (_onLongPressing != null)
                        _onLongPressing.OnNext(pos);
                }
            }
            else
            {
                if (_onShortPressing != null)
                    _onShortPressing.OnNext(pos);
            }
            _lastPos = pos;
        }

        public Observable<Vector2> OnPointingAsObservable()
        {
            return _onTouching ?? (_onTouching = new Subject<Vector2>());
        }
        #endregion

        #region Moving
        private Subject<Vector2> _onMoving;

        public void OnMoving(Vector2 screenPos)
        {
            if (_onMoving != null)
                _onMoving.OnNext(screenPos);
        }

        public Observable<Vector2> OnMovingAsObservable()
        {
            return _onMoving ?? (_onMoving = new Subject<Vector2>());
        }
        #endregion

        #region Point Up
        private Subject<Vector2> _onTouchUp;

        public void OnPointUp(Vector2 pos)
        {
            if (_onTouchUp != null) _onTouchUp.OnNext(pos);

            if (_shortPress)
            {
                _shortPress = false;
                if (_onShortPressEnd != null)
                    _onShortPressEnd.OnNext(pos);
            }
            else
            {
                if (_longPress)
                {
                    _longPress = false;
                    if (_onLongPressEnd != null)
                        _onLongPressEnd.OnNext(pos);
                }
                else
                {
                    if (_pressTime <= _clickGap && Mathf.Approximately(Vector2.Distance(pos, _startPos), 0))
                    {
                        _onClick?.OnNext(pos);
                    }
                }
            }
        }

        public Observable<Vector2> OnPointUpAsObservable()
        {
            return _onTouchUp ?? (_onTouchUp = new Subject<Vector2>());
        }
        #endregion

        #region Short Press Start
        private Subject<Vector2> _onShortPressStart;

        public Observable<Vector2> OnShortPressStartAsObservable()
        {
            return _onShortPressStart ?? (_onShortPressStart = new Subject<Vector2>());
        }
        #endregion

        #region Short Pressing
        private Subject<Vector2> _onShortPressing;

        public Observable<Vector2> OnShortPressingAsObservable()
        {
            return _onShortPressing ?? (_onShortPressing = new Subject<Vector2>());
        }
        #endregion

        #region Short PressEnd
        private Subject<Vector2> _onShortPressEnd;

        public Observable<Vector2> OnShortPressEndAsObservable()
        {
            return _onShortPressEnd ?? (_onShortPressEnd = new Subject<Vector2>());
        }
        #endregion

        #region Long Press Start
        private Subject<Vector2> _onLongPressStart;

        public Observable<Vector2> OnLongPressStartAsObservable()
        {
            return _onLongPressStart ?? (_onLongPressStart = new Subject<Vector2>());
        }
        #endregion

        #region Long Pressing
        private Subject<Vector2> _onLongPressing;

        public Observable<Vector2> OnLongPressingAsObservable()
        {
            return _onLongPressing ?? (_onLongPressing = new Subject<Vector2>());
        }
        #endregion

        #region Long PressEnd
        private Subject<Vector2> _onLongPressEnd;

        public Observable<Vector2> OnLongPressEndAsObservable()
        {
            return _onLongPressEnd ?? (_onLongPressEnd = new Subject<Vector2>());
        }
        #endregion

        #region Click
        private Subject<Vector2> _onClick;

        public Observable<Vector2> OnClickAsObservable()
        {
            return _onClick ?? (_onClick = new Subject<Vector2>());
        }
        #endregion

        #region Scale
        private Subject<float> _onScale;

        public void OnScale(float value)
        {
            if (_onScale != null)
                _onScale.OnNext(value);
        }

        public Observable<float> OnScaleAsObservable()
        {
            return _onScale ?? (_onScale = new Subject<float>());
        }
        #endregion

        public void OnDestroy()
        {
            RaiseOnCompletedOnDestroy();
        }

        private void RaiseOnCompletedOnDestroy()
        {
            if (_onTouchDown != null)
            {
                _onTouchDown.OnCompleted();
                _onTouchDown = null;
            }

            if (_onTouching != null)
            {
                _onTouching.OnCompleted();
                _onTouchDown = null;
            }

            if (_onTouchUp != null)
            {
                _onTouchUp.OnCompleted();
                _onTouchDown = null;
            }

            if (_onClick != null)
            {
                _onClick.OnCompleted();
                _onClick = null;
            }

            if (_onLongPressStart != null)
            {
                _onLongPressStart.OnCompleted();
                _onLongPressStart = null;
            }

            if (_onLongPressing != null)
            {
                _onLongPressing.OnCompleted();
                _onLongPressing = null;
            }

            if (_onLongPressEnd != null)
            {
                _onLongPressEnd.OnCompleted();
                _onLongPressEnd = null;
            }

            if (_onShortPressStart != null)
            {
                _onShortPressStart.OnCompleted();
                _onShortPressStart = null;
            }

            if (_onShortPressing != null)
            {
                _onShortPressing.OnCompleted();
                _onShortPressing = null;
            }

            if (_onShortPressEnd != null)
            {
                _onShortPressEnd.OnCompleted();
                _onShortPressEnd = null;
            }

            if (_onMoving != null)
            {
                _onMoving.OnCompleted();
                _onMoving = null;
            }

            if (_onScale != null)
            {
                _onScale.OnCompleted();
                _onScale = null;
            }
        }
    }
}
