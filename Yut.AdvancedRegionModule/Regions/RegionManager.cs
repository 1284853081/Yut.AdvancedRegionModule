using Coldairarrow.Util;
using SDG.Unturned;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Yut.AdvancedRegionModule.Database;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Regions
{
    public enum ECreateRegionResult
    {
        TryCreateWorld,
        Failed,
        Success
    }
    public enum EDestroyRegionResult
    {
        TryDestroyWorld,
        RegionNotFound,
        Failed,
        Success
    }
    public delegate void PlayerEnterRegionHandler(Player player, Region region);
    public delegate void PlayerLeaveRegionHandler(Player player, Region region);
    public delegate void VehicleEnterRegionHandler(InteractableVehicle vehicle, Region region);
    public delegate void VehicleLeaveRegionHandler(InteractableVehicle vehicle, Region region);
    public delegate void AnimalEnterRegionHandler(Animal animal, Region region);
    public delegate void AnimalLeaveRegionHandler(Animal animal, Region region);
    public delegate void RegionInitHandler(Region region);
    public delegate void RegionTypeRegisteredHandler(string regionType, bool isStatic);
    public sealed class RegionManager
    {
        private struct StructureParam
        {
            public byte[] Guid;
            public uint InstanceId;
            public ushort Health;
            public Vector3 Point;
            public byte Angle_X;
            public byte Angle_Y;
            public byte Angle_Z;
            public ulong Owner;
            public ulong Group;
            public uint ObjActiveDate;
        }
        private static RegionManager instance;
        public static RegionManager Instance
        {
            get
            {
                if (instance == null)
                    instance = new RegionManager();
                return instance;
            }
        }
        /// <summary>
        /// 静态区域配置对象池
        /// </summary>
        private readonly Dictionary<string, ObjectPoolBase<IRegionConfig>> staticConfigPools;
        /// <summary>
        /// 动态区域配置对象池
        /// </summary>
        private readonly Dictionary<string, ObjectPoolBase<IRegionConfig>> dynamicConfigPools;
        /// <summary>
        /// 静态区域对象池
        /// </summary>
        private readonly Dictionary<string, RegionPoolBase> staticRegionPools;
        /// <summary>
        /// 动态区域对象池
        /// </summary>
        private readonly Dictionary<string, RegionPoolBase> dynamicRegionPools;
        /// <summary>
        /// 静态区域
        /// </summary>
        private readonly List<Region> staticRegions;
        /// <summary>
        /// 动态区域
        /// </summary>
        private readonly List<Region> dynamicRegions;
        /// <summary>
        /// 静态区域类型集合
        /// </summary>
        private List<string> staticRegionTypes;
        /// <summary>
        /// 动态区域类型集合
        /// </summary>
        private readonly List<string> dynamicRegionTypes;
        /// <summary>
        /// 由于区域类型还未注册而未加载的区域的集合
        /// </summary>
        private readonly List<DBRegionInfo> regionsNotLoad;
        /// <summary>
        /// 加载区域的任务队列
        /// </summary>
        private readonly Queue<Task> tasks;

        /// <summary>
        /// 只读的静态区域集合
        /// </summary>
        public ReadOnlyCollection<Region> StaticRegions => staticRegions.AsReadOnly();
        /// <summary>
        /// 只读的动态区域集合
        /// </summary>
        public ReadOnlyCollection<Region> DynamicRegions => dynamicRegions.AsReadOnly();
        /// <summary>
        /// 只读的静态区域类型集合
        /// </summary>
        public ReadOnlyCollection<string> StaticRegionTypes => staticRegionTypes.AsReadOnly();
        /// <summary>
        /// 只读的动态区域类型集合
        /// </summary>
        public ReadOnlyCollection<string> DynamicRegionTypes => dynamicRegionTypes.AsReadOnly();
        /// <summary>
        /// 当玩家进入区域时触发
        /// </summary>
        public static event PlayerEnterRegionHandler OnPlayerEnterRegion;
        /// <summary>
        /// 当玩家离开区域时触发
        /// </summary>
        public static event PlayerLeaveRegionHandler OnPlayerLeaveRegion;
        /// <summary>
        /// 当载具进入区域时触发
        /// </summary>
        public static event VehicleEnterRegionHandler OnVehicleEnterRegion;
        /// <summary>
        /// 当载具离开区域时触发
        /// </summary>
        public static event VehicleLeaveRegionHandler OnVehicleLeaveRegion;
        /// <summary>
        /// 当动物进入区域时触发
        /// </summary>
        public static event AnimalEnterRegionHandler OnAnimalEnterRegion;
        /// <summary>
        /// 当动物离开区域时触发
        /// </summary>
        public static event AnimalLeaveRegionHandler OnAnimalLeaveRegion;
        /// <summary>
        /// 当区域初始化完成时触发
        /// </summary>
        public static event RegionInitHandler OnRegionInit;
        /// <summary>
        /// 当区域类型被注册时触发
        /// </summary>
        public static event RegionTypeRegisteredHandler OnRegionTypeRegistered;

        private RegionManager()
        {
            staticConfigPools = new Dictionary<string, ObjectPoolBase<IRegionConfig>>();
            dynamicConfigPools = new Dictionary<string, ObjectPoolBase<IRegionConfig>>();
            staticRegionPools = new Dictionary<string, RegionPoolBase>();
            dynamicRegionPools = new Dictionary<string, RegionPoolBase>();
            staticRegions = new List<Region>();
            dynamicRegions = new List<Region>();
            staticRegionTypes = new List<string>();
            //staticRegionTypes = DBManager.Instance.GetStaticTypes(EConfigType.Region);
            dynamicRegionTypes = new List<string>();
            regionsNotLoad = new List<DBRegionInfo>();
            tasks = new Queue<Task>();
            SaveManager.onPostSave += OnPostSave;
        }
        /// <summary>
        /// 注册指定静态区域类型
        /// </summary>
        /// <typeparam name="TRegion">需要注册的自定义区域类型</typeparam>
        /// <typeparam name="TRegionConfig">需要注册的自定义区域配置</typeparam>
        /// <param name="regionType">区域类型</param>
        public bool RegisterStaticRegionType<TRegion, TRegionConfig>(string regionType)
            where TRegion : Region<TRegionConfig>
            where TRegionConfig : IRegionConfig, new()
        {
            if (Yut.IsInit)
            {
                var flag = RegisterRegionType<TRegion, TRegionConfig>(regionType, true);
                if (flag)
                {
                    try
                    {
                        OnRegionTypeRegistered?.Invoke(regionType, true);
                    }
                    catch
                    {
                        Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionTypeRegistered");
                    }
                }
                return flag;
            }
            else
            {
                var task = new Task(() =>
                {
                    var flag = RegisterRegionType<TRegion, TRegionConfig>(regionType, true);
                    if (flag)
                    {
                        try
                        {
                            OnRegionTypeRegistered?.Invoke(regionType, true);
                        }
                        catch
                        {
                            Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionTypeRegistered");
                        }
                    }
                });
                tasks.Enqueue(task);
                return true;
            }
        }
        /// <summary>
        /// 注册指定动态区域类型
        /// </summary>
        /// <typeparam name="TRegion">需要注册的自定义区域类型</typeparam>
        /// <typeparam name="TRegionConfig">需要注册的自定义区域配置</typeparam>
        /// <param name="regionType">区域类型</param>
        public bool RegisterDynamicRegionType<TRegion, TRegionConfig>(string regionType)
            where TRegion : Region<TRegionConfig>
            where TRegionConfig : IRegionConfig, new()
        {
            if (Yut.IsInit)
            {
                var flag = RegisterRegionType<TRegion, TRegionConfig>(regionType, false);
                if (flag)
                {
                    try
                    {
                        OnRegionTypeRegistered?.Invoke(regionType, false);
                    }
                    catch
                    {
                        Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionTypeRegistered");
                    }
                }
                return flag;
            }
            else
            {
                var task = new Task(() =>
                {
                    var flag = RegisterRegionType<TRegion, TRegionConfig>(regionType, false);
                    if (flag)
                    {
                        try
                        {
                            OnRegionTypeRegistered?.Invoke(regionType, false);
                        }
                        catch
                        {
                            Rocket.Core.Logging.Logger.LogError("An error occurred while invoke OnRegionTypeRegistered");
                        }
                    }
                });
                tasks.Enqueue(task);
                return true;
            }
        }
        /// <summary>
        /// 取消注册的静态区域
        /// </summary>
        public void UnregisterStaticRegionType(string regionType)
        {
            if(Yut.IsInit)
            {
                Yut.Instance.DB.Deleteable<DBRegionInfo>().Where(x => x.RegionType == regionType).ExecuteCommandAsync();
                Yut.Instance.DB.Deleteable<DBPluginInfo>().Where(x => x.Name == regionType && x.ConfigType == EConfigType.Region).ExecuteCommandAsync();
                UnregisterRegion(regionType);
            }
            else
            {
                var task = new Task(() =>
                {
                    Yut.Instance.DB.Deleteable<DBRegionInfo>().Where(x => x.RegionType == regionType).ExecuteCommandAsync();
                    Yut.Instance.DB.Deleteable<DBPluginInfo>().Where(x => x.Name == regionType && x.ConfigType == EConfigType.Region).ExecuteCommandAsync();
                    UnregisterRegion(regionType);
                });
                tasks.Enqueue(task);
            }
            //DBManager.Instance.DeleteRegionType(regionType);
        }
        /// <summary>
        /// 取消注册的动态区域
        /// </summary>
        public void UnregisterDynamicRegionType(string regionType)
        {
            if(Yut.IsInit)
            {
                UnregisterRegion(regionType, false);
            }
            else
            {
                var task = new Task(() =>
                {
                    UnregisterRegion(regionType, false);
                });
                tasks.Enqueue(task);
            }
        }
        /// <summary>
        /// 创建静态区域
        /// </summary>
        /// <param name="regionType">区域类型</param>
        /// <param name="regionName">区域名字</param>
        /// <param name="region">创建的区域</param>
        /// <returns>创建区域的结果</returns>
        public ECreateRegionResult CreateStaticRegion(string regionType, string regionName, out Region region)
        {
            region = null;
            if (regionType == Keys.WORLD)
                return ECreateRegionResult.TryCreateWorld;
            var id = IdHelper.GetLongId();
            region = CreateRegion(regionType);
            if (region is null)
                return ECreateRegionResult.Failed;
            var count = staticRegions.Count(x => x.RegionName == regionName);
            var uniqueName = count > 0 ? regionName + count.ToString() : regionName;
            var regionInfo = Yut.Instance.GetDBRegionInfo();
            regionInfo.RegionId = id;
            regionInfo.RegionName = regionName;
            regionInfo.UniqueName = uniqueName;
            regionInfo.RegionType = regionType;
            regionInfo.Config = region.RegionConfigInternal.ConvertToBytes();
            Yut.Instance.DB.Insertable(regionInfo).ExecuteCommandAsync();
            //DBManager.Instance.InsertInfo(regionInfo);
            Yut.Instance.ReturnDBRegionInfo(regionInfo);
            region.Init(id, regionType, regionName, uniqueName, true);
            region.BindFlag(Keys.DISPLAY);
            staticRegions.Add(region);
            return ECreateRegionResult.Success;
        }
        /// <summary>
        /// 创建动态区域
        /// </summary>
        /// <param name="regionType">区域类型</param>
        /// <param name="regionName">区域名字</param>
        /// <param name="region">创建的区域</param>
        /// <returns>创建区域的结果</returns>
        public ECreateRegionResult CreateDynamicRegion(string regionType, string regionName, out Region region)
        {
            region = null;
            if (regionType == Keys.WORLD)
                return ECreateRegionResult.TryCreateWorld;
            region = CreateRegion(regionType, false);
            if (region is null)
                return ECreateRegionResult.Failed;
            var id = IdHelper.GetLongId();
            var count = dynamicRegions.Count(x => x.RegionName == regionName);
            var uniqueName = count > 0 ? regionName + count.ToString() : regionName;
            region.Init(id, regionType, regionName, uniqueName, true, false);
            region.BindFlag(Keys.DISPLAY);
            dynamicRegions.Add(region);
            return ECreateRegionResult.Success;
        }
        /// <summary>
        /// 根据区域ID销毁静态区域
        /// </summary>
        /// <param name="regionId">区域ID</param>
        public EDestroyRegionResult DestroyStaticRegion(long regionId)
        {
            if (regionId <= 0)
                return EDestroyRegionResult.TryDestroyWorld;
            var index = staticRegions.FindIndex(x => x.RegionID == regionId);
            if (index < 0)
                return EDestroyRegionResult.RegionNotFound;
            var region = staticRegions[index];
            if (region is null)
                return EDestroyRegionResult.Failed;
            region.Destroy();
            DestroyRegion(region, index);
            return EDestroyRegionResult.Success;
        }
        /// <summary>
        /// 根据区域实例销毁静态区域
        /// </summary>
        /// <param name="region">区域实例</param>
        public EDestroyRegionResult DestroyStaticRegion(Region region)
        {
            if (region.RegionID <= 0)
                return EDestroyRegionResult.TryDestroyWorld;
            region.Destroy();
            ReturnRegionToPool(region, true);
            staticRegions.Remove(region);
            return EDestroyRegionResult.Success;
        }
        /// <summary>
        /// 根据区域ID销毁动态区域
        /// </summary>
        /// <param name="regionId">区域ID</param>
        public EDestroyRegionResult DestroyDynamicRegion(long regionId)
        {
            if (regionId <= 0)
                return EDestroyRegionResult.TryDestroyWorld;
            var index = dynamicRegions.FindIndex(x => x.RegionID == regionId);
            if (index < 0)
                return EDestroyRegionResult.RegionNotFound;
            var region = dynamicRegions[index];
            if (region is null)
                return EDestroyRegionResult.Failed;
            region.Destroy();
            DestroyRegion(region, index, false);
            return EDestroyRegionResult.Success;
        }
        /// <summary>
        /// 根据区域实例销毁动态区域
        /// </summary>
        /// <param name="region">区域实例</param>
        public EDestroyRegionResult DestroyDynamicRegion(Region region)
        {
            if (region.RegionID <= 0)
                return EDestroyRegionResult.TryDestroyWorld;
            region.Destroy();
            ReturnRegionToPool(region, false);
            dynamicRegions.Remove(region);
            return EDestroyRegionResult.Success;
        }
        /// <summary>
        /// 根据区域Id查找指定区域，若未找到则为空
        /// </summary>
        /// <param name="regionId">区域id</param>
        /// <param name="isStatic">是否是静态</param>
        public Region FindRegion(long regionId, bool isStatic)
        {
            var regions = isStatic ? staticRegions : dynamicRegions;
            return regions.Find(x => x.RegionID == regionId);
        }
        /// <summary>
        /// 根据区域Id查找指定区域，若未找到则为空
        /// </summary>
        /// <param name="regionId">区域id</param>
        /// <param name="isStatic">是否是静态</param>
        public Region FindRegion(string regionId, bool isStatic)
        {
            if (long.TryParse(regionId, out var id))
                return FindRegion(id, isStatic);
            return null;
        }
        /// <summary>
        /// 查找具有指定名字的所有区域
        /// </summary>
        public ReadOnlyCollection<Region> FindRegions(string regionName, bool isStatic)
        {
            var list = new List<Region>();
            var regions = isStatic ? staticRegions : dynamicRegions;
            for (int i = 0; i < regions.Count; i++)
            {
                var region = regions[i];
                if (region.RegionName == regionName)
                    list.Add(region);
            }
            return list.AsReadOnly();
        }
        /// <summary>
        /// 根据区域唯一名字查找区域
        /// </summary>
        public Region TryFindRegion(string unique, bool isStatic)
        {
            var region = FindRegion(unique, isStatic);
            if (region is null)
            {
                var regions = isStatic ? staticRegions : dynamicRegions;
                region = regions.Find(x => x.UniqueName == unique);
            }
            return region;
        }
        /// <summary>
        /// 获得某一点的所有区域
        /// </summary>
        public ReadOnlyCollection<Region> GetRegionsAtPoint(Vector3 point, bool isStatic)
        {
            var list = new List<Region>();
            var regions = isStatic ? staticRegions : dynamicRegions;
            var count = regions.Count;
            for (var i = 0; i < count; i++)
            {
                var region = regions[i];
                if (region.InRegion(point))
                    list.Add(region);
            }
            return list.AsReadOnly();
        }
        /// <summary>
        /// 获得当前玩家所在的所有区域
        /// </summary>
        public ReadOnlyCollection<Region> GetRegionsForPlayer(Player player, bool isStatic)
            => GetRegionsAtPoint(player.transform.position, isStatic);
        /// <summary>
        /// 在指定点是否有区域
        /// </summary>
        /// <param name="point">指定的点</param>
        /// <param name="isStatic">是否静态</param>
        /// <param name="ignoreWorld">是否无视世界区域</param>
        public bool HasRegionAtPoint(Vector3 point, bool isStatic, bool ignoreWorld = true)
        {
            var regions = isStatic ? staticRegions : dynamicRegions;
            var count = regions.Count;
            for (var i = 0; i < count; i++)
            {
                var region = regions[i];
                if (ignoreWorld && region.RegionName == Keys.WORLD)
                    continue;
                if (region.InRegion(point))
                    return true;
            }
            return false;
        }
        /// <summary>
        /// 在指定点是否有指定标记的区域
        /// </summary>
        /// <param name="point">指定的点</param>
        /// <param name="isStatic">是否静态</param>
        /// <param name="flagType">标记类型</param>
        public bool HasRegionWithFlagAtPoint(Vector3 point, bool isStatic, string flagType)
        {
            var regions = isStatic ? staticRegions : dynamicRegions;
            var count = regions.Count;
            for (var i = 0; i < count; i++)
            {
                var region = regions[i];
                if (region.InRegion(point) && region.HasFlag(flagType))
                    return true;
            }
            return false;
        }

        internal void InvokePlayerEnterRegion(Player player, Region region)
        {
            try
            {
                OnPlayerEnterRegion?.Invoke(player, region);
            }
            catch { }
        }
        internal void InvokePlayerLeaveRegion(Player player, Region region)
        {
            try
            {
                OnPlayerLeaveRegion?.Invoke(player, region);
            }
            catch { }
        }
        internal void InvokeVehicleEnterRegion(InteractableVehicle vehicle, Region region)
        {
            try
            {
                OnVehicleEnterRegion?.Invoke(vehicle, region);
            }
            catch { }
        }
        internal void InvokeVehicleLeaveRegion(InteractableVehicle vehicle, Region region)
        {
            try
            {
                OnVehicleLeaveRegion?.Invoke(vehicle, region);
            }
            catch { }
        }
        internal void InvokeAnimalEnterRegion(Animal animal, Region region)
        {
            try
            {
                OnAnimalEnterRegion?.Invoke(animal, region);
            }
            catch { }
        }
        internal void InvokeAnimalLeaveRegion(Animal animal, Region region)
        {
            try
            {
                OnAnimalLeaveRegion?.Invoke(animal, region);
            }
            catch { }
        }
        internal void GenerateRegions()
        {
            //var list = DBManager.Instance.GetRegionInfos();
            var list = Yut.Instance.DB.Queryable<DBRegionInfo>().ToList();
            var count = list.Count;
            var flag = true;
            for (int i = 0; i < count; i++)
            {
                var info = list[i];
                if (info.RegionName == Keys.WORLD_NAME)
                    flag = false;
                if (InitRegion(info.RegionId, info.RegionType, info.RegionName, info.UniqueName, info.Config))
                    Yut.Instance.ReturnDBRegionInfo(info);
                //DBManager.Instance.ReturnDBRegionInfo(info);
                else
                    regionsNotLoad.Add(info);
            }
            if (flag)
                CreateWorldRegion();
        }
        internal void OnDBInit()
        {
            //staticRegionTypes = DBManager.Instance.GetStaticTypes(EConfigType.Region);
            staticRegionTypes = Yut.Instance.DB.Queryable<DBPluginInfo>().Where(x => x.ConfigType == EConfigType.Region).Select(x => x.Name).ToList();
            while (tasks.Count > 0)
            {
                var task = tasks.Dequeue();
                task.Start();
            }
        }

        private void CreateWorldRegion()
        {
            var region = CreateRegion(Keys.WORLD);
            var regionInfo = Yut.Instance.GetDBRegionInfo();
            regionInfo.RegionName = Keys.WORLD_NAME;
            regionInfo.UniqueName = Keys.WORLD_NAME;
            regionInfo.RegionType = Keys.WORLD;
            regionInfo.RegionId = Keys.WORLDID;
            regionInfo.Config = region.RegionConfigInternal.ConvertToBytes();
            Yut.Instance.DB.Insertable(regionInfo).ExecuteCommandAsync();
            //DBManager.Instance.InsertInfo(regionInfo);
            Yut.Instance.ReturnDBRegionInfo(regionInfo);
            region.Init(Keys.WORLDID, Keys.WORLD, Keys.WORLD_NAME, Keys.WORLD_NAME, true);
            region.BindFlag(Keys.DISPLAY);
            staticRegions.Add(region);
        }
        private bool RegisterRegionType<TRegion, TRegionConfig>(string regionType, bool isStatic)
            where TRegion : Region<TRegionConfig>
            where TRegionConfig : IRegionConfig, new()
        {
            if (isStatic)
            {
                if (staticRegionPools.ContainsKey(regionType))
                    return false;
            }
            else
            {
                if (dynamicRegionTypes.Contains(regionType))
                    return false;
            }
            var configPool = new ObjectPool<TRegionConfig, IRegionConfig>(1, 20);
            var regionPool = new RegionPool<TRegion>(1, 20);
            (isStatic ? staticRegionPools : dynamicRegionPools).Add(regionType, regionPool);
            (isStatic ? staticConfigPools : dynamicConfigPools).Add(regionType, configPool);
            if (isStatic)
            {
                if (!staticRegionTypes.Contains(regionType))
                {
                    staticRegionTypes.Add(regionType);
                    var pluginInfo = Yut.Instance.GetDBPluginInfo();//DBManager.Instance.GetDBPluginInfo();
                    pluginInfo.Name = regionType;
                    pluginInfo.ConfigType = EConfigType.Region;
                    Yut.Instance.DB.Insertable(pluginInfo).ExecuteCommandAsync();
                    Yut.Instance.ReturnDBPluginInfo(pluginInfo);
                    //DBManager.Instance.InsertInfo(pluginInfo);
                    //DBManager.Instance.ReturnDBPluginInfo(pluginInfo);
                }
                var count = regionsNotLoad.Count;
                for (var i = count - 1; i >= 0; i--)
                {
                    var info = regionsNotLoad[i];
                    if (info.RegionType == regionType)
                    {
                        InitRegion(info.RegionId, info.RegionType, info.RegionName, info.UniqueName, info.Config);
                        regionsNotLoad.RemoveAt(i);
                    }
                }
            }
            else
                dynamicRegionTypes.Add(regionType);
            return true;
        }
        private void UnregisterRegion(string regionType, bool isStatic = true)
        {
            var list = isStatic ? staticRegions : dynamicRegions;
            var count = list.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                var region = list[i];
                if (region.RegionType == regionType)
                {
                    region.Destroy();
                    ReturnRegionToPool(region, isStatic);
                    list.RemoveAt(i);
                }
            }
            (isStatic ? staticRegionPools : dynamicRegionPools).Remove(regionType);
            (isStatic ? staticConfigPools : dynamicConfigPools).Remove(regionType);
            (isStatic ? staticRegionTypes : dynamicRegionTypes).Remove(regionType);
            if (isStatic)
            {
                var count1 = regionsNotLoad.Count;
                for (int i = count1 - 1; i >= 0; i--)
                {
                    var regions = regionsNotLoad[i];
                    if (regions.RegionType == regionType)
                        regionsNotLoad.RemoveAt(i);
                }
            }
        }
        private bool InitRegion(long regionId, string regionType, string regionName, string uniqueName, byte[] config, bool isCreate = false, bool isStatic = true)
        {
            var region = CreateRegion(regionType, isStatic);
            if (region is null)
                return false;
            region.RegionConfigInternal.ConvertFromBytes(config);
            region.Init(regionId, regionType, regionName, uniqueName, isCreate, isStatic);
            (isStatic ? staticRegions : dynamicRegions).Add(region);
            try
            {
                OnRegionInit?.Invoke(region);
            }
            catch
            {
                Rocket.Core.Logging.Logger.LogError("Error occurred during initialization of docking area");
            }
            return true;
        }
        private Region CreateRegion(string regionType, bool isStatic = true)
        {
            var regionPools = isStatic ? staticRegionPools : dynamicRegionPools;
            var configPools = isStatic ? staticConfigPools : dynamicConfigPools;
            if (regionPools.TryGetValue(regionType, out var regionPool) &&
               configPools.TryGetValue(regionType, out var configPool))
            {
                var region = regionPool.GetFromPool();
                var config = configPool.GetFromPool();
                if (region is null || config is null)
                    return null;
                var newRegion = region.BindConfig(config);
                if (newRegion is null)
                {
                    configPool.ReturnToPool(config);
                    regionPool.ReturnToPool(region);
                }
                return newRegion;
            }
            return null;
        }
        private void DestroyRegion(Region region, int index, bool isStatic = true)
        {
            ReturnRegionToPool(region, isStatic);
            (isStatic ? staticRegions : dynamicRegions).RemoveAt(index);
        }
        private void ReturnRegionToPool(Region region, bool isStatic)
        {
            var regionPools = isStatic ? staticRegionPools : dynamicRegionPools;
            var configPools = isStatic ? staticConfigPools : dynamicConfigPools;
            if (regionPools.TryGetValue(region.RegionType, out var regionPool) &&
                configPools.TryGetValue(region.RegionType, out var configPool))
            {
                regionPool.ReturnToPool(region);
                var config = region.RegionConfigInternal;
                if (config is null)
                    return;
                else
                    configPool.ReturnToPool(config);
            }
            else
                Object.Destroy(region.gameObject);
        }
        private void OnPostSave()
        {
            var idsDic = new Dictionary<ulong, HashSet<ulong>>();
            var count = dynamicRegions.Count;
            for (var i = 0; i < count; i++)
            {
                var regionid = dynamicRegions[i].RegionID;
                var owner = (ulong)(regionid & 0xFFFFFFFF);
                var group = (ulong)(regionid >> 32);
                if (idsDic.TryGetValue(owner, out var ids))
                {
                    ids.Add(group);
                }
                else
                {
                    var hashset = new HashSet<ulong>
                    {
                        group
                    };
                    idsDic.Add(owner, hashset);
                }
            }
            var path = ReadWrite.PATH + ServerSavedata.directory;
            path = Path.Combine(path, Provider.serverID, "Level", Level.info.name, "Structures.dat");
            var bytes = File.ReadAllBytes(path);
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var river = LevelSavedata.openRiver("/Structures.dat", false);
                    var list = new List<StructureParam>();
                    river.writeByte(reader.ReadByte());
                    river.writeUInt32(reader.ReadUInt32());
                    river.writeUInt32(reader.ReadUInt32());
                    for (byte i = 0; i < SDG.Unturned.Regions.WORLD_SIZE; i++)
                    {
                        for (byte j = 0; j < SDG.Unturned.Regions.WORLD_SIZE; j++)
                        {
                            var num = reader.ReadUInt16();
                            list.Clear();
                            for (var k = 0; k < num; k++)
                            {
                                var param = new StructureParam();
                                var guidLen = reader.ReadUInt16();
                                param.Guid = reader.ReadBytes(16);
                                param.InstanceId = reader.ReadUInt32();
                                param.Health = reader.ReadUInt16();
                                var x = reader.ReadSingle();
                                var y = reader.ReadSingle();
                                var z = reader.ReadSingle();
                                param.Point = new Vector3(x, y, z);
                                param.Angle_X = reader.ReadByte();
                                param.Angle_Y = reader.ReadByte();
                                param.Angle_Z = reader.ReadByte();
                                param.Owner = reader.ReadUInt64();
                                param.Group = reader.ReadUInt64();
                                param.ObjActiveDate = reader.ReadUInt32();
                                var flag = idsDic.TryGetValue(param.Owner, out var ids) && ids.Contains(param.Group);
                                if (!flag)
                                    list.Add(param);
                            }
                            river.writeUInt16((ushort)list.Count);
                            for (var k = 0; k < list.Count; k++)
                            {
                                var param = list[k];
                                river.writeBytes(param.Guid);
                                river.writeUInt32(param.InstanceId);
                                river.writeUInt16(param.Health);
                                river.writeSingleVector3(param.Point);
                                river.writeByte(param.Angle_X);
                                river.writeByte(param.Angle_Y);
                                river.writeByte(param.Angle_Z);
                                river.writeUInt64(param.Owner);
                                river.writeUInt64(param.Group);
                                river.writeUInt32(param.ObjActiveDate);
                            }
                        }
                    }
                    river.closeRiver();
                }
            }
        }
    }
}