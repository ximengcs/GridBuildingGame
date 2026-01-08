using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace MH.GameScene.Configs
{
    public class ItemConfig : IConfig
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("type")]
        public int Type { get; set; }

        [JsonProperty("size")]
        public Vector2Int Size { get; set; }

        [JsonProperty("layer")]
        public string Layer { get; set; }

        [JsonProperty("Res")]
        public string Res { get; set; }

        [JsonProperty("directions")]
        public List<int> Directions { get; set; }
    }
}
