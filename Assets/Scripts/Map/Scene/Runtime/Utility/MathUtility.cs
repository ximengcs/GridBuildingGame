using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Utilities
{
    public class MathUtility
    {
        public static int RotateDirection(Vector2Int size, int direction, List<int> directions)
        {
            if (direction < 1 || direction > 4)
                throw new System.Exception("direction error " + direction);

            if (directions == null || directions.Count == 0)
                return direction;

            int dirIndex = directions.IndexOf(direction) + 1;
            return directions[dirIndex % directions.Count];
        }

        public static void GetDirectionRange(Vector2Int basePos, Vector2Int size, int direction, out Vector2Int x, out Vector2Int y)
        {
            int startX, endX;
            int startY, endY;
            switch (direction)
            {
                case GameConst.DIRECTION_RT:
                    startX = basePos.x;
                    endX = startX + size.x;
                    startY = basePos.y;
                    endY = startY + size.y;
                    break;

                case GameConst.DIRECTION_LT:
                    endX = basePos.x + 1;
                    startX = endX - size.y;
                    startY = basePos.y;
                    endY = startY + size.x;
                    break;

                case GameConst.DIRECTION_LB:
                    endX = basePos.x + 1;
                    startX = endX - size.x;
                    endY = basePos.y + 1;
                    startY = endY - size.y;
                    break;

                case GameConst.DIRECTION_RB:
                    startX = basePos.x;
                    endX = startX + size.x;
                    endY = basePos.y + 1;
                    startY = endY - size.y;
                    break;

                default:
                    throw new System.Exception("direction error " + direction);
            }

            x = new Vector2Int(startX, endX);
            y = new Vector2Int(startY, endY);
        }

        public static Vector2 GetGamePos(Vector2Int mainIndex, Vector2Int size, int direction)
        {
            MathUtility.GetDirectionRange(mainIndex, size, direction,
                out Vector2Int xRange, out Vector2Int yRange);
            Vector2 worldPos = MathUtility.IndexToGamePos(new Vector2Int(xRange.x, yRange.x));
            return worldPos;
        }

        public static Vector2 IndexToGamePos(Vector2Int index)
        {
            Vector2 cellSize = new Vector2(1, 0.5f);
            float cos = Mathf.Cos(45 * Mathf.Deg2Rad);
            float sin = Mathf.Sin(45 * Mathf.Deg2Rad);

            float scaleVec = 0.5f / cos;
            Vector2 pos = new Vector2(index.x * scaleVec, index.y * scaleVec);

            pos = new Vector2(pos.x * cos - pos.y * sin, pos.x * sin + pos.y * cos);
            pos.y = pos.y * cellSize.y + cellSize.y * 0.5f;

            return pos;
        }

        public static Vector2 GamePosToLogicPos(Vector2 pos)
        {
            Vector2 cellSize = new Vector2(1, 0.5f);
            pos.y = (pos.y - cellSize.y * 0.5f) / cellSize.y;

            float cos = Mathf.Cos(-45 * Mathf.Deg2Rad);
            float sin = Mathf.Sin(-45 * Mathf.Deg2Rad);
            pos = new Vector2(pos.x * cos - pos.y * sin, pos.x * sin + pos.y * cos);

            float scaleVec = 0.5f / cos;
            pos = new Vector2(pos.x / scaleVec, pos.y / scaleVec);

            return pos;
        }

        public static Vector2Int GamePosToIndex(Vector2 pos)
        {
            pos = GamePosToLogicPos(pos);
            return new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }

        public static Vector2 RoundGamePos(Vector2 pos)
        {
            return IndexToGamePos(GamePosToIndex(pos));
        }

        public static Vector2Int ScreenPosToIndex(Vector2 screenPos)
        {
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
            Vector2Int index = GamePosToIndex(worldPos);
            return index;
        }
    }
}
