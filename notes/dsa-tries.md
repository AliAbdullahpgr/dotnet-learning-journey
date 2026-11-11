# Tries (DSA)

A trie (prefix tree) stores strings by their shared prefixes: each edge is
one character, each node is a prefix, and a flag marks the end of a complete
word. Words that start the same way share the same path, so a trie is the
natural structure for anything prefix-shaped.

## Structure

```text
        (root)
       /   \
      c       d
      |       |
      a       o
      |       |
      t*      g*
```

`cat` and `dog` share only the root; `car` and `cat` would share the
`c → a` path and split at the third letter. The `*` marks a node that ends
a real word (`ca` is a prefix, not a word, unless it is marked).

## Operations

- **Insert** — walk (creating nodes as needed) and mark the last node:
  O(m), where m is the word length.
- **Search** — walk the characters; the word exists only if the walk
  succeeds *and* the last node is marked: O(m).
- **StartsWith** — walk the prefix; it exists if the walk succeeds,
  whether or not the node is marked: O(m).
- All three are independent of how many words are stored — a hash set
  can do search too, but it cannot do prefix queries.

```csharp
public class TrieNode
{
    public Dictionary<char, TrieNode> Children { get; } = new();
    public bool IsWord { get; set; }
}

public class Trie
{
    private readonly TrieNode _root = new();

    public void Insert(string word)
    {
        var node = _root;
        foreach (char c in word)
        {
            if (!node.Children.TryGetValue(c, out var next))
                node.Children[c] = next = new TrieNode();
            node = next;
        }
        node.IsWord = true;
    }

    public bool Search(string word)
        => Find(word)?.IsWord == true;

    public bool StartsWith(string prefix)
        => Find(prefix) is not null;

    private TrieNode? Find(string s)
    {
        var node = _root;
        foreach (char c in s)
            if (!node.Children.TryGetValue(c, out var next))
                return null;
            else
                node = next;
        return node;
    }
}
```

## Where tries shine

- **Autocomplete** — walk to the prefix node, then collect every word in
  its subtree.
- **Spell checking / word games** — walk the board's letters and prune
  the moment a prefix does not exist.
- **IP routing tables** — longest-prefix match over address bits.
- **T9 / predictive text** — digits map to letter sets; the trie prunes
  impossible words as the user types.

## Tries vs hash sets

- A hash set is simpler and uses less memory for plain membership tests.
- A trie wins when the workload is prefix-based (autocomplete, prefix
  counts, lexicographic listing) — a hash set cannot answer "all words
  starting with `ca`" without scanning everything.
- Cost: a trie node per character, so memory is the trade-off; a
  compressed trie (radix tree) merges single-child chains to save space.
