using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RaCollection
{
	public delegate void ItemHandler<TItem>(TItem item);
	public delegate void ItemsHandler<TItem>(TItem newItem, TItem oldItem);
	public delegate void ItemIndexHandler<TItem>(TItem item, int index);

	[Serializable]
	public class RaCollection<TItem> : IList<TItem>, IReadOnlyRaCollection<TItem>, IDisposable
	{
		public event ItemIndexHandler<TItem> AddedItemEvent;
		public event ItemIndexHandler<TItem> RemovedItemEvent;
		public event Action DirtyEvent;

		[SerializeField]
		private List<TItem> _items = new List<TItem>();
		private ItemIndexHandler<TItem> _onAddItem = null;
		private ItemIndexHandler<TItem> _onRemoveItem = null;

		public int Count => _items.Count;

		public bool IsReadOnly => false;
		
		public bool IsDisposed
		{
			get; private set;
		} 

		public RaCollection(ItemIndexHandler<TItem> onAddItem, ItemIndexHandler<TItem> onRemoveItem)
		{
			_onAddItem = onAddItem;
			_onRemoveItem = onRemoveItem;

			TItem[] itemsToAdd = _items.ToArray();
			_items.Clear();

			for (int i = 0, c = itemsToAdd.Length; i < c; i++)
			{
				Add(itemsToAdd[i]);
			}
		}

		public RaCollection()
			: this(null, null)
		{

		}

		public RaCollection(IList<TItem> items, ItemIndexHandler<TItem> onAddItem = null, ItemIndexHandler<TItem> onRemoveItem = null)
			: this(onAddItem, onRemoveItem)
		{
			if(items != null && items.Count > 0)
			{
				for(int i = 0, c = items.Count; i < c; i++)
				{
					Add(items[i]);
				}
			}
		}

		#region Queue

		public void Enqueue(TItem item)
		{
			Insert(0, item);
		}

		public TItem Dequeue()
		{
			TryRemoveAt(0, out TItem item, throwIndexOutOfRangeException: true);
			return item;
		}

		public bool TryDequeue(out TItem item)
		{
			return TryRemoveAt(0, out item);
		}

		public TItem PeekQueue()
		{
			return _items[0];
		}

		public bool TryPeekQueue(out TItem item)
		{
			if (Count > 0)
			{
				item = _items[0];
				return true;
			}

			item = default;
			return false;
		}

		#endregion

		#region Stack

		public void Push(TItem item)
		{
			Add(item);
		}

		public TItem Pop()
		{
			TryRemoveAt(Count - 1, out TItem item, throwIndexOutOfRangeException: true);
			return item;
		}

		public bool TryPop(out TItem item)
		{
			return TryRemoveAt(Count - 1, out item);
		}

		public TItem PeekStack()
		{
			return _items[Count - 1];
		}

		public bool TryPeekStack(out TItem item)
		{
			if(Count > 0)
			{
				item = _items[Count - 1];
				return true;
			}

			item = default;
			return false;
		}

		#endregion

		#region Helper

		public bool TryGetItem<T>(out T item, Predicate<T> predicate = null)
			where T : TItem
		{
			return RaCollectionUtils.TryGetItem(this, out item, predicate);
		}

		public List<T> GetItems<T>(Predicate<T> predicate)
			where T : TItem
		{
			return RaCollectionUtils.GetItems(this, predicate);
		}

		public bool TryFindItem<T>(out T item, Predicate<T> predicate = null)
		{
			return RaCollectionUtils.TryFindItem(this, out item, predicate);
		}

		public List<T> FindItems<T>(Predicate<T> predicate)
		{
			return RaCollectionUtils.FindItems(this, predicate);
		}

		#endregion

		#region Core

		public void Sort(Comparison<TItem> comparison)
		{
			_items.Sort(comparison);
		}

		public TItem this[int index]
		{
			get
			{
				return _items[index];
			}
			set
			{
				Replace(index, value);
			}
		}

		public int IndexOf(TItem item) => _items.IndexOf(item);
		public bool Contains(TItem item) => _items.Contains(item);

		public void Replace(int index, TItem item)
		{
			TItem itemToReplace = _items[index];
			if(IsValidAddCheck(item, nameof(Replace)) &&
				IsValidRemoveCheck(itemToReplace, nameof(Replace)))
			{
				_items[index] = item;
				OnRemoveItem(itemToReplace, index);
				OnAddItem(item, index);
			}
		}

		public void Insert(int index, TItem item)
		{
			if(IsValidAddCheck(item, nameof(Insert)))
			{
				_items.Insert(index, item);
				OnAddItem(item, index);
			}
		}

		public bool Remove(TItem item)
		{
			if(IsValidRemoveCheck(item, nameof(Remove)))
			{
				int index = _items.IndexOf(item);
				if(index >= 0)
				{
					_items.RemoveAt(index);
					OnRemoveItem(item, index);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			TryRemoveAt(index, out _, throwIndexOutOfRangeException: true);
		}

		public bool TryRemoveAt(int index, out TItem item, bool throwIndexOutOfRangeException = false)
		{
			try
			{
				item = _items[index];
				if(IsValidRemoveCheck(item, nameof(RemoveAt)))
				{
					_items.RemoveAt(index);
					OnRemoveItem(item, index);
					return true;
				}
			}
			catch(ArgumentOutOfRangeException indexException)
			{
				if(throwIndexOutOfRangeException)
				{
					throw indexException;
				}
			}

			item = default;
			return false;
		}

		public void AddRange(IList<TItem> items)
		{
			for (int i = 0, c = items.Count; i < c; i++)
			{
				Add(items[i]);
			}
		}

		public bool TryAdd(TItem item)
		{
			try
			{
				Add(item);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public void Add(TItem item)
		{
			if (IsValidAddCheck(item, nameof(Add)))
			{
				int index = _items.Count;
				_items.Add(item);
				OnAddItem(item, index);
			}
		}

		public void Clear()
		{
			for(int i = _items.Count - 1; i >= 0; i--)
			{
				RemoveAt(i);
			}
		}

		public virtual void Dispose()
		{
			AddedItemEvent = null;
			RemovedItemEvent = null;
			DirtyEvent = null;
			_onAddItem = null;
			_onRemoveItem = null;
			_items.Clear();
			_items = null;
			IsDisposed = true;
		}

		public void CopyTo(TItem[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

		public IEnumerator<TItem> GetEnumerator() => _items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

		protected virtual void OnAddItem(TItem item, int index)
		{
			_onAddItem?.Invoke(item, index);
			AddedItemEvent?.Invoke(item, index);
			DirtyEvent?.Invoke();
		}

		protected virtual void OnRemoveItem(TItem item, int index)
		{
			_onRemoveItem?.Invoke(item, index);
			RemovedItemEvent?.Invoke(item, index);
			DirtyEvent?.Invoke();
		}

		protected virtual bool IsValidAddCheck(TItem item, string operationName) => true;
		protected virtual bool IsValidRemoveCheck(TItem item, string operationName) => true;

		#endregion
	}

	public interface IReadOnlyRaCollection<TItem> : IReadOnlyList<TItem>
	{
		public event ItemIndexHandler<TItem> AddedItemEvent;
		public event ItemIndexHandler<TItem> RemovedItemEvent;
		public event Action DirtyEvent;
		
		public bool IsDisposed
		{
			get;
		}

		bool Contains(TItem item);

		public TItem PeekStack();
		public bool TryPeekStack(out TItem item);

		public TItem PeekQueue();

		public bool TryPeekQueue(out TItem item);
	}
}