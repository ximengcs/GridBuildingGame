using UI;
using MM.MapEditors;
using SgFramework.UI;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class GameMap : MapScene
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);

            this.AddEntity<AreaModule<MapArea>>();

            AddCom<SceneViewCom>();
            AddCom<PathFindingCom>();
            AddCom<MapdataLoadCom>(data);
            AddCom<IntersectCom>();
            AddCom<GameOperateCom>();

            _ = UIManager.Open<UIPlough>();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            _ = UIManager.Close<UIPlough>();
        }
    }
}
