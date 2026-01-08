
using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public interface IIntersect
    {
        void OnMoving(Vector2 screenPos);

        void OnPointDown(Vector2 screenPos);

        void OnPointing(Vector2 screenPos);

        void OnPointUp(Vector2 screenPos);
    }

    public interface IScenentersect
    {
        void OnMoving(Vector2 screenPos);

        void OnPointDown(Vector2 screenPoint);

        void OnPointing(Vector2 screenPoint);

        void OnPointUp(Vector2 screenPoint);

        void OnScale(float value);
    }
}
