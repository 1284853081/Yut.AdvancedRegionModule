using SDG.Unturned;

namespace Yut.AdvancedRegionModule.Flags
{
    internal class PlayerEnterMessageFlag : RegionFlag<PlayerEnterMessageConfig>
    {
        protected override void OnPlayerEnter(Player player)
        {
            if (!FlagConfig.Enabled)
                return;
            string message = string.Format(FlagConfig.Message, player?.channel.owner.playerID.characterName, Region.UniqueName);
            bool rich = FlagConfig.Rich;
            EBroadcastMode mode = FlagConfig.BroadcastMode;
            if (string.IsNullOrEmpty(message))
                return;
            switch (mode)
            {
                case EBroadcastMode.Global:
                    ChatManager.serverSendMessage(message, Palette.SERVER, null, null, EChatMode.GLOBAL, null, rich);
                    break;
                case EBroadcastMode.Player:
                    ChatManager.serverSendMessage(message, Palette.SERVER, null, player?.channel.owner, EChatMode.SAY, null, rich);
                    break;
                case EBroadcastMode.Area:
                    ChatManager.serverSendMessage(message, Palette.SERVER, null, null, EChatMode.LOCAL, null, rich);
                    break;
                case EBroadcastMode.Group:
                    ChatManager.serverSendMessage(message, Palette.SERVER, null, null, EChatMode.GROUP, null, rich);
                    break;
            }
        }
    }
}
