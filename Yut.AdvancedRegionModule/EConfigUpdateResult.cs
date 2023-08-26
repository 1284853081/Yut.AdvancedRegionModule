namespace Yut.AdvancedRegionModule
{
    public enum EConfigUpdateResult
    {
        /// <summary>
        /// 修改配置成功
        /// </summary>
        Success,
        /// <summary>
        /// 未定义的键
        /// </summary>
        UndefinedKey,
        /// <summary>
        /// 不支持的修改行为
        /// </summary>
        BehaviourNotSupport,
        /// <summary>
        /// 未定义的行为
        /// </summary>
        UndefinedBehaviour,
        /// <summary>
        /// 无效的值
        /// </summary>
        InvalidValue,
        /// <summary>
        /// 拒绝修改
        /// </summary>
        RejectUpdate
    }
}
