using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using UnityEngine;

namespace Yut.AdvancedRegionModule.Regions
{
    internal class PrismRegionConfig : IRegionConfig
    {
        private bool detectPlayers;
        private bool detectVehicles;
        private bool detectAnimals;
        private bool is3D = false;
        private readonly float[] height = new float[2];
        private readonly List<Vector3> points = new List<Vector3>();
        public bool DetectPlayers => detectPlayers;
        public bool DetectVehicles => detectVehicles;
        public bool DetectAnimals => detectAnimals;
        /// <summary>
        /// 是否检测高度
        /// </summary>
        public bool Is3D => is3D;
        /// <summary>
        /// 高度数组
        /// </summary>
        public float[] Height => height;
        /// <summary>
        /// 多边形边界点
        /// </summary>
        public ReadOnlyCollection<Vector3> Points => points.AsReadOnly();
        public void ConvertFromBytes(byte[] bytes)
        {
            using (var steam = new MemoryStream(bytes))
            {
                using (var reader = new BinaryReader(steam))
                {
                    detectPlayers = reader.ReadBoolean();
                    detectVehicles = reader.ReadBoolean();
                    detectAnimals = reader.ReadBoolean();
                    is3D = reader.ReadBoolean();
                    height[0] = reader.ReadSingle();
                    height[1] = reader.ReadSingle();
                    var num = (reader.BaseStream.Length - 9) / 8;
                    for (int i = 0; i < num; i++)
                    {
                        var x = reader.ReadSingle();
                        var z = reader.ReadSingle();
                        var point = new Vector3(x, 0, z);
                        points.Add(point);
                    }
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
                    writer.Write(is3D);
                    writer.Write(height[0]);
                    writer.Write(height[1]);
                    var count = points.Count;
                    for (int i = 0; i < count; i++)
                    {
                        var point = points[i];
                        writer.Write(point.x);
                        writer.Write(point.z);
                    }
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
            is3D = false;
            height[0] = 0;
            height[1] = 0;
            points.Clear();
        }
        public EConfigUpdateResult UpdateConfig(string key, string behaviour, string value)
        {
            if (key == Keys.DetectPlayers)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
                if (bool.TryParse(value, out var dp))
                {
                    detectPlayers = dp;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.DetectVehicles)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
                if (bool.TryParse(value, out var dv))
                {
                    detectVehicles = dv;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.DetectAnimals)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
                if (bool.TryParse(value, out var da))
                {
                    detectAnimals = da;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.POINTS)
            {
                if (behaviour == Keys.ADD)
                {
                    if (value.ToVector3(out var point))
                    {
                        points.Add(point);
                        return EConfigUpdateResult.Success;
                    }
                    return EConfigUpdateResult.InvalidValue;
                }
                else if (behaviour == Keys.REMOVE)
                {
                    if (value == Keys.LAST)
                    {
                        if (points.Count == 0)
                            return EConfigUpdateResult.RejectUpdate;
                        points.Remove(points[points.Count - 1]);
                        return EConfigUpdateResult.Success;
                    }
                    else
                    {
                        if (int.TryParse(value, out var index))
                        {
                            if (index >= points.Count)
                                return EConfigUpdateResult.InvalidValue;
                            points.RemoveAt(index);
                            return EConfigUpdateResult.Success;
                        }
                        return EConfigUpdateResult.InvalidValue;
                    }
                }
                else
                    return EConfigUpdateResult.BehaviourNotSupport;
            }
            else if (key == Keys.IS3D)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
                if (bool.TryParse(value, out var is3d))
                {
                    is3D = is3d;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.MINH)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
                if (float.TryParse(value, out var h))
                {
                    height[0] = h;
                    return EConfigUpdateResult.Success;
                }
                return EConfigUpdateResult.InvalidValue;
            }
            else if (key == Keys.MAXH)
            {
                if (behaviour != Keys.SET)
                    return EConfigUpdateResult.BehaviourNotSupport;
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
            list.Add(Keys.IS3D + ":" + is3D);
            GetLimitHeight(out var min, out var max);
            list.Add(Keys.MINH + ":" + min);
            list.Add(Keys.MAXH + ":" + max);
            for (var i = 0; i < points.Count; i++)
                list.Add(points[i].ToString());
            return list;
        }
    }
}
