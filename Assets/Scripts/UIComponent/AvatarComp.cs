using Common;
using Config;
using Cysharp.Threading.Tasks;
using Pt;
using R3;
using SgFramework.Res;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace UIComponent
{
    public class AvatarComp : ResourceToken
    {

        [SerializeField] private Image avatar;
        [SerializeField] private Image frame;
        [SerializeField] private bool changeListener;

        private int _frameId;
        private int _avatarId;
        private string _customAvatar;
        private SpriteDownloadContext _avatarDownload;
        private SpriteDownloadContext _frameDownload;
        private bool _isInit;
        private ResourceGroup _spriteGroup;
        private IDisposable _archiveDispose;

        private const string AvatarDir = "Assets/GameRes/Sprite/Avatar/";
        private const string FrameDir = "Assets/GameRes/Sprite/AvatarFrame/";


        private void Start()
        {
            if (!changeListener)
            {
                return;
            }

            _archiveDispose?.Dispose();
            _archiveDispose = DataController.OnUserInfo(OnSubscribeArchive).AddTo(this);

        }

        private void Init()
        {
            if (_isInit)
            {
                return;
            }

            _spriteGroup = ResourceManager.GetGroup("Avatar").AddTo(this);
            _isInit = true;
        }

        /// <summary>
        /// 设置头像
        /// </summary>
        /// <param name="frameId">系统头像框</param>
        /// <param name="sysAvatarId">系统头像</param>
        /// <param name="customAvatar">自定义头像</param>
        /// <param name="isListenerChange">是否监听自己头像变化</param>
        /// <returns></returns>
        public void SetData(int frameId, int sysAvatarId, string customAvatar = "", bool isListenerChange = false)
        {
            if (_avatarId != sysAvatarId || _customAvatar != customAvatar)
            {
                OnUserAvatarChange(sysAvatarId, customAvatar);
            }
            if (_frameId != frameId)
            {
                OnUserAvatarFrameChange(frameId);
            }
            if (_archiveDispose != null)
            {
                _archiveDispose.Dispose();
                _archiveDispose = null;
            }
            if (isListenerChange)
            {
                _archiveDispose = DataController.OnUserInfo(OnSubscribeArchive).AddTo(this);
            }
        }

        public static async void SetAvatar(Image avatar, ResourceGroup group, int avatarId, Func<bool> check = null)
        {
            try
            {
                var config = Table.AvatarTable.GetById(avatarId);
                await SetImage(avatar, group, config.dynamics_picture, AvatarDir + config.picture + ".png", check);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        public static async void SetAvatarFrame(Image frame, ResourceGroup group, int frameId, Func<bool> check = null)
        {
            try
            {
                var config = Table.AvatarFrameTable.GetById(frameId);
                await SetImage(frame, group, config.dynamics_picture, FrameDir + config.picture + ".png", check);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 设置头像图片资源或者动画
        /// </summary>
        /// <param name="image">被设置的图片</param>
        /// <param name="group">资源组</param>
        /// <param name="anim">动画spine</param>
        /// <param name="icon">静态图片</param>
        /// <param name="check">检查操作是否合法</param>
        /// <returns></returns>
        private static async UniTask SetImage(Image image, ResourceGroup group, string anim, string icon, Func<bool> check = null)
        {
            if (check != null && !check.Invoke())
            {
                return;
            }
            if (string.IsNullOrEmpty(anim))
            {
                image.GetComponent<Image>().enabled = true;
                var spine = image.transform.Find("spine");
                if (spine)
                {
                    spine.gameObject.SetActive(false);
                }

                var sprite = await group.GetSprite(icon);
                if (check != null && !check.Invoke())
                {
                    return;
                }

                image.sprite = sprite;
            }
            else
            {
                //var path = "Assets/GameRes/Spine/" + anim;
                //var skeletonDataAsset = await group.LoadAssetAsync<SkeletonDataAsset>(path);
                //if (check != null && !check.Invoke())
                //{
                //    return;
                //}

                //image.GetComponent<Image>().enabled = false;
                //var spine = image.transform.Find("spine");
                //if (!spine)
                //{
                //    var spineObj = new GameObject("spine");
                //    var go = Instantiate(spineObj, image.transform, false);
                //    go.transform.parent = image.transform;
                //    //go.AddComponent<SkeletonAnimation>();
                //}
                //// ftest5: 创建播放spine-idle
                ////var skeletonAnimation = spine.GetComponent<SkeletonAnimation>();
                //skeletonAnimation.skeletonDataAsset = skeletonDataAsset;
                //skeletonAnimation.Initialize(true);
                //skeletonAnimation.state.SetAnimation(0, "idle", true);
            }
        }

        /// <summary>
        /// 监听头像改变
        /// </summary>
        /// <param name="sysAvatarId"></param>
        /// <param name="customAvatar"></param>
        private async void OnUserAvatarChange(int sysAvatarId, string customAvatar)
        {
            try
            {
                _avatarId = sysAvatarId;
                _customAvatar = customAvatar;
                if (!string.IsNullOrEmpty(customAvatar))
                {
                    _avatarDownload?.Cancel();
                    _avatarDownload = new SpriteDownloadContext(customAvatar);
                    await _avatarDownload.WaitForCompletion();
                    if (_customAvatar == customAvatar)
                    {
                        avatar.sprite = _avatarDownload.Sprite;
                    }
                }
                else
                {
                    var config = Table.AvatarTable.GetById(_avatarId);
                    Init();
                    await SetImage(avatar, _spriteGroup, config.dynamics_picture, AvatarDir + config.picture + ".png", () => sysAvatarId == _avatarId);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        /// <summary>
        /// 监听头像框改变
        /// </summary>
        /// <param name="frameId"></param>
        private async void OnUserAvatarFrameChange(int frameId)
        {
            try
            {
                _frameId = frameId;
                var config = Table.AvatarFrameTable.GetById(frameId);
                Init();
                await SetImage(frame, _spriteGroup, config.dynamics_picture, 
                    FrameDir + config.picture + ".png", () => frameId == this._frameId);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        }

        private void OnSubscribeArchive(UserInfo rsp)
        {
            OnUserAvatarChange(rsp.Avatar, rsp.CustomAvatar);
            OnUserAvatarFrameChange(rsp.AvatarFrame);
        }

    }
}