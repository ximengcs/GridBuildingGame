using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Net;
using SgFramework.RedPoint;
using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Common
{
    public partial class DataController
    {
        private static readonly Subject<UserInfo> UserInfoUpdate = new Subject<UserInfo>();

        public static UserInfo UserInfo => Archive.User;

        
        private static void TriggerUserInfo()
        {
            UserInfoUpdate.OnNext(UserInfo);
        }

        public static IDisposable OnUserInfoText(TMP_Text text, Func<UserInfo, string> selector)
        {
            text.text = selector(UserInfo);
            return UserInfoUpdate.Subscribe((text, selector), static (x, state) => state.text.text = state.selector(x));
        }

        public static IDisposable OnUserInfo(Action<UserInfo> action)
        {
            action.Invoke(UserInfo);
            return UserInfoUpdate.Subscribe(action);
        }


        public static void SetLevel(PushUserLevelChange rsp)
        {
            UserInfo.Level = rsp.Level;
            UserInfo.Exp = rsp.Exp;

            TriggerUserInfo();
            Debug.Log($"等级变更：{UserInfo.Level}");
        }

        private static void SetExp(int exp)
        {
            UserInfo.Exp = exp;
            TriggerUserInfo();
        }

        public static int GetLevel()
        {
            return UserInfo.Level;
        }

        public static void RefreshAvatarRedPoint()
        {
            foreach (var it in Archive.Avatars)
            {
                RedPointManager.Instance.FindNode("playerInfo/avatar/avatar/item" + it.Key).SetValue(it.Value ? 1 : 0);
            }
            foreach (var it in Archive.AvatarFrames)
            {
                RedPointManager.Instance.FindNode("playerInfo/avatar/frame/item" + it.Key).SetValue(it.Value ? 1 : 0);
            }
        }

        public static bool TryGetAvatar(int configId, out Kvb item)
        {
            foreach (var it in Archive.Avatars)
            {
                if (it.Key != configId)
                {
                    continue;
                }
                item = it;
                return true;
            }
            item = null;
            return false;
        }

        public static bool TryGetAvatarFrame(int configId, out Kvb item)
        {
            foreach (var it in Archive.AvatarFrames)
            {
                if (it.Key == configId)
                {
                    item = it;
                    return true;
                }
            }
            item = null;
            return false;
        }

        public static async UniTask<bool> ModifyUserNameMsg(string name)
        {
            var rsp = await NetManager.Shared.Request(new ModifyUserNameMsg()
            {
                Name = name
            });
            if (rsp is not ModifyUserNameRsp { IsValid: true })
            {
                return false;
            }

            UserInfo.Name = name;
            TriggerUserInfo();

            return true;
        }

        public static async UniTask ModifyUserAvatarMsg(int newAvatar)
        {
            var rsp = await NetManager.Shared.Request(new ModifyUserAvatarMsg()
            {
                NewAvatar = newAvatar
            });
            if (rsp is Fail)
            {
                return;
            }

            UserInfo.Avatar = newAvatar;
            UserInfo.CustomAvatar = "";
            TriggerUserInfo();
        }

        public static async UniTask ModifyUserAvatarFrameMsg(int newAvatarFrame)
        {
            var rsp = await NetManager.Shared.Request(new ModifyUserAvatarFrameMsg()
            {
                NewAvatarFrame = newAvatarFrame
            });
            if (rsp is Fail)
            {
                return;
            }

            UserInfo.AvatarFrame = newAvatarFrame;
            TriggerUserInfo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="avatarId"></param>
        /// <param name="type">0:头像;1:头像框;</param>
        /// <returns></returns>
        public static async UniTask UserDelNewIdMsg(int avatarId, int type)
        {
            switch (type)
            {
                case 0 when Archive.Avatars.All(e => e.Key != avatarId):
                case 1 when Archive.AvatarFrames.All(e => e.Key != avatarId):
                    return;
            }

            var msg = new UserDelNewIdMsg
            {
                Id = avatarId
            };
            var rsp = await NetManager.Shared.Request(msg);
            if (rsp is Fail)
            {
                return;
            }

            switch (type)
            {
                case 0:
                    {
                         TryGetAvatar(avatarId, out var item);
                        if (item is { Value: true })
                        {
                            item.Value = false;
                        }
                        break;
                    }
                case 1:
                    {
                        TryGetAvatarFrame(avatarId, out var item);
                        if (item is { Value: true })
                        {
                            item.Value = false;
                        }
                        break;
                    }
            }

            RefreshAvatarRedPoint();
        }

    }
}