using System;
using System.Threading;

namespace Compression
{
    public static class ThreadStarter
    {
        public static Thread StartThread(Action action)
        {
            var newThread = new Thread(() =>
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    throw;
                }
            });
            newThread.Start();
            return newThread;
        }
    }
}
