using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using YooAsset;

namespace SgFramework.Res
{
    public static class ResourceManager
    {
        private static readonly Dictionary<string, AssetInfo> AssetInfos = new Dictionary<string, AssetInfo>();
        private static readonly List<ResourceGroup> ValidGroups = new List<ResourceGroup>();
        public static int ValidGroupCount => ValidGroups.Count;

        public static ResourceGroup GetGroup(string key)
        {
            var group = new ResourceGroup(key);
            ValidGroups.Add(group);
            return group;
        }

        public static void ReleaseGroup(ResourceGroup group)
        {
            group.DestroyAll();
            ValidGroups.Remove(group);
        }

        public static AssetHandle LoadAssetAsync<T>(string key) where T : Object
        {
            return YooAssets.LoadAssetAsync<T>(key);
        }

        public static AllAssetsHandle LoadAllAssetsAsync<T>(string key) where T : Object
        {
            return YooAssets.LoadAllAssetsAsync<T>(key);
        }

        public static async UniTask UnloadUnusedAssets(string packageName = "DefaultPackage")
        {
            await YooAssets.GetPackage(packageName).UnloadUnusedAssetsAsync();
        }

        public static void TryUnloadUnusedAsset(string key, string packageName = "DefaultPackage")
        {
            YooAssets.GetPackage(packageName).TryUnloadUnusedAsset(key);
        }

        public static bool CheckLocationValid(string key)
        {
            return YooAssets.CheckLocationValid(key);
        }

        public static ResourceDownloaderOperation GetTagDownloader(string[] tags, string packageName = "DefaultPackage")
        {
            var downloadingMaxNum = 10;
            var failedTryAgain = 3;
            var downloader = YooAssets.GetPackage(packageName)
                .CreateResourceDownloader(tags, downloadingMaxNum, failedTryAgain);
            return downloader;
        }

        public static ResourceDownloaderOperation GetBundleDownloader(string key, string packageName = "DefaultPackage")
        {
            var downloadingMaxNum = 10;
            var failedTryAgain = 3;
            var downloader = YooAssets.GetPackage(packageName)
                .CreateBundleDownloader(key, downloadingMaxNum, failedTryAgain);
            return downloader;
        }

        public static bool IsNeedDownloadFromRemote(string key, string packageName = "DefaultPackage")
        {
            return YooAssets.GetPackage(packageName).IsNeedDownloadFromRemote(key);
        }
    }
}