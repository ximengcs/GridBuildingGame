using System.Reflection;
using SgFramework.Language;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HierarchyEditor
{
    private const float MinWindowWidth = 240f; // 设置显示图标和Toggle的最小窗口宽度

    // 在加载时初始化
    [InitializeOnLoadMethod]
    static void HierarchyExtensionIcon()
    {
        var activeStyle = new GUIStyle() { normal = { textColor = Color.green } };
        var inactiveStyle = new GUIStyle() { normal = { textColor = new Color(0, 1, 0, 0.5F) } };

        EditorApplication.hierarchyWindowItemOnGUI += (instanceID, selectionRect) =>
        {
            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (go == null)
            {
                return;
            }

            var index = 0;
            CustomDraw(go, selectionRect, ref index);
            //绘制对象激活状态切换按钮
            //DrawActiveToggle(go, selectionRect, ref index);
            //绘制静态标记
            //DrawStatic(go, selectionRect, ref index);
            //绘制组件ICON
            //重绘对象名称
            //DrawGameObjectName(go, selectionRect, activeStyle, inactiveStyle);
        };
    }

    // 获取 Hierarchy 窗口的宽度
    private static float GetHierarchyWindowWidth()
    {
        var hierarchyInfo = typeof(Editor).Assembly
            .GetType("UnityEditor.SceneHierarchyWindow")
            ?.GetProperty("lastInteractedHierarchyWindow",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

        var hierarchyWindow = (EditorWindow)hierarchyInfo?.GetValue(null);
        return hierarchyWindow?.position.width ?? 0;
    }

    /// <summary>
    /// 获取Rect
    /// </summary>
    private static Rect GetRect(Rect selectionRect, int index)
    {
        var rect = new Rect(selectionRect);
        if (GetHierarchyWindowWidth() >= MinWindowWidth)
        {
            rect.x += rect.width - (18 * index);
        }
        else
        {
            rect.x += rect.width + (18 * index);
        }

        rect.width = 18;
        return rect;
    }

    private static void DrawStatic(GameObject go, Rect selectionRect, ref int index)
    {
        if (!go.isStatic)
        {
            return;
        }

        index++;
        var rect = GetRect(selectionRect, index);
        GUI.Label(rect, "S");
    }

    private static void CustomDraw(GameObject go, Rect selectionRect, ref int index)
    {
        if (go.TryGetComponent(out Graphic g) && g.raycastTarget)
        {
            index++;
            var rect = GetRect(selectionRect, index);
            var icon = EditorGUIUtility.IconContent("raycast");
            GUI.Label(rect, icon);
        }

        var hasTxt = go.TryGetComponent(out TextMeshProUGUI _);
        var hasLan = go.TryGetComponent(out LanguageText lan);

        //文本组件提示是否有本地化组件
        if (hasTxt)
        {
            if (hasLan)
            {
                index++;
                var rect = GetRect(selectionRect, index);
                var key = string.IsNullOrEmpty(lan.languageKey) ? "noKey" : "hasKey";
                var icon = EditorGUIUtility.IconContent($"txt_lan_{key}");
                GUI.Label(rect, icon);
            }
            else
            {
                index++;
                var rect = GetRect(selectionRect, index);
                var icon = EditorGUIUtility.IconContent($"txt_no_lan");
                GUI.Label(rect, icon);
            }
        }

        if (hasLan && !hasTxt)
        {
            index++;
            var rect = GetRect(selectionRect, index);
            var icon = EditorGUIUtility.IconContent($"lan_no_txt");
            GUI.Label(rect, icon);
        }
    }

    /// <summary>
    /// 绘制对象名称
    /// </summary>
    private static void DrawGameObjectName(GameObject go, Rect selectionRect, GUIStyle activeStyle,
        GUIStyle inactiveStyle)
    {
        selectionRect.x += 18;
        var style = go.activeSelf ? activeStyle : inactiveStyle;
        if (PrefabUtility.IsPartOfAnyPrefab(go)) return;
        GUI.Label(selectionRect, go.name, style);
    }
}