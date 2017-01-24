namespace Stashbox.Utils
{
    internal class TreeNode<TValue>
    {
        public int Key;
        public TValue Value;
        public TreeNode<TValue> Parent;
        public TreeNode<TValue> Left;
        public TreeNode<TValue> Right;
        public sbyte Balance;

        public TreeNode(TValue value, int key, TreeNode<TValue> parent = null)
        {
            this.Value = value;
            this.Parent = parent;
            this.Key = key;
        }
    }

    public class AvlTree<TKey, TValue>
    {
        private TreeNode<TValue> root;

        public bool Search(TKey key, out TValue value)
        {
            TreeNode<TValue> node = root;
            var k = key.GetHashCode();

            while (node != null)
            {
                if (k < node.Key)
                    node = node.Left;
                else if (k > node.Key)
                    node = node.Right;
                else
                {
                    value = node.Value;
                    return true;
                }
            }

            value = default(TValue);
            return false;
        }

        public void Add(TKey key, TValue value)
        {
            var hash = key.GetHashCode();

            if (root == null)
            {
                root = new TreeNode<TValue>(value, hash);
                return;
            }

            var node = root;

            while (node != null)
            {
                if (hash < node.Key)
                    if (node.Left == null)
                    {
                        node.Left = new TreeNode<TValue>(value, hash, node);
                        node.Balance--;

                        this.BalanceNode(node);
                        return;
                    }
                    else
                        node = node.Left;
                else if (hash > node.Key)
                    if (node.Right == null)
                    {
                        node.Right = new TreeNode<TValue>(value, hash, node);
                        node.Balance++;

                        this.BalanceNode(node);
                        return;
                    }
                    else
                        node = node.Right;
                else
                    return; //key found
            }
        }

        private void BalanceNode(TreeNode<TValue> node)
        {
            while ((node.Balance != 0) && (node.Parent != null))
            {
                if (node.Parent.Left == node)
                    node.Parent.Balance--;
                else
                    node.Parent.Balance++;

                node = node.Parent;

                if (node.Balance == -2)
                {
                    if (node.Left.Balance == -1)
                        this.RotateLeft(node);
                    else
                        this.RotateRightLeft(node);

                    break;
                }
                else if (node.Balance == 2)
                {
                    if (node.Right.Balance == 1)
                        this.RotateRight(node);
                    else
                        this.RotateLeftRight(node);

                    break;
                }
            }
        }

        private void RotateLeft(TreeNode<TValue> node)
        {
            var left = node.Left;
            left.Parent = node.Parent;

            if (node.Parent == null)
                this.root = left;
            else
            {
                if (node.Parent.Left == node)
                    node.Parent.Left = left;
                else
                    node.Parent.Right = left;
            }

            node.Left = left.Right;

            if (node.Left != null)
                node.Left.Parent = node;

            left.Right = node;
            node.Parent = left;

            left.Balance = 0;
            node.Balance = 0;
        }

        private void RotateRight(TreeNode<TValue> node)
        {
            var right = node.Right;
            right.Parent = node.Parent;

            if (node.Parent == null)
                this.root = right;
            else
            {
                if (node.Parent.Left == node)
                    node.Parent.Left = right;
                else
                    node.Parent.Right = right;
            }

            node.Right = right.Left;

            if (node.Right != null)
                node.Right.Parent = node;

            right.Left = node;
            node.Parent = right;

            right.Balance = 0;
            node.Balance = 0;
        }

        private void RotateRightLeft(TreeNode<TValue> node)
        {
            var left = node.Left;
            var right = left.Right;
            right.Parent = node.Parent;

            if (node.Parent == null)
                this.root = right;
            else
            {
                if (node.Parent.Left == node)
                    node.Parent.Left = right;
                else
                    node.Parent.Right = right;
            }

            left.Right = right.Left;

            if (left.Right != null)
                left.Right.Parent = left;

            node.Left = right.Right;

            if (node.Left != null)
                node.Left.Parent = node;

            right.Left = left;
            right.Right = node;

            left.Parent = right;
            node.Parent = right;

            if (right.Balance == -1)
            {
                left.Balance = 0;
                node.Balance = 1;
            }
            else if (right.Balance == 0)
            {
                left.Balance = 0;
                node.Balance = 0;
            }
            else
            {
                left.Balance = -1;
                node.Balance = 0;
            }

            right.Balance = 0;
        }

        private void RotateLeftRight(TreeNode<TValue> node)
        {
            var right = node.Right;
            var left = right.Left;
            left.Parent = node.Parent;

            if (node.Parent == null)
                this.root = left;
            else
            {
                if (node.Parent.Left == node)
                    node.Parent.Left = left;
                else
                    node.Parent.Right = left;
            }

            right.Left = left.Right;

            if (right.Left != null)
                right.Left.Parent = right;

            node.Right = left.Left;

            if (node.Right != null)
                node.Right.Parent = node;

            left.Right = right;
            left.Left = node;

            right.Parent = left;
            node.Parent = left;

            if (left.Balance == 1)
            {
                right.Balance = 0;
                node.Balance = -1;
            }
            else if (left.Balance == 0)
            {
                right.Balance = 0;
                node.Balance = 0;
            }
            else
            {
                right.Balance = 1;
                node.Balance = 0;
            }

            left.Balance = 0;
        }
    }
}
