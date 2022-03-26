namespace HistoricalPrices.Core {
    public class MinQueue<T> where T : IComparable<T> {
        private readonly Queue<T> queue;
        private readonly LinkedList<T> minQueue; // value is increasing from left to right

        /// <summary>
        /// A generic queue that can return min value in constant time
        /// </summary>
        public MinQueue() {
            queue = new Queue<T>();
            minQueue = new LinkedList<T>();
        }

        public void Enqueue(T x) {
            queue.Enqueue(x);
            while (minQueue.Count > 0 && minQueue.Last?.Value.CompareTo(x) > 0) {
                minQueue.RemoveLast();
            }
            minQueue.AddLast(x);
        }

        public void Dequeue() {
            if (queue.Any()) {
                T x = queue.Dequeue();
                if (minQueue.Count > 0 && minQueue.First?.Value.CompareTo(x) == 0) {
                    minQueue.RemoveFirst();
                }
            }
        }

        public T Peek() => queue.Peek();

        public T? GetMin() {
            LinkedListNode<T>? min = minQueue.First;
            if (min != null) {
                return min.Value;
            }
            return default;
        }
    }
}
