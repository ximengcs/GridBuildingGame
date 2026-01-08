
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MH.GameScene.Runtime.Utilities
{
    public static class UnityUtility
    {
        public static Vector2 Clamp(this Rect rect, Vector2 point, float distance = 0)
        {
            point.x = Mathf.Clamp(point.x, rect.xMin - distance, rect.xMax + distance);
            point.y = Mathf.Clamp(point.y, rect.yMin - distance, rect.yMax + distance);
            return point;
        }

        public static float GetOffset(this SpriteRenderer spriteRender)
        {
            if (spriteRender == null)
            {
                return 0f;
            }

            Vector2 pivot = spriteRender.sprite.pivot;
            float unit = spriteRender.sprite.pixelsPerUnit;
            float offsetUnit = pivot.y / unit;
            float spriteUnit = spriteRender.sprite.rect.width / unit;
            if (spriteUnit <= 1)
                return 0;
            offsetUnit -= 0.25f;
            return offsetUnit;
        }

        public static void FitSize(this Image img, Vector2 rectSize)
        {
            Vector2 texSize = img.sprite.rect.size;
            if (texSize.x < texSize.y)
                img.rectTransform.sizeDelta = new Vector2(rectSize.y * (texSize.x / texSize.y), rectSize.y);
            else
                img.rectTransform.sizeDelta = new Vector2(rectSize.x, rectSize.x * (texSize.y / texSize.x));
        }

        public static async UniTask Waiter(this Tween tween)
        {
            UniTaskCompletionSource taskSource = new UniTaskCompletionSource();
            tween.OnComplete(() => taskSource.TrySetResult());
            await taskSource.Task;
        }

        private static Reporter s_Reporter;

        public static bool IsPointerOverGameObject()
        {
            if (s_Reporter == null)
                s_Reporter = GameObject.FindObjectOfType<Reporter>();

            if (s_Reporter && s_Reporter.show)
                return true;

#if UNITY_EDITOR || UNITY_STANDALONE_WIN
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0) || Input.GetMouseButtonUp(0))
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
#endif
            return false;
        }

    }
}
