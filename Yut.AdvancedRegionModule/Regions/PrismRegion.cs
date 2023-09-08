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
            for (int i = 0; i < Config.Points.Count; i++)
            {
                int nextInd = GetNextInd(i);
                var curPoint = Config.Points[i];
                var nextPoint = Config.Points[nextInd];
                var flag = vector.x >= Mathf.Min(curPoint.x, nextPoint.x) &&
                    vector.x <= Mathf.Max(curPoint.x, nextPoint.x);
                if (flag)
                {
                    var A = curPoint.z - nextPoint.z;
                    var B = nextPoint.x - curPoint.x;
                    var C = curPoint.x * nextPoint.z - curPoint.z * nextPoint.x;
                    var num2 = A * vector.x + B * vector.z + C;
                    if (num2 == 0)
                        return true;
                    if (num2 < 0)
                        num++;
                }
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
            => currentInd++ < Config.Points.Count - 1 ? currentInd : 0;
    }
}