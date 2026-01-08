using Common;
using Pt;
using SgFramework.Utility;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class FriendItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtName;
        [SerializeField] private Button btnBlock;
        [SerializeField] private Button btnPrivateChat;
        [SerializeField] private Button btnDelete;
        [SerializeField] private Button btnApply;
        [SerializeField] private Button btnAgree;
        [SerializeField] private Button btnRefuse;
        [SerializeField] private Button btnUnblock;

        [SerializeField] private GameObject goList;
        [SerializeField] private GameObject goAdd;
        [SerializeField] private GameObject goApply;
        [SerializeField] private GameObject goBlock;

        [SerializeField] private GameObject goApplied;

        private UserPublicInfo _bindData;

        private void Awake()
        {
            btnBlock.BindClick(async () =>
            {
                var result = await DataController.FriendBlock(_bindData, 1);
                if (!result)
                {
                    Debug.Log("设置黑名单失败");
                    return;
                }

                Debug.Log("设置黑名单成功");
            });
            
            btnPrivateChat.BindClick(() => { Debug.Log("私聊"); });
            btnDelete.BindClick(() => DataController.FriendDel(_bindData));

            btnApply.BindClick(async () =>
            {
                var result = await DataController.FriendApply(_bindData);
                if (!result)
                {
                    Debug.Log("申请失败");
                    return;
                }

                Debug.Log("申请成功");
                goApplied.SetActive(true);
                btnApply.gameObject.SetActive(false);
            });

            btnAgree.BindClick(() => DataController.FriendAgreeApply(_bindData));
            btnRefuse.BindClick(() => DataController.FriendRefuseApply(_bindData));

            btnUnblock.BindClick(async () =>
            {
                var result = await DataController.FriendBlock(_bindData, 0);
                if (!result)
                {
                    Debug.Log("取消黑名单失败");
                    return;
                }

                Debug.Log("取消黑名单成功");
            });
        }

        public void SetData(UserPublicInfo info, int currentPage)
        {
            goList.SetActive(currentPage == 0);
            goAdd.SetActive(currentPage == 1);
            goApply.SetActive(currentPage == 2);
            goBlock.SetActive(currentPage == 3);

            btnApply.gameObject.SetActive(currentPage == 1);
            goApplied.SetActive(false);

            _bindData = info;
            txtName.SetText(info.Name);
        }
    }
}