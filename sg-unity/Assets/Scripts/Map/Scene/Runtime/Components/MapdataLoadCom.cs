using UnityEngine;
using MH.GameScene.Datas;
using MH.GameScene.Configs;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using MM.MapEditors;
using MH.GameScene.Runtime.Characters;

namespace MH.GameScene.Runtime.Entities
{
    public class MapdataLoadCom : ComponentBase
    {
        public override void OnInit(Entity entity, object data)
        {
            base.OnInit(entity, data);

            MapData mapData = (MapData)data;
            InitMapGrid(mapData);
            InitCameraRect(mapData);
            InitMapArea(mapData);
            InitNpc(mapData);
        }

        private void InitNpc(MapData mapData)
        {
            CharacterModule charModule = Entity.World.FindEntity<CharacterModule>();
            if (mapData.Npcs != null)
            {
                foreach (NpcData npcData in mapData.Npcs)
                {
                    CharacterGenParam param = new CharacterGenParam();
                    param.Index = npcData.Index;
                    param.NpcId = npcData.Id;
                    charModule.AddCharacter<Npc>(param);
                }
            }
        }

        private void InitMapArea(MapData mapData)
        {
            IMapScene scene = (IMapScene)Entity;
            IAreaModule areaModule = Entity.FindEntity<IAreaModule>();
            List<IGridEntity> grids = new List<IGridEntity>();
            if (mapData.Areas != null)
            {
                foreach (AreaData areaData in mapData.Areas)
                {
                    grids.Clear();
                    AreaBase area = (AreaBase)areaModule.AddArea(areaData.AreaId);
                    area.Center = areaData.CenterPos;

                    foreach (SerializeIntPos pos in areaData.IndexList)
                    {
                        IGridEntity grid = scene.GetGrid(pos);
                        grids.Add(grid);
                    }
                    area.Add(grids);

                    AreaView view = area.FindEntity<AreaView>();
                    if (view != null)
                        view.Color = new Color(areaData.ColorR, areaData.ColorG, areaData.ColorB);
                }
            }
        }

        private void InitCameraRect(MapData mapData)
        {
            IWorldCamera cam = Entity.World.FindEntity<IWorldCamera>();
            cam.SetRect(mapData.ViewMin, mapData.ViewMax);
        }

        private void InitMapGrid(MapData mapData)
        {
            IMapScene scene = (IMapScene)Entity;
            List<ItemGenParam> itemParams = new List<ItemGenParam>();
            foreach (GridData gridData in mapData.Elements)
            {
                Vector2Int index = gridData.Index;
                foreach (var entry in gridData.Objects)
                {
                    ItemData itemData = entry.Value;
                    ItemConfig config = Entity.World.Resource.GetConfig<ItemConfig>(itemData.Id);
                    if (config == null)
                    {
                        Debug.LogError("item config is null " + itemData.Id);
                        continue;
                    }

                    ItemGenParam itemParam = new ItemGenParam();
                    itemParam.ItemId = itemData.Id;
                    itemParam.Direction = itemData.Direction;
                    itemParam.Layer = entry.Key;
                    itemParam.Size = config.Size;
                    itemParam.Index = index;
                    itemParams.Add(itemParam);

                    scene.EnsureGrid(itemParam);
                }
            }

            foreach (ItemGenParam itemParam in itemParams)
            {
                scene.SetItem(itemParam, false);
            }
        }
    }
}
