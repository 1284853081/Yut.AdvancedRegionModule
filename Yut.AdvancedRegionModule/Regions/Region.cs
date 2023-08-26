using Rocket.Unturned;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using UnityEngine;
using Yut.AdvancedRegionModule.Database;
using Yut.AdvancedRegionModule.Flags;
using Yut.UnturnedEx.Extensions;

namespace Yut.AdvancedRegionModule.Regions
{
    public abstract class Region : MonoBehaviour
    {
        /// <summary>
        /// 区域ID
        /// </summary>
        public abstract long RegionID { get; }
        /// <summary>
        /// 区域的名字
        /// </summary>
        public abstract string RegionName { get; }
        /// <summary>
        /// 区域的唯一名字
        /// </summary>
        public abstract string UniqueName { get; }
        /// <summary>
        /// 区域类型
        /// </summary>
        public abstract string RegionType { get; }
        /// <summary>
        /// 是否为静态区域
        /// </summary>
        public abstract bool IsStatic { get; }
        /// <summary>
        /// 是否检测玩家进入区域
        /// </summary>
        public abstract bool DetectPlayers { get; set; }
        /// <summary>
        /// 是否检测车辆进入区域
        /// </summary>
        public abstract bool DetectVehicles { get; set; }
        /// <summary>
        /// 是否检测动物进入区域
        /// </summary>
        public abstract bool DetectAnimals { get; set; }
        /// <summary>
        /// 是否显示区域边界
        /// </summary>
        public abstract bool Display { get; set; }
        /// <summary>
        /// 显示边界的Flag
        /// </summary>
        public abstract DisplayFlag DisplayFlag { get; }
        /// <summary>
        /// 区域内的玩家
        /// </summary>
        public abstract ReadOnlyCollection<Player> Players { get; }
        /// <summary>
        /// 区域内的载具
        /// </summary>
        public abstract ReadOnlyCollection<InteractableVehicle> Vehicles { get; }
        /// <summary>
        /// 区域内的动物
        /// </summary>
        public abstract ReadOnlyCollection<Animal> Animals { get; }
        /// <summary>
        /// 区域的标记
        /// </summary>
        public abstract ReadOnlyCollection<RegionFlag> Flags { get; }
        /// <summary>
        /// 判断点是否在区域内
        /// </summary>
        public abstract bool InRegion(Vector3 vector);
        /// <summary>
        /// 绑定区域标记
        /// </summary>
        public abstract bool BindFlag(string flagType);
        /// <summary>
        /// 解绑区域标记
        /// </summary>
        public abstract bool UnbindFlag(string flagType);
        /// <summary>
        /// 绑定区域标记并返回标记
        /// </summary>
        public abstract bool BindFlag(string flagType, out RegionFlag flag);
        /// <summary>
        /// 是否有某一种标记
        /// </summary>
        public abstract bool HasFlag(string flagType);
        /// <summary>
        /// 解绑所有标记
        /// </summary>
        public abstract void DestroyAllFlags();
        /// <summary>
        /// 获取指定类型标记
        /// </summary>
        public abstract RegionFlag GetFlag(string flagType);
        /// <summary>
        /// 尝试获取指定类型标记
        /// </summary>
        public abstract bool TryGetFlag<TRegionFlag, TRegionFlagConfig>(string flagType,out TRegionFlag flag)
            where TRegionFlag : RegionFlag<TRegionFlagConfig>
            where TRegionFlagConfig : IRegionFlagConfig;
        /// <summary>
        /// 修改区域配置
        /// </summary>
        /// <param name="key">指定键</param>
        /// <param name="behaviour">指定行为</param>
        /// <param name="value">指定值</param>
        /// <returns>修改配置的结果</returns>
        public abstract EConfigUpdateResult UpdateConfig(string key, string behaviour, string value);
        /// <summary>
        /// 获得覆盖区域的矩形
        /// </summary>
        /// <param name="bottomLeftVertex">覆盖矩形的左下角</param>
        /// <param name="topRightVertex">覆盖矩形的右上角</param>
        public abstract void GetCoverRectangle(out Vector3 bottomLeftVertex, out Vector3 topRightVertex);
        /// <summary>
        /// 获得区域的边界点
        /// </summary>
        public abstract List<Vector3> GetDisplayPoints();
        /// <summary>
        /// 获得区域内随机的一点
        /// </summary>
        public abstract Vector3 RandomPointInRegion();

        /// <summary>
        /// 区域配置
        /// </summary>
        internal abstract IRegionConfig RegionConfigInternal { get; }
        /// <summary>
        /// 当玩家进入区域时触发
        /// </summary>
        internal abstract event Action<Player> OnPlayerEnter;
        /// <summary>
        /// 当玩家离开区域时触发
        /// </summary>
        internal abstract event Action<Player> OnPlayerLeave;
        /// <summary>
        /// 当载具进入区域时触发
        /// </summary>
        internal abstract event Action<InteractableVehicle> OnVehicleEnter;
        /// <summary>
        /// 当载具离开区域时触发
        /// </summary>
        internal abstract event Action<InteractableVehicle> OnVehicleLeave;
        /// <summary>
        /// 当动物进入区域时触发
        /// </summary>
        internal abstract event Action<Animal> OnAnimalEnter;
        /// <summary>
        /// 当动物离开区域时触发
        /// </summary>
        internal abstract event Action<Animal> OnAnimalLeave;
        /// <summary>
        /// 当区域配置将要更改时触发
        /// </summary>
        internal abstract event System.Action OnRegionPreChange;
        /// <summary>
        /// 当区域配置更改后触发
        /// </summary>
        internal abstract event System.Action OnRegionChanged;
        /// <summary>
        /// 绑定配置
        /// </summary>
        internal abstract Region BindConfig(IRegionConfig config);
        /// <summary>
        /// 销毁区域
        /// </summary>
        internal abstract void Destroy();
        /// <summary>
        /// 当区域标记绑定时触发此方法
        /// </summary>
        internal abstract void OnFlagRegister(string flagType);
        /// <summary>
        /// 绑定区域显示标记
        /// </summary>
        internal abstract void SetDisplayFlag(DisplayFlag flag);
        /// <summary>
        /// 初始化区域
        /// </summary>
        /// <param name="regionId">区域id</param>
        /// <param name="regionType">区域类型</param>
        /// <param name="regionName">区域名字</param>
        /// <param name="uniqueName">区域唯一名字</param>
        /// <param name="isCreate">是否是被创建</param>
        /// <param name="isStatic">是否是静态的</param>
        internal abstract void Init(long regionId, string regionType, string regionName, string uniqueName, bool isCreate = false, bool isStatic = true);

        /// <summary>
        /// 触发区域改变事件
        /// </summary>
        protected abstract void InvokeRegionChange();
    }
    public abstract class Region<T> : Region
        where T : IRegionConfig
    {
        private class PlayerComparer : IComparer<Player>
        {
            public int Compare(Player x, Player y)
            {
                var xid = x.GetCSteamID();
                var yxid = y.GetCSteamID();
                return xid.CompareTo(yxid);
            }
        }
        private class VehivleComparer : IComparer<InteractableVehicle>
        {
            public int Compare(InteractableVehicle x, InteractableVehicle y)
            {
                if (x is null || y is null)
                    return 0;
                return x.instanceID.CompareTo(y.instanceID);
            }
        }
        private class AnimalComparer : IComparer<Animal>
        {
            public int Compare(Animal x, Animal y)
            {
                if (x is null || y is null)
                    return 0;
                return x.index.CompareTo(y.index);
            }
        }
        private bool isInit;
        private bool isStatic;
        private bool lastDetectPlayers;
        private bool lastDetectVehicles;
        private bool lastDetectAnimals;
        private string regionName;
        private string uniqueName;
        private string regionType;
        private long regionId;
        private T config;
        private DisplayFlag displayFlag;
        private List<RegionFlag> flags;
        private List<DBFlagInfo> flagsNotLoad;
        private List<Vector3> emptyVectors;
        private PlayerComparer playerComparer;
        private VehivleComparer vehivleComparer;
        private AnimalComparer animalComparer;
        private Queue<Player> playerQueue;
        private Queue<InteractableVehicle> vehicleQueue;
        private Queue<Animal> animalQueue;
        private List<Player> players;
        private List<InteractableVehicle> vehicles;
        private List<Animal> animals;

        public sealed override string RegionName => regionName;
        public sealed override string UniqueName => uniqueName;
        public sealed override string RegionType => regionType;
        public sealed override long RegionID => regionId;
        public sealed override bool IsStatic => isStatic;
        public sealed override bool DetectPlayers
        {
            get => config.DetectPlayers;
            set => UpdateConfig(Keys.DetectPlayers, Keys.SET, value ? bool.TrueString : bool.FalseString);
        }
        public sealed override bool DetectVehicles
        {
            get => config.DetectVehicles;
            set => UpdateConfig(Keys.DetectVehicles, Keys.SET, value ? bool.TrueString : bool.FalseString);
        }
        public sealed override bool DetectAnimals
        {
            get => config.DetectAnimals;
            set => UpdateConfig(Keys.DetectAnimals, Keys.SET, value ? bool.TrueString : bool.FalseString);
        }
        public sealed override bool Display 
        {
            get => displayFlag?.Display ?? false;
            set
            {
                if (displayFlag is null)
                    return;
                displayFlag.Display = value;
            }
        }
        public sealed override DisplayFlag DisplayFlag => displayFlag;
        public sealed override ReadOnlyCollection<Player> Players => players.AsReadOnly();
        public sealed override ReadOnlyCollection<InteractableVehicle> Vehicles => vehicles.AsReadOnly();
        public sealed override ReadOnlyCollection<Animal> Animals => animals.AsReadOnly();
        public sealed override ReadOnlyCollection<RegionFlag> Flags => flags.AsReadOnly();
        public sealed override bool BindFlag(string flagType)
            => BindFlag(flagType, out _);
        public sealed override bool BindFlag(string flagType, out RegionFlag flag)
        {
            flag = null;
            if (HasFlag(flagType))
                return false;
            flag = FlagManager.Instance.GetRegionFlag(flagType, isStatic);
            if (flag is null)
                return false;
            if (isStatic)
            {
                var flagInfo = Yut.Instance.GetDBFlagInfo();
                flagInfo.RegionId = regionId;
                flagInfo.FlagType = flagType;
                flagInfo.Config = flag.Config.ConvertToBytes();
                Yut.Instance.DB.Insertable(flagInfo).ExecuteCommandAsync();
                //DBManager.Instance.InsertInfo(flagInfo);
                Yut.Instance.ReturnDBFlagInfo(flagInfo);
            }
            flag.Init(flagType, this);
            flags.Add(flag);
            return true;
        }
        public sealed override bool UnbindFlag(string flagType)
        {
            if (flagType == Keys.DISPLAY)
                return false;
            var index = flags.FindIndex(x => x.FlagType == flagType);
            if (index < 0)
                return false;
            if (isStatic)
                Yut.Instance.DB.Deleteable<DBFlagInfo>().Where(x => x.RegionId == regionId && x.FlagType == flagType).ExecuteCommandAsync();
            var flag = flags[index];
            flag.Destroy();
            //DBManager.Instance.DeleteFlag(regionId, flagType);
            FlagManager.Instance.ReturnRegionFlag(flag);
            flags.RemoveAt(index);
            return true;
        }
        public sealed override bool HasFlag(string flagType)
            => flags.Exists(x => x.FlagType == flagType);
        public sealed override RegionFlag GetFlag(string flagType)
            => flags.Find(x => x.FlagType == flagType);
        public sealed override bool TryGetFlag<TRegionFlag, TRegionFlagConfig>(string flagType, out TRegionFlag flag)
        {
            flag = GetFlag(flagType) as TRegionFlag;
            return !(flag is null);
        }
        public sealed override void DestroyAllFlags()
        {
            if (isStatic)
                Yut.Instance.DB.Deleteable<DBFlagInfo>().Where(x => x.RegionId == regionId).ExecuteCommandAsync();
            var count = flags.Count;
            for (int i = 0; i < count; i++)
            {
                var flag = flags[i];
                flag.Destroy();
                FlagManager.Instance.ReturnRegionFlag(flag, isStatic);
            }
        }
        public sealed override EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            OnRegionPreChange?.Invoke();
            var result = config.UpdateConfig(key, behaviour, value);
            switch (result)
            {
                case EConfigUpdateResult.Success:
                    OnRegionChanged?.Invoke();
                    if (isStatic)
                    {
                        var info = Yut.Instance.GetDBRegionInfo();
                        info.RegionId = regionId;
                        info.RegionName = regionName;
                        info.UniqueName = uniqueName;
                        info.RegionType = regionType;
                        info.Config = config.ConvertToBytes();
                        Yut.Instance.DB.Updateable(info).ExecuteCommandAsync();
                    }
                    break;
            }
            return result;
        }
        public float SqrMagnitude2D(Vector3 vector)
            => vector.x * vector.x + vector.z * vector.z;

        internal sealed override IRegionConfig RegionConfigInternal => config;
        internal sealed override event Action<Player> OnPlayerEnter;
        internal sealed override event Action<Player> OnPlayerLeave;
        internal sealed override event Action<InteractableVehicle> OnVehicleEnter;
        internal sealed override event Action<InteractableVehicle> OnVehicleLeave;
        internal sealed override event Action<Animal> OnAnimalEnter;
        internal sealed override event Action<Animal> OnAnimalLeave;
        internal sealed override event System.Action OnRegionPreChange;
        internal sealed override event System.Action OnRegionChanged;
        internal sealed override Region BindConfig(IRegionConfig config)
        {
            if (config is T newConfig)
            {
                this.config = newConfig;
                return this;
            }
            return null;
        }
        internal sealed override void Destroy()
        {
            DestroyFlags();
            if (isStatic)
            {
                Yut.Instance.DB.Deleteable<DBFlagInfo>().Where(x => x.RegionId == regionId).ExecuteCommandAsync();
                Yut.Instance.DB.Deleteable<DBRegionInfo>().Where(x => x.RegionId == regionId).ExecuteCommandAsync();
                //DBManager.Instance.DeleteRegion(regionId);
            };
            U.Events.OnPlayerDisconnected -= OnPlayerDisconnected;
            flags.Clear();
            playerQueue.Clear();
            vehicleQueue.Clear();
            animalQueue.Clear();
            players.Clear();
            vehicles.Clear();
            animals.Clear();
            lastDetectPlayers = false;
            lastDetectVehicles = false;
            lastDetectAnimals = false;
            displayFlag = null;
            isInit = false;
        }
        internal sealed override void SetDisplayFlag(DisplayFlag flag)
        {
            displayFlag = flag;
        }
        internal sealed override void OnFlagRegister(string flagType)
        {
            if (!isInit)
                return;
            var count = flagsNotLoad.Count;
            for (var i = count - 1; i >= 0; i--)
            {
                var info = flagsNotLoad[i];
                if (info.FlagType == flagType)
                {
                    var result = InitFlag(info.FlagType, info.Config, isStatic);
                    flagsNotLoad.RemoveAt(i);
                }
            }
        }
        internal sealed override void Init(long regionId, string regionType, string regionName, string uniqueName, bool isCreate = false, bool isStatic = true)
        {
            this.regionId = regionId;
            this.regionType = regionType;
            this.regionName = regionName;
            this.isStatic = isStatic;
            this.uniqueName = uniqueName;
            if (!isCreate)
                GenerateFlags();
            U.Events.OnPlayerDisconnected += OnPlayerDisconnected;
            isInit = true;
        }

        /// <summary>
        /// 区域配置
        /// </summary>
        protected T Config => config;
        /// <summary>
        /// 获得一个空的边界点列表
        /// </summary>
        protected List<Vector3> GetEmptyDisplayPoints()
        {
            emptyVectors.Clear();
            return emptyVectors;
        }
        protected sealed override void InvokeRegionChange()
            => OnRegionChanged?.Invoke();

        /// <summary>
        /// 初始化标记
        /// </summary>
        private void GenerateFlags()
        {
            Task.Run(() =>
            {
                //var flags = DBManager.Instance.GetRegionFlagInfos(regionId);
                var flags = Yut.Instance.DB.Queryable<DBFlagInfo>().Where(x => x.RegionId == regionId).ToList();
                var count = flags.Count;
                for (int i = 0; i < count; i++)
                {
                    var info = flags[i];
                    if (InitFlag(info.FlagType, info.Config, true))
                        Yut.Instance.ReturnDBFlagInfo(info);
                    else
                        flagsNotLoad.Add(info);
                }
            });
            //var flags = DBManager.Instance.GetRegionFlagInfos(regionId);
            //var count = flags.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    var info = flags[i];
            //    if (InitFlag(info.FlagType, info.Config, true))
            //        DBManager.Instance.ReturnDBFlagInfo(info);
            //    else
            //        flagsNotLoad.Add(info);
            //}
        }
        /// <summary>
        /// 销毁所有标记
        /// </summary>
        private void DestroyFlags()
        {
            var count = flags.Count;
            for (int i = 0; i < count; i++)
            {
                var flag = flags[i];
                flag.Destroy();
                FlagManager.Instance.ReturnRegionFlag(flag, isStatic);
            }
        }
        /// <summary>
        /// 初始化指定标记
        /// </summary>
        private bool InitFlag(string flagType, byte[] config, bool isStatic)
        {
            var flag = FlagManager.Instance.GetRegionFlag(flagType, isStatic);
            if (flag is null)
                return false;
            flag.Config.ConvertFromBytes(config);
            flag.Init(flagType, this);
            flags.Add(flag);
            return true;
        }
        private void DetectPlayersPrivate()
        {
            if (playerQueue.Count == 0)
            {
                var count = Provider.clients.Count;
                for (int i = 0; i < count; i++)
                    playerQueue.Enqueue(Provider.clients[i]?.player);
            }
            else
            {
                var count = playerQueue.Count > 5 ? 5 : playerQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    var player = playerQueue.Dequeue();
                    if (player is null)
                        continue;
                    var index = players.BinarySearch(player, playerComparer);
                    var contains = index >= 0;
                    if (player.life.isDead)
                    {
                        if (contains)
                        {
                            players.RemoveAt(index);
                            OnPlayerLeave?.Invoke(player);
                            RegionManager.Instance.InvokePlayerLeaveRegion(player, this);
                        }
                    }
                    else
                    {
                        Vector3? position = null;
                        try
                        {
                            position = player?.gameObject?.transform?.position;
                        }
                        catch { }
                        if (position.HasValue)
                        {
                            var inRegion = InRegion(position.Value);
                            if (!contains && inRegion)
                            {
                                players.Insert(~index, player);
                                OnPlayerEnter?.Invoke(player);
                                RegionManager.Instance.InvokePlayerEnterRegion(player, this);
                            }
                            else if (contains && !inRegion)
                            {
                                players.RemoveAt(index);
                                OnPlayerLeave?.Invoke(player);
                                RegionManager.Instance.InvokePlayerLeaveRegion(player, this);
                            }
                        }
                    }
                }
            }
        }
        private void DetectVehiclesPrivate()
        {
            if (vehicleQueue.Count == 0)
            {
                var count = VehicleManager.vehicles.Count;
                for (int i = 0; i < count; i++)
                    vehicleQueue.Enqueue(VehicleManager.vehicles[i]);
            }
            else
            {
                var count = vehicleQueue.Count > 5 ? 5 : vehicleQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    var vehicle = vehicleQueue.Dequeue();
                    if (vehicle is null)
                        continue;
                    var index = vehicles.BinarySearch(vehicle, vehivleComparer);
                    var contains = index >= 0;
                    if (vehicle.isDead || vehicle.isExploded)
                    {
                        if (contains)
                        {
                            vehicles.RemoveAt(index);
                            OnVehicleLeave?.Invoke(vehicle);
                            RegionManager.Instance.InvokeVehicleLeaveRegion(vehicle, this);
                        }
                    }
                    else
                    {
                        var inRegion = InRegion(vehicle.transform.position);
                        if (!contains && inRegion)
                        {
                            vehicles.Insert(~index, vehicle);
                            OnVehicleEnter?.Invoke(vehicle);
                            RegionManager.Instance.InvokeVehicleEnterRegion(vehicle, this);
                        }
                        else if (contains && !inRegion)
                        {
                            vehicles.RemoveAt(index);
                            OnVehicleLeave?.Invoke(vehicle);
                            RegionManager.Instance.InvokeVehicleLeaveRegion(vehicle, this);
                        }
                    }
                }
            }
        }
        private void DetectAnimalsPrivate()
        {
            if (animalQueue.Count == 0)
            {
                var count = AnimalManager.animals.Count;
                for (int i = 0; i < count; i++)
                    animalQueue.Enqueue(AnimalManager.animals[i]);
            }
            else
            {
                var count = animalQueue.Count > 5 ? 5 : animalQueue.Count;
                for (int i = 0; i < count; i++)
                {
                    var animal = animalQueue.Dequeue();
                    if (animal is null)
                        continue;
                    var index = animals.BinarySearch(animal, animalComparer);
                    var contains = index >= 0;
                    if (animal.isDead)
                    {
                        if (contains)
                        {
                            animals.RemoveAt(index);
                            OnAnimalLeave?.Invoke(animal);
                            RegionManager.Instance.InvokeAnimalLeaveRegion(animal, this);
                        }
                    }
                    else
                    {
                        var inRegion = InRegion(animal.transform.position);
                        if (!contains && inRegion)
                        {
                            animals.Insert(~index, animal);
                            OnAnimalEnter?.Invoke(animal);
                            RegionManager.Instance.InvokeAnimalEnterRegion(animal, this);
                        }
                        else if (contains && !inRegion)
                        {
                            animals.RemoveAt(index);
                            OnAnimalLeave?.Invoke(animal);
                            RegionManager.Instance.InvokeAnimalLeaveRegion(animal, this);
                        }
                    }
                }
            }
        }
        private void OnPlayerDisconnected(UnturnedPlayer player)
        {
            var index = players.BinarySearch(player.Player, playerComparer);
            if (index < 0)
                return;
            players.RemoveAt(index);
            OnPlayerLeave?.Invoke(player.Player);
            RegionManager.Instance.InvokePlayerLeaveRegion(player.Player, this);
        }
        private void Awake()
        {
            players = new List<Player>(Provider.maxPlayers);
            vehicles = new List<InteractableVehicle>(64);
            animals = new List<Animal>(64);
            playerQueue = new Queue<Player>(Provider.maxPlayers);
            vehicleQueue = new Queue<InteractableVehicle>(64);
            animalQueue = new Queue<Animal>(64);
            playerComparer = new PlayerComparer();
            vehivleComparer = new VehivleComparer();
            animalComparer = new AnimalComparer();
            flags = new List<RegionFlag>();
            flagsNotLoad = new List<DBFlagInfo>();
            emptyVectors = new List<Vector3>();
            isInit = false;
        }
        private void Update()
        {
            if (!isInit)
                return;
            if (config.DetectPlayers)
            {
                if (!(Provider.clients is null) && Provider.clients.Count > 0)
                    DetectPlayersPrivate();
                if (!lastDetectPlayers)
                    lastDetectPlayers = true;
            }
            else
            {
                if (lastDetectPlayers)
                {
                    playerQueue.Clear();
                    players.Clear();
                    lastDetectPlayers = false;
                }
            }
            if (config.DetectVehicles)
            {
                if (!(VehicleManager.vehicles is null) && VehicleManager.vehicles.Count > 0)
                    DetectVehiclesPrivate();
                if (!lastDetectVehicles)
                    lastDetectVehicles = true;
            }
            else
            {
                if (lastDetectVehicles)
                {
                    vehicleQueue.Clear();
                    vehicles.Clear();
                    lastDetectVehicles = false;
                }
            }
            if (config.DetectAnimals)
            {
                if (!(AnimalManager.animals is null) && AnimalManager.animals.Count > 0)
                    DetectAnimalsPrivate();
                if (!lastDetectAnimals)
                    lastDetectAnimals = true;
            }
            else
            {
                if (lastDetectAnimals)
                {
                    animalQueue.Clear();
                    animals.Clear();
                    lastDetectAnimals = false;
                }
            }
        }
    }
}
