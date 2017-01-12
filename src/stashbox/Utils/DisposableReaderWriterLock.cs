using System;
using System.Threading;

namespace Stashbox.Utils
{
    /// <summary>
    /// Represents a disposable reader writer lock.
    /// </summary>
    public class DisposableReaderWriterLock
    {
        private readonly ReaderWriterLockSlim lockSlim;

        /// <summary>
        /// Constructs a DisposableReaderWriterLock.
        /// </summary>
        /// <param name="recursionPolicy">Policy used for the instantination of the internal ReaderWriterLockSlim.</param>
        public DisposableReaderWriterLock(LockRecursionPolicy recursionPolicy)
        {
            this.lockSlim = new ReaderWriterLockSlim(recursionPolicy);
        }

        /// <summary>
        /// Constructs a DisposableReaderWriterLock with LockRecursionPolicy.NoRecursion.
        /// </summary>
        public DisposableReaderWriterLock()
            : this(LockRecursionPolicy.NoRecursion)
        {
        }

        /// <summary>
        /// Acquires a read lock.
        /// </summary>
        /// <returns>A lock releaser objects, which will release the lock when it's dispose is called.</returns>
        public LockReleaser AcquireReadLock()
        {
            return new LockReleaser(this.lockSlim, true);
        }

        /// <summary>
        /// Acquires a write lock.
        /// </summary>
        /// <returns>A lock releaser objects, which will release the lock when it's dispose is called.</returns>
        public LockReleaser AcquireWriteLock()
        {
            return new LockReleaser(this.lockSlim, false);
        }

        /// <summary>
        /// Acquires an upgradeable read lock.
        /// </summary>
        /// <returns>A lock releaser objects, which will release the lock when it's dispose is called.</returns>
        public UpgreadableLockReleaser AcquireUpgreadeableReadLock()
        {
            return new UpgreadableLockReleaser(this.lockSlim);
        }
    }

    /// <summary>
    /// Represents a lock releaser object
    /// </summary>
    public class LockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim lockSlim;
        private readonly bool isRead;

        internal LockReleaser(ReaderWriterLockSlim lockSlim, bool isRead)
        {
            this.lockSlim = lockSlim;
            this.isRead = isRead;

            if (isRead)
                this.lockSlim.EnterReadLock();
            else
                this.lockSlim.EnterWriteLock();
        }

        /// <summary>
        /// Releases the acquired resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the acquired resources.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing || this.lockSlim == null) return;
            if (isRead && this.lockSlim.IsReadLockHeld)
                this.lockSlim.ExitReadLock();
            else if (this.lockSlim.IsWriteLockHeld)
                this.lockSlim.ExitWriteLock();
        }
    }

    /// <summary>
    /// Represents an upgradeable lock releaser object.
    /// </summary>
    public class UpgreadableLockReleaser : IDisposable
    {
        private readonly ReaderWriterLockSlim lockSlim;
        internal UpgreadableLockReleaser(ReaderWriterLockSlim lockSlim)
        {
            this.lockSlim = lockSlim;
            this.lockSlim.EnterUpgradeableReadLock();
        }

        /// <summary>
        /// Acquires a write lock.
        /// </summary>
        /// <returns>A lock releaser objects, which will release the lock when it's dispose is called.</returns>
        public LockReleaser AcquireWriteLock()
        {
            return new LockReleaser(this.lockSlim, false);
        }

        /// <summary>
        /// Releases the acquired resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the acquired resources.
        /// </summary>
        protected virtual void Dispose(bool isDisposing)
        {
            if (!isDisposing || this.lockSlim == null || !this.lockSlim.IsUpgradeableReadLockHeld) return;
            this.lockSlim.ExitUpgradeableReadLock();
        }
    }
}
