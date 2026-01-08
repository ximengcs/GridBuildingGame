using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Views
{
    public class GridViewCom : ComponentBase
    {
        private SceneViewCom _sceneViewCom;
        private GridEntity _gridEntity;

        public override void OnStart()
        {
            base.OnStart();
            _gridEntity = (GridEntity)Entity;
            _sceneViewCom = Entity.Parent.GetCom<SceneViewCom>();
        }
    }
}
