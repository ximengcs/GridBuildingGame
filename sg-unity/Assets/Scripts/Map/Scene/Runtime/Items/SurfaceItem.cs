
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class SurfaceItem : ItemBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            AddCom<SurfaceItemView>();
        }
    }
}
