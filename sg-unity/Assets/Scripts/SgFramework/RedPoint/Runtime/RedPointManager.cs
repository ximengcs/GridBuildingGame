using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using SgFramework.Utility;

namespace SgFramework.RedPoint
{
    public class RedPointManager
    {
        private LogicNode _root;
        private readonly HashSet<Action> _staticCheckFunc = new HashSet<Action>();
        public static RedPointManager Instance { get; } = new RedPointManager();

        public void Initialize()
        {
            _root = new LogicNode("root");

            AddStaticCheckFunc(RedPointStaticCheckFunction.CheckEmail);
            AddStaticCheckFunc(RedPointStaticCheckFunction.CheckAvatar);
            AddStaticCheckFunc(RedPointStaticCheckFunction.CheckTask);
            
            CheckingChain.Instance.Register(RefreshRedPoint, -1000);
        }

        public void Dispose()
        {
            _root.ResetValue();
            _staticCheckFunc.Clear();
        }

#pragma warning disable CS1998 // 异步方法缺少 "await" 运算符，将以同步方式运行
        private async UniTask<bool> RefreshRedPoint()
        {
            foreach (var action in _staticCheckFunc)
            {
                action.Invoke();
            }

            return false;
        }

        public IDisposable Subscribe(RedPointComponent com)
        {
            if (string.IsNullOrEmpty(com.path))
            {
                return null;
            }

            var node = FindNode(com.path);
            return node.Subscribe(com.SetData).AddTo(com);
        }

        public LogicNode FindNode(string path)
        {
            var node = _root;
            var keys = path.Split('/');
            foreach (var key in keys)
            {
                node = node.FindNode(key);
            }

            return node;
        }

        public void AddStaticCheckFunc(Action action)
        {
            _staticCheckFunc.Add(action);
        }
    }
}