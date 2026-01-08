
using Newtonsoft.Json;

namespace MH.GameScene.Datas
{
    public class ItemData
    {
        [JsonProperty("id")]    
        public int Id { get; set; }

        [JsonProperty("direction")]
        public int Direction { get; set; }  
    }
}
