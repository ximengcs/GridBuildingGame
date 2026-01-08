using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Net;
using SgFramework.RedPoint;
using UnityEngine;

namespace Common
{
    public partial class DataController
    {
        public static readonly Subject<int> FriendUpdate = new Subject<int>();

        private static readonly Dictionary<int, List<UserPublicInfo>> UserPublicInfoMap =
            new Dictionary<int, List<UserPublicInfo>>();

        public static List<UserPublicInfo> GetFriendList(int type)
        {
            if (type == 2)
            {
                RedPointManager.Instance.FindNode("friend/apply").ResetValue();
            }

            return UserPublicInfoMap.GetValueOrDefault(type);
        }

        public static async UniTask RefreshFriendList(int type)
        {
            var msg = await NetManager.Shared.Request(new GetFriendListMsg
            {
                Type = type
            });

            if (msg is not GetFriendListRsp rsp)
            {
                return;
            }

            UserPublicInfoMap[type] = rsp.List.ToList();
        }

        public static async UniTask<List<UserPublicInfo>> GetRecommendFriendList()
        {
            var msg = await NetManager.Shared.Request(new FriendRecommendMsg());
            return msg is not FriendRecommendRsp rsp ? null : rsp.RecommendList.ToList();
        }

        public static async UniTask RefreshFriendAll()
        {
            await RefreshFriendList(1);
            await RefreshFriendList(2);
            await RefreshFriendList(3);
        }

        public static async UniTask FriendRefuseApply(UserPublicInfo info)
        {
            if (!UserPublicInfoMap.TryGetValue(2, out var data))
            {
                return;
            }


            if (!data.Contains(info))
            {
                return;
            }

            var req = new FriendRefuseApplyMsg();
            req.PlayerIds.Add(info.PlayerId);

            var msg = await NetManager.Shared.Request(req);
            if (msg is not Ok)
            {
                return;
            }

            data.Remove(info);
            FriendUpdate.OnNext(2);
        }

        public static async UniTask FriendRefuseApplyAll()
        {
            if (!UserPublicInfoMap.TryGetValue(2, out var data))
            {
                return;
            }

            var req = new FriendRefuseApplyMsg();
            foreach (var info in data)
            {
                req.PlayerIds.Add(info.PlayerId);
            }

            var msg = await NetManager.Shared.Request(req);
            if (msg is not Ok)
            {
                return;
            }

            data.Clear();
            FriendUpdate.OnNext(2);
        }

        public static async UniTask FriendAgreeApply(UserPublicInfo info)
        {
            if (!UserPublicInfoMap.TryGetValue(2, out var data))
            {
                return;
            }

            if (!data.Contains(info))
            {
                return;
            }

            var req = new FriendAgreeApplyMsg();
            req.PlayerIds.Add(info.PlayerId);

            var msg = await NetManager.Shared.Request(req);
            if (msg is not FriendAgreeApplyRsp)
            {
                return;
            }

            if (!UserPublicInfoMap.TryGetValue(1, out var list))
            {
                list = new List<UserPublicInfo>();
                UserPublicInfoMap.Add(1, list);
            }

            list.Add(info);
            data.Remove(info);
            FriendUpdate.OnNext(2);
        }

        public static async UniTask FriendAgreeApplyAll()
        {
            if (!UserPublicInfoMap.TryGetValue(2, out var data))
            {
                return;
            }

            var req = new FriendAgreeApplyMsg();
            foreach (var info in data)
            {
                req.PlayerIds.Add(info.PlayerId);
            }

            var msg = await NetManager.Shared.Request(req);
            if (msg is not FriendAgreeApplyRsp rsp)
            {
                return;
            }

            if (!UserPublicInfoMap.TryGetValue(1, out var list))
            {
                list = new List<UserPublicInfo>();
                UserPublicInfoMap.Add(1, list);
            }

            foreach (var playerId in rsp.PlayerIds)
            {
                var info = data.Find(x => x.PlayerId == playerId);
                list.Add(info);
                data.Remove(info);
            }

            FriendUpdate.OnNext(2);
        }

        public static async UniTask<bool> FriendBlock(UserPublicInfo info, int type)
        {
            //取消黑名单
            if (type == 0)
            {
                if (!UserPublicInfoMap.TryGetValue(3, out var data))
                {
                    return false;
                }

                if (!data.Contains(info))
                {
                    return false;
                }

                var req = new FriendBlackOpMsg
                {
                    PlayerId = info.PlayerId,
                    Type = type
                };

                var msg = await NetManager.Shared.Request(req);
                if (msg is not Ok)
                {
                    return false;
                }

                data.Remove(info);
                FriendUpdate.OnNext(3);
            }
            //设置黑名单
            else
            {
                var req = new FriendBlackOpMsg
                {
                    PlayerId = info.PlayerId,
                    Type = type
                };

                var msg = await NetManager.Shared.Request(req);
                if (msg is not Ok)
                {
                    return false;
                }

                if (UserPublicInfoMap.TryGetValue(1, out var data))
                {
                    data.Remove(info);
                    FriendUpdate.OnNext(0);
                    Debug.Log("设置好友到黑名单，会删除好友");
                }

                if (!UserPublicInfoMap.TryGetValue(3, out data))
                {
                    data = new List<UserPublicInfo>();
                    UserPublicInfoMap.Add(3, data);
                }

                data.Add(info);
            }

            return true;
        }

        public static async UniTask<bool> FriendApply(UserPublicInfo info)
        {
            var req = new FriendApplyMsg
            {
                PlayerId = info.PlayerId
            };

            var msg = await NetManager.Shared.Request(req);
            return msg is Ok;
        }

        public static async UniTask FriendDel(UserPublicInfo info)
        {
            if (!UserPublicInfoMap.TryGetValue(1, out var data))
            {
                return;
            }

            if (!data.Contains(info))
            {
                return;
            }

            var req = new FriendDelMsg
            {
                PlayerId = info.PlayerId
            };

            var msg = await NetManager.Shared.Request(req);
            if (msg is not Ok)
            {
                return;
            }

            data.Remove(info);
            FriendUpdate.OnNext(0);
        }

        public static void PushFriendDel(PushFriendDel rsp)
        {
            if (!UserPublicInfoMap.TryGetValue(1, out var list))
            {
                return;
            }

            list.RemoveAll(info => info.PlayerId == rsp.PlayerId);
            FriendUpdate.OnNext(0);
            Debug.Log("好友删除");
        }

        public static void PushNewFriend(PushNewFriend rsp)
        {
            if (!UserPublicInfoMap.TryGetValue(1, out var list))
            {
                list = new List<UserPublicInfo>();
                UserPublicInfoMap.Add(1, list);
            }

            list.Add(rsp.PlayerInfo);
            FriendUpdate.OnNext(0);
            Debug.Log("你有一个新的好友");
        }

        public static void PushNewFriendApply(PushNewFriendApply rsp)
        {
            if (!UserPublicInfoMap.TryGetValue(2, out var list))
            {
                list = new List<UserPublicInfo>();
                UserPublicInfoMap.Add(2, list);
            }

            list.Add(rsp.PlayerInfo);
            FriendUpdate.OnNext(2);
            Debug.Log("新的好友请求");
            RedPointManager.Instance.FindNode("friend/apply").SetValue(1);
        }

        public static async UniTask<List<UserPublicInfo>> FriendSearch(string playerId)
        {
            var msg = await NetManager.Shared.Request(new FriendSearchMsg
            {
                PlayerId = playerId
            });

            return msg is not FriendSearchRsp rsp ? null : rsp.PlayerInfo.ToList();
        }
    }
}