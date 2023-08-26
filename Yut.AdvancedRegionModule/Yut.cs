using Coldairarrow.Util;
using Rocket.API.Collections;
using Rocket.Core.Plugins;
using SqlSugar;
using System.Collections.Generic;
using UnityEngine;
using Yut.AdvancedRegionModule.Database;
using Yut.AdvancedRegionModule.Flags;
using Yut.AdvancedRegionModule.Regions;
using Yut.PoolModule;
using Logger = Rocket.Core.Logging.Logger;

namespace Yut.AdvancedRegionModule
{
    public sealed class Yut : RocketPlugin<Config>
    {
        private static bool isInit;
        private static ConnectionConfig connectionConfig;
        /// <summary>
        /// 数据库标记Model类对象池
        /// </summary>
        private static readonly ObjectPool<DBFlagInfo, IPoolObject> flagInfoPool;
        /// <summary>
        /// 数据库插件Model类对象池
        /// </summary>
        private static readonly ObjectPool<DBPluginInfo, IPoolObject> pluginInfoPool;
        /// <summary>
        /// 数据库区域Model类对象池
        /// </summary>
        private static readonly ObjectPool<DBRegionInfo, IPoolObject> regionInfoPool;
        public SqlSugarClient DB => new SqlSugarClient(connectionConfig);
        /// <summary>
        /// 数据库是否初始化成功
        /// </summary>
        public static bool IsInit => isInit;

        static Yut()
        {
            flagInfoPool = new ObjectPool<DBFlagInfo, IPoolObject>(0, 40);
            pluginInfoPool = new ObjectPool<DBPluginInfo, IPoolObject>(0, 40);
            regionInfoPool = new ObjectPool<DBRegionInfo, IPoolObject>(0, 40);
            isInit = false;
        }

        private static Yut instance;
        public static Yut Instance => instance;
        public override TranslationList DefaultTranslations
        {
            get
            {
                TranslationList list = new TranslationList();
                list.Add(Keys.KEY_ERROR_SYNTAX, Keys.KEY_ERROR_SYNTAX);
                list.Add(Keys.KEY_STATIC_REGION_TYPES, Keys.VALUE_STATIC_REGION_TYPES);
                list.Add(Keys.KEY_DYNAMIC_REGION_TYPES, Keys.VALUE_DYNAMIC_REGION_TYPES);
                list.Add(Keys.KEY_STATIC_REGION, Keys.VALUE_STATIC_REGION);
                list.Add(Keys.KEY_DYNAMIC_REGION, Keys.VALUE_DYNAMIC_REGION);
                list.Add(Keys.KEY_STATIC_FLAG_TYPES, Keys.VALUE_STATIC_FLAG_TYPES);
                list.Add(Keys.KEY_DYNAMIC_FLAG_TYPES, Keys.VALUE_DYNAMIC_FLAG_TYPES);
                list.Add(Keys.KEY_REGION_NOT_FOUND, Keys.VALUE_REGION_NOT_FOUND);
                list.Add(Keys.KEY_DESTROY_REGION_WORLD, Keys.VALUE_DESTROY_REGION_WORLD);
                list.Add(Keys.KEY_DESTROY_REGION_SUCCESS, Keys.VALUE_DESTROY_REGION_SUCCESS);
                list.Add(Keys.KEY_REGION_FLAG, Keys.VALUE_REGION_FLAG);
                list.Add(Keys.KEY_CREATE_REGION_WORLD, Keys.VALUE_CREATE_REGION_WORLD);
                list.Add(Keys.KEY_CREATE_REGION_SUCCESS, Keys.VALUE_CREATE_REGION_SUCCESS);
                list.Add(Keys.KEY_BIND_FLAG_FAILED, Keys.VALUE_BIND_FLAG_FAILED);
                list.Add(Keys.KEY_BIND_FLAG_SUCCESS, Keys.VALUE_BIND_FLAG_SUCCESS);
                list.Add(Keys.KEY_UNBIND_FLAG_FAILED, Keys.VALUE_UNBIND_FLAG_FAILED);
                list.Add(Keys.KEY_UNBIND_FLAG_SUCCESS, Keys.VALUE_UNBIND_FLAG_SUCCESS);
                list.Add(Keys.KEY_UPDATE_CONFIG_UNDEFINEDKEY, Keys.VALUE_UPDATE_CONFIG_UNDEFINEDKEY);
                list.Add(Keys.KEY_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT, Keys.VALUE_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT);
                list.Add(Keys.KEY_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR, Keys.VALUE_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR);
                list.Add(Keys.KEY_UPDATE_CONFIG_INVALIDVALUE, Keys.VALUE_UPDATE_CONFIG_INVALIDVALUE);
                list.Add(Keys.KEY_UPDATE_CONFIG_REJECTUPDATE, Keys.VALUE_UPDATE_CONFIG_REJECTUPDATE);
                list.Add(Keys.KEY_UPDATE_CONFIG_SUCCESS, Keys.VALUE_UPDATE_CONFIG_SUCCESS);
                list.Add(Keys.KEY_FLAG_NOT_FOUND, Keys.VALUE_FLAG_NOT_FOUND);
                return list;
            }
        }
        protected override void Load()
        {
            instance = this;
            new IdHelperBootstrapper().SetWorkderId(1).Boot();
            InitDB();
            RegisterDefault();
            RegionManager.Instance.GenerateRegions();
            Logger.Log("* * * * * * * * * * * * * * * * * * *");
            Logger.Log("*         出品方：Yuthung           *");
            Logger.Log("*            作者：月鸿             *");
            Logger.Log("*   插件：Yut.AdvancedRegionModule  *");
            Logger.Log("*       状态：已加载                *");
            Logger.Log("* * * * * * * * * * * * * * * * * * *\n");
        }
        protected override void Unload()
        {
            Logger.Log("* * * * * * * * * * * * * * * * * * *");
            Logger.Log("*         出品方：Yuthung           *");
            Logger.Log("*            作者：月鸿             *");
            Logger.Log("*   插件：Yut.AdvancedRegionModule  *");
            Logger.Log("*       状态：已加载                *");
            Logger.Log("* * * * * * * * * * * * * * * * * * *\n");
        }

        internal DBFlagInfo GetDBFlagInfo()
            => flagInfoPool.GetFromPool() as DBFlagInfo;
        internal DBPluginInfo GetDBPluginInfo()
            => pluginInfoPool.GetFromPool() as DBPluginInfo;
        internal DBRegionInfo GetDBRegionInfo()
            => regionInfoPool.GetFromPool() as DBRegionInfo;
        internal void ReturnDBFlagInfo(DBFlagInfo info)
            => flagInfoPool.ReturnToPool(info);
        internal void ReturnDBPluginInfo(DBPluginInfo info)
            => pluginInfoPool.ReturnToPool(info);
        internal void ReturnDBRegionInfo(DBRegionInfo info)
            => regionInfoPool.ReturnToPool(info);

        private void InitDB()
        {
            MysqlConnectionConfig config = Yut.Instance.Configuration.Instance.MysqlConnectionConfig;
            connectionConfig = new ConnectionConfig()
            {
                ConnectionString = $"SERVER={config.DataSource};DATABASE={config.Database};UID={config.UserID};PWD={config.Password};PORT={config.Port}",
                DbType = DbType.MySql,
                IsAutoCloseConnection = true,
                InitKeyType = InitKeyType.Attribute
            };
            DB.CodeFirst.InitTables(typeof(DBPluginInfo), typeof(DBRegionInfo), typeof(DBFlagInfo));
            isInit = true;
            FlagManager.Instance.OnDBInit();
            RegionManager.Instance.OnDBInit();
        }
        private void RegisterDefault()
        {
            RegionManager.Instance.RegisterStaticRegionType<WorldRegion, WorldRegionConfig>(Keys.WORLD);
            FlagManager.Instance.RegisterStaticFlagType<DisplayFlag, DisplayConfig>(Keys.DISPLAY);
            FlagManager.Instance.RegisterDynamicFlagType<DisplayFlag, DisplayConfig>(Keys.DISPLAY);
            FlagManager.Instance.RegisterStaticFlagType<PlayerEnterMessageFlag, PlayerEnterMessageConfig>(Keys.PLAYER_ENTER_MESSGE);
            FlagManager.Instance.RegisterStaticFlagType<PlayerLeaveMessageFlag, PlayerLeaveMessageConfig>(Keys.PLAYER_LEAVE_MESSGE);
            RegionManager.Instance.RegisterStaticRegionType<CylinderRegion, CylinderRegionConfig>(Keys.CYLINDER);
            RegionManager.Instance.RegisterStaticRegionType<PrismRegion, PrismRegionConfig>(Keys.PRISM);
            RegionManager.Instance.RegisterStaticRegionType<SphereRegion, SphereRegionConfig>(Keys.SPHERE);
            RegionManager.Instance.RegisterDynamicRegionType<CylinderRegion, CylinderRegionConfig>(Keys.CYLINDER);
            RegionManager.Instance.RegisterDynamicRegionType<PrismRegion, PrismRegionConfig>(Keys.PRISM);
            RegionManager.Instance.RegisterDynamicRegionType<SphereRegion, SphereRegionConfig>(Keys.SPHERE);
        }
    }
}
