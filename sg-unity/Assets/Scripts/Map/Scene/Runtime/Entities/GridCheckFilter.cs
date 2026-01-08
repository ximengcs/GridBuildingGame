using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    public delegate void GridCheckFilter(IItemEntity selfItem, IGridEntity grid, ItemGenParam genParam, ICollection<IItemEntity> result);
}
