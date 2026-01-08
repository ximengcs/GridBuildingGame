
using MH.GameScene.Core.Entites;
using MM.MapEditors;

namespace MH.GameScene.Runtime.Entities
{
    public class MapArea : AreaBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            this.AddEntity<GameAreaView>();
        }
    }
}
