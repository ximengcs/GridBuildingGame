
namespace MH.GameScene.Core.Entites
{
    public interface IComponent
    {
        IEntity Entity { get; }

        bool Active { get; set; }

        void OnInit(Entity entity, object data);

        void OnStart();

        void OnDestroy();
    }
}
