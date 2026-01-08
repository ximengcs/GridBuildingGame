
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Views
{
    public interface IItemView : IComponent
    {
        IItemEntity Item { get; }
    }
}
