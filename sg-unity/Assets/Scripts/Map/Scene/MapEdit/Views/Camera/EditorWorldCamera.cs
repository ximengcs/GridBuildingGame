#if UNITY_EDITOR
using UnityEngine;
using MH.GameScene.Core.Entites;
using System;

namespace MM.MapEditors
{
    public class EditorWorldCamera : Entity, IWorldCamera
    {
        private Transform _camTransform;

        public event Action PosChangeEvent;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _camTransform = Camera.main.transform;
        }

        public void SetPos(Vector3 pos)
        {
            _camTransform.position -= pos;
        }

        public void SetRect(Vector2 min, Vector2 max)
        {

        }

        public void SetPosEnd(Vector3 pos)
        {

        }

        public void SetSize(float value)
        {

        }
    }
}
#endif