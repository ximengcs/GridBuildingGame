using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Runtime.Utilities
{
    public class PointsUtility
    {
        public static Vector2 GetCenter(List<Vector2> points)
        {
            float x = 0;
            float y = 0;
            foreach (Vector2 point in points)
            {
                x += point.x;
                y += point.y;
            }
            Vector2 result = new Vector2(x / points.Count, y / points.Count);
            return result;
        }
    }
}
