# Binary Search Trees (DSA)

A binary search tree (BST) is a binary tree where every node follows one rule: values in the left subtree are smaller than the node's value, and values in the right subtree are larger. That single rule turns the tree into a searchable structure — lookup, insert, and delete all run in O(log n) on a balanced tree, and degrade to O(n) when the tree becomes a chain.

## The Node

A BST node holds a value and references to its left and right children. In C# this is a small class; the tree itself only needs a reference to the root node.

```csharp
public class TreeNode
{
    public int Value { get; set; }
    public TreeNode? Left { get; set; }
    public TreeNode? Right { get; set; }

    public TreeNode(int value) => Value = value;
}
```

## Insert

Insert walks down from the root, going left when the new value is smaller and right when it is larger, until it finds an empty spot. Duplicates are usually ignored or counted, not stored twice.

```csharp
public static TreeNode Insert(TreeNode? root, int value)
{
    if (root is null) return new TreeNode(value);

    if (value < root.Value)
        root.Left = Insert(root.Left, value);
    else if (value > root.Value)
        root.Right = Insert(root.Right, value);

    return root;
}
```

## Search

Search follows the same path: compare with the current node, go left or right, and stop when the value is found or a null child is reached.

```csharp
public static TreeNode? Search(TreeNode? root, int value)
{
    while (root is not null)
    {
        if (value == root.Value) return root;
        root = value < root.Value ? root.Left : root.Right;
    }
    return null;
}
```

## Delete

Delete has three cases. A leaf is simply removed. A node with one child is replaced by that child. A node with two children is replaced by its in-order successor — the smallest value in its right subtree — and that successor is then deleted from its old position.

```csharp
public static TreeNode? Delete(TreeNode? root, int value)
{
    if (root is null) return null;

    if (value < root.Value) root.Left = Delete(root.Left, value);
    else if (value > root.Value) root.Right = Delete(root.Right, value);
    else
    {
        if (root.Left is null) return root.Right;
        if (root.Right is null) return root.Left;

        TreeNode successor = MinValue(root.Right);
        root.Value = successor.Value;
        root.Right = Delete(root.Right, successor.Value);
    }
    return root;
}

private static TreeNode MinValue(TreeNode node)
{
    while (node.Left is not null) node = node.Left;
    return node;
}
```

## In-Order Traversal

Visiting the left subtree, then the node, then the right subtree prints the values in sorted order. This is the most useful BST traversal and the reason the structure is used for ordered data.

```csharp
public static void InOrder(TreeNode? root)
{
    if (root is null) return;
    InOrder(root.Left);
    Console.WriteLine(root.Value);
    InOrder(root.Right);
}
```

## Balanced vs Degenerate

A BST built from sorted input becomes a straight line — every operation costs O(n), no better than a linked list. A balanced tree keeps the height near log n. That is why real libraries rarely use a plain BST: AVL trees rebalance after every insert and delete by rotating nodes, and red-black trees keep the tree roughly balanced with looser rules and fewer rotations. In .NET, `SortedSet<T>` and `SortedDictionary<TKey, TValue>` are red-black trees under the hood, so prefer them over a hand-rolled BST in production code.
