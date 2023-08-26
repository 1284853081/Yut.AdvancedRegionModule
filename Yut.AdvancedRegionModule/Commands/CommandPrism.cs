using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Yut.AdvancedRegionModule.Commands
{
    internal class CommandPrism : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => Keys.PRISM;
        public string Help => string.Empty;
        public string Syntax => string.Empty;
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>();
        public void Execute(IRocketPlayer caller, string[] command)
        {
            bool isStatic;
            if (command.Length >= 3)
            {
                if (command[0] == Keys.COMMAN_STATIC)
                    isStatic = true;
                else if (command[0] == Keys.COMMAN_DYNAMIC)
                    isStatic = false;
                else
                {
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                    return;
                }
            }
            else
            {
                UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
                return;
            }
            if (command.Length == 3)
            {
                if (command[1] == Keys.CREATE)
                {
                    if (isStatic)
                        CommandStatic.Command.ExecuteParams(caller, command[1], Keys.PRISM, command[2]);
                    else
                        CommandDynamic.Command.ExecuteParams(caller, command[1], Keys.PRISM, command[2]);
                }
                else if (command[1] == Keys.DESTROY)
                {
                    if (isStatic)
                        CommandStatic.Command.ExecuteParams(caller, command[2], command[3]);
                    else
                        CommandDynamic.Command.ExecuteParams(caller, command[2], command[3]);
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 4)
            {
                if (command[2] == Keys.POINTS && command[3] == Keys.ADD)
                {
                    var player = caller as UnturnedPlayer;
                    var point = player.Position.GetString();
                    if (isStatic)
                        CommandStatic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, Keys.POINTS, Keys.ADD, point);
                    else
                        CommandDynamic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, Keys.POINTS, Keys.ADD, point);
                }
                else if (command[2] == Keys.POINTS && command[3] == Keys.REMOVE)
                {
                    if (isStatic)
                        CommandStatic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, Keys.POINTS, Keys.REMOVE, Keys.LAST);
                    else
                        CommandDynamic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, Keys.POINTS, Keys.REMOVE, Keys.LAST);
                }
                else
                    UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
            }
            else if (command.Length == 5)
            {
                if (isStatic)
                    CommandStatic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, command[2], command[3], command[4]);
                else
                    CommandDynamic.Command.ExecuteParams(caller, Keys.UPDATE, command[1], Keys.CONFIG, command[2], command[3], command[4]);
            }
            else
                UnturnedChat.Say(caller, Yut.Instance.Translate(Keys.KEY_ERROR_SYNTAX));
        }
    }
}
