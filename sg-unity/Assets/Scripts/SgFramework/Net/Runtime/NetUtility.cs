using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Google.Protobuf;
using Debug = UnityEngine.Debug;

namespace SgFramework.Net
{
    public static class NetUtility
    {
        private static readonly Dictionary<string, byte[]> MsgNames = new Dictionary<string, byte[]>();
        private static readonly Dictionary<string, Type> MsgTypes = new Dictionary<string, Type>();

        public static void Initialize()
        {
            if (MsgNames.Count > 0)
            {
                return;
            }

            var sw = new Stopwatch();
            sw.Start();
            var t = typeof(IMessage<>);
            var assembly = Assembly.GetCallingAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (!type.GetInterfaces().Any(x => x.IsGenericType && x.GetGenericTypeDefinition() == t))
                {
                    continue;
                }

                MsgNames.Add(type.Name, Encoding.UTF8.GetBytes(type.Name));
                MsgTypes.Add(type.Name, type);
            }

            sw.Stop();
            Debug.Log($"Net utility init cost:{sw.ElapsedMilliseconds}ms");
        }

        public static bool GetName(IMessage msg, out byte[] data)
        {
            var type = msg.GetType();
            return MsgNames.TryGetValue(type.Name, out data);
        }

        public static bool GetMessage(string name, out IMessage msg)
        {
            msg = null;
            if (!MsgTypes.TryGetValue(name, out var type))
            {
                return false;
            }

            msg = Activator.CreateInstance(type) as IMessage;
            return true;
        }
    }
}