using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;
using Yut.AdvancedRegionModule.Flags;
using Yut.AdvancedRegionModule.Regions;

namespace Yut.AdvancedRegionModule.Commands
{
    public sealed class CommandDynamic : IRocketCommand
    {
        private static CommandDynamic command;
        public static CommandDynamic Command
        {
            get
            {
                if (command == null)
                    command = new CommandDynamic();
                return command;
            }
        }
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => Keys.COMMAN_DYNAMIC;
        public string Help => string.Empty;
        public string Syntax => string.Empty;
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 1)
            {
                if (command[0] == Keys.TYPES)
                {
                    var regions = RegionManager.Instance.DynamicRegionTypes;
                    var count = regions.Count;
                    for (int i = 0; i < count; i++)
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_DYNAMIC_REGION_TYPES, regions[i]));
                }
                else if (command[0] == Keys.REGIONS)
                {
                    var regions = RegionManager.Instance.DynamicRegions;
                    var count = regions.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var region = regions[i];
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_DYNAMIC_REGION, region.RegionID, region.RegionType, region.UniqueName));
                    }
                }
                else if (command[0] == Keys.FLAGS)
                {
                    var flags = FlagManager.Instance.DynamicFlagTypes;
                    var count = flags.Count;
                    for (int i = 0; i < count; i++)
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_DYNAMIC_FLAG_TYPES, flags[i]));
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 2)
            {
                if (command[0] == Keys.DESTROY)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    var result = RegionManager.Instance.DestroyDynamicRegion(region);
                    switch (result)
                    {
                        case EDestroyRegionResult.TryDestroyWorld:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_DESTROY_REGION_WORLD));
                            break;
                        default:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_DESTROY_REGION_SUCCESS, region.RegionID, region.UniqueName));
                            break;
                    }
                }
                else if (command[0] == Keys.FLAGS)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    var flags = region.Flags;
                    var count = flags.Count;
                    for (int i = 0; i < count; i++)
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_FLAG, flags[i].FlagType));
                }
                else if (command[0] == Keys.CONFIG)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    var configs = region.RegionConfigInternal.ConvertToString();
                    for (var i = 0; i < configs.Count; i++)
                        UnturnedChat.Say(caller, configs[i]);
                }
                else if (command[0] == Keys.SHOW)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    region.Display = !region.Display;
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 3)
            {
                if (command[0] == Keys.CREATE)
                {
                    var result = RegionManager.Instance.CreateDynamicRegion(command[1], command[2], out var region);
                    switch (result)
                    {
                        case ECreateRegionResult.TryCreateWorld:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_CREATE_REGION_WORLD));
                            break;
                        case ECreateRegionResult.Failed:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_CREATE_REGION_FAILED));
                            break;
                        default:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_CREATE_REGION_SUCCESS, region.RegionID, region.UniqueName));
                            break;
                    }
                }
                else if (command[0] == Keys.CONFIG)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    var flag = region.GetFlag(command[2]);
                    if (flag is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_FLAG_NOT_FOUND, command[2]));
                        return;
                    }
                    var configs = flag.RegionFlagConfig.ConvertToString();
                    for (var i = 0; i < configs.Count; i++)
                        UnturnedChat.Say(caller, configs[i]);
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 5)
            {
                if (command[0] == Keys.UPDATE)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    if (command[2] != Keys.FLAG)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                        return;
                    }
                    if (command[4] == Keys.BIND)
                    {
                        var result = region.BindFlag(command[3]);
                        if (result)
                        {
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_BIND_FLAG_SUCCESS, command[3]));
                            return;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_BIND_FLAG_FAILED, command[3]));
                            return;
                        }
                    }
                    else if (command[4] == Keys.UNBIND)
                    {
                        var result = region.UnbindFlag(command[3]);
                        if (result)
                        {
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UNBIND_FLAG_SUCCESS, command[3]));
                            return;
                        }
                        else
                        {
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UNBIND_FLAG_FAILED, command[3]));
                            return;
                        }
                    }
                    else
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 6)
            {
                if (command[0] == Keys.UPDATE)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    if (command[2] != Keys.CONFIG)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                        return;
                    }
                    var result = region.UpdateConfig(command[3], command[4], command[5]);
                    switch (result)
                    {
                        case EConfigUpdateResult.UndefinedKey:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_UNDEFINEDKEY, command[3]));
                            break;
                        case EConfigUpdateResult.BehaviourNotSupport:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.VALUE_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT, command[3], command[4]));
                            break;
                        case EConfigUpdateResult.UndefinedBehaviour:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR, command[4]));
                            break;
                        case EConfigUpdateResult.InvalidValue:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_INVALIDVALUE, command[5]));
                            break;
                        case EConfigUpdateResult.RejectUpdate:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_REJECTUPDATE, command[3], command[4]));
                            break;
                        default:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_SUCCESS, command[3], command[4]));
                            break;
                    }
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 8)
            {
                if (command[0] == Keys.UPDATE)
                {
                    var region = RegionManager.Instance.TryFindRegion(command[1], false);
                    if (region is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_REGION_NOT_FOUND, command[1]));
                        return;
                    }
                    if (command[2] != Keys.FLAG || command[4] != Keys.UPDATE)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                        return;
                    }
                    var flag = region.GetFlag(command[3]);
                    if (flag is null)
                    {
                        UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_FLAG_NOT_FOUND, command[3]));
                        return;
                    }
                    var result = flag.UpdateConfig(command[5], command[6], command[7]);
                    switch (result)
                    {
                        case EConfigUpdateResult.UndefinedKey:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_UNDEFINEDKEY, command[5]));
                            break;
                        case EConfigUpdateResult.BehaviourNotSupport:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_BEHAVIOURNOTSUPPORT, command[5], command[6]));
                            break;
                        case EConfigUpdateResult.UndefinedBehaviour:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_UNDEFINEDBEHAVIOUR, command[6]));
                            break;
                        case EConfigUpdateResult.InvalidValue:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_INVALIDVALUE, command[7]));
                            break;
                        case EConfigUpdateResult.RejectUpdate:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_REJECTUPDATE, command[5], command[6]));
                            break;
                        default:
                            UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_UPDATE_CONFIG_SUCCESS, command[5], command[6]));
                            break;
                    }
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else
                UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
        }
        public void ExecuteParams(IRocketPlayer caller, params string[] command)
          => Execute(caller, command);
    }
}
