using System.Collections.Concurrent;

namespace Module.Engine.Classes
{
    public class FixedSizeQueue<T>
    {
        private readonly ConcurrentQueue<T> _queue = new();
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