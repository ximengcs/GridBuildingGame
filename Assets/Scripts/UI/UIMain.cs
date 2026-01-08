using SgFramework.UI;
using SgFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UIMain.prefab")]
    public class UIMain : UIForm
    {
        [SerializeField] private Button btnSettings;
        [SerializeField] private Button btnMail;
        [SerializeField] private Button btnTask;
        [SerializeField] private Button btnFriend;
        [SerializeField] private Button btnBag;

        private void Start()
        {
            btnSettings.BindClick(() => UIManager.Open<UISettings>());
            btnMail.BindClick(() => UIManager.Open<UIPopMail>());
            btnTask.BindClick(() => UIManager.Open<UIPopTask>());
            btnBag.BindClick(() => UIManager.Open<UIPopBag>());
            btnFriend.BindClick(async () =>
            {
                if (!SgFunctionUnlocked.AddFriend)
                {
                    UIToast.Instance.ShowToast("功能暂未解锁").Forget();
                    return;
                }

                await UIManager.Open<UIPopFriend>();
            });
        }
    }
}