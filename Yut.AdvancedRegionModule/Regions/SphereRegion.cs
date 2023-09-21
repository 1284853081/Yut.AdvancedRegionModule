using System.Collections.Generic;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    /// <summary>
    /// 球形区域
    /// </summary>
    internal class SphereRegion : Region<SphereRegionConfig>
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
            if ((Config.Center - vector).sqrMagnitude <= Config.Radius * Config.Radius)
                return true;
            return false;
        }
        public override Vector3 RandomPointInRegion()
        {
            if (Config.Radius <= 0)
                return Config.Center;
            else
                return Config.Center + Random.Range(0, Config.Radius) * Random.insideUnitSphere;
        }
    }
}
