using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    internal class SphereRegionConfig : IRegionConfig
    {
        private bool detectPlayers;
        private bool detectVehicles;
        private bool detectAnimals;
        private Vector3 center;
        private float radius;
        public bool DetectPlayers => detectPlayers;
        public bool DetectVehicles => detectVehicles;
        public bool DetectAnimals => detectAnimals;
        /// <summary>
        /// 球心
        /// </summary>
        public Vector3 Center => center;
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius => radius;
        public void ConvertFromBytes(byte[] bytes)
        {
            using (var steam = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(steam))
                {
                    detectPlayers = reader.ReadBoolean();
                    detectVehicles = reader.ReadBoolean();
                    detectAnimals = reader.ReadBoolean();
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    center = new Vector3(x, y, z);
                    radius = reader.ReadSingle();
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
                    writer.Write(center.x);
                    writer.Write(center.y);
                    writer.Write(center.z);
                    writer.Write(radius);
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
            list.Add(Keys.CENTER + ":" + center);
            list.Add(Keys.RADIUS + ":" + radius);
            return list;
        }
        public void Divest() { }
        public void Reset()
        {
            detectPlayers = false;
            detectVehicles = false;
            detectAnimals = false;
            center = Vector3.zero;
            radius = 0;
        }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            if (behaviour != Keys.SET)
                return EConfigUpdateResult.UndefinedBehaviour;
            if (key == Keys.DetectPlayers)
            {
                if (bool.TryParse(value, out var dp))
                {
                    detectPlayers = dp;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.DetectVehicles)
            {
                if (bool.TryParse(value, out var dv))
                {
                    detectVehicles = dv;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.DetectAnimals)
            {
                if (bool.TryParse(value, out var da))
                {
                    detectAnimals = da;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.CENTER)
            {
                if (value.ToVector3(out var point))
                {
                    center = point;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.RADIUS)
            {
                if (float.TryParse(value, out var r))
                {
                    radius = r;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            return EConfigUpdateResult.UndefinedKey;
        }
    }
}
