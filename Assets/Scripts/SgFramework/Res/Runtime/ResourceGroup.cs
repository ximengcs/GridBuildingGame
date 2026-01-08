using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;
using YooAsset;
using Object = UnityEngine.Object;


namespace SgFramework.Res
{
    public class ResourceGroup
    {
        private readonly Dictionary<string, AssetHandle> _handles = new Dictionary<string, AssetHandle>();
        private readonly Dictionary<string, AllAssetsHandle> _allHandles = new Dictionary<string, AllAssetsHandle>();

        private readonly Dictionary<string, Stack<ResourceToken>> _tokenPools =
            new Dictionary<string, Stack<ResourceToken>>();

        private readonly Dictionary<string, int> _tokenRef = new Dictionary<string, int>();

        private readonly Dictionary<string, SpriteDownloadContext> _dynamicSprites =
            new Dictionary<string, SpriteDownloadContext>();

        private readonly GameObject _poolRoot;

        internal ResourceGroup(string key)
        {
            _poolRoot = new GameObject($"[ResourceGroup] - {key}");
            _poolRoot.SetActive(false);
            Object.DontDestroyOnLoad(_poolRoot);
        }

        public async UniTask<ResourceToken> GetObject(string key, Transform parent = default,
            Vector3 position = default,
            Quaternion rotation = default, CancellationToken cancellationToken = default)
        {
            if (!_handles.TryGetValue(key, out var handle))
            {
                handle = ResourceManager.LoadAssetAsync<GameObject>(key);
                _handles.Add(key, handle);
            }

            await handle;
            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            if (!_tokenPools.TryGetValue(key, out var pool) || !pool.TryPop(out var token))
            {
                var go = handle.InstantiateSync(parent);
                if (!go.TryGetComponent(out token))
                {
                    token = go.AddComponent<ResourceToken>();
                    token.OnCreate();
                }
            }
            else
            {
                token.transform.SetParent(parent, false);
            }

            token.transform.SetLocalPositionAndRotation(position, rotation);
            token.transform.localScale = Vector3.one;
            token.ResourceKey = key;
            token.Active = true;
            token.OnGet();
            _tokenRef[key] = _tokenRef.GetValueOrDefault(key) + 1;
            return token;
        }

        public void ReleaseObject(ResourceToken token)
        {
            if (!token.CanReuse)
            {
                Object.Destroy(token.gameObject);
                if (!_handles.Remove(token.ResourceKey, out var handle))
                {
                    return;
                }

                handle.Release();
                TryUnloadUnusedAsset(token.ResourceKey);
                return;
            }

            if (!_tokenPools.TryGetValue(token.ResourceKey, out var pool))
            {
                pool = new Stack<ResourceToken>();
                _tokenPools.Add(token.ResourceKey, pool);
            }

            token.OnRelease();
            token.transform.SetParent(_poolRoot.transform, false);
            token.Active = false;
            pool.Push(token);
            _tokenRef[token.ResourceKey] = _tokenRef.GetValueOrDefault(token.ResourceKey) - 1;
        }

        public async UniTask<T> LoadAssetAsync<T>(string key) where T : Object
        {
            if (!ResourceManager.CheckLocationValid(key))
            {
                throw new NotSupportedException($"[{key}] is invalid");
            }

            if (!_handles.TryGetValue(key, out var handle))
            {
                handle = ResourceManager.LoadAssetAsync<T>(key);
                _handles.Add(key, handle);
            }

            await handle;
            return handle.GetAssetObject<T>();
        }

        public void ReleaseAsset(string key)
        {
            if (!_handles.TryGetValue(key, out var handle))
            {
                return;
            }

            handle.Release();
            _handles.Remove(key);
        }

        public void TryUnloadUnusedAsset(string key)
        {
            ResourceManager.TryUnloadUnusedAsset(key);
        }

        public async UniTask<List<T>> LoadAllAssetsAsync<T>(string key) where T : Object
        {
            if (!ResourceManager.CheckLocationValid(key))
            {
                throw new NotSupportedException($"[{key}] is invalid");
            }

            if (!_allHandles.TryGetValue(key, out var handle))
            {
                handle = ResourceManager.LoadAllAssetsAsync<T>(key);
                _allHandles.Add(key, handle);
            }

            await handle;
            return handle.AllAssetObjects.Select(obj => obj as T).ToList();
        }

        public async UniTask<Sprite> GetSprite(string key, string atlasName = default,
            CancellationToken cancellationToken = default)
        {
            //直接加载Sprite
            {
                if (string.IsNullOrEmpty(atlasName))
                {
                    if (!_handles.TryGetValue(key, out var handle))
                    {
                        handle = ResourceManager.LoadAssetAsync<Sprite>(key);
                        _handles.Add(key, handle);
                    }

                    await handle;

                    if (cancellationToken.IsCancellationRequested)
                    {
                        return null;
                    }

                    return handle.GetAssetObject<Sprite>();
                }
            }
            //使用图集加载
            {
                if (!_handles.TryGetValue(atlasName, out var handle))
                {
                    handle = ResourceManager.LoadAssetAsync<SpriteAtlas>(atlasName);
                    _handles.Add(atlasName, handle);
                }

                await handle;

                if (cancellationToken.IsCancellationRequested)
                {
                    return null;
                }

                return handle.GetAssetObject<SpriteAtlas>().GetSprite(key);
            }
        }

        public SpriteDownloadContext GetSpriteWithUrl(string url)
        {
            if (_dynamicSprites.TryGetValue(url, out var context) && context.IsDone)
            {
                return context;
            }

            if (context != null)
            {
                return context;
            }

            context = new SpriteDownloadContext(url);
            _dynamicSprites.Add(url, context);
            return context;
        }

        public void ReleaseObject(SpriteDownloadContext context)
        {
            context.Release();
            _dynamicSprites.Remove(context.Url);
        }

        public void DestroyAll()
        {
            foreach (var (_, context) in _dynamicSprites)
            {
                context.Release();
            }

            _dynamicSprites.Clear();

            Object.Destroy(_poolRoot);
            _tokenPools.Clear();
            _tokenRef.Clear();

            foreach (var (_, handle) in _handles)
            {
                handle.Release();
            }

            _handles.Clear();

            foreach (var (_, handle) in _allHandles)
            {
                handle.Release();
            }

            _allHandles.Clear();
        }

        public static bool CheckLocationValid(string key)
        {
            return ResourceManager.CheckLocationValid(key);
        }

        public async UniTask UnloadUnusedAssets()
        {
            var list = new List<string>();
            foreach (var (key, handle) in _handles)
            {
                if (_tokenPools.TryGetValue(key, out var stack) && stack.Count > 0)
                {
                    continue;
                }

                if (_tokenRef.GetValueOrDefault(key) > 0)
                {
                    continue;
                }

                list.Add(key);
                handle.Release();
            }

            foreach (var key in list)
            {
                _handles.Remove(key);
            }

            await ResourceManager.UnloadUnusedAssets();
        }

        public ResourceGroup AddTo(Component com)
        {
            com.GetCancellationTokenOnDestroy().Register(ReleaseFunc, this);
            return this;

            static void ReleaseFunc(object obj)
            {
                if (obj is not ResourceGroup group)
                {
                    return;
                }

                ResourceManager.ReleaseGroup(group);
            }
        }
    }
}