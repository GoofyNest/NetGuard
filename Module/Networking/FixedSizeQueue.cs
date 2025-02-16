using System.Collections.Concurrent;

namespace Module.Networking
{
    public class FixedSizeQueue<T>(int maxSize)
    {
        private readonly ConcurrentQueue<T> _queue = new();
        private readonly int _maxSize = maxSize;

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            while (_queue.Count > _maxSize)
            {
                _queue.TryDequeue(out _);
            }
        }

        public IEnumerable<T> GetAllItems()
        {
            return [.. _queue];
        }
    }
}