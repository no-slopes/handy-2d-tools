using UnityEngine;
using System;

namespace H2DT.Utils
{
    public static class Math
    {
        public static float Binary(float directionSign)
        {
            return directionSign != 0 ? Mathf.Sign(directionSign) : 0;
        }

        public static Vector2 Sign(Vector2 vector)
        {
            float x = vector.x != 0 ? MathF.Sign(vector.x) : 0;
            float y = vector.y != 0 ? Mathf.Sign(vector.y) : 0;
            return new Vector2(x, y);
        }

        public static Vector2 RoundDirections(Vector2 directions, float limit = 0.4f)
        {
            float x = Mathf.Abs(directions.x) > limit ? MathF.Sign(directions.x) : directions.x;
            float y = Mathf.Abs(directions.y) > limit ? Mathf.Sign(directions.y) : directions.y;
            return new Vector2(x, y);
        }

        public static float ConvertScale(float value, float maxValue, float min, float max)
        {
            float delta = max - min;
            return ((delta * value) + (min * maxValue)) / maxValue;
        }

        public static Vector2Int WorldIntoCellPosAddingOrigin(Vector2 worldPos, Vector2 origin)
        {
            int x = Mathf.FloorToInt((worldPos + origin).x);
            int y = Mathf.FloorToInt((worldPos + origin).y);

            return new Vector2Int(x, y);
        }

        public static Vector2Int WorldIntoCellPosDeducingOrigin(Vector2 worldPos, Vector2 origin)
        {
            int x = Mathf.FloorToInt((worldPos - origin).x);
            int y = Mathf.FloorToInt((worldPos - origin).y);

            return new Vector2Int(x, y);
        }

        public static Vector2 WorldIntoNodePos(Vector2 worldPos)
        {
            int x = Mathf.FloorToInt((worldPos).x);
            int y = Mathf.FloorToInt((worldPos).y);

            return new Vector2(x, y);
        }

        public static Vector2 CenterInWorldPosition(Vector2 worldPos)
        {
            return worldPos + new Vector2(1, 1) * .5f;
        }

    }
}