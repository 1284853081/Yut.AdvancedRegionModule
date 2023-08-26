using System.Collections.Generic;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Flags
{
    public interface IRegionFlagConfig : IConfig
    {
        /// <summary>
        /// 修改配置
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="behaviour">行为</param>
        /// <param name="value">值</param>
        EConfigUpdateResult UpdateConfig(string key, string behaviour, string value);
        /// <summary>
        /// 将配置转为字符串列表
        /// </summary>
        List<string> ConvertToString();
    }
}
