#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEVELOPMENT
#endif // DEVELOPMENT

using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace RuleScript
{
    static internal class Log
    {
        static private StringBuilder s_CachedStringBuilder = new StringBuilder(1024);

        /// <summary>
        /// Logs out to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Msg(string inMessage)
        {
            UnityEngine.Debug.Log(inMessage);
        }

        /// <summary>
        /// Logs out to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Msg(string inMessage, params object[] inMessageParams)
        {
            s_CachedStringBuilder.AppendFormat(inMessage, inMessageParams);
            UnityEngine.Debug.Log(s_CachedStringBuilder.ToString());
            s_CachedStringBuilder.Length = 0;
        }

        /// <summary>
        /// Logs a warning to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Warn(string inMessage)
        {
            UnityEngine.Debug.LogWarning(inMessage);
        }

        /// <summary>
        /// Logs a warning to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Warn(string inMessage, params object[] inMessageParams)
        {
            s_CachedStringBuilder.AppendFormat(inMessage, inMessageParams);
            UnityEngine.Debug.LogWarning(s_CachedStringBuilder.ToString());
            s_CachedStringBuilder.Length = 0;
        }

        /// <summary>
        /// Logs an error to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Error(string inMessage)
        {
            UnityEngine.Debug.LogError(inMessage);
        }

        /// <summary>
        /// Logs an error to the console.
        /// </summary>
        [Conditional("DEVELOPMENT")]
        [DebuggerHidden]
        [DebuggerStepThrough]
        static public void Error(string inMessage, params object[] inMessageParams)
        {
            s_CachedStringBuilder.AppendFormat(inMessage, inMessageParams);
            UnityEngine.Debug.LogError(s_CachedStringBuilder.ToString());
            s_CachedStringBuilder.Length = 0;
        }
    }
}