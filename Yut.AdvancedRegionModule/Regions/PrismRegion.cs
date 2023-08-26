using System.Collections.Generic;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    /// <summary>
    /// 棱柱区域
    /// </summary>
    internal class PrismRegion : Region<PrismRegionConfig>
    {
        public override void GetCoverRectangle(out Vector3 bottomLeftVertex, out Vector3 topRightVertex)
        {
            var count = Config.Points.Count;
            if (count == 0)
            {
                bottomLeftVertex = Vector3.zero;
                topRightVertex = Vector3.zero;
                return;
            }
            var minx = float.MaxValue;
            var miny = float.MaxValue;
            var maxx = float.MinValue;
            var maxy = float.MinValue;
            for (int i = 0; i < count; i++)
            {
                var point = Config.Points[i];
                if (point.x < minx)
                    minx = point.x;
                if (point.x > maxx)
                    maxx = point.x;
                if (point.z < miny)
                    miny = point.z;
                if (point.z > maxy)
                    maxy = point.z;
            }
            bottomLeftVertex = new Vector3(minx, 0, miny);
            topRightVertex = new Vector3(maxx, 0, maxy);
        }
        public override List<Vector3> GetDisplayPoints()
        {
            var points = GetEmptyDisplayPoints();
            var count = Config.Points.Count;
            if (count == 1)
            {
                points.Add(Config.Points[0]);
            }
            else if (count == 2)
            {
                var start = Config.Points[0];
                var end = Config.Points[1];
                var forward = end - start;
                var dist = forward.magnitude;
                var num = Mathf.Clamp(Mathf.FloorToInt(dist / 10), 0, 6);
                for (int j = 0; j < num; j++)
                    points.Add(start + forward / num * j);
                points.Add(end);
            }
            else
            {
                for (int i = 0; i < count; i++)
                {
                    var nextInd = GetNextInd(i);
                    var start = Config.Points[i];
                    var end = Config.Points[nextInd];
                    var forward = end - start;
                    var dist = forward.magnitude;
                    var num = Mathf.Clamp(Mathf.FloorToInt(dist / 10), 0, 6);
                    for (int j = 0; j < num; j++)
                        points.Add(start + forward / num * j);
                }
            }
            return points;
        }
        public override bool InRegion(Vector3 vector)
        {
            if (Config.Points.Count < 3)
                return false;
            if (Config.Is3D)
            {
                Config.GetLimitHeight(out var min, out var max);
                if (vector.y < min || vector.y > max)
                    return false;
            }
            int num = 0;
            var count = Config.Points.Count;
            for (int i = 0; i < count; i++)
            {
                int nextInd = GetNextInd(i);
                var curPoint = Config.Points[i];
                var nextPoint = Config.Points[nextInd];
                if (OnLine(vector, curPoint, nextPoint))
                    return true;
                if (CrossLine(vector, curPoint, nextPoint))
                    num++;
            }
            return num % 2 == 1;
        }
        public override Vector3 RandomPointInRegion()
        {
            GetCoverRectangle(out var bottom, out var top);
            for (var i = 0; i < 3; i++)
            {
                var x = Random.Range(bottom.x, top.x);
                var y = Random.Range(bottom.z, top.z);
                var result = new Vector3(x, 0, y);
                if (InRegion(result))
                    return result;
            }
            return Vector3.zero;
        }
        private int GetNextInd(int currentInd)
        {
            if (currentInd < Config.Points.Count - 1)
                return currentInd + 1;
            return 0;
        }
        private bool OnLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            GetLimit(lineStart.x, lineEnd.x, out var num1, out var num2);
            GetLimit(lineStart.z, lineEnd.z, out var num3, out var num4);
            if (point.x < num1 || point.x > num2 || point.z < num3 || point.z > num4)
                return false;
            if (point.Equals(lineStart) || point.Equals(lineEnd))
                return true;
            if (point.x == num1 && point.x == num2)
                return true;
            var num5 = (point.z - lineStart.z) / (point.x - lineStart.x);
            var num6 = (lineEnd.z - point.z) / (lineEnd.x - point.x);
            if (num5 != num6)
                return false;
            return true;
        }
        private bool CrossLine(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
        {
            GetLimitPoint(lineStart, lineEnd, out var point2, out var point3);
            if (point.x > Mathf.Max(point2.x, point3.x) || point.z > point2.z || point.z < point3.z)
                return false;
            if (point.z == point2.z && point.z == point3.z)
                return false;
            if (point.z == point2.z && point.z != point3.z)
                return true;
            if (point.z == point3.z && point.z != point2.z)
                return false;
            if (point2.x == point3.x)
                return true;
            if ((point.z - point3.z) * (point2.x - point3.x) / (point2.z - point3.z) + point3.x < point.x)
                return false;
            return true;
        }
        private void GetLimit(float x, float y, out float min, out float max)
        {
            if (x < y)
            {
                min = x;
                max = y;
            }
            else
            {
                min = y;
                max = x;
            }
        }
        private void GetLimitPoint(Vector3 left, Vector3 right, out Vector3 up, out Vector3 down)
        {
            if (left.z < right.z)
            {
                down = left;
                up = right;
            }
            else
            {
                down = right;
                up = left;
            }
        }
    }
}
