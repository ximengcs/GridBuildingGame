using System;
using UnityEngine;
using SgFramework.UI;
using SgFramework.Res;
using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using MH.GameScene.Core.Entites;

namespace UI.UIScenes
{
    [UIConfig(UILayer.Default, "Assets/GameRes/Prefabs/UI/UIScene.prefab")]
    public class UIScene : UIForm
    {
        [SerializeField]
        private RectTransform ItemsNode;

        private Vector2 _rectRate;
        private ResourceGroup _resourceGroup;
        private Dictionary<Type, Queue<UISceneItem>> _pool;
        private IWorldCamera _cam;

        public override void OnCreate(object[] args)
        {
            base.OnCreate(args);

            _cam = (IWorldCamera)args[0];
            Vector2 rectSize = ItemsNode.rect.size;
            _rectRate = rectSize / new Vector2(Screen.width, Screen.height);
            _resourceGroup = ResourceManager.GetGroup(nameof(UIScene));
            _pool = new Dictionary<Type, Queue<UISceneItem>>();
        }

        public override void SetLayer(int sortingOrder)
        {
        }

        public override void OnRelease()
        {
            base.OnRelease();
            ResourceManager.ReleaseGroup(_resourceGroup);
            _resourceGroup = null;
            _pool = null;
        }

        public async UniTask<T> Open<T>(IUISceneBinder binder, object args = null) where T : UISceneItem
        {
            if (_cam == null)
                return default;

            Type type = typeof(T);
            T item = null;
            if (_pool.TryGetValue(type, out Queue<UISceneItem> poolItem))
            {
                if (poolItem.Count > 0)
                    item = (T)poolItem.Dequeue();
            }

            if (item == null)
            {
                string resPath = $"Assets/GameRes/Prefabs/UI/UIScene/{type.Name}.prefab";
                var token = await _resourceGroup.GetObject(resPath, ItemsNode, Vector3.zero);
                item = token as T;
                item.OnItemCreate();
            }

            if (item != null)
            {
                item.OnInit(_cam, binder, _rectRate, args);
                item.OnOpen();
            }

            return item;
        }

        public void Close(UISceneItem item)
        {
            if (_cam == null)
                return;

            item.OnClose();
            Type type = item.GetType();
            if (!_pool.TryGetValue(type, out Queue<UISceneItem> items))
            {
                items = new Queue<UISceneItem>();
                _pool.Add(type, items);
            }

            items.Enqueue(item);
            item.OnRecycle();
        }
    }
}
