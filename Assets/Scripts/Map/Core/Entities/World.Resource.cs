using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using MH.GameScene.Runtime;
using MH.GameScene.Configs;
using Cysharp.Threading.Tasks;
using MH.GameScene.Runtime.Views;
using System.Collections.Generic;
using SgFramework.Res;
using UnityEngine.U2D;

namespace MH.GameScene.Core.Entites
{
    public partial class World
    {
        public interface IResourceModule
        {
            UniTask<T> LoadData<T>(string name);

            IReadOnlyCollection<T> GetConfigs<T>() where T : IConfig;

            ItemConfig GetConfig<T>(int id);

            UniTask<GameObject> LoadObject(string name);

            UniTask<Sprite> GetSprite(int itemId, string layer, int direction);

            UniTask<Sprite> GetSprite(string relativePath);

            UniTask<SpriteAtlas> GetAtlas(string relativePath);

            void Dispose();
        }

        private class RuntimeResourceModule : IResourceModule
        {
            private string basePath;
            private ResourceGroup group;
            private Dictionary<Type, Dictionary<int, IConfig>> configs;
            private WorldView worldView;
            private SpriteAtlas _atlas;

            public static async UniTask<RuntimeResourceModule> Create(World world)
            {
                RuntimeResourceModule resModule = new RuntimeResourceModule();
                resModule.worldView = world.FindEntity<WorldView>();
                resModule.basePath = "Assets/GameRes/Map/";
                resModule.configs = new Dictionary<Type, Dictionary<int, IConfig>>();
                resModule.group = ResourceManager.GetGroup($"world_map_{World.Count}");
                await resModule.LoadConfig<ItemConfig>("Item.json");
                await resModule.LoadConfig<NpcConfig>("Npc.json");
                return resModule;
            }

            private async UniTask LoadConfig<T>(string name) where T : IConfig
            {
                var dic = new Dictionary<int, IConfig>();
                string path = Path.Combine(basePath, "Configs/", name);
                TextAsset itemCfgAsset = await group.LoadAssetAsync<TextAsset>(path);
                List<T> list = JsonConvert.DeserializeObject<List<T>>(itemCfgAsset.text);
                foreach (IConfig config in list)
                    dic.Add(config.Id, config);
                configs.Add(typeof(T), dic);
            }

            public async UniTask<T> LoadData<T>(string name)
            {
                string path = Path.Combine(basePath, "Data/", name);
                TextAsset asset = await group.LoadAssetAsync<TextAsset>(path);
                return JsonConvert.DeserializeObject<T>(asset.text);
            }

            public IReadOnlyCollection<T> GetConfigs<T>() where T : IConfig
            {
                if (configs.TryGetValue(typeof(T), out var dic))
                {
                    List<T> result = new List<T>();
                    foreach (var config in dic)
                        result.Add((T)config.Value);
                    return result;
                }
                return null;
            }

            public ItemConfig GetConfig<T>(int id)
            {
                if (configs.TryGetValue(typeof(T), out var values))
                {
                    if (values.TryGetValue(id, out var value))
                        return (ItemConfig)value;
                }
                return null;
            }

            public async UniTask<GameObject> LoadObject(string name)
            {
                string path = Path.Combine(basePath, "Prefabs/", name);
                var token = await group.GetObject(path);
                worldView.AddChild(token.gameObject);
                return token.gameObject;
            }

            public async UniTask<Sprite> GetSprite(int itemId, string layer, int direction)
            {
                string name = null;
                if (direction == GameConst.DIRECTION_RT)
                    name = $"{itemId}.png";
                else
                    name = $"{itemId}_{direction}.png";

                if (_atlas == null)
                    _atlas = await GetAtlas("Map");
                Sprite sprite = _atlas.GetSprite(name);
                if (sprite != null)
                    return sprite;

                string resKey = Path.Combine(basePath, "Textures/", $"{layer.ToLower()}/", name);
                return await group.GetSprite(resKey);
            }

            public async UniTask<Sprite> GetSprite(string relativePath)
            {
                string name = Path.GetFileName(relativePath);

                if (_atlas == null)
                    _atlas = await GetAtlas("Map");
                Sprite sprite = _atlas.GetSprite(name);
                if (sprite != null)
                    return sprite;

                string resKey = Path.Combine(basePath, "Textures/", relativePath);
                return await group.GetSprite(resKey);
            }

            public async UniTask<SpriteAtlas> GetAtlas(string relativePath)
            {
                string path = Path.Combine(basePath, "Textures/atlas/", $"{relativePath}.spriteatlasv2");
                var assets = await group.LoadAssetAsync<SpriteAtlas>(path);
                return assets;
            }

            public void Dispose()
            {
                ResourceManager.ReleaseGroup(group);
                group = null;
            }
        }

#if UNITY_EDITOR
        private class EditorResourceModule : IResourceModule
        {
            private string basePath;
            private Dictionary<Type, Dictionary<int, IConfig>> configs;
            private WorldView worldView;

            public EditorResourceModule(World world)
            {
                worldView = world.FindEntity<WorldView>();
            }

            public static async UniTask<EditorResourceModule> Create(World world)
            {
                EditorResourceModule resModule = new EditorResourceModule(world);
                resModule.basePath = "Assets/GameRes/Map/";
                resModule.configs = new Dictionary<Type, Dictionary<int, IConfig>>();
                resModule.LoadConfig<ItemConfig>("Item.json");
                resModule.LoadConfig<NpcConfig>("Npc.json");
                await UniTask.CompletedTask;
                return resModule;
            }

            private void LoadConfig<T>(string name) where T : IConfig
            {
                var dic = new Dictionary<int, IConfig>();
                string path = Path.Combine(basePath, "Configs/", name);
                TextAsset itemCfgAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                List<T> list = JsonConvert.DeserializeObject<List<T>>(itemCfgAsset.text);
                foreach (IConfig config in list)
                    dic.Add(config.Id, config);
                configs.Add(typeof(T), dic);
            }

            public ItemConfig GetConfig<T>(int id)
            {
                if (configs.TryGetValue(typeof(T), out var values))
                {
                    if (values.TryGetValue(id, out var value))
                        return (ItemConfig)value;
                }
                return null;
            }

            public IReadOnlyCollection<T> GetConfigs<T>() where T : IConfig
            {
                if (configs.TryGetValue(typeof(T), out var dic))
                {
                    List<T> result = new List<T>();
                    foreach (var config in dic)
                        result.Add((T)config.Value);
                    return result;
                }
                return null;
            }

            public async UniTask<Sprite> GetSprite(int itemId, string layer, int direction)
            {
                string name = null;
                if (direction == GameConst.DIRECTION_RT)
                    name = $"{itemId}.png";
                else
                    name = $"{itemId}_{direction}.png";
                string resKey = Path.Combine(basePath, "Textures/", $"{layer.ToLower()}/", name);
                await UniTask.CompletedTask;
                return AssetDatabase.LoadAssetAtPath<Sprite>(resKey);
            }

            public async UniTask<Sprite> GetSprite(string relativePath)
            {
                string resKey = Path.Combine(basePath, "Textures/", relativePath);
                await UniTask.CompletedTask;
                return AssetDatabase.LoadAssetAtPath<Sprite>(resKey);
            }

            public async UniTask<T> LoadData<T>(string name)
            {
                string path = Path.Combine(basePath, "Data/", $"{name}.json");
                TextAsset asset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
                await UniTask.CompletedTask;
                if (asset == null)
                    return default;
                return JsonConvert.DeserializeObject<T>(asset.text);
            }

            public async UniTask<GameObject> LoadObject(string name)
            {
                string path = Path.Combine(basePath, "Prefabs/", $"{name}.prefab");
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                await UniTask.CompletedTask;
                GameObject inst = GameObject.Instantiate(prefab);
                worldView.AddChild(inst);
                return inst;
            }

            public async UniTask<SpriteAtlas> GetAtlas(string relativePath)
            {
                string path = Path.Combine(basePath, "Textures/atlas/", $"{relativePath}.spriteatlasv2");
                SpriteAtlas atlas = AssetDatabase.LoadAssetAtPath<SpriteAtlas>(path);
                await UniTask.CompletedTask;
                return atlas;
            }

            public void Dispose()
            {

            }
        }
#endif
    }
}
