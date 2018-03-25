using System;
using System.Threading;

namespace Compression.Queue
{
    /// <summary>
    /// Wrapper which implements thread-safe work with a queue.
    /// </summary>
    /// <typeparam name="T">Type of queue items</typeparam>
    internal class SharedQueue<T> : ISharedQueue<T> where T : class
    {
        private readonly int _size;
        private readonly IQueue<T> _queue;
        private readonly object _locker = new object();
        private int _activeWritersNumber;

        public int Count => _queue.Count;
        public bool IsActive { get; private set; } = true;

        /// <param name="queue">The queue which will be used as a shared.</param>
        /// <param name="size">Queue size.</param>
        /// <param name="writersNumber">The number of thread-writers which will use it.</param>
        public SharedQueue(IQueue<T> queue, int size = Int16.MaxValue, int writersNumber = 1)
        {
            _size = size;
            _queue = queue;
            _activeWritersNumber = writersNumber;
        }

        public void Push(T task)
        {
            lock (_locker)
            {
                if (!IsActive)
                    throw new InvalidOperationException(Messages.QueueWasStopped);

                while (_queue.Count == _size)
                    Monitor.Wait(_locker);

                _queue.Push(task);
                Monitor.Pulse(_locker);
            }
        }

        public T Pop()
        {
            lock (_locker)
            {
                while (_queue.Count == 0 && IsActive)
                    Monitor.Wait(_locker);

                var result = _queue.Count == 0
                    ? null
                    : _queue.Pop();

                Monitor.Pulse(_locker);
                return result;
            }
        }

        public T Peek()
        {
            lock (_locker)
            {
                if (_queue.Count == 0)
                    return null;

                return _queue.Peek();
            }
        }

        /// <summary>
        /// Notifies that writer's adding is complete. 
        /// When all writers have finished adding, the queue becomes inactive and closed for writing.
        /// </summary>
        public void StopWrite()
        {
            lock (_locker)
            {
                if (--_activeWritersNumber == 0)
                    IsActive = false;
                Monitor.PulseAll(_locker);
            }
        }
    }
}
