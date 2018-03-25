using System.Collections.Generic;

namespace Compression.Queue
{
    /// <summary>
    /// Simple wrapper for the <see cref="Queue{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of queue.</typeparam>
    internal class SimpleQueue<T> : IQueue<T>
    {
        private readonly Queue<T> _queue = new Queue<T>();

        public int Count => _queue.Count;

        public void Push(T task)
        {
            _queue.Enqueue(task);
        }

        public T Pop()
        {
            return _queue.Dequeue();
        }

        public T Peek()
        {
            return _queue.Peek();
        }
    }
}
