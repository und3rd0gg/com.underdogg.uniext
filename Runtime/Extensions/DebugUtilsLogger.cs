using System;
using UnityEngine;

namespace com.underdogg.uniext.Runtime.Extensions
{
    public static class DebugUtilsLogger
    {
        public static void Log(
            string message,
            DebugMessageType debugMessageType = DebugMessageType.GameEvent,
            DebugMessageLevel debugMessageLevel = DebugMessageLevel.Info)
        {
            var title = debugMessageType switch
            {
                DebugMessageType.GameEvent => "<color=yellow>GAMEEVENT</color>",
                DebugMessageType.Analytics => "<color=blue>ANALYTICS</color>",
                DebugMessageType.Performance => "<color=purple>PERFORMANCE</color>",
                _ => throw new ArgumentOutOfRangeException(nameof(debugMessageType), debugMessageType, null)
            };

            var debugMessage = $"{title}: {message ?? string.Empty}";

            switch (debugMessageLevel)
            {
                case DebugMessageLevel.Info:
                    Debug.Log(debugMessage);
                    break;
                case DebugMessageLevel.Warning:
                    Debug.LogWarning(debugMessage);
                    break;
                case DebugMessageLevel.Error:
                    Debug.LogError(debugMessage);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(debugMessageLevel), debugMessageLevel, null);
            }
        }
    }

    public enum DebugMessageType
    {
        GameEvent,
        Performance,
        Analytics
    }

    public enum DebugMessageLevel
    {
        Info,
        Warning,
        Error
    }
}
