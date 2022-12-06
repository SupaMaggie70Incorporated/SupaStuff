using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SupaStuff.Util
{
    /// <summary>
    /// Logger class. Create a new one with GetLogger(string name). To use with unity, use SupaStuff.Unity.dll and run SupaStuff.Unity.Main.Init
    /// </summary>
    public struct Logger
    {
        public readonly string Name;
        public static bool IsUnity { get; private set; }
        /// <summary>
        /// Get or create the logger with name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Logger GetLogger(string name)
        {
            return new Logger(name);
        }
        /// <summary>
        /// For use in unity projects. Use SupaStuff.Unity.Main.Init instead of doing this yourself.
        /// </summary>
        /// <param name="debug"></param>
        public static void SetUnity()
        {
            IsUnity = true;
        }
        /// <summary>
        /// Creates a logger with the name
        /// </summary>
        /// <param name="name"></param>
        private Logger(string name)
        {
            Name = name;
        }
        /// <summary>
        /// Logs the message
        /// </summary>
        /// <param name="contents"></param>
        public void Log(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) Debug.Log(message);
            else Console.WriteLine(message);
        }
        /// <summary>
        /// Logs the warning
        /// </summary>
        /// <param name="contents"></param>
        public void Warn(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) Debug.LogWarning("[WARNING]" + message);
            else Console.WriteLine(message);
        }
        /// <summary>
        /// Logs the error
        /// </summary>
        /// <param name="contents"></param>
        public void Error(object contents)
        {
            string message = "[" + Name + "] " + contents.ToString();
            if (IsUnity) Debug.LogError("[ERROR]" + message);
            else Console.WriteLine(message);
        }
    }
}