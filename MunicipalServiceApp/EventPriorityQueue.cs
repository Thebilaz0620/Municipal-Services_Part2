using System;
using System.Collections.Generic;

namespace MunicipalServiceApp
{
    // Max-heap priority queue. Higher priority events are returned first.
    internal class EventPriorityQueue
    {
        private readonly List<LocalEvent> heap = new List<LocalEvent>();

        public int Count { get { return heap.Count; } }

        public void Enqueue(LocalEvent item)
        {
            if (item == null) return;

            heap.Add(item);
            int index = heap.Count - 1;

            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (Compare(heap[index], heap[parent]) <= 0)
                    break;

                Swap(index, parent);
                index = parent;
            }
        }

        public LocalEvent Dequeue()
        {
            if (heap.Count == 0)
                return null;

            LocalEvent result = heap[0];
            LocalEvent last = heap[heap.Count - 1];
            heap.RemoveAt(heap.Count - 1);

            if (heap.Count > 0)
            {
                heap[0] = last;
                int index = 0;

                while (true)
                {
                    int left = index * 2 + 1;
                    int right = index * 2 + 2;
                    int largest = index;

                    if (left < heap.Count && Compare(heap[left], heap[largest]) > 0)
                        largest = left;
                    if (right < heap.Count && Compare(heap[right], heap[largest]) > 0)
                        largest = right;

                    if (largest == index)
                        break;

                    Swap(index, largest);
                    index = largest;
                }
            }

            return result;
        }

        public LocalEvent Peek()
        {
            return heap.Count == 0 ? null : heap[0];
        }

        public List<LocalEvent> ToSortedList()
        {
            EventPriorityQueue copy = new EventPriorityQueue();
            foreach (LocalEvent item in heap)
                copy.Enqueue(item);

            List<LocalEvent> result = new List<LocalEvent>();
            while (copy.Count > 0)
                result.Add(copy.Dequeue());

            return result;
        }

        private static int Compare(LocalEvent first, LocalEvent second)
        {
            int priorityCompare = first.Priority.CompareTo(second.Priority);
            if (priorityCompare != 0)
                return priorityCompare;

            return second.EventDate.CompareTo(first.EventDate);
        }

        private void Swap(int first, int second)
        {
            LocalEvent temp = heap[first];
            heap[first] = heap[second];
            heap[second] = temp;
        }
    }
}
