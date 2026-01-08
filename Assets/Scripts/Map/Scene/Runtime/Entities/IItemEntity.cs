using System;
using UnityEngine;
using System.Collections.Generic;
using MH.GameScene.Core.Entites;

namespace MH.GameScene.Runtime.Entities
{
    /// <summary>
    /// 元素
    /// </summary>
    public interface IItemEntity : IEntity
    {
        /// <summary>
        /// 格子改变事件
        /// </summary>
        event Action GridChangeEvent;

        /// <summary>
        /// 元素所在的格子集合
        /// </summary>
        IReadOnlyCollection<IGridEntity> Grids { get; }

        /// <summary>
        /// 主格子
        /// </summary>
        IGridEntity MainGrid { get; }

        /// <summary>
        /// 所在层
        /// </summary>
        string Layer { get; }

        /// <summary>
        /// Id
        /// </summary>
        int ItemId { get; }

        /// <summary>
        /// 方向
        /// </summary>
        int Direction { get; }

        /// <summary>
        /// 大小
        /// </summary>
        Vector2Int Size { get; }
    }
}
