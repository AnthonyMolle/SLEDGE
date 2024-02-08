using System;

// minHeap implementation for AStarNodes
// Allows us to efficently find which node we should visit first
public class AStarHeap<T> where T : IHeapItem<T>
{
    T[] items; // Items in heap
    int currentItemCount; // # of items in heap

    // Create heap with set size
    public AStarHeap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }
    
    // Add item to minHeap sorted by compareTo implementation of T
    public void Add(T item)
    {
        // Add item to end of Heap
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;

        // Populate sort element up the heap until at right spot
        SortUp(item);

        // Increase # of items in heap
        currentItemCount++;
       
    }

    // Get item by index in heap
    public T GetItem(int index)
    {
        return items[index];
    }

    // Pop item from heap with lowest compareTo value
    public T RemoveFirst()
    {
        // Get head of heap
        T firstItem = items[0];

        // Decrease # of items in heap
        currentItemCount--;

        // Move back of heap to front
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;

        // Populate Sort element down the heap
        SortDown(items[0]);

        return firstItem;
    }

    // Update items position to match updates to heap
    public void UpdateItem(T item)
    {
        SortUp(item);
    }

    // Return true if item in heap, false otherwise
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }

    // Get # of items in heap
    public int Count
    {
        get{ return currentItemCount; }
    }

    // Take a item and sort it down the heap (smallest to largest)
    void SortDown(T item)
    {
        while (true)
        {
            // Get children of node
            int childIndexLeft = item.HeapIndex * 2 + 1;
            int childIndexRight = item.HeapIndex * 2 + 2;

            // Index to swap with
            int swapIndex = 0;

            // If left child exists...
            if (childIndexLeft < currentItemCount)
            {
                // Target left child for swap
                swapIndex = childIndexLeft;

                // If right child exists...
                if (childIndexRight < currentItemCount)
                {
                    // If right child is higher priority than left...
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        // Target right child for swap
                        swapIndex = childIndexRight;
                    }
                }

                // Check if swap target is higher priority than passed in item
                if (item.CompareTo(items[swapIndex]) < 0)
                {
                    // If so, swap with target
                    Swap(item, items[swapIndex]);
                }
                else
                {
                    // Otherwise, found correct spot and hault sort
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }

    // Take a item and sort up the heap (largest -> smallest)
    void SortUp(T item)
    {
        // Get parent of item
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parentItem = items[parentIndex];

            // Check if parent has lower priority than passed in item...
            if (item.CompareTo(parentItem) > 0)
            {
                // If lower priority, swap with item
                Swap(item, parentItem);
            }
            else
            {
                // Otherwise both parent and passed in item in right spot
                break;
            }

            // Update parent index
            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    // Swap a and b in heap
    void Swap(T a, T b)
    {
        items[a.HeapIndex] = b;
        items[b.HeapIndex] = a;
        int temp = a.HeapIndex;
        a.HeapIndex = b.HeapIndex;
        b.HeapIndex = temp;
    }
}

// All heap T types must implement IHeapItems interface
// Insures heapIndex is tracked by T type data and 
// insures compareTo implemented by T
public interface IHeapItem<T> : IComparable<T>
{
    int HeapIndex
    {
        get;
        set;
    }
}
