using System.Linq;
using Config;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Google.Protobuf.Collections;
using Pt;
using R3;
using SgFramework.Net;
using SgFramework.RedPoint;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 任务类型
    /// </summary>
    public abstract class TaskType
    {
        /// <summary>
        ///主线任务
        /// </summary>
        public const int Main = 1;

        /// <summary>
        ///每日任务
        /// </summary>
        public const int Daily = 2;

        /// <summary>
        ///成就任务
        /// </summary>
        public const int Achievement = 3;
    }

    /// <summary>
    /// 任务状态
    /// </summary>
    public abstract class TaskStatus
    {
        /// <summary>
        ///未解锁
        /// </summary>
        public const int Locked = 0;

        /// <summary>
        ///任务已解锁
        /// </summary>
        public const int Unlocked = 1;

        /// <summary>
        ///任务已完成
        /// </summary>
        public const int Accomplished = 2;

        /// <summary>
        ///任务已领奖
        /// </summary>
        public const int AwardTaken = 3;
    }

    public partial class DataController
    {
        /// <summary>
        /// 当前id的任务已更新
        /// </summary>
        public static readonly Subject<int> TaskUpdate = new Subject<int>();

        public static void RefreshTaskRedPoint()
        {
            foreach (var (type, info) in Archive.Tasks)
            {
                var value = info.TaskInfo.Sum(pair =>
                    CheckTaskValid(pair.Value) && pair.Value.Status == TaskStatus.Accomplished ? 1 : 0);
                RedPointManager.Instance.FindNode($"task/{type}").SetValue(value);
            }
        }

        public static bool TryGetTask(int type, int id, out Task task)
        {
            task = null;
            return Archive.Tasks.TryGetValue(type, out var info) && info.TaskInfo.TryGetValue(id, out task);
        }

        public static bool TryGetTaskMap(int type, out MapField<int, Task> taskMap)
        {
            taskMap = null;
            if (!Archive.Tasks.TryGetValue(type, out var info))
            {
                return false;
            }

            taskMap = info.TaskInfo;
            return true;
        }

        public static void UpdateTask(PushTaskInfo rsp)
        {
            foreach (var (type, value) in rsp.Info)
            {
                if (!Archive.Tasks.TryGetValue(type, out var info))
                {
                    Archive.Tasks.Add(type, info = new ClientTaskInfo());
                }

                foreach (var task in value.Tasks_)
                {
                    if (!info.TaskInfo.TryGetValue(task.Id, out var ct))
                    {
                        info.TaskInfo.Add(task.Id, ct = task);
                    }
                    else
                    {
                        ct.Process = task.Process;
                        ct.FinishTimes = task.FinishTimes;
                        ct.Status = task.Status;
                    }

                    TaskUpdate.OnNext(ct.Id);
                }
            }

            RefreshTaskRedPoint();
        }

        public static UniTask<IMessage> GetTaskReward(Task task)
        {
            return NetManager.Shared.Request(new GetTaskRewardMsg
            {
                TaskType = task.Type,
                TaskId = task.Id
            });
        }


        public static bool CheckTaskValid(Task task)
        {
            switch (task.Type)
            {
                case TaskType.Main:
                {
                    return Table.TaskMainTable.TryGetById(task.Id, out _);
                }
                case TaskType.Daily:
                {
                    return Table.TaskDailyTable.TryGetById(task.Id, out _);
                }
                case TaskType.Achievement:
                {
                    return Table.TaskAchievementTable.TryGetById(task.Id, out _);
                }
                default:
                {
                    Debug.LogWarning($"任务数据存在，但没有找到对应的配置表 {task.Type} - {task.Id}");
                    return false;
                }
            }
        }

        public static int GetTaskOrder(Task task)
        {
            var offset = 0;
            if (task.Status == TaskStatus.AwardTaken)
            {
                offset = 10000000;
            }

            switch (task.Type)
            {
                case TaskType.Main:
                {
                    // 主线任务不需要
                    // if (Table.TaskMainTable.TryGetById(task.Id, out var config))
                    // {
                    //     return 0;
                    // }
                    //
                    // break;

                    return 0;
                }
                case TaskType.Daily:
                {
                    if (Table.TaskDailyTable.TryGetById(task.Id, out var config))
                    {
                        return offset + config.order;
                    }

                    break;
                }
                case TaskType.Achievement:
                {
                    if (Table.TaskAchievementTable.TryGetById(task.Id, out var config))
                    {
                        return offset + config.order;
                    }

                    break;
                }
            }

            return int.MaxValue;
        }
    }
}