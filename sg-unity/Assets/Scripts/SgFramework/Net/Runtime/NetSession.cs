using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Best.HTTP;
using Best.HTTP.Request.Upload;
using Common;
using Config;
using Cysharp.Threading.Tasks;
using Google.Protobuf;
using Newtonsoft.Json;
using Pt;
using SgFramework.RedPoint;
using SgFramework.Utility;
using UI;
using UnityEngine;

namespace SgFramework.Net
{
    public class NetSession : IDisposable
    {
        private readonly Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly string _host;
        private readonly string _account;
        private readonly string _uuid;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private readonly Dictionary<int, NetPackage> _ackPackages =
            new Dictionary<int, NetPackage>();

        private readonly Dictionary<string, List<Action<IMessage>>> _msgListener =
            new Dictionary<string, List<Action<IMessage>>>();

        public bool IsSessionReady => DataController.ArchiveReady;


        private int _messageCounter;

        /// <summary>
        /// 消息号
        /// </summary>
        private int MessageCounter
        {
            get
            {
                _messageCounter++;
                return _messageCounter;
            }
        }

        private long _timeOffset;

        /// <summary>
        /// 当前时间，由服务器时间为准
        /// </summary>
        public long TimeNow => DateTimeOffset.UtcNow.ToUnixTimeSeconds() + _timeOffset;

        public event Action<Exception> Error;
        public event Action<HttpApi.MaintainInfo> Notice;

        public NetSession(string host, string account, string uuid)
        {
            _host = host;
            _account = account;
            _uuid = uuid;

            AddListener<PushNewNavNotice>(OnPushNewNavNotice);

            AddListener<PushFriendDel>(OnPushFriendDel);
            AddListener<PushNewFriend>(OnPushNewFriend);
            AddListener<PushNewFriendApply>(OnPushNewFriendApply);

            AddListener<PushAddRewards>(OnPushAddRewards);
            AddListener<PushTaskInfo>(OnPushTaskInfo);

            AddListener<PushLampMsg>(OnPushLampMsg);

            AddListener<PushUserLevelChange>(OnPushUserLevelChange);

            AddListener<ChatNotice>(OnChatNotice);
            AddListener<PushPrivateChat>(OnPushPrivateChat);

            AddListener<PushCurrencyAmountInfos>(OnPushCurrencyAmountInfos);
            AddListener<PushItemInfos>(OnPushItemInfos);

            AddListener<PushAddMail>(OnPushAddMail);
            AddListener<PushUpdateMail>(OnPushUpdateMail);

            AddListener<HeartbeatRsp>(OnHeartbeatRsp);
            AddListener<Fail>(OnFail);
        }

        private void OnPushNewNavNotice(IMessage obj)
        {
            if (obj is not PushNewNavNotice)
            {
                return;
            }

            UIPopNoticeList.RequestNotice(true);
        }

        private void OnPushFriendDel(IMessage obj)
        {
            if (obj is not PushFriendDel rsp)
            {
                return;
            }

            DataController.PushFriendDel(rsp);
        }

        private void OnPushNewFriend(IMessage obj)
        {
            if (obj is not PushNewFriend rsp)
            {
                return;
            }

            DataController.PushNewFriend(rsp);
        }

        private void OnPushNewFriendApply(IMessage obj)
        {
            if (obj is not PushNewFriendApply rsp)
            {
                return;
            }

            DataController.PushNewFriendApply(rsp);
        }

        private void OnPushAddRewards(IMessage obj)
        {
            if (obj is not PushAddRewards rsp)
            {
                return;
            }

            foreach (var rspReward in rsp.Rewards)
            {
                if (rspReward.Category == Table.Global.RewardTypeItem)
                {
                    DataController.SetItemRed(rspReward.ConfId, true);
                }
            }

            UIPopShowReward.ShowReward(rsp).Forget();
        }

        private static void OnPushTaskInfo(IMessage obj)
        {
            if (obj is not PushTaskInfo rsp)
            {
                return;
            }

            DataController.UpdateTask(rsp);
        }

        private static void OnPushLampMsg(IMessage obj)
        {
            if (obj is not PushLampMsg rsp)
            {
                return;
            }

            UIToast.Instance.ShowLamp(rsp).Forget();
        }

        private static void OnPushPrivateChat(IMessage obj)
        {
            if (obj is not PushPrivateChat rsp)
            {
                return;
            }

            DataController.ReceivePrivateChatMsg(rsp);
        }

        private static void OnPushUserLevelChange(IMessage obj)
        {
            if (obj is not PushUserLevelChange rsp)
            {
                return;
            }

            DataController.SetLevel(rsp);
        }

        private static void OnChatNotice(IMessage obj)
        {
            if (obj is not ChatNotice rsp)
            {
                return;
            }

            DataController.ReceiveChatMsg(rsp);
        }

        private static void OnPushAddMail(IMessage obj)
        {
            if (obj is not PushAddMail rsp)
            {
                return;
            }

            DataController.AddNewMail(rsp.Mail);
        }

        private static void OnPushUpdateMail(IMessage obj)
        {
            if (obj is not PushUpdateMail rsp)
            {
                return;
            }

            DataController.UpdateMail(rsp);
        }

        private static void OnPushItemInfos(IMessage obj)
        {
            if (obj is not PushItemInfos rsp)
            {
                return;
            }

            foreach (var item in rsp.Items)
            {
                DataController.SetItem(item);
            }

            Debug.Log("物品变更");
        }

        private static void OnPushCurrencyAmountInfos(IMessage obj)
        {
            if (obj is not PushCurrencyAmountInfos rsp)
            {
                return;
            }

            foreach (var currency in rsp.Info)
            {
                DataController.SetCurrency(currency);
            }

            Debug.Log("货币变更");
        }

        private void OnHeartbeatRsp(IMessage obj)
        {
            if (obj is not HeartbeatRsp rsp)
            {
                return;
            }

            _timeOffset = rsp.Now - DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }

        private static void OnFail(IMessage obj)
        {
            if (obj is not Fail rsp)
            {
                return;
            }

            ErrorCenter.OnError(rsp.Fail_);
        }

        public void AddListener<T>(Action<IMessage> action)
        {
            var key = typeof(T).Name;
            if (!_msgListener.TryGetValue(key, out var list))
            {
                list = new List<Action<IMessage>>();
                _msgListener.Add(key, list);
            }

            list.Add(action);
        }

        public void RemoveListener<T>(Action<IMessage> action)
        {
            var key = typeof(T).Name;
            if (!_msgListener.TryGetValue(key, out var list))
            {
                return;
            }

            list.Remove(action);
        }

        public async UniTask<ESessionCode> GuestLogin()
        {
            var resp = await LoginWorld(_host, _account, _uuid);
            if (resp == null)
            {
                OnError(new Exception($"login world error. resp == null"));
                return ESessionCode.Failure;
            }

            if (!resp.Status)
            {
                OnError(new Exception($"login world error. {resp.ErrorCode}"));
                switch (resp.ErrorCode)
                {
                    case "error_maintain":
                    {
                        var info = JsonConvert.DeserializeObject<HttpApi.MaintainInfo>(resp.ErrorValue);
                        Notice?.Invoke(info);
                        return ESessionCode.Maintain;
                    }
                }

                return ESessionCode.Failure;
            }

            var ret = await LoginGame(resp);
            return ret ? ESessionCode.Success : ESessionCode.Failure;
        }

        private async UniTask<HttpApi.LoginSgResp> LoginWorld(string host, string account, string uuid)
        {
            try
            {
                Debug.Log($"login world {host}");
                const string path = "/sg/loginsgguest";
                var request = HTTPRequest.CreatePost($"{host}{path}");
                request.UploadSettings.UploadStream = new JSonDataStream<HttpApi.LoginSgGuest>(new HttpApi.LoginSgGuest
                {
                    ServerID = uuid,
                    Uid = account,
                    LangCode = SgUtility.GetLanguage()
                });
                request.TimeoutSettings.Timeout = TimeSpan.FromSeconds(5f);

                var resp = await request.GetHTTPResponseAsync(_cancellationTokenSource.Token);
                if (resp.IsSuccess)
                {
                    return JsonConvert.DeserializeObject<HttpApi.LoginSgResp>(resp.DataAsText);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }

            return null;
        }

        private async UniTask<bool> LoginGame(HttpApi.LoginSgResp resp)
        {
            try
            {
                await _socket.ConnectAsync(resp.Host, int.Parse(resp.Port));
                Receive();
                Debug.Log($"session auth");

                var authRsp = await Request(new SessionAuthMsg
                {
                    AccountId = resp.RoleId,
                    Token = resp.SessionToken
                });

                if (authRsp is not SessionAuthRsp { Success: true })
                {
                    Debug.Log($"session auth fail");
                    return false;
                }

                Debug.Log($"login game");
                //LoginMsg
                var loginRsp = await Request(new LoginMsg
                {
                    AccountId = resp.AccountId,
                    LoginMark = resp.LoginMark,
                    RoleId = resp.RoleId,
                    Language = SgUtility.GetLanguage()
                });

                if (loginRsp is not LoginRsp rsp)
                {
                    Debug.Log($"login game fail");
                    return false;
                }

                DataController.Initialize(rsp);
                Debug.Log("连接已就绪");
                AutoHeartBeat();
            }
            catch (Exception e)
            {
                OnError(e);
                return false;
            }

            return true;
        }

        private async void AutoHeartBeat()
        {
            try
            {
                var msg = new HeartbeatMsg();
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    await UniTask.WaitForSeconds(10f, cancellationToken: _cancellationTokenSource.Token);
                    Send(msg);
                }
            }
            catch (Exception)
            {
                //不处理
            }
        }

        public async UniTask<IMessage> Request(IMessage msg)
        {
            if (!NetUtility.GetName(msg, out var msgName))
            {
                throw new NotSupportedException($"无法解析协议名称{msg}");
            }

            var package = new NetPackage
            {
                MsgId = MessageCounter,
                MsgName = msgName,
                Msg = msg,
                CompletionSource = new UniTaskCompletionSource<IMessage>()
            };

            Debug.Log($"请求消息[{package.MsgId}]{Encoding.UTF8.GetString(msgName)}");
            _ackPackages.Add(package.MsgId, package);
            Send(package);
            var (ok, result) =
                await UniTask.WhenAny(package.Task, UniTask.WaitUntilCanceled(_cancellationTokenSource.Token));

            return ok ? result : null;
        }

        public void Send(IMessage msg)
        {
            if (!NetUtility.GetName(msg, out var data))
            {
                Debug.LogError($"无法解析协议名称{msg}");
                return;
            }

            Send(MessageCounter, data, msg);
        }

        private void Send(NetPackage package)
        {
            Send(package.MsgId, package.MsgName, package.Msg);
        }

        private async void Send(int msgId, byte[] msgName, IMessage msgBody)
        {
            var header = 10 + msgName.Length;
            var bodyLen = msgBody.CalculateSize();
            var totalLength = header + bodyLen;
            var body = ArrayPool<byte>.Shared.Rent(totalLength);
            try
            {
                BinaryPrimitives.WriteInt32BigEndian(body, totalLength - 4);
                BinaryPrimitives.WriteInt32BigEndian(body.AsSpan(4), msgId);
                BinaryPrimitives.WriteInt16BigEndian(body.AsSpan(8), (short)msgName.Length);
                Buffer.BlockCopy(msgName, 0, body, 10, msgName.Length);
                msgBody.WriteTo(body.AsSpan(header, bodyLen));
                await _socket.SendAsync(body.AsMemory(0, totalLength), SocketFlags.None,
                    _cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                OnError(e);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(body);
            }
        }

        private async void Receive()
        {
            var body = ArrayPool<byte>.Shared.Rent(65536);
            try
            {
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    var len = await _socket.ReceiveAsync(body.AsMemory(0, 4), SocketFlags.None,
                        _cancellationTokenSource.Token);
                    if (len == 0)
                    {
                        throw new Exception("remote close");
                    }

                    var length = BinaryPrimitives.ReadInt32BigEndian(body);
                    if (length < 4 || length > body.Length)
                    {
                        throw new InvalidOperationException("Invalid message length.");
                    }

                    var index = 0;
                    while (index < length)
                    {
                        len = await _socket.ReceiveAsync(body.AsMemory(4, length), SocketFlags.None,
                            _cancellationTokenSource.Token);
                        if (len == 0)
                        {
                            throw new Exception("remote close");
                        }

                        index += len;
                    }

                    await UniTask.SwitchToMainThread();
                    OnReceivePackage(body);
                }
            }
            catch (OperationCanceledException)
            {
                //主动取消的
                Debug.Log("socket close");
            }
            catch (Exception e)
            {
                OnError(e);
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(body);
            }
        }

        private void OnReceivePackage(byte[] bytes)
        {
            try
            {
                var reader = new BinaryReader(bytes);

                var length = reader.ReadInt32() + 4; //包体长度没包含自己
                reader.Length = length;
                //是否gzip压缩
                var isGzip = reader.ReadBoolean();
                if (isGzip)
                {
                    //解压缩
                    using var memoryStream = new MemoryStream(bytes);
                    memoryStream.Position = 5;
                    memoryStream.SetLength(length - 5);
                    using var gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress);
                    using var outputStream = new MemoryStream(bytes);
                    outputStream.Position = length;
                    gzipStream.CopyTo(outputStream);
                    reader.Position = length;
                    reader.Length = (int)outputStream.Position;
                }

                reader.ReadInt32(); //server msg Id
                while (!reader.Eof)
                {
                    var msgLength = reader.ReadInt32();
                    var msgId = reader.ReadInt32();
                    var msgNameCount = reader.ReadInt16();
                    var msgName = reader.ReadString(msgNameCount);
                    var bodyLen = msgLength - 6 - msgNameCount;
                    if (!NetUtility.GetMessage(msgName, out var msg))
                    {
                        Debug.LogError($"不支持这个协议：{msgName}");
                        continue;
                    }

                    var stream = new CodedInputStream(bytes, reader.Position, bodyLen);
                    msg.MergeFrom(stream);
                    reader.Position += bodyLen;

                    Debug.Log($"收到消息[{msgId}]{msgName}");

                    var used = false;
                    //with ack
                    if (_ackPackages.Remove(msgId, out var package))
                    {
                        package.CompletionSource.TrySetResult(msg);
                        used = true;
                    }

                    //with listener
                    if (!_msgListener.TryGetValue(msgName, out var list))
                    {
                        if (!used && msgName != "Ok")
                        {
                            Debug.LogWarning($"收到消息但没有处理{msgName}");
                        }

                        continue;
                    }

                    foreach (var action in list)
                    {
                        action?.Invoke(msg);
                    }
                }

                if (!reader.ReadClean)
                {
                    throw new Exception("收到的数据没有全部解析完毕。");
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel(false);
            _socket?.Close();
            DataController.Dispose();
            Error = null;
        }

        private void OnError(Exception e)
        {
            Debug.LogError(e);
            Error?.Invoke(e);
            Dispose();
        }
    }
}