using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;
namespace SupaStuff.Util
{
[Obsolete]
    internal static class ThreadManager
    {
        public static List<Thread> threads
        {
            get
            {
                return _threads;
            }
        }
        private static List<Thread> _threads = new List<Thread>();
        public static Thread GetThread(int index)
        {
            return threads[index];
        }
        public static void StartThread(System.Action action)
        {
            Thread thread = new Thread(() => threadStart(action));
            threads.Add(thread);
            thread.Start();

        }
        private static void threadStart(Action action)
        {
            action();
            if(_threads.Contains(Thread.CurrentThread)) threads.Remove(Thread.CurrentThread);
        }
    }
}