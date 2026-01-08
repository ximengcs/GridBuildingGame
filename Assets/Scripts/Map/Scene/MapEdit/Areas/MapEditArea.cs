#if UNITY_EDITOR
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;

namespace MM.MapEditors
{
    public class MapEditArea : AreaBase
    {
        protected override void OnInit(object data)
        {
            base.OnInit(data);
            this.AddEntity<EditAreaView>();
        }
    }
}
#endif