using Common;

namespace SgFramework.RedPoint
{
    public static class RedPointStaticCheckFunction
    {
        public static void CheckEmail()
        {
            DataController.RefreshMailRedPoint();
        }

        public static void CheckAvatar() 
        {
            DataController.RefreshAvatarRedPoint();
        }
        public static void CheckTask()
        {
            DataController.RefreshTaskRedPoint();
        }
    }
}