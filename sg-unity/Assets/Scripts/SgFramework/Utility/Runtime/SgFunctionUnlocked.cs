using Common;
using Config;

namespace SgFramework.Utility
{
    public abstract class SgFunctionUnlocked
    {
        public static bool AddFriend => DataController.GetLevel() >= Table.Global.AddLevel;
    }
}