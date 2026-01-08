
using Cysharp.Threading.Tasks;
using MH.GameScene.Datas;
using MH.GameScene.Runtime.Characters;
using MH.GameScene.Runtime.Entities;
using MH.GameScene.Runtime.Views;
using MM.MapEditors;
using SgFramework.UI;
using System.Collections.Generic;
using UI.UIScenes;
using UnityEngine;

namespace MH.GameScene.Core.Entites
{
    public partial class World
    {

        private static Dictionary<int, World> s_worlds = new Dictionary<int, World>();

        public static World Current { get; private set; }

        public static int Count => s_worlds.Count;

        public static void Destory(int mapId)
        {
            if (s_worlds.TryGetValue(mapId, out World world))
            {
                world.Destroy();
                s_worlds.Remove(mapId);
            }
        }

#if UNITY_EDITOR
        public static async UniTask<World> CreateEditor(int mapId)
        {
            if (s_worlds.TryGetValue(mapId, out World world))
                return world;

            world = new World();
            world.Initialize();
            GameObject obj = new GameObject($"[{nameof(World)}_{mapId}]");
            WorldObject worldObject = obj.AddComponent<WorldObject>();
            world.AddEntity<WorldView>(worldObject);
            worldObject.Set(world);
            world._sceneObj = obj;
            world.Resource = await EditorResourceModule.Create(world);

            MapData mapData = await world.Resource.LoadData<MapData>($"map{mapId}");
            if (mapData == null)
            {
                mapData = new MapData();
                mapData.Id = mapId;
                mapData.Elements = new List<GridData>();
                mapData.Npcs = new List<NpcData>();
                mapData.Areas = new List<AreaData>();
            }
            world.AddEntity<EditorWorldCamera>();
            world.AddEntity<CharacterModule>();
            world.AddEntity<MapEditorEntity>(mapData);

            s_worlds.Add(mapId, world);
            Current = world;
            return world;
        }
#endif

        public static async UniTask<World> Create(int mapId)
        {
            if (s_worlds.TryGetValue(mapId, out World world))
                return world;

            world = new World();
            world.Initialize();
            GameObject obj = new GameObject($"[{nameof(World)}_{mapId}]");
            WorldObject worldObject = obj.AddComponent<WorldObject>();
            world.AddEntity<WorldView>(worldObject);
            worldObject.Set(world);
            world._sceneObj = obj;
            world.Resource = await RuntimeResourceModule.Create(world);
            WorldCamera cam = world.AddEntity<WorldCamera>();
            world.UIScene = await UIManager.Open<UIScene>(cam);

            MapData mapData = await world.Resource.LoadData<MapData>($"map{mapId}");
            world.AddEntity<CharacterModule>();
            world.AddEntity<GameMap>(mapData);

            s_worlds.Add(mapId, world);
            Current = world;
            return world;
        }
    }
}
