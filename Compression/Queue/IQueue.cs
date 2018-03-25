namespace Compression.Queue
{
    internal interface IQueue<T>
    {
        int Count { get; }
        void Push(T item);
        T Pop();
        T Peek();
    }
}
