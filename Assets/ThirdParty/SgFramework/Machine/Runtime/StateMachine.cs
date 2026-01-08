using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace SgFramework.Machine
{
    public class StateMachine
    {
        private readonly Dictionary<string, object> _blackboard = new Dictionary<string, object>(100);
        private readonly Dictionary<string, IStateNode> _nodes = new Dictionary<string, IStateNode>(100);

        public Func<IStateNode, IStateNode, UniTask> OnStateChangeStart;
        public Func<IStateNode, IStateNode, UniTask> OnStateChangeEnd;

        private readonly Queue<IStateNode> _stateQueue = new Queue<IStateNode>();

        /// <summary>
        /// 状态机持有者
        /// </summary>
        public object Owner { private set; get; }

        /// <summary>
        /// 当前运行的节点名称
        /// </summary>
        public IStateNode CurrentNode { get; private set; }


        private StateMachine()
        {
        }

        public StateMachine(object owner)
        {
            Owner = owner;
        }

        /// <summary>
        /// 更新状态机
        /// </summary>
        public void Update()
        {
            CurrentNode?.OnUpdate();
        }

        /// <summary>
        /// 启动状态机
        /// </summary>
        public void Run<TNode>() where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;

            if (!TryGetNode(nodeName, out var node))
            {
                throw new Exception($"Not found entry node: {nodeName}");
            }

            CurrentNode = node;
            CurrentNode.OnEnter();
        }

        /// <summary>
        /// 加入一个节点
        /// </summary>
        public void AddNode<TNode>() where TNode : IStateNode, new()
        {
            var stateNode = new TNode();
            AddNode(stateNode);
        }

        public void AddNode(IStateNode stateNode)
        {
            if (stateNode == null)
            {
                throw new ArgumentNullException();
            }

            var nodeType = stateNode.GetType();
            var nodeName = nodeType.FullName;

            if (!_nodes.ContainsKey(nodeName!))
            {
                stateNode.OnCreate(this);
                _nodes.Add(nodeName, stateNode);
            }
            else
            {
                SgLogger.Error($"State node already existed : {nodeName}");
            }
        }

        public async UniTask ChangeState<TNode>() where TNode : IStateNode
        {
            var nodeType = typeof(TNode);
            var nodeName = nodeType.FullName;

            if (!TryGetNode(nodeName, out var node))
            {
                SgLogger.Error($"Can not found state node : {nodeName}");
                return;
            }

            _stateQueue.Enqueue(node);
            if (_stateQueue.Count > 1)
            {
                return;
            }

            while (_stateQueue.TryPeek(out node))
            {
                var cur = CurrentNode;
                SgLogger.Log($"{cur.GetType().FullName} --> {node.GetType().FullName}");
                if (OnStateChangeStart != null)
                {
                    await OnStateChangeStart.Invoke(cur, node);
                }

                await CurrentNode.OnExit();
                CurrentNode = node;
                await CurrentNode.OnEnter();
                if (OnStateChangeEnd != null)
                {
                    await OnStateChangeEnd.Invoke(cur, node);
                }

                _stateQueue.Dequeue();
            }

            CurrentNode.OnStart();
        }

        /// <summary>
        /// 设置黑板数据
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void SetBlackboardValue(string key, object value)
        {
            _blackboard[key] = value;
        }

        /// <summary>
        /// 获取黑板数据
        /// </summary>
        public object GetBlackboardValue(string key)
        {
            if (_blackboard.TryGetValue(key, out var value))
            {
                return value;
            }
            else
            {
                SgLogger.Warning($"Not found blackboard value : {key}");
                return null;
            }
        }

        private bool TryGetNode(string nodeName, out IStateNode result)
        {
            return _nodes.TryGetValue(nodeName, out result);
        }
    }
}