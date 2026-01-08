using UnityEngine;
using Newtonsoft.Json;

namespace MH.GameScene.Datas
{
    public struct SerializeIntPos
    {
        [JsonProperty("x")]
        public int X { get; set; }

        [JsonProperty("y")]
        public int Y { get; set; }

        public SerializeIntPos(Vector2Int index)
        {
            X = index.x;
            Y = index.y;
        }

        public static implicit operator SerializeIntPos(Vector2Int pos)
        {
            return new SerializeIntPos(pos);
        }

        public static implicit operator Vector2Int(SerializeIntPos pos)
        {
            return new Vector2Int(pos.X, pos.Y);
        }
    }

    public struct SerializePos
    {
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        public SerializePos(Vector2 pos)
        {
            X = pos.x;
            Y = pos.y;
        }

        public static implicit operator SerializePos(Vector2 pos)
        {
            return new SerializePos(pos);
        }

        public static implicit operator Vector2(SerializePos pos)
        {
            return new Vector2(pos.X, pos.Y);
        }
    }
}
