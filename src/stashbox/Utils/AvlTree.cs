namespace Stashbox.Utils
{
    public class AvlTree<TKey, TValue>
    {
        private readonly int storedHash;
        private readonly TValue storedValue;

        private readonly AvlTree<TKey, TValue> leftNode;
        private readonly AvlTree<TKey, TValue> rightNode;

        private readonly int height;
        private readonly bool isEmpty;

        private AvlTree(int hash, TValue value, AvlTree<TKey, TValue> left, AvlTree<TKey, TValue> right)
        {
            this.storedHash = hash;
            this.leftNode = left;
            this.rightNode = right;
            this.storedValue = value;
            this.isEmpty = false;
            this.height = 1 + (left.height > right.height ? left.height : right.height);
        }

        private AvlTree(int hash, TValue value)
            : this(hash, value, new AvlTree<TKey, TValue>(), new AvlTree<TKey, TValue>())
        {
        }

        public AvlTree()
        {
            this.isEmpty = true;
        }

        public AvlTree<TKey, TValue> Add(TKey key, TValue value)
        {
            var hash = key.GetHashCode();
            return this.Add(hash, value);
        }

        private AvlTree<TKey, TValue> Add(int hash, TValue value)
        {
            if (this.isEmpty)
                return new AvlTree<TKey, TValue>(hash, value);

            if (hash == this.storedHash)
                return new AvlTree<TKey, TValue>(hash, value, this.leftNode, this.rightNode);

            var result = hash < this.storedHash
                ? this.SelfCopy(this.leftNode.Add(hash, value), this.rightNode)
                : this.SelfCopy(this.leftNode, this.rightNode.Add(hash, value));

            return result.Balance();
        }

        public TValue Get(TKey key)
        {
            var hash = key.GetHashCode();

            var root = this;
            while (!root.isEmpty && root.storedHash != hash)
                root = hash < root.storedHash ? root.leftNode : root.rightNode;
            return !root.isEmpty ? root.storedValue : default(TValue);
        }

        private AvlTree<TKey, TValue> Balance()
        {
            var balance = this.GetBalance();
            if (balance >= 2)
                return this.leftNode.GetBalance() == -1 ? this.RotateLeftRight() : this.RotateRight();

            if (balance <= -2)
                return this.rightNode.GetBalance() == 1 ? this.RotateRightLeft() : this.RotateLeft();

            return this;
        }

        private AvlTree<TKey, TValue> RotateLeft()
        {
            return this.rightNode.SelfCopy(this.SelfCopy(this.leftNode, this.rightNode.leftNode), this.rightNode.rightNode);
        }

        private AvlTree<TKey, TValue> RotateRight()
        {
            return this.leftNode.SelfCopy(this.leftNode.leftNode, this.SelfCopy(this.leftNode.rightNode, this.rightNode));
        }

        private AvlTree<TKey, TValue> RotateRightLeft()
        {
            return this.SelfCopy(this.leftNode, this.rightNode.RotateRight()).RotateLeft();
        }

        private AvlTree<TKey, TValue> RotateLeftRight()
        {
            return this.SelfCopy(this.leftNode.RotateLeft(), this.rightNode).RotateRight();
        }

        private AvlTree<TKey, TValue> SelfCopy(AvlTree<TKey, TValue> left, AvlTree<TKey, TValue> right) =>
            new AvlTree<TKey, TValue>(this.storedHash, this.storedValue, left, right);

        private int GetBalance() => this.leftNode.height - this.rightNode.height;
    }
}
