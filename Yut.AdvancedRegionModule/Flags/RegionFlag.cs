using SDG.Unturned;
using System.Collections;
using UnityEngine;
using Yut.AdvancedRegionModule.Database;
using Yut.AdvancedRegionModule.Regions;
using Yut.PoolModule;

namespace Yut.AdvancedRegionModule.Flags
{
    public abstract class RegionFlag : IObject
    {
        /// <summary>
        /// 绑定的区域
        /// </summary>
        public abstract Region Region { get; }
        /// <summary>
        /// 标记类型
        /// </summary>
        public abstract string FlagType { get; }
        /// <summary>
        /// 标记配置
        /// </summary>
        public abstract IConfig Config { get; }
        /// <summary>
        /// 标记配置
        /// </summary>
        public abstract IRegionFlagConfig RegionFlagConfig { get; }
        /// <summary>
        /// 绑定标记配置，并返回绑定成功的标记实例
        /// </summary>
        public abstract IObject BindConfig(IConfig config);
        public abstract void Divest();
        public abstract void Reset();
        public abstract EConfigUpdateResult UpdateConfig(string key, string behaviour, string value);
        public abstract Coroutine StartCoroutine(IEnumerator routine);
        public abstract void StopCoroutine(Coroutine coroutine);

        internal abstract void Init(string flagType, Region region);
        internal abstract void Destroy();
    }
    public abstract class RegionFlag<T> : RegionFlag
        where T : IRegionFlagConfig
    {
        private T config;
        private string flagType;
        private Region region;
        public sealed override Region Region => region;
        public sealed override IConfig Config => config;
        public sealed override IRegionFlagConfig RegionFlagConfig => config;
        public sealed override string FlagType => flagType;
        public sealed override IObject BindConfig(IConfig config)
        {
            if (config is T newConfig)
            {
                this.config = newConfig;
                return this;
            }
            return null;
        }
        public sealed override EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            var result = config.UpdateConfig(key, behaviour, value);
            switch (result)
            {
                case EConfigUpdateResult.Success:
                    OnConfigUpdated();
                    if (region.IsStatic)
                    {
                        var info = Yut.Instance.GetDBFlagInfo();
                        info.RegionId = region.RegionID;
                        info.FlagType = flagType;
                        info.Config = config.ConvertToBytes();
                        Yut.Instance.DB.Updateable(info).ExecuteCommandAsync();
                    }
                    break;
            }
            return result;
        }
        public sealed override Coroutine StartCoroutine(IEnumerator routine)
            => region.StartCoroutine(routine);
        public sealed override void StopCoroutine(Coroutine coroutine)
            => region.StopCoroutine(coroutine);

        public sealed override void Divest() { }
        public sealed override void Reset() { }

        internal sealed override void Init(string flagType, Region region)
        {
            this.flagType = flagType;
            this.region = region;
            region.OnPlayerEnter += OnPlayerEnter;
            region.OnPlayerLeave += OnPlayerLeave;
            region.OnVehicleEnter += OnVehicleEnter;
            region.OnVehicleLeave += OnVehicleLeave;
            region.OnAnimalEnter += OnAnimalEnter;
            region.OnAnimalLeave += OnAnimalLeave;
            region.OnRegionPreChange += OnRegionPreChange;
            region.OnRegionChanged += OnRegionChanged;
            OnInit();
        }
        internal sealed override void Destroy()
        {
            OnDestroy();
            region.OnPlayerEnter -= OnPlayerEnter;
            region.OnPlayerLeave -= OnPlayerLeave;
            region.OnVehicleEnter -= OnVehicleEnter;
            region.OnVehicleLeave -= OnVehicleLeave;
            region.OnAnimalEnter -= OnAnimalEnter;
            region.OnAnimalLeave -= OnAnimalLeave;
            region.OnRegionPreChange -= OnRegionPreChange;
            region.OnRegionChanged -= OnRegionChanged;
        }

        /// <summary>
        /// 区域标记配置
        /// </summary>
        protected T FlagConfig => config;
        /// <summary>
        /// 当标记初始化时调用
        /// </summary>
        protected virtual void OnInit() { }
        /// <summary>
        /// 当玩家进入区域时调用
        /// </summary>
        protected virtual void OnPlayerEnter(Player player) { }
        /// <summary>
        /// 当玩家离开区域时调用
        /// </summary>
        protected virtual void OnPlayerLeave(Player player) { }
        /// <summary>
        /// 当载具进入区域时调用
        /// </summary>
        protected virtual void OnVehicleEnter(InteractableVehicle vehicle) { }
        /// <summary>
        /// 当载具离开区域时调用
        /// </summary>
        protected virtual void OnVehicleLeave(InteractableVehicle vehicle) { }
        /// <summary>
        /// 当动物进入区域时调用
        /// </summary>
        protected virtual void OnAnimalEnter(Animal animal) { }
        /// <summary>
        /// 当动物离开区域时调用
        /// </summary>
        protected virtual void OnAnimalLeave(Animal animal) { }
        /// <summary>
        /// 当配置更改时调用
        /// </summary>
        protected virtual void OnConfigUpdated() { }
        /// <summary>
        /// 当区域将要改变时调用
        /// </summary>
        protected virtual void OnRegionPreChange() { }
        /// <summary>
        /// 当区域已经改变时调用
        /// </summary>
        protected virtual void OnRegionChanged() { }
        /// <summary>
        /// 当标记销毁时调用
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}
