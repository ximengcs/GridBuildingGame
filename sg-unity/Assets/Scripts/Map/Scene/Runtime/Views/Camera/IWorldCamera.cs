
using System;
using UnityEngine;

namespace MH.GameScene.Core.Entites
{
    public interface IWorldCamera : IEntity
    {
        event Action PosChangeEvent;

        void SetRect(Vector2 min, Vector2 max);
        void SetPos(Vector3 pos);
        void SetSize(float value);
        void SetPosEnd(Vector3 pos);
    }
}
