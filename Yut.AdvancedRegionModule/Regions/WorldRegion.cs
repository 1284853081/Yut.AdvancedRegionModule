using System.Collections.Generic;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    /// <summary>
    /// 世界区域
    /// </summary>
    internal sealed class WorldRegion : Region<WorldRegionConfig>
    {
        public override void GetCoverRectangle(out Vector3 bottomLeftVertex, out Vector3 topRightVertex)
        {
            bottomLeftVertex = Vector3.zero;
            topRightVertex = Vector3.zero;
        }
        public override List<Vector3> GetDisplayPoints()
            => GetEmptyDisplayPoints();
        public override bool InRegion(Vector3 vector)
            => true;
        public override Vector3 RandomPointInRegion()
            => Vector3.zero;
    }
}
