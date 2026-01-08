using Cysharp.Threading.Tasks;
using SgFramework.Net;
using SgFramework.UI;
using SgFramework.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopNetError.prefab")]
    public class UIPopNetError : UIPop
    {
        [SerializeField] private Button btnClose;

        private void Start()
        {
            btnClose.BindClick(OnClickRetry);
        }

        private void OnClickRetry()
        {
            UIManager.Close<UIPopNetError>().Forget();
            var session = NetManager.Create();
            session.Error += NetManager.DefaultError;
            session.GuestLogin().Forget();
        }
    }
}