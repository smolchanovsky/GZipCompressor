using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Copied from MoreLINQ repository: https://github.com/morelinq/MoreLINQ
    /// </summary>
    public static class EnumerableExtensions
    {
        public static IEnumerable<TSource> TakeLast<TSource>(this IEnumerable<TSource> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            return source.TryGetCollectionCount() is int collectionCount
                ? source.Slice(Math.Max(0, collectionCount - count), int.MaxValue)
                : _();

            IEnumerable<TSource> _()
            {
                if (count <= 0)
                    yield break;

                var q = new Queue<TSource>(count);

                foreach (var item in source)
                {
                    if (q.Count == count)
                        q.Dequeue();
                    q.Enqueue(item);
                }

                foreach (var item in q)
                    yield return item;
            }
        }

        public static IEnumerable<T> SkipLast<T>(this IEnumerable<T> source, int count)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            if (count < 1)
                return source;

            return source.TryGetCollectionCount() is int collectionCount
                ? source.Take(collectionCount - count)
                : _();

            IEnumerable<T> _()
            {
                var queue = new Queue<T>(count);

                foreach (var item in source)
                {
                    if (queue.Count < count)
                    {
                        queue.Enqueue(item);
                        continue;
                    }

                    yield return queue.Dequeue();
                    queue.Enqueue(item);
                }
            }
        }

        public static IEnumerable<T> Slice<T>(this IEnumerable<T> sequence, int startIndex, int count)
        {
            if (sequence == null)
                throw new ArgumentNullException(nameof(sequence));
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            return sequence is IList<T> list 
                ? SliceList(list.Count, i => list[i])
                : sequence is IReadOnlyList<T> readOnlyList 
                    ? SliceList(readOnlyList.Count, i => readOnlyList[i])
                    : sequence.Skip(startIndex).Take(count);

            IEnumerable<T> SliceList(int listCount, Func<int, T> indexer)
            {
                var countdown = count;
                var index = startIndex;
                while (index < listCount && countdown-- > 0)
                    yield return indexer(index++);
            }
        }

        private static int? TryGetCollectionCount<T>(this IEnumerable<T> source)
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            return source is ICollection<T> collection ? collection.Count
                : source is IReadOnlyCollection<T> readOnlyCollection 
                    ? readOnlyCollection.Count
                    : (int?)null;
        }
    }
}
