
using System.Collections.Generic;

namespace MH.GameScene.Core.PathFinding
{
    /// <summary>
    /// A星寻路辅助器
    /// </summary>
    public interface IAStarHelper
    {
        /// <summary>
        /// 获取单元格周围的单元
        /// </summary>
        /// <param name="item">目标单元格实例</param>
        /// <param name="result">结果列表, 传入列表为空，不用手动清空</param>
        void GetItemRound(object item, HashSet<object> result);

        /// <summary>
        /// 获取单元格实例的唯一标识
        /// </summary>
        /// <param name="item">目标单元格实例</param>
        /// <returns>每个单元格唯一标识</returns>
        int GetUniqueId(object item);

        /// <summary>
        /// 获取H权重值(预估值)
        /// </summary>
        /// <param name="start">起始单元格</param>
        /// <param name="end">结束单元格</param>
        /// <returns>H值</returns>
        int GetHValue(object start, object end);

        /// <summary>
        /// 获取G权重值
        /// </summary>
        /// <param name="from">源单元格</param>
        /// <param name="to">目标单元格</param>
        /// <returns>G值</returns>
        int GetGValue(object from, object to);
    }
}
