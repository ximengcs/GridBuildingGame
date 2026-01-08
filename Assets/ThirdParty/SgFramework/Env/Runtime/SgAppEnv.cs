using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace SgFramework.Env
{
    public class SgAppEnv
    {
        public static SgAppEnv Shared { get; private set; }
        [JsonProperty("env")] public string Env { get; private set; }
        [JsonProperty("config")] public JToken Config { get; private set; }

        public int PlayMode
        {
            get => this["playMode"].Value<int>();
            set => this["playMode"] = value;
        }

        public string Version
        {
            get => this["version"].Value<string>();
            set => this["version"] = value;
        }

        public string HttpServer
        {
            get => this["httpServer"].Value<string>();
            set => this["httpServer"] = value;
        }

        public string CdnServer
        {
            get => this["cdnServer"].Value<string>();
            set => this["cdnServer"] = value;
        }

        public int FrameRate => this["frameRate"].Value<int>();

        public bool LogReportEnable => this["logReportEnable"].Value<bool>();

        private static bool _initialized;

        public static bool Initialize(string path = "game_config")
        {
            if (_initialized)
            {
                return true;
            }

            var asset = Resources.Load<TextAsset>(path);
            if (asset == null)
            {
                return false;
            }

            Shared = JsonConvert.DeserializeObject<SgAppEnv>(asset.text);
            _initialized = true;
            return true;
        }

        public JToken this[string key]
        {
            get => Config[key];
            set => Config[key] = value;
        }
    }
}