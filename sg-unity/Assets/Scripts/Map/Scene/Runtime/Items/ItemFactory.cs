using MH.GameScene.Configs;
using MH.GameScene.Core.Entites;

namespace MH.GameScene.Runtime.Entities
{
    public static class ItemFactory
    {
        public static IItemEntity GenerateItem(this IEntity entity, ItemGenParam param)
        {
            int itemId = param.ItemId;
            ItemConfig config = entity.World.Resource.GetConfig<ItemConfig>(itemId);
            switch (config.Type)
            {
                case GameConst.TYPE_SURFACE: return entity.AddEntity<SurfaceItem>(param);
                case GameConst.TYPE_SURFACE_DECORATE:
                    switch (itemId)
                    {
                        case 200004: return entity.AddEntity<PloughItem>(param);
                    }
                    return entity.AddEntity<SurfaceDecorateItem>(param);
                case GameConst.TYPE_COMMON:
                    switch (itemId)
                    {
                        case 300013: return entity.AddEntity<DestinationItem>(param);
                        case 300014: return entity.AddEntity<CommonCrop>(param);
                        case 300016: return entity.AddEntity<HouseItem>(param);
                        default: return entity.AddEntity<BoxItem>(param);
                    }
                case GameConst.TYPE_CROP: return entity.AddEntity<CommonCrop>(param);
                default: return null;
            }
        }
    }
}
