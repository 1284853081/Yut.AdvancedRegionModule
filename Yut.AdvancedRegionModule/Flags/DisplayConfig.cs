using System.Collections.Generic;
using System.IO;

namespace Yut.AdvancedRegionModule.Flags
{
    public sealed class DisplayConfig : EnableConfig, IRegionFlagConfig
    {
        public void ConvertFromBytes(byte[] bytes)
        {
            using (var steam = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(steam))
                {
                    Enabled = reader.ReadBoolean();
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
                }
                return stream.ToArray();
            }
        }
        public List<string> ConvertToString()
        {
            var result = new List<string>();
            result.Add(Keys.ENABLED + ":" + Enabled);
            return result;
        }
        public void Divest() { }
        public void Reset()
        {
            Enabled = false;
        }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            if (behaviour != Keys.SET)
                return EConfigUpdateResult.UndefinedBehaviour;
            if (key != Keys.ENABLED)
                return EConfigUpdateResult.UndefinedKey;
            if (bool.TryParse(value, out var enabled))
            {
                if (Enabled == enabled)
                    return EConfigUpdateResult.RejectUpdate;
                Enabled = enabled;
                return EConfigUpdateResult.Success;
            }
            return EConfigUpdateResult.InvalidValue;
        }
    }
}
