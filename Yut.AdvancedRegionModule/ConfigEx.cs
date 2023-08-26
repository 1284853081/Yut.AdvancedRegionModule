using System.Text;
using UnityEngine;

namespace Yut.AdvancedRegionModule
{
    public static class ConfigEx
    {
        /// <summary>
        /// 将字符串转为Vector3
        /// </summary>
        public static bool ToVector3(this string str, out Vector3 vector)
        {
            vector = Vector3.zero;
            string[] args = str.Split(',');
            if (args.Length == 2)
            {
                if (float.TryParse(args[0], out var x) &&
                    float.TryParse(args[1], out var z))
                {
                    vector.x = x;
                    vector.z = z;
                    return true;
                }
            }
            else if (args.Length == 3)
            {
                if (float.TryParse(args[0], out var x) &&
                    float.TryParse(args[1], out var y) &&
                    float.TryParse(args[2], out var z))
                {
                    vector.x = x;
                    vector.y = y;
                    vector.z = z;
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 将Vecter3转为字符串
        /// </summary>
        public static string GetString(this Vector3 vector)
        {
            var sb = new StringBuilder();
            var str = ",";
            sb.Append(vector.x);
            sb.Append(str);
            sb.Append(vector.y);
            sb.Append(str);
            sb.Append(vector.z);
            return sb.ToString();
        }
    }
}
