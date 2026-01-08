using System;
using System.Collections.Generic;

namespace SgFramework.Event
{
    public class EventGroup
    {
        private readonly Dictionary<Type, List<Action<IEventMessage>>> _cachedListener = new Dictionary<Type, List<Action<IEventMessage>>>();

        /// <summary>
        /// 添加一个监听
        /// </summary>
        public void AddListener<TEvent>(Action<IEventMessage> listener) where TEvent : IEventMessage
        {
            var eventType = typeof(TEvent);
            if (_cachedListener.ContainsKey(eventType) == false)
                _cachedListener.Add(eventType, new List<Action<IEventMessage>>());

            if (_cachedListener[eventType].Contains(listener) == false)
            {
                _cachedListener[eventType].Add(listener);
                SgEvent.AddListener(eventType, listener);
            }
            else
            {
                SgLogger.Warning($"Event listener is exist : {eventType}");
            }
        }

        /// <summary>
        /// 移除所有缓存的监听
        /// </summary>
        public void RemoveAllListener()
        {
            foreach (var (eventType, list) in _cachedListener)
            {
                foreach (var listener in list)
                {
                    SgEvent.RemoveListener(eventType, listener);
                }
                list.Clear();
            }
            _cachedListener.Clear();
        }
    }
}