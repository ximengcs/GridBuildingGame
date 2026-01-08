
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public interface IAreaModule : IEntity
    {
        IReadOnlyCollection<IArea> Areas { get; }

        bool Contains(int areaId);

        bool Register(Vector2Int index);

        void UnRegister(Vector2Int index);

        IArea AddArea(int id);

        void RemoveArea(int id);
    }
}
