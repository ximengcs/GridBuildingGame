using Common;
using SgFramework.Language;
using SgFramework.RedPoint;
using SgFramework.Res;
using SgFramework.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;
using Avatar = Config.Avatar;

namespace UIComponent
{
    public class UIAvatarItem : MonoBehaviour
    {
        [SerializeField] private Button btnSelect;
        [SerializeField] private Image imgIcon;
        [SerializeField] private GameObject imgSelect;
        [SerializeField] private GameObject imgUse;
        [SerializeField] private GameObject imgLock;
        [SerializeField] private LanguageText txtName;
        [SerializeField] private RedPointComponent redPoint;

        public delegate void OnItemSelect(UIAvatarItem item);
        [NonSerialized] public int ID;
        [NonSerialized] public bool IsUnlock;
        [NonSerialized] public bool IsUse;
        [NonSerialized] private bool _isAvatar;
        [NonSerialized] public bool IsNew;
        [NonSerialized] public string ItName;
        [NonSerialized] public object Config;

        private OnItemSelect _itemSelect;
        private ResourceGroup _resGroup;

        
        private void Start()
        {
            btnSelect.BindClick(() =>
            {
                _itemSelect.Invoke(this);
            });
        }

        //public void SetData(bool isAvatar, ResourceGroup resGroup, OnItemSelect call, bool isUnlock, bool isNew, bool isUse, object config)
        public void SetData(bool isAvatar, ResourceGroup resGroup, OnItemSelect call, object config)
        {
            _isAvatar = isAvatar;
            _itemSelect = call;
            Config = config;

            if (isAvatar)
            {
                if (config is Avatar avatarCfg)
                {
                    ID = avatarCfg.id;
                    IsUnlock = DataController.TryGetAvatar(ID, out var item);
                    IsUse = DataController.UserInfo.Avatar == ID;
                    IsNew = item is { Value: true };

                    redPoint.SetPath("playerInfo/avatar/avatar/item" + ID);
                    ItName = avatarCfg.avatar_name;
                    txtName.SetKey(ItName);
                    AvatarComp.SetAvatar(imgIcon, resGroup, ID, () => _isAvatar && ID == avatarCfg.id);
                }
            }
            else if (config is Config.AvatarFrame frameCfg)
            {
                ID = frameCfg.id;
                IsUnlock = DataController.TryGetAvatarFrame(ID, out var item);
                IsUse = DataController.UserInfo.AvatarFrame == ID;
                IsNew = item is { Value: true };

                redPoint.SetPath("playerInfo/avatar/frame/item" + ID);
                ItName = frameCfg.avatarframe_name;
                txtName.SetKey(ItName);
                AvatarComp.SetAvatarFrame(imgIcon, resGroup, ID, () => !_isAvatar  && ID == frameCfg.id);
            }

            imgUse.SetActive(IsUse);
            imgLock.SetActive(!IsUnlock);
            SetSelect(false);
        }

        public void SetSelect(bool isSelect)
        {
            imgSelect.SetActive(isSelect);
        }

    }
}