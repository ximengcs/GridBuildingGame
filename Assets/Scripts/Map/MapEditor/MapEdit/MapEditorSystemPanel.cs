#if UNITY_EDITOR
using TMPro;
using System.IO;
using UnityEngine;
using MM.MapEditors;
using UnityEngine.UI;
using Newtonsoft.Json;
using MH.GameScene.Datas;
using System.Collections.Generic;
using MH.GameScene.Core.Entites;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Characters;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorSystemPanel : FeaturePanelBase
    {
        [SerializeField]
        private Button openBtn;

        [SerializeField]
        private Button saveBtn;

        [SerializeField]
        private Button closeBtn;

        [SerializeField]
        private Button quitBtn;

        [SerializeField]
        private TMP_InputField input;

        [SerializeField]
        private Slider slider;

        private int _curMapId;

        private const float CAM_MINSIZE = 1.5f;
        private const float CAM_MAXSIZE = 20;

        public World World
        {
            get => _editorUI.World;
            set => _editorUI.World = value;
        }

        private void Awake()
        {
            openBtn.onClick.AddListener(InnerOpenHandler);
            saveBtn.onClick.AddListener(InnerSaveHandler);
            closeBtn.onClick.AddListener(CloseHandler);
            quitBtn.onClick.AddListener(Application.Quit);
            slider.onValueChanged.AddListener(InnerClick);
            RefreshCamSize();
        }

        private void RefreshCamSize()
        {
            float size = Camera.main.orthographicSize;
            float pro = (size - CAM_MINSIZE) / (CAM_MAXSIZE - CAM_MINSIZE);
            slider.value = pro;
        }

        private void InnerClick(float value)
        {
            float size = value * (CAM_MAXSIZE - CAM_MINSIZE) + CAM_MINSIZE;
            Camera.main.orthographicSize = size;
        }

        private async void InnerOpenHandler()
        {
            if (World != null)
                CloseHandler();

            _curMapId = int.Parse(input.text);
            World = await World.CreateEditor(_curMapId);
            _editorUI.SetScene(World.GetEntity<MapEditorEntity>());
        }

        private void InnerSaveHandler()
        {
            _curMapId = int.Parse(input.text);
            MapData data = new MapData();
            data.Id = _curMapId;
            data.Elements = new List<GridData>();

            Dictionary<Vector2Int, GridData> gridMap = new Dictionary<Vector2Int, GridData>();

            IMapScene map = World.FindEntity<IMapScene>();
            foreach (IGridEntity grid in map.Grids)
            {
                foreach (IItemEntity item in grid.Items)
                {
                    if (item.MainGrid == grid)
                    {
                        ItemData itemData = new ItemData();
                        itemData.Id = item.ItemId;
                        itemData.Direction = item.Direction;

                        if (!gridMap.TryGetValue(grid.Index, out GridData gridData))
                        {
                            gridData = new GridData();
                            gridData.Index = grid.Index;
                            gridData.Objects = new Dictionary<string, ItemData>();
                            data.Elements.Add(gridData);
                            gridMap.Add(grid.Index, gridData);
                        }

                        gridData.Objects[item.Layer] = itemData;
                    }
                }
            }

            data.Npcs = new List<NpcData>();
            CharacterModule charModule = World.FindEntity<CharacterModule>();
            foreach (ICharacter character in charModule.Characters)
            {
                NpcData npcData = new NpcData();
                npcData.Id = character.NpcId;
                npcData.X = character.Index.x;
                npcData.Y = character.Index.y;
                data.Npcs.Add(npcData);
            }

            data.Areas = new List<AreaData>();
            IAreaModule areaModule = _editorUI.Scene.FindEntity<IAreaModule>();
            foreach (AreaBase area in areaModule.Areas)
            {
                AreaData areaData = new AreaData();
                area.CalculateCenterPos();
                areaData.CenterPos = area.Center;

                Color color = area.FindEntity<AreaView>().Color;
                areaData.Color = color;
                areaData.AreaId = area.AreaId;
                areaData.IndexList = new List<SerializeIntPos>();

                foreach (IGridEntity grid in area.Grids)
                {
                    areaData.IndexList.Add(new SerializeIntPos(grid.Index));
                }
                data.Areas.Add(areaData);
            }

            string saveJson = JsonConvert.SerializeObject(data, Formatting.Indented);
            File.WriteAllText($"Assets/GameRes/Map/Data/map{data.Id}.json", saveJson);
            Debug.Log("save success");

#if UNITY_EDITOR
            UnityEditor.AssetDatabase.Refresh();
#endif
        }

        private void CloseHandler()
        {
            _editorUI.SetScene(null);
            World.Destory(_curMapId);
            World = null;
        }
    }
}
#endif