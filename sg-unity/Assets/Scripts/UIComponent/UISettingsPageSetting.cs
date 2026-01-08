using Common;
using SgFramework.Audio;
using SgFramework.Utility;
using State;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class UISettingsPageSetting : MonoBehaviour
    {
        [SerializeField] private UISettingsSettingItem bgmSetting;
        [SerializeField] private UISettingsSettingItem sfxSetting;

        [SerializeField] private Button btnQuit;
        [SerializeField] private Button btnChangeServer;

        private void Start()
        {
            bgmSetting.BindKey(SettingKey.BgmSetting);
            sfxSetting.BindKey(SettingKey.SfxSetting);

            bgmSetting.OnSwitchChanged += v => { AudioManager.Instance.BgmOn = v; };
            bgmSetting.OnValueChanged += v => { AudioManager.Instance.BgmVolume = v; };
            sfxSetting.OnSwitchChanged += v => { AudioManager.Instance.SfxOn = v; };
            sfxSetting.OnValueChanged += v => { AudioManager.Instance.SfxVolume = v; };

            btnQuit.BindClick(GameMain.Instance.StateMachine.ChangeState<Restart>);
        }
    }
}