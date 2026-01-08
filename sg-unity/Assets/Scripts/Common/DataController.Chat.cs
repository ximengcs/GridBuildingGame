using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.Net;
using SgFramework.Utility;
using UnityEngine;

namespace Common
{
    public abstract class ChatChannel
    {
        public const string SystemChannel = "system_channel";
        public const string World = "world";
    }

    public partial class DataController
    {
        private static readonly Dictionary<string, List<ChatNotice>> ChatRecord =
            new Dictionary<string, List<ChatNotice>>();

        private static List<string> _roomList = new List<string>();

        public static List<ChatNotice> GetRoomHistory(string roomId)
        {
            if (ChatRecord.TryGetValue(roomId, out var list))
            {
                return list;
            }

            list = new List<ChatNotice>();
            ChatRecord.Add(roomId, list);
            return list;
        }

        public static List<string> GetChatRoomList()
        {
            if (_roomList.Count < ChatRecord.Count)
            {
                _roomList = ChatRecord.Keys.ToList();
            }

            return _roomList;
        }

        public static void SendChatMsg(string roomId, string content = "")
        {
            NetManager.Shared.Send(new SendChatMsg
            {
                Content = SgUtility.RemoveRichTextTags(content),
                RoomId = roomId
            });
        }

        public static void SendPrivateChatMsg(string playerId, string content = "")
        {
            NetManager.Shared.Send(new PrivateChatMsg
            {
                Content = SgUtility.RemoveRichTextTags(content),
                PlayerId = playerId
            });
        }

        public static void ReceivePrivateChatMsg(PushPrivateChat rsp)
        {
            if (!ChatRecord.TryGetValue(rsp.PlayerId, out var list))
            {
                list = new List<ChatNotice>();
                ChatRecord.Add(rsp.PlayerId, list);
            }

            list.Add(rsp.Notice);
            Debug.Log($"收到消息：{rsp.Notice.Content}");
        }

        public static void ReceiveChatMsg(ChatNotice rsp)
        {
            if (!ChatRecord.TryGetValue(rsp.RoomId, out var list))
            {
                list = new List<ChatNotice>();
                ChatRecord.Add(rsp.RoomId, list);
            }

            list.Add(rsp);
            Debug.Log($"收到消息：{rsp.Content}");
        }

        public static async UniTaskVoid GetChatHistory(string channel)
        {
            var msg = await NetManager.Shared.Request(new HistoryMsg
            {
                Index = 0,
                RoomId = channel,
                MessageNum = 30,
            });

            if (msg is not HistoryRsp rsp)
            {
                return;
            }

            var list = rsp.History.ToList();
            list.Sort((a, b) => a.CreatedAt < b.CreatedAt ? -1 : 1);
            foreach (var notice in list)
            {
                ReceiveChatMsg(notice);
            }
        }
    }
}