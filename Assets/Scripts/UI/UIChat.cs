using System;
using System.Collections.Generic;
using Common;
using Pt;
using R3;
using SgFramework.UI;
using SgFramework.Utility;
using SuperScrollView;
using TMPro;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UIChat.prefab")]
    public class UIChat : UIForm
    {
        [SerializeField] private LoopListView2 loopList;
        [SerializeField] private TMP_InputField inputContent;
        [SerializeField] private Button btnSend;

        [SerializeField] private Toggle tabTemplate;
        [SerializeField] private RectTransform tabRoot;

        private List<ChatNotice> _chatNotices;
        private int Count => _chatNotices?.Count ?? 0;
        private string _roomId;

        private int _oldCount = -1;

        private struct ChannelConfig
        {
            public string Name { get; set; }
            public bool ReadOnly { get; set; }
        }

        private readonly Dictionary<string, ChannelConfig> _tabList = new Dictionary<string, ChannelConfig>
        {
            {
                ChatChannel.SystemChannel, new ChannelConfig
                {
                    Name = "系统",
                    ReadOnly = true
                }
            },
            {
                ChatChannel.World, new ChannelConfig
                {
                    Name = "世界"
                }
            },
        };

        private void Start()
        {
            DataController.GetChatHistory(ChatChannel.SystemChannel).Forget();
            DataController.GetChatHistory(ChatChannel.World).Forget();

            tabTemplate.gameObject.SetActive(false);
            loopList.InitListView(0, OnGetItemByIndex);

            Observable.Interval(TimeSpan.FromSeconds(1f)).Subscribe(_ =>
                {
                    CheckRoomList();
                    if (_oldCount == Count)
                    {
                        return;
                    }

                    _oldCount = Count;
                    loopList.SetListItemCount(Count, false);
                    loopList.MovePanelToItemIndex(Count, 0f);
                })
                .AddTo(this);

            btnSend.BindClick(() =>
            {
                if (_roomId == ChatChannel.World)
                {
                    DataController.SendChatMsg(_roomId, inputContent.text);
                }
                else
                {
                    DataController.SendPrivateChatMsg(_roomId, inputContent.text);
                }

                inputContent.text = "";
            });

            foreach (var (key, conf) in _tabList)
            {
                var logicKey = key;
                var t = Instantiate(tabTemplate, tabRoot);
                t.SetIsOnWithoutNotify(false);
                t.gameObject.SetActive(true);
                t.GetComponentInChildren<TextMeshProUGUI>().text = conf.Name;
                t.onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    SwitchRoom(logicKey);
                });

                if (key != ChatChannel.SystemChannel)
                {
                    continue;
                }

                t.isOn = true;
            }
        }

        private LoopListViewItem2 OnGetItemByIndex(LoopListView2 list, int index)
        {
            if (index < 0 || index > Count)
            {
                return null;
            }

            var item = list.NewListViewItem("MsgItem");
            if (item.TryGetComponent(out UIChatMsgItem msg))
            {
                msg.SetData(_chatNotices[index]);
            }

            return item;
        }

        private void CheckRoomList()
        {
            foreach (var key in DataController.GetChatRoomList())
            {
                if (!_tabList.TryAdd(key, new ChannelConfig
                    {
                        Name = key
                    }))
                {
                    continue;
                }

                var logicKey = key;
                var t = Instantiate(tabTemplate, tabRoot);
                t.SetIsOnWithoutNotify(false);
                t.gameObject.SetActive(true);
                t.GetComponentInChildren<TextMeshProUGUI>().text = key;
                t.onValueChanged.AddListener(v =>
                {
                    if (!v)
                    {
                        return;
                    }

                    SwitchRoom(logicKey);
                });
            }
        }

        private void SwitchRoom(string roomId)
        {
            _roomId = roomId;
            _chatNotices = DataController.GetRoomHistory(roomId);
            loopList.SetListItemCount(Count);
            loopList.MovePanelToItemIndex(Count, 0f);
            _oldCount = Count;
        }
    }
}