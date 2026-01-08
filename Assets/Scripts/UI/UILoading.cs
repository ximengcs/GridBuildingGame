using Cysharp.Threading.Tasks;
using DG.Tweening;
using SgFramework.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    [UIConfig(UILayer.Loading, "Assets/GameRes/Prefabs/UI/UILoading.prefab")]
    public class UILoading : UIForm
    {
        [SerializeField] private Slider sliderProgress;
        [SerializeField] private TextMeshProUGUI txtProgress;

        public static UILoading Instance { get; private set; }

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            Instance = null;
        }

        public override async UniTask ShowClose()
        {
            await DOVirtual.Float(0f, 1f, 1f, p =>
            {
                sliderProgress.value = p;
                txtProgress.text = $"{p:0%}";
            }).AsyncWaitForCompletion();
        }
    }
}