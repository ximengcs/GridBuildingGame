
using MH.GameScene.Runtime.Views;

namespace MH.GameScene.Runtime.Entities
{
    public class DestinationItem : ItemBase
    {
        protected override void OnStart()
        {
            base.OnStart();
            AddCom<DestinationItemView>();
        }
    }
}
