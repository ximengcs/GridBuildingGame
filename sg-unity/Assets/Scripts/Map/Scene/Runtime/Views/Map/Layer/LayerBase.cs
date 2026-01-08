
namespace MH.GameScene.Runtime.Views
{
    public abstract class LayerBase
    {
        public virtual string SortingLayer { get; }

        public virtual int SortingOrder { get; }

        public virtual string Name { get; }

        public abstract void OnDestroy();
    }
}
