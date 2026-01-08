
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Datas
{
    public class GridData
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        [JsonProperty("objects")]
        public Dictionary<string, ItemData> Objects;

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
