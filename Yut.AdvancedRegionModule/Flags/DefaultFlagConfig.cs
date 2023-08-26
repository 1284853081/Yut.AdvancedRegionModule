using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yut.AdvancedRegionModule.Flags
{
    /// <summary>
    /// 默认的标记配置
    /// </summary>
    public sealed class DefaultFlagConfig : IRegionFlagConfig
    {
        public void ConvertFromBytes(byte[] bytes) { }
        public byte[] ConvertToBytes() => Array.Empty<byte>();
        public List<string> ConvertToString() => new List<string>();
        public void Divest() { }
        public void Reset() { }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value) => EConfigUpdateResult.RejectUpdate;
    }
}
