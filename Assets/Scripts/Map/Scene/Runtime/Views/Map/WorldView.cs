
using MH.GameScene.Core.Entites;
using UnityEngine;

namespace MH.GameScene.Runtime.Views
{
    public class WorldView : Entity
    {
        private WorldObject _obj;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            _obj = (WorldObject)data;
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            GameObject.Destroy(_obj);
        }

        public void AddChild(Transform child)
        {
            child.SetParent(_obj.transform);
        }

        public void AddChild(GameObject child)
        {
            child.transform.SetParent(_obj.transform);
        }
    }
}
