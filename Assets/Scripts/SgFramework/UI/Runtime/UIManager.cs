using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SgFramework.Res;
using Sirenix.Utilities;
using UnityEngine;

namespace SgFramework.UI
{
    public class UIManager : MonoBehaviour
    {
        private static UIManager _instance;

        [SerializeField] private RectTransform safeArea;

        public static UniTask Initialize()
        {
            return UniTask.WaitUntil(() => _instance);
        }

        private static ResourceGroup _resourceGroup;

        private static readonly Dictionary<int, UIForm> UIDict = new Dictionary<int, UIForm>();

        private static readonly Dictionary<int, List<UIForm>> LayerStack = new Dictionary<int, List<UIForm>>();

        private void Awake()
        {
            _instance = this;
            _resourceGroup = ResourceManager.GetGroup("UI");
        }

        private void OnDestroy()
        {
            ResourceManager.ReleaseGroup(_resourceGroup);
            UIDict.Clear();
            LayerStack.Clear();
            _instance = null;
        }

        public static async UniTask<T> Open<T>(params object[] args) where T : UIForm
        {
            var type = typeof(T);

            var attr = type.GetCustomAttribute<UIConfigAttribute>();
            if (attr == null)
            {
                throw new NotSupportedException("not have UIConfig");
            }

            if (!ResourceGroup.CheckLocationValid(attr.ResourceKey))
            {
                throw new NotSupportedException($"ResourceKey not valid [{attr.ResourceKey}]");
            }

            if (UIDict.TryGetValue(type.GetHashCode(), out var form))
            {
                // 当一个界面在同一帧调用两次打开，会出现这种情况，可以打开该注释处理。但应该不需要。
                // await UniTask.WaitUntil(() =>
                // {
                //     if (!UIDict.ContainsKey(type.GetHashCode()))
                //     {
                //         return true;
                //     }
                //
                //     form = UIDict[type.GetHashCode()];
                //     return form;
                // });

                if (form == null)
                {
                    Debug.LogWarning($"界面还在加载中就调用第二次打开，使用上面注释的代码可以等待加载完成后调用第二次。");
                    return null;
                }

                form.OnCreate(args);
                form.transform.SetParent(_instance.safeArea, false);
                PushLayerStack(attr.Layer, form);
                await form.ShowOpen();
                return form as T;
            }

            UIDict[type.GetHashCode()] = null;
            var token = await _resourceGroup.GetObject(attr.ResourceKey, _instance.safeArea, Vector3.zero);
            var ui = token as T;
            if (ui == null)
            {
                UIDict.Remove(type.GetHashCode());
                throw new InvalidOperationException($"{attr.ResourceKey} 加载成功，但UI类型有误。 [{type.Name}]");
            }

            UIDict[type.GetHashCode()] = ui;

            ui.OnCreate(args);
            PushLayerStack(attr.Layer, ui);
            await ui.ShowOpen();
            return ui;
        }

        private static void PushLayerStack(int layer, UIForm ui)
        {
            if (!LayerStack.TryGetValue(layer, out var stack))
            {
                stack = new List<UIForm>();
                LayerStack.Add(layer, stack);
            }

            var dirty = stack.Remove(ui);
            ui.SetLayer(layer + stack.Count);
            stack.Add(ui);
            if (!dirty)
            {
                return;
            }

            SortLayer(layer);
        }

        private static void SortLayer(int layer)
        {
            if (!LayerStack.TryGetValue(layer, out var stack))
            {
                throw new NotSupportedException();
            }

            for (var i = 0; i < stack.Count; i++)
            {
                stack[i].SetLayer(layer + i);
            }
        }

        private static void PopLayerStack(int layer, UIForm ui)
        {
            if (!LayerStack.TryGetValue(layer, out var stack))
            {
                throw new NotSupportedException();
            }

            stack.Remove(ui);
            SortLayer(layer);
        }

        public static async UniTask Close<T>()
        {
            var type = typeof(T);
            if (!UIDict.TryGetValue(type.GetHashCode(), out var form))
            {
                return;
            }

            var attr = type.GetCustomAttribute<UIConfigAttribute>();
            PopLayerStack(attr.Layer, form);
            UIDict.Remove(type.GetHashCode());
            await form.ShowClose();
            _resourceGroup.ReleaseObject(form);
        }

        public static UniTask UnloadUnusedAssets()
        {
            return _resourceGroup.UnloadUnusedAssets();
        }
    }
}