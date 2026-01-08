using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SgFramework.Res;
using UnityEngine;

namespace Common
{
    public class RuntimeConfig
    {
        public static RuntimeConfig Shared { get; } = new RuntimeConfig();
        private JToken Data { get; set; } = new JObject();

        private static bool _initialized;

        private RuntimeConfig()
        {
        }

        public static async UniTask Initialize()
        {
            if (_initialized)
            {
                return;
            }

            var handle = ResourceManager.LoadAssetAsync<TextAsset>("Assets/GameRes/RuntimeConfig.json");
            await handle;
            Shared.Data = JsonConvert.DeserializeObject<JToken>(handle.GetAssetObject<TextAsset>().text);
            handle.Release();
            _initialized = true;
        }

        public JToken this[string key] => Data[key];
    }
}