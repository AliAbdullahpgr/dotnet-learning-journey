// Sorting algorithms in C#: bubble sort, insertion sort and quick sort.
// Bubble sort: O(n^2) — simple, only for learning or tiny inputs.
// Insertion sort: O(n^2) worst case, O(n) on nearly sorted data.
// Quick sort: O(n log n) average, O(n^2) worst case.
// In real code, prefer Array.Sort (introsort, O(n log n)).

using System;

public static class SortingExamples
{
    public static void BubbleSort(int[] arr)
    {
        for (int i = 0; i < arr.Length - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < arr.Length - 1 - i; j++)
            {
                if (arr[j] > arr[j + 1])
                {
                    (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
                    swapped = true;
                }
            }
            if (!swapped) break; // Already sorted.
        }
    }

    public static void InsertionSort(int[] arr)
    {
        for (int i = 1; i < arr.Length; i++)
        {
            int key = arr[i];
            int j = i - 1;
            while (j >= 0 && arr[j] > key)
            {
                arr[j + 1] = arr[j];
                j--;
            }
            arr[j + 1] = key;
        }
    }

    public static void QuickSort(int[] arr, int left, int right)
    {
        if (left >= right) return;

        int pivot = arr[(left + right) / 2];
        int i = left;
        int j = right;

        while (i <= j)
        {
            while (arr[i] < pivot) i++;
            while (arr[j] > pivot) j--;
            if (i <= j)
            {
                (arr[i], arr[j]) = (arr[j], arr[i]);
                i++;
                j--;
            }
        }

        if (left < j) QuickSort(arr, left, j);
        if (i < right) QuickSort(arr, i, right);
    }

    public static void Main()
    {
        int[] numbers = { 64, 34, 25, 12, 22, 11, 90 };

        int[] bubble = (int[])numbers.Clone();
        BubbleSort(bubble);
        Console.WriteLine("Bubble:    " + string.Join(", ", bubble));

        int[] insertion = (int[])numbers.Clone();
        InsertionSort(insertion);
        Console.WriteLine("Insertion: " + string.Join(", ", insertion));

        int[] quick = (int[])numbers.Clone();
        QuickSort(quick, 0, quick.Length - 1);
        Console.WriteLine("Quick:     " + string.Join(", ", quick));

        int[] builtIn = (int[])numbers.Clone();
        Array.Sort(builtIn);
        Console.WriteLine("Array.Sort:" + string.Join(", ", builtIn));
    }
}
