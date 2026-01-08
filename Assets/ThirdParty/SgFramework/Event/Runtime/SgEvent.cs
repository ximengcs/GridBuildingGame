using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SgFramework.Event
{
    public static class SgEvent
    {
        private static bool _isInitialize = false;
        private static GameObject _driver = null;

        private static readonly Dictionary<int, LinkedList<Action<IEventMessage>>> Listeners =
            new Dictionary<int, LinkedList<Action<IEventMessage>>>(1000);

        /// <summary>
        /// 初始化事件系统
        /// </summary>
        public static void Initialize()
        {
            if (_isInitialize)
            {
                Debug.Log($"{nameof(SgEvent)} is initialized !");
                return;
            }

            // 创建驱动器
            _isInitialize = true;
            _driver = new GameObject($"[{nameof(SgEvent)}]");
            Object.DontDestroyOnLoad(_driver);
            SgLogger.Log($"{nameof(SgEvent)} initialize !");
        }

        /// <summary>
        /// 销毁事件系统
        /// </summary>
        public static void Destroy()
        {
            if (!_isInitialize)
            {
                return;
            }

            ClearAll();

            _isInitialize = false;
            if (_driver != null)
            {
                Object.Destroy(_driver);
            }
            SgLogger.Log($"{nameof(SgEvent)} destroy all !");
        }

        /// <summary>
        /// 清空所有监听
        /// </summary>
        public static void ClearAll()
        {
            foreach (var eventId in Listeners.Keys)
            {
                Listeners[eventId].Clear();
            }

            Listeners.Clear();
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            var eventType = typeof(TEvent);
            var eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener(Type eventType, Action<IEventMessage> listener)
        {
            var eventId = eventType.GetHashCode();
            AddListener(eventId, listener);
        }

        /// <summary>
        /// 添加监听
        /// </summary>
        public static void AddListener(int eventId, Action<IEventMessage> listener)
        {
            if (!Listeners.ContainsKey(eventId))
            {
                Listeners.Add(eventId, new LinkedList<Action<IEventMessage>>());
            }
            
            if (!Listeners[eventId].Contains(listener))
            {
                Listeners[eventId].AddLast(listener);
            }
        }


        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            var eventType = typeof(TEvent);
            var eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener(Type eventType, Action<IEventMessage> listener)
        {
            var eventId = eventType.GetHashCode();
            RemoveListener(eventId, listener);
        }

        /// <summary>
        /// 移除监听
        /// </summary>
        public static void RemoveListener(int eventId, Action<IEventMessage> listener)
        {
            if (Listeners.ContainsKey(eventId))
            {
                if (Listeners[eventId].Contains(listener))
                    Listeners[eventId].Remove(listener);
            }
        }


        /// <summary>
        /// 实时广播事件
        /// </summary>
        public static void SendMessage(IEventMessage message)
        {
            var eventId = message.GetType().GetHashCode();
            SendMessage(eventId, message);
        }

        /// <summary>
        /// 实时广播事件
        /// </summary>
        private static void SendMessage(int eventId, IEventMessage message)
        {
            if (!Listeners.TryGetValue(eventId, out var listeners))
            {
                return;
            }

            if (listeners.Count <= 0)
            {
                return;
            }

            var currentNode = listeners.Last;
            while (currentNode != null)
            {
                currentNode.Value.Invoke(message);
                currentNode = currentNode.Previous;
            }
        }
    }
}