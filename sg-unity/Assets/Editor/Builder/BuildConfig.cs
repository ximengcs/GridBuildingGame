using UnityEditor;

namespace Builder
{
    public class BuildConfig
    {
        public BuildTarget BuildTarget { get; set; } = BuildTarget.Android;
        public string Version { get; set; } = "1.0.0";
        public string GameConfig { get; set; } = @"GameConfig\config_dev.json";
        public bool GenProto { get; set; }
        public bool GenConfig { get; set; }
        public bool GenerateAll { get; set; }
        public bool CompileTarget { get; set; }
        public bool BuildRes { get; set; }
        public bool CopyRes { get; set; }

        public string CopyTags { get; set; } = "base";

        public bool BuildPlayer { get; set; }
    }
}