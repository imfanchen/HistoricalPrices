namespace HistoricalPrices.Core {
    public class MaxQueue<T> where T : IComparable<T> {
        private readonly Queue<T> queue;
        private readonly LinkedList<T> maxQueue; // value is decreasing from left to right

        /// <summary>
        /// A generic queue that can return max value in constant time
        /// </summary>
        public MaxQueue() {
            queue = new Queue<T>();
            maxQueue = new LinkedList<T>();
        }

        public void Enqueue(T x) {
            queue.Enqueue(x);
            while (maxQueue.Count > 0 && maxQueue.Last?.Value.CompareTo(x) < 0) {
                maxQueue.RemoveLast();
            }
            maxQueue.AddLast(x);
        }

        public void Dequeue() {
            if (queue.Any()) {
                T x = queue.Dequeue();
                if (maxQueue.Count > 0 && maxQueue.First?.Value.CompareTo(x) == 0) {
                    maxQueue.RemoveFirst();
                }
            }
        }

        public T Peek() => queue.Peek();

        public T? GetMax() {
            LinkedListNode<T>? max = maxQueue.First;
            if (max != null) {
                return max.Value;
            }
            return default;
        }
    }
}
