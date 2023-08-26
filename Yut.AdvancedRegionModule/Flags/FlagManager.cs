using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Yut.AdvancedRegionModule.Database;
using Yut.AdvancedRegionModule.Regions;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Flags
{
    public delegate void RegionFlagTypeRegisteredHandler(string flagType, bool isStatic);
    public sealed class FlagManager
    {
        private static FlagManager instance;
        public static FlagManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new FlagManager();
                return instance;
            }
        }
        private readonly ObjectDictionary<string> staticRegionFlagPools;
        private readonly ObjectDictionary<string> dynamicRegionFlagPools;
        private List<string> staticFlagTypes;
        private readonly List<string> dynamicFlagTypes;
        private readonly Queue<Task> registerTasks;
        /// <summary>
        /// 当区域标记注册时触发
        /// </summary>
        public static event RegionFlagTypeRegisteredHandler OnRegionFlagTypeRegistered;

        /// <summary>
        /// 只读的静态标记列表
        /// </summary>
        public ReadOnlyCollection<string> StaticFlagTypes => staticFlagTypes.AsReadOnly();
        /// <summary>
        /// 只读的动态标记列表
        /// </summary>
        public ReadOnlyCollection<string> DynamicFlagTypes => dynamicFlagTypes.AsReadOnly();
        private FlagManager()
        {
            staticRegionFlagPools = new ObjectDictionary<string>(1, 20);
            dynamicRegionFlagPools = new ObjectDictionary<string>(1, 20);
            staticFlagTypes = new List<string>();
            //staticFlagTypes = DBManager.Instance.GetStaticTypes(EConfigType.RegionFlag);
            dynamicFlagTypes = new List<string>();
            registerTasks = new Queue<Task>();
        }
        /// <summary>
        /// 注册静态标记
        /// </summary>
        public bool RegisterStaticFlagType<TRegionFlag, TRegionFlagType>(string flagType)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
        {
            if (Yut.IsInit)
            {
                var flag = RegisterFlagType<TRegionFlag, TRegionFlagType>(flagType, true);
                if (flag)
                {
                    try
                    {
                        OnRegionFlagTypeRegistered?.Invoke(flagType, true);
                    }
                    catch
                    {
                        Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionFlagTypeRegistered");
                    }
                }
                return flag;
            }
            else
            {
                var task = new Task(() =>
                {
                    var flag = RegisterFlagType<TRegionFlag, TRegionFlagType>(flagType, true);
                    if (flag)
                    {
                        try
                        {
                            OnRegionFlagTypeRegistered?.Invoke(flagType, true);
                        }
                        catch
                        {
                            Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionFlagTypeRegistered");
                        }
                    }
                });
                registerTasks.Enqueue(task);
                return true;
            }
        }
        /// <summary>
        /// 注册动态标记
        /// </summary>
        public bool RegisterDynamicFlagType<TRegionFlag, TRegionFlagType>(string flagType)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
        {
            if (Yut.IsInit)
            {
                var flag = RegisterFlagType<TRegionFlag, TRegionFlagType>(flagType, false);
                if (flag)
                {
                    try
                    {
                        OnRegionFlagTypeRegistered?.Invoke(flagType, false);
                    }
                    catch
                    {
                        Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionFlagTypeRegistered");
                    }
                }
                return flag;
            }
            else
            {
                var task = new Task(() =>
                {
                    var flag = RegisterFlagType<TRegionFlag, TRegionFlagType>(flagType, false);
                    if (flag)
                    {
                        try
                        {
                            OnRegionFlagTypeRegistered?.Invoke(flagType, false);
                        }
                        catch
                        {
                            Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionFlagTypeRegistered");
                        }
                    }
                });
                registerTasks.Enqueue(task);
                return true;
            }
        }
        /// <summary>
        /// 取消注册静态标记
        /// </summary>
        public bool UnregisterStaticFlagType<TRegionFlag, TRegionFlagType>(string flagType)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
        {
            if (!Yut.IsInit)
                return false;
            var flag = UnregisterFlagType<TRegionFlag, TRegionFlagType>(flagType, true);
            if (flag)
            {
                Yut.Instance.DB.Deleteable<DBFlagInfo>().Where(x => x.FlagType == flagType).ExecuteCommandAsync();
                Yut.Instance.DB.Deleteable<DBPluginInfo>().Where(x => x.Name == flagType && x.ConfigType == EConfigType.RegionFlag).ExecuteCommandAsync();
            }
            //DBManager.Instance.DeleteRegionFlagType(flagType);
            return flag;
        }
        /// <summary>
        /// 取消注册动态标记
        /// </summary>
        public bool UnregisterDynamicFlagType<TRegionFlag, TRegionFlagType>(string flagType)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
            => UnregisterFlagType<TRegionFlag, TRegionFlagType>(flagType, false);

        internal RegionFlag GetRegionFlag(string flagType, bool isStatic = true)
        {
            var pool = isStatic ? staticRegionFlagPools : dynamicRegionFlagPools;
            var flag = pool.Get(flagType);
            if (flag is RegionFlag result)
                return result;
            else
            {
                pool.Return(flagType, flag);
                return null;
            }
        }
        internal void ReturnRegionFlag(RegionFlag flag, bool isStatic = true)
        {
            var pool = isStatic ? staticRegionFlagPools : dynamicRegionFlagPools;
            pool.Return(flag.FlagType, flag);
        }
        internal void OnDBInit()
        {
            staticFlagTypes = Yut.Instance.DB.Queryable<DBPluginInfo>().Where(x => x.ConfigType == EConfigType.RegionFlag).Select(x => x.Name).ToList();
            //staticFlagTypes = DBManager.Instance.GetStaticTypes(EConfigType.RegionFlag);
            while (registerTasks.Count > 0)
            {
                var task = registerTasks.Dequeue();
                task.Start();
            }
        }

        private bool RegisterFlagType<TRegionFlag, TRegionFlagType>(string flagType, bool isStatic)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
        {
            var pool = isStatic ? staticRegionFlagPools : dynamicRegionFlagPools;
            var flag = pool.Register<TRegionFlag, TRegionFlagType>(flagType);
            if (!flag)
                return false;
            if (isStatic)
            {
                if (!staticFlagTypes.Contains(flagType))
                {
                    staticFlagTypes.Add(flagType);
                    var pluginInfo = Yut.Instance.GetDBPluginInfo();
                    pluginInfo.Name = flagType;
                    pluginInfo.ConfigType = EConfigType.RegionFlag;
                    Yut.Instance.DB.Insertable(pluginInfo).ExecuteCommandAsync();
                    //DBManager.Instance.InsertInfo(pluginInfo);
                    Yut.Instance.ReturnDBPluginInfo(pluginInfo);
                }
                var count = RegionManager.Instance.StaticRegions.Count;
                for (var i = 0; i < count; i++)
                {
                    var region = RegionManager.Instance.StaticRegions[i];
                    region.OnFlagRegister(flagType);
                }
            }
            else
                dynamicFlagTypes.Add(flagType);
            //(isStatic ? staticFlagTypes : dynamicFlagTypes).Add(flagType);
            return true;
        }
        private bool UnregisterFlagType<TRegionFlag, TRegionFlagType>(string flagType, bool isStatic)
            where TRegionFlag : RegionFlag<TRegionFlagType>, new()
            where TRegionFlagType : IRegionFlagConfig, new()
        {
            var pool = isStatic ? staticRegionFlagPools : dynamicRegionFlagPools;
            var result = pool.Unregister(flagType);
            if (result)
            {
                (isStatic ? staticFlagTypes : dynamicFlagTypes).Remove(flagType);
                if (isStatic)
                {
                    var count = RegionManager.Instance.StaticRegions.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var region = RegionManager.Instance.StaticRegions[i];
                        region.UnbindFlag(flagType);
                    }
                }
                else
                {
                    var count = RegionManager.Instance.DynamicRegions.Count;
                    for (var i = 0; i < count; i++)
                    {
                        var region = RegionManager.Instance.DynamicRegions[i];
                        region.UnbindFlag(flagType);
                    }
                }
                return true;
            }
            return false;
        }
    }
}
