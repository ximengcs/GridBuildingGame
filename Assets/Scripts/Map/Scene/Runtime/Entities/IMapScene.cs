using System;
using UnityEngine;
using MH.GameScene.Core.Entites;
using System.Collections.Generic;

namespace MH.GameScene.Runtime.Entities
{
    /// <summary>
    /// 地图场景
    /// </summary>
    public interface IMapScene : IEntity
    {
        /// <summary>
        /// 所有格子集合
        /// </summary>
        IReadOnlyCollection<IGridEntity> Grids { get; }

        /// <summary>
        /// 获取临近的格子
        /// </summary>
        /// <param name="index">目标格子坐标</param>
        /// <param name="result">存放结果的集合</param>
        /// <param name="filter">过滤器</param>
        void GetAdjacentGrid(Vector2Int index, ICollection<object> result, Func<IGridEntity, bool> filter = null);

        /// <summary>
        /// 注册场景初始化完成事件
        /// </summary>
        /// <param name="handler">回调</param>
        void RegisterInitFinish(Action handler);

        /// <summary>
        /// 指定坐标上是否存在格子
        /// </summary>
        /// <param name="index">格子坐标</param>
        /// <returns>true表示格子存在</returns>
        bool HasGrid(Vector2Int index);

        /// <summary>
        /// 添加指定坐标的格子，如果已经存在则直接返回存在的格子
        /// </summary>
        /// <param name="index">格子坐标</param>
        /// <returns>不存在时返回新添加的格子，已经存在时直接返回存在的格子</returns>
        IGridEntity AddGrid(Vector2Int index);

        /// <summary>
        /// 获取指定坐标的格子
        /// </summary>
        /// <param name="index">格子坐标</param>
        /// <returns>存在时直接返回，不存在时返回null</returns>
        IGridEntity GetGrid(Vector2Int index);

        /// <summary>
        /// 确保给定参数的元素参数格子组存在，不存在时添加新格子组
        /// </summary>
        /// <param name="param">元素生成参数</param>
        void EnsureGrid(ItemGenParam param);

        /// <summary>
        /// <para>
        ///     当指定格子组存在元素时，会直接删除这个元素，并添加新的元素
        /// </para>
        /// <para>
        ///     当指定格子组不存在元素时，会直接添加新的元素
        /// </para>
        /// 可在调用该方法前通过<see cref="CheckItemSet"/>方法检查格子组的情况，不同情况不同处理
        /// </summary>
        /// <param name="param">元素生成参数</param>
        /// <param name="ensureGrid">
        /// <para>
        /// 此方法不会检查格子组存在性,该参数为true时会确保格子组都存在,
        /// 该参数为false时会出现错误
        /// </para>
        /// </param>
        IItemEntity SetItem(ItemGenParam param, bool ensureGrid);

        /// <summary>
        /// 移动目标元素到指定的参数位置上
        /// <para>
        /// 此方法不会检查目标格子组的元素是否存在,所以在调用前需要根据不同情况处理,
        /// 可在调用该方法前通过<see cref="CheckItemSet"/>方法检查格子组的情况，不同情况不同处理
        /// </para>
        /// </summary>
        /// <param name="item">目标元素</param>
        /// <param name="param">目标参数位置</param>
        /// <param name="ensureGrid">
        /// <para>
        /// 此方法不会检查格子存在性,该参数为true时会确保元素所覆盖的格子都存在,
        /// 该参数为false时会出现错误
        /// </para>
        /// </param>
        void MoveItem(IItemEntity item, ItemGenParam param, bool ensureGrid);

        /// <summary>
        /// 移除指定坐标指定层的元素
        /// </summary>
        /// <param name="index">坐标</param>
        /// <param name="layer">层</param>
        void RemoveItem(Vector2Int index, string layer);

        /// <summary>
        /// 检查给定参数的格子组的存在状态或元素存在状态
        /// </summary>
        /// <param name="param">元素格子组参数</param>
        /// <param name="checkSelf">是否检查原始格子的元素</param>
        /// <param name="index">当checkSelf生效时，此参数为原始格子坐标</param>
        /// <param name="extraLayers">额外检查层列表，当需要同时检查其他层元素时传入此参数</param>
        /// <returns>
        /// <para> 当格子元素没有发生变化时，返回<see cref="ItemSetResultType.Self"/> </para>
        /// <para> 当至少有一个格子不存在时，返回<see cref="ItemSetResultType.NoGrid"/> </para>
        /// <para> 当存在多种不同的元素时，返回<see cref="ItemSetResultType.MultiItem"/> </para>
        /// <para> 当只存在一种元素，且是自身，同时其他位置是空格子，返回<see cref="ItemSetResultType.SelfItemOk"/> </para>
        /// <para> 当只存在一种元素，且类型相同时，返回<see cref="ItemSetResultType.SameTypeItem"/> </para>
        /// <para> 当只存在一种元素，且类型不相同时，返回<see cref="ItemSetResultType.DiffTypeItem"/> </para>
        /// <para> 当所有格子都存在，且不存在任何元素时，返回<see cref="ItemSetResultType.Ok"/> </para>
        /// </returns>
        ItemSetResult CheckItemSet(ItemGenParam param, bool checkSelf = false, Vector2Int index = default, GridCheckFilter filter = null);

        /// <summary>
        /// 获取第一个添加的此类型元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>元素实例</returns>
        T GetFirstItem<T>() where T : IItemEntity;

        /// <summary>
        /// 获取所有类型的元素
        /// </summary>
        /// <typeparam name="T">元素类型</typeparam>
        /// <returns>获取的集合</returns>
        IReadOnlyCollection<T> GetItems<T>() where T : IItemEntity;
    }
}
