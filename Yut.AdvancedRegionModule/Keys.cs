﻿namespace Yut.AdvancedRegionModule
{
    public static class Keys
    {
        public const long WORLDID = 0L;
        public const string COMMAN_STATIC = "static";
        public const string COMMAN_DYNAMIC = "dynamic";

        public const string SHOW = "show";
        public const string CREATE = "create";
        public const string REMOVE = "remove";
        public const string UPDATE = "update";
        public const string FLAGS = "flags";
        public const string FLAG = "flag";
        public const string CONFIG = "config";
        public const string BIND = "bind";
        public const string UNBIND = "unbind";
        public const string TYPES = "types";
        public const string REGIONS = "regions";
        public const string SET = "set";
        public const string CENTER = "center";
        public const string RADIUS = "radius";
        public const string IS3D = "is3d";
        public const string MINH = "minh";
        public const string MAXH = "maxh";
        public const string POINTS = "points";
        public const string ADD = "add";
        public const string ENABLED = "enabled";
        public const string MESSAGE = "message";
        public const string BROADCAST = "broadcast";
        public const string RICH = "rich";
        public const string CYLINDER = "cylinder";
        public const string PRISM = "prism";
        public const string SPHERE = "sphere";
        public const string WORLD = "world";
        public const string LAST = "last";
        public const string DESTROY = "destroy";
        public const string DetectPlayers = "DetectPlayers";
        public const string DetectVehicles = "DetectVehicles";
        public const string DetectAnimals = "DetectAnimals";
        public const string PLAYER_ENTER_MESSGE = "PlayerEnterMessage";
        public const string PLAYER_LEAVE_MESSGE = "PlayerLeaveMessage";
        public const string DISPLAY = "Display";
        public const string WORLD_NAME = "__World__";
        public const string ENTER_MESSAGE = "Player {0} enter the region {1}";
        public const string LEAVE_MESSAGE = "Player {0} leave the region {1}";

        public const string KEY_ERROR_SYNTAX = "ERROR_SYNTAX";
        public const string VALUE_ERROR_SYNTAX = "错误的语法";
        public const string KEY_STATIC_REGION_TYPES = "STATIC_REGION_TYPES";
        public const string VALUE_STATIC_REGION_TYPES = "static {0}";
        public const string KEY_DYNAMIC_REGION_TYPES = "DYNAMIC_REGION_TYPES";
        public const string VALUE_DYNAMIC_REGION_TYPES = "dynamic {0}";
        public const string KEY_STATIC_REGION = "STATIC_REGION";
        public const string VALUE_STATIC_REGION = "static ID:{0} Type:{1} Name:{2} UniqueName{3}";
        public const string KEY_DYNAMIC_REGION = "DYNAMIC_REGION";
        public const string VALUE_DYNAMIC_REGION = "dynamic ID:{0} Type:{1} Name:{2} UniqueName{3}";
        public const string KEY_STATIC_FLAG_TYPES = "STATIC_FLAG_TYPES";
        public const string VALUE_STATIC_FLAG_TYPES = "static {0}";
        public const string KEY_DYNAMIC_FLAG_TYPES = "DYNAMIC_FLAG_TYPES";
        public const string VALUE_DYNAMIC_FLAG_TYPES = "dynamic {0}";
        public const string KEY_REGION_NOT_FOUND = "REGION_NOT_FOUND";
        public const string VALUE_REGION_NOT_FOUND = "不存在id为{0}的区域";
        public const string KEY_DESTROY_REGION_WORLD = "DESTROY_REGION_WORLD";
        public const string VALUE_DESTROY_REGION_WORLD = "不可移除全局区域";
        public const string KEY_DESTROY_REGION_SUCCESS = "DESTROY_REGION_SUCCESS";
        public const string VALUE_DESTROY_REGION_SUCCESS = "区域ID:{0},唯一标识:{1}移除成功";
        public const string KEY_REGION_FLAG = "REGION_FLAG";
        public const string VALUE_REGION_FLAG = "flag {0}";
        public const string KEY_CREATE_REGION_WORLD = "CREATE_REGION_WORLD";
        public const string VALUE_CREATE_REGION_WORLD = "不可创建全局区域";
        public const string KEY_CREATE_REGION_FAILED = "CREATE_REGION_FAILED";
        public const string VALUE_CREATE_REGION_FAILED = "创建区域失败";
        public const string KEY_CREATE_REGION_SUCCESS = "CREATE_REGION_SUCCESS";
        public const string VALUE_CREATE_REGION_SUCCESS = "成功创建区域，区域ID:{0},区域唯一标识:{1}";
        public const string KEY_BIND_FLAG_FAILED = "BIND_FLAG_FAILED";
        public const string VALUE_BIND_FLAG_FAILED = "区域绑定标记{0}失败，请确保标记类型存在且标记可绑定至区域";
        public const string KEY_BIND_FLAG_SUCCESS = "BIND_FLAG_SUCCESS";
        public const string VALUE_BIND_FLAG_SUCCESS = "成功绑定标记{0}";
        public const string KEY_UNBIND_FLAG_FAILED = "UNBIND_FLAG_FAILED";
        public const string VALUE_UNBIND_FLAG_FAILED = "区域解除绑定标记{0}失败";
        public const string KEY_UNBIND_FLAG_SUCCESS = "UNBIND_FLAG_SUCCESS";
        public const string VALUE_UNBIND_FLAG_SUCCESS = "成功解除绑定标记{0}";
        public const string KEY_UPDATE_CONFIG_UNDEFINEDKEY = "UPDATE_CONFIG_UNDEFINEDKEY";
        public const string VALUE_UPDATE_CONFIG_UNDEFINEDKEY = "未定义键{0}";
        public const string KEY_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT = "UPDATE_CONFIG_BEHAVIOURNOTSUPPORT";
        public const string VALUE_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT = "键{0}不支持{1}行为";
        public const string KEY_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR = "UPDATE_CONFIG_UNDEFINEDBEHAVIOUR";
        public const string VALUE_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR = "未定义行为{0}";
        public const string KEY_UPDATE_CONFIG_INVALIDVALUE = "UPDATE_CONFIG_INVALIDVALUE";
        public const string VALUE_UPDATE_CONFIG_INVALIDVALUE = "无效的值:{0}";
        public const string KEY_UPDATE_CONFIG_REJECTUPDATE = "UPDATE_CONFIG_REJECTUPDATE";
        public const string VALUE_UPDATE_CONFIG_REJECTUPDATE = "对键{0}做出的行为{1}被拒绝";
        public const string KEY_UPDATE_CONFIG_SUCCESS = "UPDATE_CONFIG_SUCCESS";
        public const string VALUE_UPDATE_CONFIG_SUCCESS = "对键{0}做出的行为{1}被成功执行";
        public const string KEY_FLAG_NOT_FOUND = "FLAG_NOT_FOUND";
        public const string VALUE_FLAG_NOT_FOUND = "标记{0}不存在";
    }
}
