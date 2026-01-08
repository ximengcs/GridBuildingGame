using Newtonsoft.Json;

namespace MH.GameScene.Configs
{
    public class NpcConfig : IConfig
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("layer")]
        public string Layer { get; set; }

        [JsonProperty("res")]
        public string Res { get; set; }
    }
}
