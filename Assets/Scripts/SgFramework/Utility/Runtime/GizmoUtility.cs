using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SgFramework.Utility {

    public class GizmoUtility
    {
#if UNITY_EDITOR
        private static GUIStyle _gUILabel;
#endif
        public static Color Color { get => Gizmos.color; set => Gizmos.color = value; }

        /// <summary>
        /// 绘制一个点
        /// </summary>
        /// <param name="vector3">位置</param>
        public static void DrawPoint(Vector3 vector3) {

            Gizmos.DrawIcon(vector3,string.Empty);
        }

        /// <summary>
        /// 绘制文字字符
        /// </summary>
        public static void DrawLabel(Vector3 center, string text,int size = 10, TextAnchor textAnchor = TextAnchor.MiddleCenter) 
        {
#if UNITY_EDITOR
            var handSize = (int)UnityEditor.HandleUtility.GetHandleSize(center);

            _gUILabel ??= new GUIStyle();
            _gUILabel.alignment = textAnchor;
            _gUILabel.normal.textColor = Color;
            _gUILabel.fontSize = handSize < size ? size : Mathf.Max(1,size - (handSize - size));
            
            UnityEditor.Handles.Label(center, text, _gUILabel);
#endif
        }

        /// <summary>
        /// 绘制一条线
        /// </summary>
        /// <param name="a">端点 a</param>
        /// <param name="b">端点 b</param>
        public static void DrawLine(Vector3 a, Vector3 b)
        {
            Gizmos.DrawLine(a, b);
        }

        /// <summary>
        /// 绘制一虚条线
        /// </summary>
        /// <param name="a">端点 a</param>
        /// <param name="b">端点 b</param>
        /// <param name="space">虚线空白长度</param>
        public static void DrawXLine(Vector3 a, Vector3 b, float space = 0.01f)
        {
            var c = (int)(Vector3.Distance(a, b) / space);

            for (var i = 0; i < c; i += 2)
            {
                var t = (float)i / c;
                var t2 = (float)(i + 1) / c;

                Gizmos.DrawLine(Vector3.Lerp(a, b, t), Vector3.Lerp(a, b, t2));
            }
        }

        /// <summary>
        /// 绘制一个箭头
        /// </summary>
        /// <param name="from">起点</param>
        /// <param name="to">终点</param>
        /// <param name="arrowHeadLength">箭头占身体的比例</param>
        /// <param name="arrowHeadAngle">箭头与身体的角度</param>
        public static void DrawArrow(Vector3 from, Vector3 to, float arrowHeadLength = 0.25f, float arrowHeadAngle = 20.0f)
        {
            var direction = to - from;
            var right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            var left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);

            Gizmos.DrawLine(from, to);
            Gizmos.DrawLine(to, to + right * arrowHeadLength* direction.magnitude);
            Gizmos.DrawLine(to, to + left * arrowHeadLength * direction.magnitude);
        }

        /// <summary>
        /// 绘制一个围绕中心旋转的圆弧
        /// </summary>
        /// <param name="center">中心点</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度</param>
        /// <param name="segments">段数</param>
        /// <param name="rotation">方向 - 以Y轴中心开始</param>
        public static void DrawArc(Vector3 center, float radius, float angle, Quaternion rotation, int segments = 20)
        {
            var old = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            var from = Vector3.forward * radius;
            for (var n = 0; n < segments; n++)
            {
                var i = ((n + 1) / (float)segments) * angle;
                var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad));
                Gizmos.DrawLine(from, to);
                from = to;
            }

            Gizmos.matrix = old;
        }

        /// <summary>
        /// 围绕任意旋转中心绘制一个旋转的圆弧
        /// </summary>
        /// <param name="center">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">角度</param>
        /// <param name="segments">段数</param>
        /// <param name="rotation">方向 - 以Y轴中心开始</param>
        /// <param name="centerOfRotation">旋转中心</param>
        public static void DrawArc(Vector3 center, float radius, float angle, int segments, Quaternion rotation, Vector3 centerOfRotation)
        {
            var old = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(centerOfRotation, rotation, Vector3.one);
            var deltaTranslation = centerOfRotation - center;
            var from = deltaTranslation + Vector3.forward * radius;

            for (var n = 0; n < segments; n++)
            {
                var i = ((n + 1) / (float)segments) * angle;
                var to = new Vector3(radius * Mathf.Sin(i * Mathf.Deg2Rad), 0, radius * Mathf.Cos(i * Mathf.Deg2Rad)) + deltaTranslation;
                Gizmos.DrawLine(from, to);
                from = to;
            }

            Gizmos.matrix = old;
        }

        /// <summary>
        /// 绘制一个环
        /// </summary>
        /// <param name="center">圆心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="rotation"></param>
        /// <param name="segments">段数</param>
        public static void DrawCirce(Vector3 center, float radius, Quaternion rotation, int segments = 15)
        {
            DrawArc(center, radius, 360, rotation, segments);
        }

        /// <summary>
        /// 绘制一个线框平面
        /// </summary>
        /// <param name="center">平面中心位置</param>
        /// <param name="size"></param>
        /// <param name="rotation"></param>
        public static void DrawPlane(Vector3 center,Vector2 size, Quaternion rotation) 
        {
            DrawCube(center, new Vector3(size.x, size.y,0), rotation);
        }

        /// <summary>
        /// 绘制一个线框立方体
        /// </summary>
        /// <param name="center">平面中心位置</param>
        /// <param name="size"></param>
        /// <param name="rotation">方向</param>
        public static void DrawCube(Vector3 center, Vector3 size, Quaternion rotation)
        {
            var old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, size);
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
            Gizmos.matrix = old;
        }

        /// <summary>
        /// 绘制一个线框立方体
        /// </summary>
        /// <param name="matrix"></param>
        public static void DrawMatrixCube(Matrix4x4 matrix)
        {
            DrawCube(matrix.GetPosition(), matrix.lossyScale, matrix.rotation);
        }

        /// <summary>
        /// 绘制路径线段
        /// </summary>
        /// <param name="path">路径点集合</param>
        /// <param name="loop">循环</param>
        public static void DrawPath(IEnumerable<Vector3> path, bool loop = false)
        {
            var arr = path.ToArray();

            for (var i = 0; i < arr.Length - 1; i++)
            {
                Gizmos.DrawLine(arr[i], arr[i + 1]);
            }
            if (loop && arr.Length > 1)
            {
                Gizmos.DrawLine(arr[0], arr[^1]);
            }
        }

        /// <summary>
        /// 绘制路径线段
        /// </summary>
        /// <param name="path">路径点集合</param>
        /// <param name="loop">循环</param>
        public static void DrawPath(IEnumerable<Vector2> path, bool loop = false)
        {
            var arr = path.ToArray();

            for (var i = 0; i < arr.Length - 1; i++)
            {
                Gizmos.DrawLine(arr[i], arr[i + 1]);
            }
            if (loop && arr.Length > 1)
            {
                Gizmos.DrawLine(arr[0], arr[arr.Length - 1]);
            }
        }

        /// <summary>
        /// 绘制坐标系
        /// </summary>
        /// <param name="p">中心位置</param>
        /// <param name="quaternion">角度</param>
        /// <param name="size">尺寸</param>
        public static void DrawCoord(Vector3 p, float size, Quaternion quaternion)
        {
            var color = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(p, p + quaternion * Vector3.right * size);
            Gizmos.color = Color.green;
            Gizmos.DrawLine(p, p + quaternion * Vector3.up * size);
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(p, p + quaternion * Vector3.forward * size);
            Gizmos.color = color;
        }

        /// <summary>
        /// 绘制线框球
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="rotation">角度</param>
        public static void DrawSphere(Vector3 center, float radius, Quaternion rotation)
        {
            var old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            Gizmos.DrawWireSphere(Vector3.zero, radius);
            Gizmos.matrix = old;
        }

        /// <summary>
        /// 绘制一个面朝上的线框圆柱体，并围绕中心旋转
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高</param>
        /// <param name="rotation">角度</param>
        public static void DrawCylinder(Vector3 center, float radius, float height, Quaternion rotation)
        {
            var old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            var half = height / 2;

            //绘制出4条外线
            Gizmos.DrawLine(Vector3.right * radius - Vector3.up * half, Vector3.right * radius + Vector3.up * half);
            Gizmos.DrawLine(-Vector3.right * radius - Vector3.up * half, -Vector3.right * radius + Vector3.up * half);
            Gizmos.DrawLine(Vector3.forward * radius - Vector3.up * half, Vector3.forward * radius + Vector3.up * half);
            Gizmos.DrawLine(-Vector3.forward * radius - Vector3.up * half, -Vector3.forward * radius + Vector3.up * half);

            //绘制两个圆，旋转的中心是圆柱体的中心，而不是圆本身的中心
            DrawArc(center + Vector3.up * half, radius, 360, 20, rotation, center);
            DrawArc(center + Vector3.down * half, radius, 360, 20, rotation, center);
            Gizmos.matrix = old;
        }

        /// <summary>
        /// 绘制一个面朝上的线框胶囊
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="height">高</param>
        /// <param name="rotation">角度</param>
        public static void DrawCapsule(Vector3 center, float radius, float height, Quaternion rotation)
        {
            var old = Gizmos.matrix;
            Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
            var half = height / 2 - radius;

            //胶囊底座
            DrawCylinder(center, radius, height - radius * 2, rotation);

            //绘制上帽
            //do some cool stuff with orthogonal matrices
            var mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.forward));
            DrawArc(mat.GetPosition(), radius, 180, mat.rotation, 20);
            mat = Matrix4x4.Translate(center + rotation * Vector3.up * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(90, Vector3.forward));
            DrawArc(mat.GetPosition(), radius, 180, mat.rotation, 20);

            //绘制下帽
            mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(90, Vector3.up) * Quaternion.AngleAxis(-90, Vector3.forward));
            DrawArc(mat.GetPosition(), radius, 180, mat.rotation, 20);
            mat = Matrix4x4.Translate(center + rotation * Vector3.down * half) * Matrix4x4.Rotate(rotation * Quaternion.AngleAxis(-90, Vector3.forward));
            DrawArc(mat.GetPosition(), radius, 180, mat.rotation, 20);

            Gizmos.matrix = old;
        }

        /// <summary>
        /// 绘制线框立体角
        /// </summary>
        /// <param name="center">中心位置</param>
        /// <param name="radius">半径</param>
        /// <param name="angle">立体角度</param>
        /// <param name="rotation">角度</param>
        /// <param name="segments">段数</param>
        public static void DrawSolidAngle(Vector3 center, float radius, float angle, Quaternion rotation, int segments = 20)
        {
            angle = Mathf.Clamp(angle, 0, 180);

            var matrix = Matrix4x4.identity;

            matrix = Matrix4x4.TRS(center, rotation * Quaternion.Euler(0, 0, 0) * Quaternion.Euler(0, -angle / 2f, 0), Vector3.one);
            DrawArc(matrix.GetPosition(), radius, angle, matrix.rotation, segments);

            matrix = Matrix4x4.TRS(center, rotation * Quaternion.Euler(0, 0, 90) * Quaternion.Euler(0, -angle / 2f, 0), Vector3.one);
            DrawArc(matrix.GetPosition(), radius, angle, matrix.rotation, segments);

            Vector3 a = matrix * Vector3.forward * radius;
            Vector3 b = matrix * new Vector3(radius * Mathf.Sin(angle * Mathf.Deg2Rad), 0, radius * Mathf.Cos(angle * Mathf.Deg2Rad));

            var c = (a + b) / 2f;
            var radius2 = Vector3.Distance(a, b) / 2f;
            DrawArc(c + center, radius2, 360f, rotation * Quaternion.Euler(90, 0, 0), segments);

            var old = Gizmos.matrix;

            Gizmos.matrix = Matrix4x4.TRS(c + center, rotation * Quaternion.Euler(90, 0, 0), Vector3.one);

            var pos = Mathf.Sqrt(radius * radius - radius2 * radius2) * Vector3.down;

            Gizmos.DrawLine(pos, new Vector3(radius2 * Mathf.Sin(0 * Mathf.Deg2Rad), 0, radius2 * Mathf.Cos(0 * Mathf.Deg2Rad)));
            Gizmos.DrawLine(pos, new Vector3(radius2 * Mathf.Sin(90 * Mathf.Deg2Rad), 0, radius2 * Mathf.Cos(90 * Mathf.Deg2Rad)));
            Gizmos.DrawLine(pos, new Vector3(radius2 * Mathf.Sin(180 * Mathf.Deg2Rad), 0, radius2 * Mathf.Cos(180 * Mathf.Deg2Rad)));
            Gizmos.DrawLine(pos, new Vector3(radius2 * Mathf.Sin(270 * Mathf.Deg2Rad), 0, radius2 * Mathf.Cos(270 * Mathf.Deg2Rad)));

            Gizmos.matrix = old;
        }
    }
}