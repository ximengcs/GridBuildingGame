using SgFramework.Event;

public struct UserEventDefine
{
    /// <summary>
    /// 用户尝试再次初始化资源包
    /// </summary>
    public struct UserTryInitialize : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new UserTryInitialize();
            SgEvent.SendMessage(msg);
        }
    }

    /// <summary>
    /// 用户开始下载网络文件
    /// </summary>
    public struct UserBeginDownloadWebFiles : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new UserBeginDownloadWebFiles();
            SgEvent.SendMessage(msg);
        }
    }

    /// <summary>
    /// 用户尝试再次更新静态版本
    /// </summary>
    public struct UserTryUpdatePackageVersion : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new UserTryUpdatePackageVersion();
            SgEvent.SendMessage(msg);
        }
    }

    /// <summary>
    /// 用户尝试再次更新补丁清单
    /// </summary>
    public struct UserTryUpdatePatchManifest : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new UserTryUpdatePatchManifest();
            SgEvent.SendMessage(msg);
        }
    }

    /// <summary>
    /// 用户尝试再次下载网络文件
    /// </summary>
    public struct UserTryDownloadWebFiles : IEventMessage
    {
        public static void SendEventMessage()
        {
            var msg = new UserTryDownloadWebFiles();
            SgEvent.SendMessage(msg);
        }
    }
}