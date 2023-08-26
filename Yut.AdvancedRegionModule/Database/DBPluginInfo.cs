using SqlSugar;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Database
{
    internal sealed class DBPluginInfo : IPoolObject
    {
        [SugarColumn(IsPrimaryKey = true, Length = 40)]
        public string Name { get; set; }
        [SugarColumn(IsPrimaryKey = true)]
        public EConfigType ConfigType { get; set; }


        public void Divest() { }
        public void Reset() { }
    }
}
