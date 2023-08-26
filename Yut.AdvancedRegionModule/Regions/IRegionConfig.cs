using System.Collections.Generic;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Regions
{
    public interface IRegionConfig : IConfig
    {
        /// <summary>
        /// 是否检测玩家进入区域
        /// </summary>
        bool DetectPlayers { get; }
        /// <summary>
        /// 是否检测车辆进入区域
        /// </summary>
        bool DetectVehicles { get; }
        /// <summary>
        /// 是否检测动物进入区域
        /// </summary>
        bool DetectAnimals { get; }
        /// <summary>
        /// 更新区域配置
        /// </summary>
        /// <param name="key">更新的键</param>
        /// <param name="behaviour">更新的行为</param>
        /// <param name="value">更新的值</param>
        /// <returns></returns>
        EConfigUpdateResult UpdateConfig(string key, string behaviour, string value);
        /// <summary>
        /// 将配置转为字符串列表
        /// </summary>
        List<string> ConvertToString();
    }
}
