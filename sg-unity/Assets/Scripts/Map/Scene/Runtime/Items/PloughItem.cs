
using MH.GameScene.Configs;
using MH.GameScene.Runtime.Views;
using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public class PloughItem : ItemBase
    {
        private ICrop _crop;

        public ICrop Crop => _crop;

        protected override void OnInit(object data)
        {
            base.OnInit(data);
            AddCom<PloughItemView>();
        }

        protected override void OnGridChange()
        {
            base.OnGridChange();
            if (_crop != null)
            {
                ItemGenParam genParam = new ItemGenParam(_crop);
                genParam.Index = MainGrid.Index;
                MainGrid.Scene.MoveItem(_crop, genParam, false);
            }
        }

        public void GainCrop()
        {
            if (_crop != null)
            {
                IGridEntity grid = _crop.MainGrid;
                IMapScene scene = grid.Scene;
                scene.RemoveItem(grid.Index, _crop.Layer);
                _crop = null;
            }
        }

        public void SetCrop(int cropId)
        {
            if (_crop != null)
                return;

            ItemConfig config = World.Resource.GetConfig<ItemConfig>(cropId);

            IGridEntity grid = MainGrid;
            IMapScene mapScene = grid.Scene;
            ItemGenParam param = new ItemGenParam();
            param.ItemId = cropId;
            param.Index = grid.Index;
            param.Direction = GameConst.DIRECTION_RT;
            param.Layer = config.Layer;
            param.Size = config.Size;
            ItemSetResult check = mapScene.CheckItemSet(param);
            switch (check.Type)
            {
                case ItemSetResultType.Ok:
                    _crop = (ICrop)mapScene.SetItem(param, false);
                    CommonCrop commonCrop = (CommonCrop)_crop;
                    commonCrop.Bind(this);
                    break;
            }
        }
    }
}
