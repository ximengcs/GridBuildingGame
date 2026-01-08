using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Datas
{
    public class AreaData
    {
        [JsonProperty("areaId")]
        public int AreaId { get; set; }

        [JsonProperty("colorR")]
        public float ColorR { get; set; }

        [JsonProperty("colorG")]
        public float ColorG { get; set; }

        [JsonProperty("colorB")]
        public float ColorB { get; set; }

        [JsonProperty("indexList")]
        public List<SerializeIntPos> IndexList { get; set; }

        [JsonProperty("centerX")]
        public float CenterX { get; set; }

        [JsonProperty("centerY")]
        public float CenterY { get; set; }

        [JsonIgnore]
        public Vector2 CenterPos
        {
            get => new Vector2(CenterX, CenterY);
            set
            {
                CenterX = value.x;
                CenterY = value.y;
            }
        }

        [JsonIgnore]
        public Color Color
        {
            get => new Color(ColorR, ColorG, ColorB);
            set
            {
                ColorR = value.r;
                ColorG = value.g;
                ColorB = value.b;
            }
        }
    }
}
