using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class BoxItem : ItemBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            AddCom<BoxItemView>();
        }
    }
}
