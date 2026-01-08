#if UNITY_EDITOR
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Views;

namespace MM.MapEditors
{
    public class MapEditorEntity : MapScene
    {
        protected override void OnSceneInit(object data)
        {
            base.OnSceneInit(data);

            this.AddEntity<EditSelector>();
            this.AddEntity<AreaModule<MapEditArea>>();

            AddCom<SceneViewCom>();
            AddCom<GridOutlineCom>();
            AddCom<PathFindingCom>();

            AddCom<MapdataLoadCom>(data);
            AddCom<IntersectCom>();
            AddCom<SceneDragCom>();
        }
    }
}
#endif