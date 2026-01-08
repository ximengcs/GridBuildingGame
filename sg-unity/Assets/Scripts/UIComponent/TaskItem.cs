using Common;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.Language;
using SgFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class TaskItem : MonoBehaviour
    {
        [SerializeField] private LanguageText txtName;
        [SerializeField] private Slider sliderProgress;
        [SerializeField] private Button btnGet;
        [SerializeField] private GameObject objIsGot;

        private Task _bindData;

        private void Awake()
        {
            btnGet.BindClick(OnClickGet);
        }

        private async UniTask OnClickGet()
        {
            await DataController.GetTaskReward(_bindData);
        }

        public void SetData(Task data)
        {
            _bindData = data;
            RefreshView();
        }

        private void RefreshView()
        {
            switch (_bindData.Type)
            {
                case TaskType.Main:
                {
                    if (!Table.TaskDailyTable.DataDict.TryGetValue(_bindData.Id, out var config))
                    {
                        break;
                    }

                    txtName.SetText(SgTaskUtility.GetTaskDesc(config.task_desc, config.condition));
                    sliderProgress.value = _bindData.Process;
                    sliderProgress.maxValue = config.condition.para1;
                    break;
                }
                case TaskType.Daily:
                {
                    if (!Table.TaskDailyTable.DataDict.TryGetValue(_bindData.Id, out var config))
                    {
                        break;
                    }

                    txtName.SetText(SgTaskUtility.GetTaskDesc(config.task_desc, config.condition));
                    sliderProgress.value = _bindData.Process;
                    sliderProgress.maxValue = config.condition.para1;
                    break;
                }
                case TaskType.Achievement:
                {
                    if (!Table.TaskAchievementTable.DataDict.TryGetValue(_bindData.Id, out var config))
                    {
                        break;
                    }

                    txtName.SetText(SgTaskUtility.GetAchievementDesc(config, _bindData));
                    sliderProgress.value = _bindData.Process;
                    sliderProgress.maxValue = config.achievement_type.para1;
                    break;
                }
            }

            btnGet.gameObject.SetActive(_bindData.Status == TaskStatus.Accomplished);
            objIsGot.SetActive(_bindData.Status == TaskStatus.AwardTaken);
        }
    }
}