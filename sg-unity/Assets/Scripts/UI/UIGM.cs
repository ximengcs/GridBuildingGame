using Common;
using Cysharp.Threading.Tasks;
using MH.GameScene.Core.Entites;
using Pt;
using SgFramework.Net;
using SgFramework.UI;
using SgFramework.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UI.UIScenes;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.GM, "Assets/GameRes/Prefabs/UI/UIGM.prefab")]
    public class UIGM : UIForm
    {
        private class CmdOp
        {
            public string Name { get; }
            public Action Action { get; }

            public CmdOp(string name, Action action)
            {
                Name = name;
                Action = action;
            }
        }

        private readonly Dictionary<string, List<CmdOp>> _cmd = new Dictionary<string, List<CmdOp>>();

        [SerializeField] private GameObject inputBox;

        [SerializeField] private TextMeshProUGUI txtTitle;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Button btnConfirmInput;
        [SerializeField] private Button btnCancelInput;
        [SerializeField] private Button btnClose;

        [SerializeField] private Toggle tabTemplate;
        [SerializeField] private Button btnOpTemplate;

        [SerializeField] private RectTransform tabRoot;
        [SerializeField] private RectTransform btnRoot;

        private List<GameObject> _buttons = new List<GameObject>();

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIGM>);

            inputBox.SetActive(false);
            AddCmd("Common", new CmdOp("开启日志", () => { FindObjectOfType<Reporter>().enabled = true; }));
            AddCmd("Common", new CmdOp("关闭日志", () => { FindObjectOfType<Reporter>().enabled = false; }));
            AddCmd("Common", new CmdOp("发送邮件", () => { NetManager.Shared.Send(new GMMailMsg()); }));
            AddCmd("Common", new CmdOp("发送聊天", async () =>
            {
                var content = await Input("Content");
                DataController.SendChatMsg(ChatChannel.World, content);
            }));
            AddCmd("Common", new CmdOp("发送私聊", async () =>
            {
                var playerId = await Input("PlayerId");
                var content = await Input("Content");
                DataController.SendPrivateChatMsg(playerId, content);
            }));
            AddCmd("Common", new CmdOp("跑马灯", () =>
            {
                UIToast.Instance.ShowLamp(new PushLampMsg
                {
                    Content =
                    {
                        { 1, "江雪 千山鸟飞绝， 万径人踪灭。 孤舟蓑笠翁， 独钓寒江雪。" },
                        {
                            3,
                            "River SnowA thousand mountains, no birds in flight,Ten thousand paths, no footprints in sight.A lone boat, an old man in cloak and hat,Fishing alone in the cold river snow."
                        }
                    }
                }).Forget();
            }));

            AddCmd("玩家", new CmdOp("加经验", async () =>
            {
                var str = await Input();
                if (!int.TryParse(str, out var exp))
                {
                    UIToast.Instance.ShowToast("请输入有效的数字").Forget();
                    return;
                }

                NetManager.Shared.Send(new GMAddExpMsg
                {
                    Exp = exp,
                });
            }));
            AddCmd("玩家", new CmdOp("加物品", async () =>
            {
                var arr = (await Input()).Split(":");
                int Category, ConfId, Amount;
                if (arr.Length != 3 || !int.TryParse(arr[0], out Category) || !int.TryParse(arr[1], out ConfId) ||
                    !int.TryParse(arr[2], out Amount))
                {
                    UIToast.Instance.ShowToast("请输入【类型:物品id:数量】").Forget();
                    return;
                }

                NetManager.Shared.Send(new GMAddRewards
                {
                    Category = Category,
                    ConfId = ConfId,
                    Amount = Amount,
                });
            }));
            AddCmd("Other", new CmdOp("测试分页", () => { Debug.Log("other"); }));
            AddCmd("地图", new("打开", async () =>
            {
                var input = await Input("输入地图Id");
                if (int.TryParse(input, out var mapId))
                    await World.Create(mapId);
            }));
            AddCmd("地图", new("关闭", async () =>
            {
                var input = await Input("输入地图Id");
                if (int.TryParse(input, out var mapId))
                    World.Destory(mapId);
            }));
            AddCmd("地图", new("Test", TestAsync));

            tabTemplate.gameObject.SetActive(false);
            btnOpTemplate.gameObject.SetActive(false);

            var init = false;
            foreach (var key in _cmd.Keys.ToList())
            {
                var rkey = key;
                var t = Instantiate(tabTemplate, tabRoot);
                t.SetIsOnWithoutNotify(false);
                t.gameObject.SetActive(true);
                t.onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    ShowPage(rkey);
                });
                t.GetComponentInChildren<TextMeshProUGUI>().text = key;
                if (init)
                {
                    continue;
                }

                t.isOn = true;
                init = true;
            }
        }

        private CancellationTokenSource _tokenSource = new CancellationTokenSource();

        private async void TestAsync()
        {
            Debug.Log("Test1");
            _tokenSource.Cancel();

            bool cancel = await TestAsync2(_tokenSource.Token).SuppressCancellationThrow();
            if (cancel)
            {
                Debug.Log($"subtask is cancel");
            }
        }

        private async UniTask TestAsync2(CancellationToken token)
        {
            Debug.Log("Test2");
            await UniTask.Delay(1000);
            token.ThrowIfCancellationRequested();
            Debug.Log("Test22");
        }

        private void ShowPage(string key)
        {
            foreach (var go in _buttons)
            {
                Destroy(go);
            }

            _buttons.Clear();

            foreach (var op in _cmd[key])
            {
                var action = op.Action;
                var btn = Instantiate(btnOpTemplate, btnRoot);
                btn.gameObject.SetActive(true);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = op.Name;
                btn.onClick.AddListener(() => { action(); });
                _buttons.Add(btn.gameObject);
            }
        }

        private void AddCmd(string cate, CmdOp op)
        {
            if (!_cmd.TryGetValue(cate, out var list))
            {
                list = new List<CmdOp>();
                _cmd.Add(cate, list);
            }

            list.Add(op);
        }

        private async UniTask<string> Input(string title = "")
        {
            txtTitle.text = string.IsNullOrEmpty(title) ? "输入" : title;
            var tcs = new UniTaskCompletionSource();
            var cs = new CancellationTokenSource();
            inputField.text = "";
            btnConfirmInput.onClick.RemoveAllListeners();
            btnCancelInput.onClick.RemoveAllListeners();
            btnConfirmInput.onClick.AddListener(() => { tcs.TrySetResult(); });
            btnCancelInput.onClick.AddListener(() => { cs.Cancel(false); });
            inputBox.SetActive(true);
            await UniTask.WhenAny(tcs.Task, UniTask.WaitUntilCanceled(cs.Token));
            inputBox.SetActive(false);
            return !cs.IsCancellationRequested ? inputField.text : "";
        }
    }
}