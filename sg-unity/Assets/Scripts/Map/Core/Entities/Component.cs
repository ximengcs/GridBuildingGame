
using System.Threading;

namespace MH.GameScene.Core.Entites
{
    public class ComponentBase : IComponent
    {
        protected CancellationTokenSource _destroyTokenSource;

        public IEntity Entity { get; private set; }

        public virtual bool Active { get; set; }

        public virtual void OnInit(Entity entity, object data)
        {
            Entity = entity;
            Active = true;
            _destroyTokenSource = new CancellationTokenSource();
        }

        public virtual void OnStart()
        {
        }

        public virtual void OnDestroy()
        {
            _destroyTokenSource.Cancel();
            _destroyTokenSource.Dispose();
        }
    }
}
