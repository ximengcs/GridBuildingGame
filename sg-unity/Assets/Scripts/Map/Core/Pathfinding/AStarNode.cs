
namespace MH.GameScene.Core.PathFinding
{
    internal class AStarNode
    {
        private int _hValue;
        private int _gValue;
        private int _originGValue;
        private object _item;

        public object Item => _item;

        public AStarNode Parent { get; set; }

        public int GValue
        {
            get => _gValue;
            set => _gValue = value;
        }

        public int OriginGValue
        {
            get => _originGValue;
            set => _originGValue = value;
        }

        public int FValue => _hValue + _gValue;

        public int HValue => _hValue;

        public void Init(object item, int hValue)
        {
            _item = item;
            _hValue = hValue;
            _gValue = int.MaxValue;
            _originGValue = int.MaxValue;
            Parent = null;
        }
    }
}
