using System.Collections.Generic;
using System.Linq;
using Common;
using Config;
using Pt;
using R3;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopTask.prefab")]
    public class UIPopTask : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private LoopListView2 taskList;

        [SerializeField] private List<Toggle> typeTab;

        private readonly List<int> _tabMapper = new List<int>
        {
            Common.TaskType.Daily,
            Common.TaskType.Achievement
        };

        private int Count => ListData?.Count ?? 0;

        private int _taskType;

        private int TaskType
        {
            set
            {
                _taskType = value;

                ListData = DataController.TryGetTaskMap(_taskType, out var taskMap)
                    ? taskMap.Values.Where(DataController.CheckTaskValid).ToList()
                    : null;
                SortList(ListData);
                taskList.SetListItemCount(Count);
                taskList.ResetListView();
            }
        }

        private List<Task> ListData { get; set; }

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopTask>);
            for (var i = 0; i < typeTab.Count; i++)
            {
                var idx = i;
                typeTab[i].onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    TaskType = _tabMapper[idx];
                });
            }

            taskList.InitListView(Count, OnGetItemByIndex);
            TaskType = Common.TaskType.Daily;

            DataController.TaskUpdate.Subscribe(_ =>
            {
                SortList(ListData);
                taskList.RefreshAllShownItem();
            }).AddTo(this);
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index > Count)
            {
                return null;
            }

            var task = ListData[index];
            var item = list.NewListViewItem("TaskItem");
            if (item.TryGetComponent(out TaskItem taskItem))
            {
                taskItem.SetData(task);
            }

            return item;
        }

        private static void SortList(List<Task> taskData)
        {
            if (taskData == null || taskData.Count < 2)
            {
                return;
            }

            taskData.Sort((a, b) =>
            {
                var ao = DataController.GetTaskOrder(a);
                var bo = DataController.GetTaskOrder(b);
                if (ao == bo)
                {
                    return 0;
                }

                return ao < bo ? -1 : 1;
            });
        }
    }
}