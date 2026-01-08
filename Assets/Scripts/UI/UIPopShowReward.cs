using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Pt;
using SgFramework.Res;
using SgFramework.UI;
using SgFramework.Utility;
using UIComponent;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Pop, "Assets/GameRes/Prefabs/UI/UIPopShowReward.prefab")]
    public class UIPopShowReward : UIPop
    {
        [SerializeField] private Button btnClose;
        [SerializeField] private GameObject rewardItem;
        [SerializeField] private GameObject rewardRoot;

        private ResourceGroup _group;

        private void Start()
        {
            btnClose.BindClick(UIManager.Close<UIPopShowReward>);
        }

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);
            if (args[0] is not PushAddRewards rewards)
            {
                return;
            }

            _group = ResourceManager.GetGroup("Rewards").AddTo(this);
            rewardItem.SetActive(false);
            foreach (var it in rewards.Rewards)
            {
                var item = Instantiate(rewardItem, rewardRoot.transform, false);
                item.SetActive(true);
                item.GetComponent<UIBagItem>().SetRewardID(_group, it.Category, it.ConfId, it.Amount);
            }
        }

        private static readonly Queue<PushAddRewards> RewardsQueue = new Queue<PushAddRewards>();

        public static async UniTaskVoid ShowReward(PushAddRewards addRewards)
        {
            RewardsQueue.Enqueue(addRewards);
            if (RewardsQueue.Count > 1)
            {
                return;
            }

            while (RewardsQueue.TryPeek(out var rewards))
            {
                var ui = await UIManager.Open<UIPopShowReward>(rewards);
                await ui.destroyCancellationToken.WaitUntilCanceled();
                RewardsQueue.Dequeue();
            }
        }
    }
}