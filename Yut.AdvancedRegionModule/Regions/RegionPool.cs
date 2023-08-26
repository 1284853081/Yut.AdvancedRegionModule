using System.Collections.Generic;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    internal sealed class RegionPool<T> : RegionPoolBase
        where T : Region
    {
        private readonly Queue<T> _objects;
        private ushort maxCapacity;
        private readonly GameObject original;
        public ushort MaxCapacity
        {
            get => maxCapacity;
            set => maxCapacity = value;
        }
        public RegionPool(ushort initCapacity, ushort maxCapacity)
        {
            this.maxCapacity = maxCapacity;
            _objects = new Queue<T>(maxCapacity);
            original = new GameObject();
            Object.DontDestroyOnLoad(original);
            for (int i = 0; i < initCapacity; i++)
            {
                var region = GenerateRegion();
                region.enabled = false;
                if (!(region is null))
                    _objects.Enqueue(region);
            }
        }
        public override Region GetFromPool()
        {
            if (_objects.Count > 0)
            {
                var region = _objects.Dequeue();
                region.enabled = true;
                return region;
            }
            else
                return GenerateRegion();
        }
        public override void ReturnToPool(Region region)
        {
            if (_objects.Count < maxCapacity && region is T obj && !_objects.Contains(obj))
            {
                obj.enabled = false;
                _objects.Enqueue(obj);
            }
        }
        private T GenerateRegion()
        {
            var clone = Object.Instantiate(original);
            Object.DontDestroyOnLoad(clone);
            var region = clone.AddComponent<T>();
            if (region is null)
            {
                Object.Destroy(clone);
                return null;
            }
            return region;
        }
    }
}
