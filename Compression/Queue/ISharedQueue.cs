namespace Compression.Queue
{
    internal interface ISharedQueue<T> : IQueue<T> where T : class
    {
        bool IsActive { get; }
        void StopWrite();
    }
}