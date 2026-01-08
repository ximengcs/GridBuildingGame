
namespace MH.GameScene.Runtime.Views
{
    public interface IGridSelectable : IItemView
    {
        void OnSelect();

        void OnUnSelect();
    }
}
