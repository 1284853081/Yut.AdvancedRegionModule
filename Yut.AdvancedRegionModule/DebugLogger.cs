using System;
using System.Diagnostics;

namespace Yut.AdvancedRegionModule
{
    internal static class DebugLogger
    {
        public static void Log(string message)
        {
            var trace = new StackTrace();
            var frame = trace.GetFrame(1);
            var method = frame.GetMethod();
            Console.WriteLine($"[{method.DeclaringType.Name}]->[{method.Name}]>>{message}");
        }
    }
}
