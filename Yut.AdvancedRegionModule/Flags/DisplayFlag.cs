using SDG.Unturned;
using UnityEngine;
using Yut.UnturnedEx.Extensions;

namespace Yut.AdvancedRegionModule.Flags
{
    public sealed class DisplayFlag : RegionFlag<DisplayConfig>
    {
        private ItemStructureAsset asset;
        private Vector3 lastMin;
        private Vector3 lastMax;
        private ulong owner;
        private ulong group;
        public bool Display
        {
            get => FlagConfig.Enabled;
            set => UpdateConfig(Keys.ENABLED, Keys.SET, value.ToString());
        }
        protected override void OnInit()
        {
            asset = Assets.find(EAssetType.ITEM, 36) as ItemStructureAsset;
            var id = Region.RegionID;
            owner = (ulong)(id & 0xFFFFFFFF);
            group = (ulong)(id >> 32);
            Region.SetDisplayFlag(this);
        }
        protected override void OnRegionPreChange()
        {
            Region.GetCoverRectangle(out lastMin, out lastMax);
        }
        protected override void OnRegionChanged()
        {
            if (!FlagConfig.Enabled)
                return;
            ReDisplayRegion();
        }
        protected override void OnConfigUpdated()
        {
            if (FlagConfig.Enabled)
                DisplayRegion();
            else
                CancelDisplayRegion();
        }
        protected override void OnDestroy()
        {
            if(!Region.IsStatic)
                CancelDisplayRegion();
        }
        private void DisplayRegion()
        {
            var points = Region.GetDisplayPoints();
            var count = points.Count;
            for (int i = 0; i < count; i++)
            {
                var vector = points[i];
                var structure = new Structure(asset, ushort.MaxValue);
                vector.ToSurface(out _);
                StructureManager.dropReplicatedStructure(structure, vector, Quaternion.Euler(-90f, 0, 0), owner, group);
            }
        }
        private void CancelDisplayRegion()
        {
            if (StructureManager.regions is null)
                return;
            Region.GetCoverRectangle(out var min, out var max);
            min.GetUnsafeCoordinates(out var x1, out var y1);
            max.GetUnsafeCoordinates(out var x2, out var y2);
            if (x1 >= SDG.Unturned.Regions.WORLD_SIZE || y1 >= SDG.Unturned.Regions.WORLD_SIZE || x2 < 0 || y2 < 0)
                return;
            x1 = Mathf.Max(x1, 0);
            x2 = Mathf.Min(x2, SDG.Unturned.Regions.WORLD_SIZE - 1);
            y1 = Mathf.Max(y1, 0);
            y2 = Mathf.Min(y2, SDG.Unturned.Regions.WORLD_SIZE - 1);
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    var structureRegion = StructureManager.regions[i, j];
                    if (structureRegion is null)
                        continue;
                    var count = structureRegion.drops.Count;
                    for (int k = count - 1; k >= 0; k--)
                    {
                        var drop = structureRegion.drops[k];
                        StructureData data = drop.GetServersideData();
                        if (data.owner == owner && data.group == group)
                            StructureManager.destroyStructure(drop, (byte)i, (byte)j, data.point);
                    }
                }
            }
        }
        private void ReDisplayRegion()
        {
            if (StructureManager.regions is null)
                return;
            lastMin.GetUnsafeCoordinates(out var x1, out var y1);
            lastMax.GetUnsafeCoordinates(out var x2, out var y2);
            if (x1 >= SDG.Unturned.Regions.WORLD_SIZE || y1 >= SDG.Unturned.Regions.WORLD_SIZE || x2 < 0 || y2 < 0)
                return;
            x1 = Mathf.Max(x1, 0);
            x2 = Mathf.Min(x2, SDG.Unturned.Regions.WORLD_SIZE - 1);
            y1 = Mathf.Max(y1, 0);
            y2 = Mathf.Min(y2, SDG.Unturned.Regions.WORLD_SIZE - 1);
            for (int i = x1; i <= x2; i++)
            {
                for (int j = y1; j <= y2; j++)
                {
                    var structureRegion = StructureManager.regions[i, j];
                    if (structureRegion is null)
                        continue;
                    var count = structureRegion.drops.Count;
                    for (int k = count - 1; k >= 0; k--)
                    {
                        var drop = structureRegion.drops[k];
                        StructureData data = drop.GetServersideData();
                        if (data.owner == owner && data.group == group)
                            StructureManager.destroyStructure(drop, (byte)i, (byte)j, data.point);
                    }
                }
            }
            DisplayRegion();
        }
    }
}
