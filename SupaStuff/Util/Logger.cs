using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SupaStuff.Util
{
    /// <summary>
    /// Logger class. Create a new one with GetLogger(string name). To use with unity, use SupaStuff.Unity.dll and run SupaStuff.Unity.Main.Init
    /// </summary>
    public class Logger
    {
        public readonly string Name;
        private static Dictionary<string, Logger> s_loggers = new Dictionary<string, Logger>();
        /// <summary>
        /// Contains the methods for logging in unity
        /// </summary>
        public struct UnityDebug
        {
            /// <summary>
            /// Delegate for unity based logging
            /// </summary>
            /// <param name="message"></param>
            public delegate void LogDel(object message);
            /// <summary>
            /// Delegate for unity based logging with context
            /// </summary>
            /// <param name="message"></param>
            /// <param name="context"></param>
            public delegate void LogDelContext(object message,object context);
            /// <summary>
            /// Logs the message
            /// </summary>
            public static LogDel Log { get; internal set; }
            /// <summary>
            /// Logs the message with context
            /// </summary>
            public static LogDelContext LogContext { get; internal set; }
            /// <summary>
            /// Logs the warning
            /// </summary>
            public static LogDel LogWarning { get; internal set; }
            /// <summary>
            /// Logs the warning with context
            /// </summary>
            public static LogDelContext LogWarningContext { get; internal set; }
            /// <summary>
            /// Logs the error
            /// </summary>
            public static LogDel LogError { get; internal set; }
            /// <summary>
            /// Logs the error with context
            /// </summary>
            public static LogDelContext LogErrorContext { get; internal set; }
        }
        public static bool IsUnity { get; private set; }
        /// <summary>
        /// Get or create the logger with name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Logger GetLogger(string name)
        {
            Logger logger;
            if (LoggerExists(name, out logger))
            {
                return logger;
            }
            else
            {
                return new Logger(name);
            }
        }
        /// <summary>
        /// For use in unity projects. Use SupaStuff.Unity.Main.Init instead of doing this yourself.
        /// </summary>
        /// <param name="debug"></param>
        public static void SetUnity(UnityDebug.LogDel log,UnityDebug.LogDelContext logContext, UnityDebug.LogDel warn, UnityDebug.LogDelContext warnContext, UnityDebug.LogDel error, UnityDebug.LogDelContext errorContext)
        {
            UnityDebug.Log = log;
            UnityDebug.LogContext = logContext;
            UnityDebug.LogWarning = warn;
            UnityDebug.LogError = error;
            UnityDebug.LogErrorContext = errorContext;
            UnityDebug.LogWarningContext = warnContext;
            IsUnity = true;
        }
        /// <summary>
        /// Creates a logger with the name
        /// </summary>
        /// <param name="name"></param>
        private Logger(string name)
        {
            this.Name = name;
            s_loggers.Add(name, this);
        }
        /// <summary>
        /// Checks if the logger exists, and returns it if so
        /// </summary>
        /// <param name="name"></param>
        /// <param name="_logger"></param>
        /// <returns></returns>
        private static bool LoggerExists(string name, out Logger _logger)
        {
            return s_loggers.TryGetValue(name, out _logger);
        }
        /// <summary>
        /// Logs the message
        /// </summary>
        /// <param name="contents"></param>
        public void Log(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) UnityDebug.Log(message);
            else Console.WriteLine(message);
        }
        /// <summary>
        /// Logs the warning
        /// </summary>
        /// <param name="contents"></param>
        public void Warn(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) UnityDebug.LogWarning("[WARNING]"+message);
            else Console.WriteLine(message);
        }
        /// <summary>
        /// Logs the error
        /// </summary>
        /// <param name="contents"></param>
        public void Error(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) UnityDebug.LogError("[ERROR]"+message);
            else Console.WriteLine(message);
        }
        /// <summary>
        /// Logs the message with context(unity only)
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="context"></param>
        public void Log(object contents, object context)
        {
            if (!IsUnity) throw LogException.NotUnity();
            UnityDebug.LogContext("[" + Name + "] " + contents.ToString(), context);
        }
        /// <summary>
        /// Logs the warning with context(unity only)
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="context"></param>
        public void Warn(object contents, object context)
        {
            if (!IsUnity) throw LogException.NotUnity();
            UnityDebug.LogWarningContext("[WARNING][" + Name + "] " + contents.ToString(), context);
        }
        /// <summary>
        /// Logs the error with context(unity only)
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="context"></param>
        public void Error(object contents, object context)
        {
            if (!IsUnity) throw LogException.NotUnity();
            UnityDebug.LogErrorContext("[ERROR][" + Name + "] " + contents.ToString(), context);
        }
    }
}