namespace Yut.AdvancedRegionModule.Regions
{
    internal abstract class RegionPoolBase
    {
        /// <summary>
        /// 从对象池中获取区域
        /// </summary>
        public abstract Region GetFromPool();
        /// <summary>
        /// 将实例返回给对象池
        /// </summary>
        public abstract void ReturnToPool(Region region);
    }
}
