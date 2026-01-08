using Cysharp.Threading.Tasks;
using SgFramework.UI;

namespace UI
{
    [UIConfig(UILayer.Top, "Assets/GameRes/Prefabs/UI/UIBlockAllClick.prefab")]
    public class UIBlockAllClick : UIForm
    {
        private static int _ref;

        public static async UniTask Ref()
        {
            _ref++;
            if (_ref == 1)
            {
                await UIManager.Open<UIBlockAllClick>();
                if (_ref == 0)
                {
                    UIManager.Close<UIBlockAllClick>().Forget();
                }
            }
        }

        public static void UnRef()
        {
            _ref--;
            if (_ref == 0)
            {
                UIManager.Close<UIBlockAllClick>().Forget();
            }
        }
    }
}