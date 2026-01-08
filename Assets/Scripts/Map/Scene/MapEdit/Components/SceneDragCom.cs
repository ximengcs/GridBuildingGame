#if UNITY_EDITOR
using R3;
using UnityEngine;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Utilities;

namespace MM.MapEditors
{
    public class SceneDragCom : ComponentBase, IUpdate
    {
        private IWorldCamera _cam;
        private Vector3 _lastPos;

        public override void OnStart()
        {
            base.OnStart();
            _cam = Entity.World.FindEntity<IWorldCamera>();
        }

        public void OnUpdate(float deltaTime)
        {
            if (Input.GetMouseButtonDown(1))
            {
                _lastPos = Input.mousePosition;
            }
            else if (Input.GetMouseButton(1))
            {
                Vector3 camPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                camPos -= Camera.main.ScreenToWorldPoint(_lastPos);
                camPos.z = 0;
                if (camPos != Vector3.zero)
                    _cam.SetPos(camPos);
                _lastPos = Input.mousePosition;
            }
        }
    }
}
#endif