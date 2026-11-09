# Binary Tree Basics

A binary tree is a data structure in which each node stores a value and has at most two children, called the left child and the right child.

## Key points

- The top node is the root; a node with no children is a leaf.
- A node's depth is its distance from the root, and a tree's height is the longest root-to-leaf path.
- The three classic depth-first traversals are in-order (left, node, right), pre-order (node, left, right), and post-order (left, right, node).
- In a binary search tree (BST), every value in the left subtree is smaller than the node's value, and every value in the right subtree is larger.
- In-order traversal of a BST visits the values in sorted order.
- Searching a balanced BST takes O(log n) time, but a skewed tree that is really a chain degrades to O(n).
- Trees are a natural fit for hierarchical data, expression parsing, and sorted collections.

## Example

```csharp
class TreeNode
{
    public int Value;
    public TreeNode? Left, Right;
    public TreeNode(int value) => Value = value;
}

// In-order traversal of a small BST prints the values in order.
IEnumerable<int> InOrder(TreeNode? node)
{
    if (node is null) yield break;
    foreach (int value in InOrder(node.Left)) yield return value;
    yield return node.Value;
    foreach (int value in InOrder(node.Right)) yield return value;
}
// For a tree holding 2, 1 and 3, InOrder returns 1, 2, 3.
```
