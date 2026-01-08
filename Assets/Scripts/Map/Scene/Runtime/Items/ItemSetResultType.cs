
namespace MH.GameScene.Runtime.Entities
{
    /// <summary>
    /// 格子或元素存在状态
    /// </summary>
    public enum ItemSetResultType
    {
        /// <summary>
        /// 格子不存在
        /// </summary>
        NoGrid, 

        /// <summary>
        /// 存在多种元素
        /// </summary>
        MultiItem,

        /// <summary>
        /// 格子上元素自身无变化
        /// </summary>
        Self,

        /// <summary>
        /// 格子上是元素自身，存在空位置
        /// </summary>
        SelfItemOk,

        /// <summary>
        /// 格子存在一种相同的元素
        /// </summary>
        SameTypeItem,

        /// <summary>
        /// 格子存在一种不同的元素
        /// </summary>
        DiffTypeItem,

        /// <summary>
        /// 格子存在, 但不存在元素
        /// </summary>
        Ok
    }
}
