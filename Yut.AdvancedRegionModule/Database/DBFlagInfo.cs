using SqlSugar;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Database
{
    internal sealed class DBFlagInfo : IPoolObject
    {
        [SugarColumn(IsPrimaryKey = true)]
        public long RegionId { get; set; }
        [SugarColumn(IsPrimaryKey = true, Length = 40)]
        public string FlagType { get; set; }
        [SugarColumn(ColumnDataType = "blob")]
        public byte[] Config { get; set; }

        public void Divest() { }
        public void Reset() { }
    }
}