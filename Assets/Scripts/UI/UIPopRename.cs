using Common;
using Config;
using SgFramework.Language;
using SgFramework.UI;
using SgFramework.Utility;
using System;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopRename.prefab")]
    public class UIPopRename : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private Button btnOk;
        [SerializeField] private TextMeshProUGUI txtPrice;
        [SerializeField] private TMP_InputField inputName;


        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopRename>);
            btnOk.BindClick(OnBtnOk);
        }

        private async void OnBtnOk()
        {
            try
            {
                if (DataController.GetCurrency(SgConst.CurrencyGem) < Table.Global.ChangeNickNameCost)
                {
                    UIToast.Instance.ShowToast(LanguageManager.Get("Player_information3")).Forget();
                    return;
                }

                var minLen = Table.Global.NicknameMinLength;
                var maxLen = Table.Global.NicknameMaxLength;
                var nickName = inputName.text;
                if (nickName.Length < minLen || nickName.Length > maxLen)
                {
                    UIToast.Instance.ShowToast(string.Format(LanguageManager.Get("Player_information4"), minLen, maxLen)).Forget();
                    return;
                }

                if (!await CheckSensor(nickName))
                {
                    UIToast.Instance.ShowToast(LanguageManager.Get("Player_information5")).Forget();
                    return;
                }
            
                if (await DataController.ModifyUserNameMsg(nickName))
                {
                    await UIManager.Close<UIPopRename>();
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private static async UniTask<bool> CheckSensor(string nickName)
        {
            var tcs =  new UniTaskCompletionSource<bool>();

            void Callback(bool isLegal)
            {
                tcs.TrySetResult(isLegal);
            }
            
            Callback(!nickName.Contains("fuck"));
            return await tcs.Task;
        }

    }
}