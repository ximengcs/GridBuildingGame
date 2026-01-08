using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    /// <summary>
    /// 格子
    /// </summary>
    public interface IGridEntity : IEntity
    {
        /// <summary>
        /// 所属地图场景
        /// </summary>
        IMapScene Scene { get; }

        /// <summary>
        /// 坐标
        /// </summary>
        Vector2Int Index { get; }

        /// <summary>
        /// 层数
        /// </summary>
        int LayerCount { get; }

        /// <summary>
        /// 检查指定层是否存在元素
        /// </summary>
        /// <param name="layer">层</param>
        /// <returns>true表示存在元素</returns>
        bool HasItem(string layer);

        /// <summary>
        /// 检查元素是否在格子中
        /// </summary>
        /// <param name="item">元素</param>
        /// <returns>true表示在格子中</returns>
        bool InGrid(IItemEntity item);

        /// <summary>
        /// 获取所有层的元素集合
        /// </summary>
        IReadOnlyCollection<IItemEntity> Items { get; }

        /// <summary>
        /// 获取给定层的元素
        /// </summary>
        /// <param name="layer">层</param>
        /// <returns>元素</returns>
        IItemEntity GetItem(string layer);
    }
}
