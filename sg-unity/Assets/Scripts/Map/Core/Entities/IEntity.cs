
namespace MH.GameScene.Core.Entites
{
    public interface IEntity : IUpdate
    {
        int Id { get; }

        IEntity Parent { get; }

        World World {  get; }

        bool Active { get; set; }

        T GetCom<T>() where T : IComponent;

        T AddCom<T>(object data = null) where T : IComponent, new();

        void RemoveCom<T>() where T : IComponent;

        T FindEntity<T>() where T : IEntity;

        T FindCom<T>() where T : IComponent;

        void Init(World world, IEntity parent, int id, object data);

        void Start();

        void Destroy();
    }
}
