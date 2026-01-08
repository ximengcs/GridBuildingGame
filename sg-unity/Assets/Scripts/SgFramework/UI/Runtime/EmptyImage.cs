
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SgFramework.UI
{
    [AddComponentMenu("UI/EmptyImage")]
    public class EmptyImage : Image
    {
#if UNITY_EDITOR
        [MenuItem("GameObject/UI/EmptyImage", false, 10)]
        private static void AddEmptyImage(MenuCommand menuCommand)
        {
            var go = new GameObject(nameof(EmptyImage));
            go.AddComponent<EmptyImage>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, $"Create {go.name}");
            Selection.activeObject = go;
        }
#endif

        protected override void OnPopulateMesh(VertexHelper toFill)
        {
            toFill.Clear();
        }
    }
}