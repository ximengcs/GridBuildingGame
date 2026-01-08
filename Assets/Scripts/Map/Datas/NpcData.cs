
using Newtonsoft.Json;
using UnityEngine;

namespace MH.GameScene.Datas
{
    public class NpcData
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonIgnore]
        public Vector2Int Index
        {
            get => new Vector2Int(X, Y);
            set
            {
                X = value.x;
                Y = value.y;
            }
        }
    }
}
