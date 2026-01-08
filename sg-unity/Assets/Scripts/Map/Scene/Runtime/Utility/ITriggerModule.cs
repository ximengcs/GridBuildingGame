
using MH.GameScene.Runtime.Entities;

namespace MH.GameScene.Runtime.Utilities
{
    public interface ITriggerModule
    {
        bool CheckMove { get; set; }
        void Register(IScenentersect subject);
        void Unregister(IScenentersect subject);
    }
}
