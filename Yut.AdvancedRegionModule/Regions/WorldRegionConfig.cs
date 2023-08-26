using System.Collections.Generic;
using System.IO;

namespace Yut.AdvancedRegionModule.Regions
{
    internal sealed class WorldRegionConfig : IRegionConfig
    {
        private bool detectPlayers;
        private bool detectVehicles;
        private bool detectAnimals;
        public bool DetectPlayers => detectPlayers;
        public bool DetectVehicles => detectVehicles;
        public bool DetectAnimals => detectAnimals;
        public void ConvertFromBytes(byte[] bytes)
        {
            using (var steam = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(steam))
                {
                    detectPlayers = reader.ReadBoolean();
                    detectVehicles = reader.ReadBoolean();
                    detectAnimals = reader.ReadBoolean();
                }
            }
        }
        public byte[] ConvertToBytes()
        {
            using (var stream = new MemoryStream())
            {
                using (var writer = new BinaryWriter(stream))
                {
                    writer.Write(detectPlayers);
                    writer.Write(detectVehicles);
                    writer.Write(detectAnimals);
                }
                return stream.ToArray();
            }
        }
        public List<string> ConvertToString()
        {
            var list = new List<string>();
            list.Add(Keys.DetectPlayers + ":" + detectPlayers);
            list.Add(Keys.DetectVehicles + ":" + detectVehicles);
            list.Add(Keys.DetectAnimals + ":" + detectAnimals);
            return list;
        }
        public void Divest() { }
        public void Reset()
        {
            detectPlayers = true;
            detectVehicles = true;
            detectAnimals = true;
        }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
            => EConfigUpdateResult.RejectUpdate;
    }
}
