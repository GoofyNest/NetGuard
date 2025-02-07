using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetGuard.Engine
{
    public class FixedSizeQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly int _maxSize;

        public FixedSizeQueue(int maxSize)
        {
            _maxSize = maxSize;
        }

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
            return _queue.ToArray();
        }
    }
}