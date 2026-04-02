using System.Diagnostics;

using static UnityEngine.Debug;
using UObject = UnityEngine.Object;

namespace USP.Utility
{
      public static class Log
      {
            public static bool Enabled { get; set; } = true;


            private const string UNITY_EDITOR = "UNITY_EDITOR";
            [Conditional(UNITY_EDITOR)] internal static void Info(object message, UObject context = null) { if (Enabled) Log(message, context); }
            [Conditional(UNITY_EDITOR)] internal static void Warning(object message, UObject context = null) { if (Enabled) LogWarning(message, context); }
            [Conditional(UNITY_EDITOR)] internal static void Error(object message, UObject context = null) { if (Enabled) LogError(message, context); }
      }
}