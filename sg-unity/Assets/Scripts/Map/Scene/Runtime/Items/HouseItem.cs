
using System;
using UI.UIScenes;
using UnityEngine;
using MH.GameScene.Runtime.Views;
using MH.GameScene.Runtime.Utilities;

namespace MH.GameScene.Runtime.Entities
{
    public class HouseItem : ItemBase, IUISceneBinder
    {
        private Action _posChangeEvent;

        public Vector3 WorldPos
        {
            get
            {
                return MathUtility.IndexToGamePos(MainGrid.Index);
            }
        }

        public event Action PosChangeEvent
        {
            add { _posChangeEvent += value; }
            remove { _posChangeEvent -= value; }
        }

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            GridChangeEvent += GridhangeHandler;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GridChangeEvent -= GridhangeHandler;
        }

        private void GridhangeHandler()
        {
            _posChangeEvent?.Invoke();
        }

        protected override void OnStart()
        {
            base.OnStart();
            AddCom<HouseItemView>();
        }
    }
}
