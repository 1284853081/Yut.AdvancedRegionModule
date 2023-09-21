using System.Collections.Generic;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    /// <summary>
    /// 圆柱区域
    /// </summary>
    internal class CylinderRegion : Region<CylinderRegionConfig>
    {
        public override void GetCoverRectangle(out Vector3 bottomLeftVertex, out Vector3 topRightVertex)
        {
            bottomLeftVertex = new Vector3(Config.Center.x - Config.Radius, Config.Center.y, Config.Center.z - Config.Radius);
            topRightVertex = new Vector3(Config.Center.x + Config.Radius, Config.Center.y, Config.Center.z + Config.Radius);
        }
        public override List<Vector3> GetDisplayPoints()
        {
            var points = GetEmptyDisplayPoints();
            var round = Mathf.PI * Config.Radius * 2;
            var num = 4;
            if (round >= 40)
                num = Mathf.Clamp(Mathf.FloorToInt(round / 10), 4, 48);
            var angle = 360f / num;
            for (int i = 0; i < num; i++)
            {
                Quaternion quaternion = Quaternion.AngleAxis(angle * i, Vector3.up);
                points.Add(Config.Center + quaternion * Vector3.forward * Config.Radius);
            }
            return points;
        }
        public override bool InRegion(Vector3 vector)
        {
            if (Config.Is3D)
            {
                Config.GetLimitHeight(out var min, out var max);
                if (vector.y < min || vector.y > max)
                    return false;
            }
            var sqrRadius = Config.Radius * Config.Radius;
            if (SqrMagnitude2D(vector - Config.Center) <= sqrRadius)
                return true;
            return false;
        }
        public override Vector3 RandomPointInRegion()
        {
            if (Config.Radius <= 0)
                return Config.Center;
            else
            {
                var circle = Random.insideUnitCircle;
                var result = Config.Center + Random.Range(0,Config.Radius) * new Vector3(circle.x, 0, circle.y);
                if(Config.Is3D)
                {
                    Config.GetLimitHeight(out var min, out var max);
                    result += Vector3.up * Random.Range(min, max);
                }
                return result;
            }
        }
    }
}
