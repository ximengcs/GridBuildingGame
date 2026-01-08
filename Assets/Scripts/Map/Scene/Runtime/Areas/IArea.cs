
using System;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;
using UnityEngine;

namespace MH.GameScene.Runtime.Entities
{
    public interface IArea : IEntity
    {
        int AreaId { get; }

        Vector2 Center { get; }

        IReadOnlyCollection<IGridEntity> Grids { get; }


        event Action GridChangeEvnet;

        void Add(IEnumerable<IGridEntity> grids);

        void Add(IGridEntity grid);

        void Remove(IGridEntity grid);
    }
}
