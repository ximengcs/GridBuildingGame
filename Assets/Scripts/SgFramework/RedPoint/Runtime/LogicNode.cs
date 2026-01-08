using System;
using System.Collections.Generic;
using R3;

namespace SgFramework.RedPoint
{
    public class LogicNode
    {
        private readonly Dictionary<string, LogicNode> _nodes = new Dictionary<string, LogicNode>();

        private int _childTotalValue;

        private string _key;
        private LogicNode _parent;
        private int _value;

        private readonly Subject<LogicNode> _subject = new Subject<LogicNode>();

        public LogicNode(string key)
        {
            _key = key;
        }

        public LogicNode(LogicNode parent, string key)
        {
            _parent = parent;
            _key = key;
        }

        public bool Valid => Value > 0;
        public int Value => _value + _childTotalValue;

        public LogicNode FindNode(string key)
        {
            if (_nodes.TryGetValue(key, out var node))
            {
                return node;
            }

            node = new LogicNode(this, key);
            _nodes.Add(key, node);
            node.Subscribe(_ => { Calculate(); });
            return node;
        }

        public IDisposable Subscribe(Action<LogicNode> action)
        {
            action(this);
            return _subject.Subscribe(action);
        }

        public void SetValue(int val)
        {
            if (_value == val)
            {
                return;
            }

            _value = val;
            Calculate();
        }

        public void ResetValue()
        {
            SetValue(0);
        }

        private void Calculate()
        {
            _childTotalValue = 0;
            foreach (var node in _nodes.Values)
            {
                _childTotalValue += node.Value;
            }

            _subject.OnNext(this);
        }
    }
}