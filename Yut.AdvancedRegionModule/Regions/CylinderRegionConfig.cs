using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    internal class CylinderRegionConfig : IRegionConfig
    {
        private bool detectPlayers;
        private bool detectVehicles;
        private bool detectAnimals;
        private Vector3 center = Vector3.zero;
        private float radius = 0;
        private bool is3D = false;
        private readonly float[] height = new float[2];
        /// <summary>
        /// 圆心
        /// </summary>
        public Vector3 Center => center;
        /// <summary>
        /// 半径
        /// </summary>
        public float Radius => radius;
        /// <summary>
        /// 高度数组
        /// </summary>
        public float[] Height => height;
        /// <summary>
        /// 是否检测高度
        /// </summary>
        public bool Is3D => is3D;
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
                    var x = reader.ReadSingle();
                    var y = reader.ReadSingle();
                    var z = reader.ReadSingle();
                    center = new Vector3(x, y, z);
                    radius = reader.ReadSingle();
                    is3D = reader.ReadBoolean();
                    height[0] = reader.ReadSingle();
                    height[1] = reader.ReadSingle();
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
                    writer.Write(is3D);
                    writer.Write(height[0]);
                    writer.Write(height[1]);
                }
                return stream.ToArray();
            }
        }
        public void Divest() { }
        public void Reset()
        {
            detectPlayers = false;
            detectVehicles = false;
            detectAnimals = false;
            center = Vector3.zero;
            radius = 0;
            is3D = false;
            height[0] = 0;
            height[1] = 0;
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
            else if (key == Keys.IS3D)
            {
                if (bool.TryParse(value, out var is3d))
                {
                    is3D = is3d;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.MINH)
            {
                if (float.TryParse(value, out var h))
                {
                    height[0] = h;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.MAXH)
            {
                if (float.TryParse(value, out var h))
                {
                    height[1] = h;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            return EConfigUpdateResult.UndefinedKey;
        }
        internal void GetLimitHeight(out float min, out float max)
        {
            if (height[0] < height[1])
            {
                min = height[0];
                max = height[1];
            }
            else
            {
                min = height[1];
                max = height[0];
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
            list.Add(Keys.IS3D + ":" + is3D);
            GetLimitHeight(out var min, out var max);
            list.Add(Keys.MINH + ":" + min);
            list.Add(Keys.MAXH + ":" + max);
            return list;
        }
    }
}
