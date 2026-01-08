using System;
using UnityEngine;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class CommonCrop : ItemBase, ICrop
    {
        public const int NOT_FINISH = 0;
        public const int FINISH = 1;
        public const float FINISHTIME = 10;

        private PloughItem _plough;
        private int _state;
        private float _riseTime;
        private Action _finishEvent;
        private Action<float> _timeUpdateEvent;
        private Action _ploughChangeEvent;

        public PloughItem Plough => _plough;

        public event Action PloughChangeEvent
        {
            add { _ploughChangeEvent += value; }
            remove { _ploughChangeEvent -= value; }
        }

        protected override void OnStart()
        {
            base.OnStart();
            _state = NOT_FINISH;
            _riseTime = 0;
            AddCom<CommonCropView>();
        }

        public void Bind(PloughItem plough)
        {
            this._plough = plough;
            plough.GridChangeEvent += PloughGridChangeHandler;
        }

        private void PloughGridChangeHandler()
        {
            _ploughChangeEvent?.Invoke();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _plough.GridChangeEvent -= PloughGridChangeHandler;
            _finishEvent = null;
            _timeUpdateEvent = null;
        }

        public void RegisterTime(Action<float> handler)
        {
            _timeUpdateEvent += handler;
        }

        public void RegisterFinish(Action handler)
        {
            if (_state == FINISH)
                handler();
            else
                _finishEvent += handler;
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            switch (_state)
            {
                case NOT_FINISH:
                    _riseTime += Time.deltaTime;
                    if (_riseTime >= FINISHTIME)
                    {
                        _riseTime = FINISHTIME;
                        _timeUpdateEvent?.Invoke(_riseTime);
                        _state = FINISH;
                        _finishEvent?.Invoke();
                        _finishEvent = null;
                    }
                    else
                    {
                        _timeUpdateEvent?.Invoke(_riseTime);
                    }
                    break;
            }
        }
    }
}
