using Rocket.API;

namespace Yut.AdvancedRegionModule
{
    public sealed class Config : IRocketPluginConfiguration
    {
        public MysqlConnectionConfig MysqlConnectionConfig;
        public void LoadDefaults()
        {
            MysqlConnectionConfig = new MysqlConnectionConfig()
            {
                DataSource = "127.0.0.1",
                Database = "Unturned",
                UserID = "用户名",
                Password = "密码",
                Port = "3306"
            };
        }
    }
}
