using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Stashbox.Utils.Data.Concurrent;

internal class ConcurrentList<T> : IList<T>, IReadOnlyList<T>
{
    private readonly SemaphoreSlim _lock = new(1, 1);
    private readonly List<T> _list;

    /// <inheritdoc/>
    public T this[int index]
    {
        get => _list[index];
        set
        {
            _list[index] = value;
        }
    }

    /// <inheritdoc/>
    public int Count => _list.Count;

    /// <inheritdoc/>
    public bool IsReadOnly => false;

    /// <summary>
    /// Conctructs a new instance of <see cref="ConcurrentList{T}"/>.
    /// </summary>
    public ConcurrentList()
    {
        _list = new List<T>();
    }

    /// <summary>
    /// Conctructs a new instance of <see cref="ConcurrentList{T}"/>.
    /// </summary>
    /// <param name="capacity"></param>
    public ConcurrentList(int capacity)
    {
        _list = new List<T>(capacity);
    }

    /// <inheritdoc/>
    public void Add(T item)
    {
        _lock.Wait();

        try
        {
            _list.Add(item);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public void Clear()
    {
        _lock.Wait();

        try
        {
            _list.Clear();
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public void Insert(int index, T item)
    {
        _lock.Wait();

        try
        {
            _list.Insert(index, item);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public bool Remove(T item)
    {
        _lock.Wait();

        try
        {
            return _list.Remove(item);
        }
        finally
        {
            _lock.Release();
        }
    }

    /// <inheritdoc/>
    public void RemoveAt(int index)
    {
        _lock.Wait();

        try
        {
            _list.RemoveAt(index);
        }
        finally
        {
            _lock.Release();
        }
    }

    public bool Contains(T item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public int IndexOf(T item)
    {
        return _list.IndexOf(item);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}
