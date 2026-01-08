using System;
using SgFramework.Event;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PatchLogic
{
    public class PatchWindow : MonoBehaviour
    {
        private readonly EventGroup _eventGroup = new EventGroup();

        // UGUI相关
        public MessageBox messageBoxObj;
        public Slider slider;
        public TextMeshProUGUI tips;

        private void Awake()
        {
            tips.text = "Initializing the game world !";
            messageBoxObj.Hide();

            _eventGroup.AddListener<PatchEventDefine.InitializeFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchStatesChange>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.FoundUpdateFiles>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.DownloadProgressUpdate>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PackageVersionUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.PatchManifestUpdateFailed>(OnHandleEventMessage);
            _eventGroup.AddListener<PatchEventDefine.WebFileDownloadFailed>(OnHandleEventMessage);
        }

        private void OnDestroy()
        {
            _eventGroup.RemoveAllListener();
        }

        /// <summary>
        /// 接收事件
        /// </summary>
        private void OnHandleEventMessage(IEventMessage message)
        {
            switch (message)
            {
                case PatchEventDefine.InitializeFailed:
                {
                    ShowMessageBox($"Failed to initialize package !",
                        UserEventDefine.UserTryInitialize.SendEventMessage);
                    break;
                }
                case PatchEventDefine.PatchStatesChange msg:
                {
                    tips.text = msg.Tips;
                    break;
                }
                case PatchEventDefine.FoundUpdateFiles msg:
                {
                    var sizeMb = msg.TotalSizeBytes / 1048576f;
                    sizeMb = Mathf.Clamp(sizeMb, 0.1f, float.MaxValue);
                    var totalSizeMb = sizeMb.ToString("f1");
                    ShowMessageBox($"Found update patch files, Total count {msg.TotalCount} Total size {totalSizeMb}MB",
                        UserEventDefine.UserBeginDownloadWebFiles.SendEventMessage);
                    break;
                }
                case PatchEventDefine.DownloadProgressUpdate msg:
                {
                    slider.value = (float)msg.CurrentDownloadSizeBytes / msg.TotalDownloadSizeBytes;
                    var currentSizeMb = (msg.CurrentDownloadSizeBytes / 1048576f).ToString("f1");
                    var totalSizeMb = (msg.TotalDownloadSizeBytes / 1048576f).ToString("f1");
                    tips.text =
                        $"{msg.CurrentDownloadCount}/{msg.TotalDownloadCount} {currentSizeMb}MB/{totalSizeMb}MB";
                    break;
                }
                case PatchEventDefine.PackageVersionUpdateFailed:
                {
                    ShowMessageBox($"Failed to update static version, please check the network status.",
                        UserEventDefine.UserTryUpdatePackageVersion.SendEventMessage);
                    break;
                }
                case PatchEventDefine.PatchManifestUpdateFailed:
                {
                    ShowMessageBox($"Failed to update patch manifest, please check the network status.",
                        UserEventDefine.UserTryUpdatePatchManifest.SendEventMessage);
                    break;
                }
                case PatchEventDefine.WebFileDownloadFailed msg:
                {
                    ShowMessageBox($"Failed to download file : {msg.FileName}",
                        UserEventDefine.UserTryDownloadWebFiles.SendEventMessage);
                    break;
                }
                default:
                    throw new NotImplementedException($"{message.GetType()}");
            }
        }

        /// <summary>
        /// 显示对话框
        /// </summary>
        private void ShowMessageBox(string content, Action ok)
        {
            // 显示对话框
            messageBoxObj.Show(content, ok);
        }
    }
}