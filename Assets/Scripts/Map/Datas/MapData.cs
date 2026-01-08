using Newtonsoft.Json;
using System.Collections.Generic;

namespace MH.GameScene.Datas
{
    public class MapData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("viewMin")]
        public SerializePos ViewMin { get; set; }

        [JsonProperty("viewMax")]
        public SerializePos ViewMax { get; set; }

        [JsonProperty("elements")]
        public List<GridData> Elements { get; set; }

        [JsonProperty("npcs")]
        public List<NpcData> Npcs { get; set; }

        [JsonProperty("areas")]
        public List<AreaData> Areas { get; set; }
    }
}
