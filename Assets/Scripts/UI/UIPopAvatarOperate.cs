using Common;
using Config;
using Cysharp.Threading.Tasks;
using SgFramework.Language;
using SgFramework.Res;
using SgFramework.UI;
using SgFramework.Utility;
using System;
using System.Collections.Generic;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopAvatarOperate.prefab")]
    public class UIPopAvatarOperate : UIPop
    {

        [SerializeField] private GameObject frameScroll;
        [SerializeField] private GameObject frameCtx;
        [SerializeField] private GameObject frameItem;
        [SerializeField] private GameObject avatarScroll;
        [SerializeField] private GameObject avatarCtx;
        [SerializeField] private GameObject avatarItem;
        [SerializeField] private Button btnOk;
        [SerializeField] private LanguageText btnOkText;
        [SerializeField] private Button btnClose;
        [SerializeField] private ToggleGroup page;
        [SerializeField] private Toggle togAvatar;
        [SerializeField] private Toggle togFrame;
        [SerializeField] private LanguageText txtGetWay;

        // 当前显示的界面：0:头像;1:头像框;
        private int _curPage = -1;
        private readonly List<GameObject> _pool = new(0);
        private UIAvatarItem _curAvatar;
        private UIAvatarItem _curFrame;
        private ResourceGroup _resGroup;
        private Vector3 _tmpScale = Vector3.one;


        private void Start()
        {

            btnClose.BindClick(UIManager.Close<UIPopAvatarOperate>);
            btnOk.BindClick(OnBtnOk);
            togAvatar.onValueChanged.AddListener(OnTogAvatar);
            togFrame.onValueChanged.AddListener(OnTogFrame);

            _resGroup = ResourceManager.GetGroup("Avatar").AddTo(this);
            RefreshAvatar();
        }

        private void RefreshAvatar()
        {
            if (_curPage == 0)
            {
                return;
            }
            _curPage = 0;
            frameScroll.SetActive(false);
            avatarScroll.SetActive(true);
            OnAvatarSelect(null);

            _pool.Clear();
            for (var i = 0; i < avatarCtx.transform.childCount; i++)
            {
                var it = avatarCtx.transform.GetChild(i).gameObject;
                it.SetActive(false);
                _pool.Add(it);
            }

            var data = Table.AvatarTable.DataList;
            for (var i = 0; i < data.Count; i++)
            {
                GameObject it;
                if (_pool.Count > 0)
                {
                    it = _pool[^1];
                    _pool.RemoveAt(_pool.Count - 1);
                }
                else
                {
                    it = Instantiate(avatarItem, avatarCtx.transform, false);
                }
                it.SetActive(true);
                _tmpScale.Set(1, 1, 1);
                it.transform.localScale = _tmpScale;
                it.transform.SetSiblingIndex(i);
                var config = data[i];
                it.GetComponent<UIAvatarItem>().SetData(true, _resGroup, this.OnAvatarSelect, config);

                if (config.id == DataController.UserInfo.Avatar)
                {
                    OnAvatarSelect(it.GetComponent<UIAvatarItem>());
                }
            }
        }

        private void OnAvatarSelect(UIAvatarItem item)
        {
            if (_curAvatar != null)
            {
                _curAvatar.SetSelect(false);
            }
            _curAvatar = item;
            if (_curAvatar == null)
            {
                txtGetWay.gameObject.SetActive(false);
                return;
            }

            txtGetWay.gameObject.SetActive(!item.IsUnlock);
            _curAvatar.SetSelect(true);
            btnOk.interactable = !item.IsUse;
            btnOk.gameObject.SetActive(item.IsUnlock);
            btnOkText.SetKey(item.IsUse ? "UI_Text_Player_information11" : "UI_Text_Player_information8");

            if (item.IsNew)
            {
                DataController.UserDelNewIdMsg(item.ID, 0).Forget();
            }
            if (item.IsUnlock)
            {
                txtGetWay.SetKey(item.ItName);
            }
            else
            {
                var config = item.Config as Config.Avatar;
                if (config is { unlock_type: 4 })
                {

                }
                else
                {
                    if (config == null)
                    {
                        Debug.LogError("Task not found ", item);
                        return;
                    }
                    if (config.condition == null)
                    {
                        txtGetWay.SetKey(config.avatar_desc);
                        return;
                    }

                    switch (config.condition.type)
                    {
                        case 3038:
                            txtGetWay.SetKey(config.avatar_desc, Table.ItemTable.GetById(config.condition.para1).item_name);
                            break;
                        case 3009:
                            txtGetWay.SetKey(config.avatar_desc, Table.CurrencyTable.GetById(config.condition.para1).currency_name);
                            break;
                        default:
                            {
                                var progress = 0;
                                if (DataController.TryGetTask(4, config.id, out var task))
                                {
                                    progress = task.Process;
                                }
                                txtGetWay.SetKey(config.avatar_desc, progress, config.condition.para2);
                            }
                            break;
                    }
                }
            }
        }

        private void RefreshFrame()
        {
            if (_curPage == 1)
            {
                return;
            }
            _curPage = 1;
            frameScroll.SetActive(true);
            avatarScroll.SetActive(false);
            OnFrameSelect(null);

            _pool.Clear();
            for (var i = 0; i < frameCtx.transform.childCount; i++)
            {
                var it = frameCtx.transform.GetChild(i).gameObject;
                it.SetActive(false);
                _pool.Add(it);
            }

            var data = Table.AvatarFrameTable.DataList;
            for (int i = 0; i < data.Count; i++)
            {
                var config = data[i];
                GameObject it;
                if (_pool.Count > 0)
                {
                    it = _pool[^1];
                    _pool.RemoveAt(_pool.Count - 1);
                }
                else
                {
                    it = Instantiate(frameItem, frameCtx.transform, false);
                }
                it.SetActive(true);
                _tmpScale.Set(1, 1, 1);
                it.transform.localScale = _tmpScale;
                it.transform.SetSiblingIndex(i);
                it.GetComponent<UIAvatarItem>().SetData(false, _resGroup, this.OnFrameSelect, config);

                if (config.id == DataController.UserInfo.AvatarFrame)
                {
                    OnFrameSelect(it.GetComponent<UIAvatarItem>());
                }
            }
        }

        private void OnFrameSelect(UIAvatarItem item)
        {
            if (_curFrame != null)
            {
                _curFrame.SetSelect(false);
            }
            _curFrame = item;
            if (_curFrame == null)
            {
                txtGetWay.gameObject.SetActive(false);
                return;
            }

            txtGetWay.gameObject.SetActive(!item.IsUnlock);
            _curFrame.SetSelect(true);
            btnOk.interactable = !item.IsUse;
            btnOk.gameObject.SetActive(item.IsUnlock);
            btnOkText.SetKey(item.IsUse ? "UI_Text_Player_information11" : "UI_Text_Player_information8");

            if (item.IsNew)
            {
                DataController.UserDelNewIdMsg(item.ID, 0).Forget();
            }
            if (item.IsUnlock)
            {
                txtGetWay.SetKey(item.ItName);
            }
            else
            {
                if (item.Config is not AvatarFrame config)
                {
                    Debug.LogError("Task not found ", item);
                    return;
                }
                if (config.condition == null)
                {
                    txtGetWay.SetKey(config.avatarframe_desc);
                    return;
                }

                var progress = 0;
                if (DataController.TryGetTask(9, config.id, out var task))
                {
                    progress = task.Process;
                }

                if (config.condition.type == 3009)
                {
                    txtGetWay.SetKey(config.avatarframe_desc, Table.CurrencyTable.GetById(config.condition.para1).currency_name);
                }
                else if (progress > 0)
                {
                    txtGetWay.SetKey(config.avatarframe_desc, progress, config.condition.para2);
                }
                else if (config.avatarframe_desc != null)
                {
                    txtGetWay.SetKey(config.avatarframe_desc, config.avatarframe_name);
                }
            }
        }

        private async void OnBtnOk()
        {
            try
            {
                switch (_curPage)
                {
                    case 0:
                    case 1 when !_curAvatar:
                        UIToast.Instance.ShowToast(LanguageManager.Get("Player_information1")).Forget();
                        return;
                    case 2 when !_curFrame:
                        UIToast.Instance.ShowToast(LanguageManager.Get("Player_information2")).Forget();
                        return;
                    case 1:
                        {
                            if (_curAvatar.ID != DataController.UserInfo.Avatar)
                            {
                                if (_curAvatar.IsUse)
                                {
                                    return;
                                }
                                if (!_curAvatar.IsUnlock)
                                {
                                    return;
                                }
                                await DataController.ModifyUserAvatarMsg(_curAvatar.ID);
                            }

                            break;
                        }
                    case 2:
                        {
                            if (_curFrame.ID != DataController.UserInfo.AvatarFrame)
                            {
                                if (_curFrame.IsUse)
                                {
                                    return;
                                }
                                if (!_curFrame.IsUnlock)
                                {
                                    return;
                                }
                                await DataController.ModifyUserAvatarFrameMsg(_curFrame.ID);
                            }

                            break;
                        }
                }

                await UIManager.Close<UIPopAvatarOperate>();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }


        private void OnTogAvatar(bool newValue)
        {
            if (newValue)
            {
                RefreshAvatar();
            }
        }

        private void OnTogFrame(bool newValue)
        {
            if (newValue)
            {
                RefreshFrame();
            }
        }

    }
}