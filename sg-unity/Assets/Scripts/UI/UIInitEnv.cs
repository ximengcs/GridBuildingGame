using SgFramework.UI;

namespace UI
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UIInitEnv.prefab")]
    public class UIInitEnv : UIForm
    {
        public override bool CanReuse => false;
    }
}