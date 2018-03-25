using System;
using System.Collections.Generic;
using System.Linq;
using Infrastructure.Interfaces;

namespace Compression.Queue
{
    /// <summary>
    /// Queue is sorted by item Id.
    /// </summary>
    internal class SortedQueue<T> : IQueue<T> where T : IIdentifiable
    {
        private readonly SortedDictionary<long, T> _values = new SortedDictionary<long, T>();

        public int Count => _values.Count;

        public void Push(T item)
        {
            _values.Add(item.Id, item);
        }

        public T Pop()
        {
            if (Count == 0)
                throw new InvalidOperationException(Messages.QueueIsEmpty);

            var result = _values.First();
            _values.Remove(result.Key);
            return result.Value;
        }

        public T Peek()
        {
            if (Count == 0)
                throw new InvalidOperationException(Messages.QueueIsEmpty);

            return _values.First().Value;
        }
    }
}
