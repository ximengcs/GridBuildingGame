
namespace MH.GameScene.Core.Entites
{
    internal class IDGenerator
    {
        private int _curId;

        public IDGenerator(int startId)
        {
            _curId = startId;
        }

        public int Next
        {
            get { return _curId++; }
        }

        public void Dispose()
        {

        }
    }
}
