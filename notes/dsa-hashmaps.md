# Hash Maps and Dictionaries

A hash map stores key-value pairs and finds a value by its key in roughly constant time, by turning the key into a number with a hash function.

## Key points

- A hash function maps a key to a bucket index; the value is stored in that bucket, so lookup does not need to scan the whole collection.
- Insert, lookup, and delete are O(1) on average. They degrade when many keys collide in the same bucket, so a good hash function spreads keys evenly.
- Collisions are handled by chaining (a list in each bucket) or by open addressing (probing for another empty slot).
- In C#, use `Dictionary<TKey, TValue>`; a key must be unique, and the key type needs consistent `GetHashCode` and `Equals` implementations.
- `TryGetValue` is the safe way to read a key that might be missing: it returns `false` instead of throwing `KeyNotFoundException`.
- Use the indexer `dict[key] = value` to add or replace an entry, and `Add` when a duplicate key should be an error.
- Iteration order is not guaranteed, so do not rely on a dictionary to keep keys sorted or in insertion order.
- Hash maps are the usual tool for counting, caching, grouping, and fast membership tests.

## Example

```csharp
var counts = new Dictionary<string, int>();

foreach (string word in new[] { "red", "blue", "red" })
{
    counts[word] = counts.TryGetValue(word, out int count) ? count + 1 : 1;
}

Console.WriteLine(counts["red"]); // 2
```
