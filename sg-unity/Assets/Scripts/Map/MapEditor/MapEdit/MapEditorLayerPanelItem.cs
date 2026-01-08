#if UNITY_EDITOR
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MH.GameScene.UIs.MapEdit
{
    public class MapEditorLayerPanelItem : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI title;

        [SerializeField]
        private Button selectBtn;

        [SerializeField]
        private Button eyeBtn;

        [SerializeField]
        private Image state1;

        [SerializeField]
        private Image state2;

        private string _layer;
        private IEditorLayerHelper _helper;

        public void Init(string layer, IEditorLayerHelper helper)
        {
            _layer = layer;
            title.text = layer;
            _helper = helper;
            eyeBtn.onClick.AddListener(InnerClickEyeHandler);
            selectBtn.onClick.AddListener(InnerSelectHandler);
            RefreshShowState();
            RefreshSelectState();
        }

        private void InnerSelectHandler()
        {
            bool currentState = _helper.LayerSelectState[_layer];
            currentState = !currentState;
            _helper.OnSelectStateChange(_layer, currentState);
            RefreshSelectState();
        }

        private void InnerClickEyeHandler()
        {
            bool currentState = _helper.LayerShowState[_layer];
            currentState = !currentState;
            _helper.OnShowStateChange(_layer, currentState);
            RefreshShowState();
        }

        private void RefreshShowState()
        {
            bool currentState = _helper.LayerShowState[_layer];
            if (currentState)
            {
                state1.enabled = true;
                state2.enabled = false;
            }
            else
            {
                state1.enabled = false;
                state2.enabled = true;
            }
        }

        private void RefreshSelectState()
        {
            bool currentState = _helper.LayerSelectState[_layer];
            if (currentState)
            {

            }
            else
            {

            }
        }

        public void OnClear()
        {
            gameObject.SetActive(false);
            eyeBtn.onClick.RemoveAllListeners();
            selectBtn.onClick.RemoveAllListeners();
        }
    }
}
#endif