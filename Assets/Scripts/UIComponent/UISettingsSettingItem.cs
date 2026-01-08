using System;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UISettingsSettingItem : MonoBehaviour
    {
        [SerializeField] private Toggle toggleSwitch;
        [SerializeField] private Slider sliderValue;

        public event Action<float> OnValueChanged;
        public event Action<bool> OnSwitchChanged;

        private string _bindKey;

        public void BindKey(string key)
        {
            _bindKey = key;

            {
                toggleSwitch.SetIsOnWithoutNotify(DataController.GetSettingAsBool(_bindKey));
                toggleSwitch.onValueChanged.AddListener(OnToggleSwitched);
                sliderValue.interactable = toggleSwitch.isOn;
            }

            {
                sliderValue.SetValueWithoutNotify(DataController.GetSettingAsFloat($"{_bindKey}_value"));
                sliderValue.onValueChanged.AddListener(OnSliderValueChanged);
            }
        }

        private void OnToggleSwitched(bool value)
        {
            OnSwitchChanged?.Invoke(value);
            sliderValue.interactable = value;
            DataController.SetSetting(_bindKey, value);
        }

        private void OnSliderValueChanged(float value)
        {
            OnValueChanged?.Invoke(value);
            DataController.SetSetting($"{_bindKey}_value", value);
        }
    }
}