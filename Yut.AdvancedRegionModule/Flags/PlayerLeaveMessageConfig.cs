using System;
using System.Collections.Generic;
using System.IO;

namespace Yut.AdvancedRegionModule.Flags
{
    internal class PlayerLeaveMessageConfig : EnableConfig, IRegionFlagConfig
    {
        private string message;
        private EBroadcastMode broadcastMode;
        private bool rich;
        public string Message => message;
        public EBroadcastMode BroadcastMode => broadcastMode;
        public bool Rich => rich;
        public void ConvertFromBytes(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(stream))
                {
                    Enabled = reader.ReadBoolean();
                    broadcastMode = (EBroadcastMode)reader.ReadInt32();
                    rich = reader.ReadBoolean();
                    message = reader.ReadString();
                }
            }
        }
        public byte[] ConvertToBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(Enabled);
                    writer.Write((int)broadcastMode);
                    writer.Write(Rich);
                    writer.Write(message);
                }
                return stream.ToArray();
            }
        }
        public List<string> ConvertToString()
        {
            var result = new List<string>();
            result.Add(Keys.ENABLED + ":" + Enabled);
            result.Add(Keys.BROADCAST + ":" + BroadcastMode);
            result.Add(Keys.RICH + ":" + Rich);
            result.Add(Keys.MESSAGE + ":" + Message);
            return result;
        }
        public void Divest() { }
        public void Reset()
        {
            Enabled = true;
            broadcastMode = EBroadcastMode.Player;
            rich = true;
            message = Keys.LEAVE_MESSAGE;
        }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            if (behaviour != Keys.SET)
                return EConfigUpdateResult.UndefinedBehaviour;
            if (key == Keys.ENABLED)
            {
                if (bool.TryParse(value, out var enabled))
                {
                    if (Enabled == enabled)
                        return EConfigUpdateResult.RejectUpdate;
                    Enabled = enabled;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.MESSAGE)
            {
                message = value;
                return EConfigUpdateResult.Success;
            }
            else if (key == Keys.BROADCAST)
            {
                if (Enum.TryParse<EBroadcastMode>(value, out var broadcast))
                {
                    broadcastMode = broadcast;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.RICH)
            {
                if (bool.TryParse(value, out var r))
                {
                    rich = r;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            return EConfigUpdateResult.UndefinedKey;
        }
    }
}
