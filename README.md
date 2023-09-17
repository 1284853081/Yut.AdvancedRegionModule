# Yut.AdvancedRegionModule
<div align="center">
   <img width="160" src="/Images/Yuthung.jpg" alt="logo"></br>   
</div>           

# 目录
+ [简介](#简介)
+ [使用者入门](#使用者入门)
+ [开发者入门](#开发者入门)
+ [指令](#指令)

## 简介
这是一个基于Rocket实现的区域插件，内置四种类型区域，分别是世界区域，球体区域，圆柱体区域，多棱柱区域，同时每一种区域都能够附带不同的标记来实现不同的功能，内置标记有玩家进入区域提示，玩家离开区域提示，区域边界显示。改区域插件将区域划分为两大类：静态，动态，其中静态区域会将所有相关数据都保存至数据库，动态区域则不会，同理标记也分为静态和动态。创建的区域默认不检测任何对象。该插件同时也提供了给其他插件二次开发的接口，你可以根据相关接口实现自己想实现的区域以及不同功能的标记。
## 使用者入门
以下是一个创建名为Test静态区域的例子，以多棱柱区域为例
1.创建静态区域
```
/static create prism Test
```
2.显示区域边界（可开启可不开启）
```
/static show Test
```
3.添加棱柱边界点
```
/prism static Test points add
```
> 添加三个以上的点后区域开始生效
4.删除棱柱末尾边界点
```
/prism static Test points remove
```
5.绑定玩家进入提示标记
```
/static update Test flag PlayerEnterMessage bind
```
6.开启检测玩家进入事件
```
/static update Test config set DetectPlayers True
```
> 至此已经成功创建了一个棱柱区域
## 开发者入门
1.自定义区域
> 要实现自己的区域，首先需要创建一个区域类RegionClass,一个区域配置类RegionClassConfig，其中RegionClass继承Region<RegionClassConfig>并实现其中的抽象方法，RegionClassConfig需要实现IRegionConfig接口
以下是一个简单的示例
```C#
//RegionClass
public sealed class RegionClass : Region<RegionClassConfig>
{
    // 获得覆盖区域的矩形
    public override void GetCoverRectangle(out Vector3 bottomLeftVertex, out Vector3 topRightVertex)
        => throw new NotImplementedException();
    // 获取区域的边界点，用于显示区域边界
    public override List<Vector3> GetDisplayPoints()
        => throw new NotImplementedException();
    // 判断当前点是否在区域内
    public override bool InRegion(Vector3 vector)
        => throw new NotImplementedException();
    // 返回区域内随机的一点
    public override Vector3 RandomPointInRegion()
        => throw new NotImplementedException();
}
//RegionClassConfig
public sealed class RegionClassConfig : IRegionConfig
{
    //是否检测玩家事件
    public bool DetectPlayers => throw new NotImplementedException();
    //是否检测载具事件
    public bool DetectVehicles => throw new NotImplementedException();
    //是否检测动物事件
    public bool DetectAnimals => throw new NotImplementedException();
    //将字节流反序列化成配置
    public void ConvertFromBytes(byte[] bytes) => throw new NotImplementedException();
    //将配置序列化成字节流
    public byte[] ConvertToBytes() => throw new NotImplementedException();
    //将配置转化成游戏内配置提示的字符串列表
    public List<string> ConvertToString() => throw new NotImplementedException();
    //释放资源
    public void Divest() => throw new NotImplementedException();
    //重置配置
    public void Reset() => throw new NotImplementedException();
    //修改区域配置，key为修改的关键字，建议与字段名相同，behaviour为更改的行为，内置有set,add,remove,也可以自定义，value为更改的值
    public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value) => throw new NotImplementedException();
}
```
实现以上两个类后只需要将自定义的区域注册至本插件即可
```C#
//区域类型名字不可重复
RegionManager.Instance.RegisterStaticRegionType<RegionClass, RegionClassConfig>("区域类型名字");
```
2.自定义区域标记
> 与自定义区域相同，继承RegionFlag<YourRegionFlagConfig>即可，其中YourRegionFlagConfig需要实现IRegionFlagConfig接口
## 指令
指令名字|指令类型|指令功能
:-:|:-:|:-:
static|基础|操控静态区域
dynamic|基础|操控动态区域
prism|扩展|操控多棱柱区域
cylinder|扩展|操控圆柱区域
sphere|扩展|操控球体区域
### 指令大全
> 区域有名字，唯一名字，id之分，名字为创建时指定的名称，但创建的同时会分配一个唯一名字防止出现名称冲突的情况，以下指令查找区域均通过唯一名字或id来查找，并统一用nid表示
#### 基础系列
1. /static types
> 查看已注册的所有区域类型名字      
2. /static regions
> 查看所有已定义的静态区域      
3. /static flags
> 查看所有已注册的静态区域标记名字      
4. /static destroy <nid>
> 通过区域的id或唯一名字来销毁区域      
5. /static flags <nid>
> 查看指定区域绑定的所有标记      
6. /static config <nid>
> 查看指定区域的区域配置信息      
7. /static show <nid>
> 显示/关闭指定区域的区域边界      
8. /static create <区域类型> <区域名字>
> 创建指定名字指定类型的区域      
9. /static config <nid> <标记名称>
> 查看区域绑定的指定标记的配置信息      
10. /static update <nid> flag <标记名称> bind/unbind
> 为指定区域绑定/取消绑定指定标记      
11. /static update <nid> config <key> <behaviour> <value>
> 修改指定区域的配置      
12. /static update <nid> flag <标记名称> update <key> <behaviour> <value>
> 修改指定区域绑定的指定标记的配置      

>  dynamic指令同上
#### 扩展指令
扩展指令用于单独调整不同区域，为区域设置提供便利      
