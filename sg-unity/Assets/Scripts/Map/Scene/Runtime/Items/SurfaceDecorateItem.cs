using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class SurfaceDecorateItem : ItemBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            AddCom<SurfaceDecorateItemView>();
        }
    }
}
