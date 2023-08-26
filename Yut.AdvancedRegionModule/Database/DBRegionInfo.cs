using SqlSugar;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Database
{
    internal sealed class DBRegionInfo : IPoolObject
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long RegionId { get; set; }
        [SugarColumn(Length = 40)]
        public string RegionName { get; set; }
        [SugarColumn(Length = 42)]
        public string UniqueName { get; set; }
        [SugarColumn(Length = 20)]
        public string RegionType { get; set; }
        [SugarColumn(ColumnDataType = "blob")]
        public byte[] Config { get; set; }
        public void Divest() { }
        public void Reset() { }
    }
}

