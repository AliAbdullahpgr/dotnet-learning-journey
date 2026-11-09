# Sorting Algorithms Basics

Sorting arranges elements into order (usually ascending). The main things to compare between sorting algorithms are time complexity, extra space, and stability — a stable sort keeps equal elements in their original relative order.

## Key points

- **Bubble sort — O(n^2):** repeatedly swaps adjacent elements that are out of order. Simple and stable, but slow on large inputs.
- **Selection sort — O(n^2):** repeatedly picks the smallest remaining element and moves it to the front. Uses few swaps, but is not stable.
- **Insertion sort — O(n^2) worst case, O(n) on nearly sorted data:** builds the sorted part one element at a time. Stable and efficient for small or nearly sorted inputs.
- **Merge sort — O(n log n):** splits the input in half, sorts each half, then merges. Stable, but needs O(n) extra space.
- **Quick sort — O(n log n) average, O(n^2) worst case:** partitions around a pivot and recurses. In-place and fast in practice, but not stable.
- **Heap sort — O(n log n):** builds a heap and repeatedly extracts the maximum. In-place, but not stable.
- In C#, `Array.Sort` uses introspective sort — quick sort that falls back to heap sort and insertion sort when that is better.

## Example

```csharp
static void InsertionSort(int[] values)
{
    for (int i = 1; i < values.Length; i++)
    {
        int key = values[i];
        int j = i - 1;
        while (j >= 0 && values[j] > key)
        {
            values[j + 1] = values[j];
            j--;
        }
        values[j + 1] = key;
    }
}
```

Insertion sort takes each new element and shifts larger elements right until it finds the element's place — the same way most people sort a hand of playing cards.
