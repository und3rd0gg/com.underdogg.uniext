using System;
using UnityEngine;

namespace UniExt.Extensions {
    namespace _Color_sphere.CodeBase.Infrastructure.Utils {
        public static class DebugUtils {
            public static void Log(string message, DebugMessageType debugMessageType = default, DebugMessageLevel debugMessageLevel = default) {
                var debugMessage = debugMessageType switch {
                    DebugMessageType.GameEvent => $"<color=yellow>{nameof(DebugMessageType.GameEvent).ToUpper()}",
                    DebugMessageType.Analytics => $"<color=blue>{nameof(DebugMessageType.Analytics).ToUpper()}",
                    DebugMessageType.Performance => $"<color=purple>{nameof(DebugMessageType.Performance).ToUpper()}",
                    _ => throw new ArgumentOutOfRangeException(nameof(debugMessageType), debugMessageType, null)
                };

                debugMessage += ": </color>" + message;
            
                switch (debugMessageLevel) {
                    case DebugMessageLevel.Info:
                        Debug.Log(debugMessage); break;
                    case DebugMessageLevel.Warning:
                        Debug.LogWarning(debugMessage); break;
                    case DebugMessageLevel.Error:
                        Debug.LogError(debugMessage); break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(debugMessageLevel), debugMessageLevel, null);
                }
            }
        }
    
        public enum DebugMessageType {
            GameEvent,
            Performance,
            Analytics,
        }

        public enum DebugMessageLevel {
            Info,
            Warning,
            Error,
        }
    }

}