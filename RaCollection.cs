using System;
using System.Collections;
using System.Collections.Generic;

namespace RaCollection
{
	public delegate void ItemHandler<TItem>(TItem item, int index);
	public delegate void ItemSourceHandler<TItem>(TItem item, int index, RaCollection<TItem> source);

	public class RaCollection<TItem> : IList<TItem>, IReadOnlyRaCollection<TItem>
	{
		public event ItemSourceHandler<TItem> AddedItemEvent;
		public event ItemSourceHandler<TItem> RemovedItemEvent;

		private readonly List<TItem> _items = new List<TItem>();
		private readonly ItemHandler<TItem> _onAddItem = null;
		private readonly ItemHandler<TItem> _onRemoveItem = null;

		public int Count => _items.Count;

		public bool IsReadOnly => false;

		public RaCollection(ItemHandler<TItem> onAddItem = null, ItemHandler<TItem> onRemoveItem = null)
		{
			_onAddItem = onAddItem;
			_onRemoveItem = onRemoveItem;
		}

		public RaCollection(TItem[] items, ItemHandler<TItem> onAddItem = null, ItemHandler<TItem> onRemoveItem = null)
			: this(onAddItem, onRemoveItem)
		{
			if(items != null && items.Length > 0)
			{
				for(int i = 0, c = items.Length; i < c; i++)
				{
					Add(items[i]);
				}
			}
		}

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
			var item = _items[index];
			if(IsValidRemoveCheck(item, nameof(RemoveAt)))
			{
				_items.RemoveAt(index);
				OnRemoveItem(item, index);
			}
		}

		public void Add(TItem item)
		{
			if(IsValidAddCheck(item, nameof(Add)))
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

		public void CopyTo(TItem[] array, int arrayIndex) => _items.CopyTo(array, arrayIndex);

		public IEnumerator<TItem> GetEnumerator() => _items.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

		protected virtual void OnAddItem(TItem item, int index)
		{
			_onAddItem?.Invoke(item, index);
			AddedItemEvent?.Invoke(item, index, this);
		}

		protected virtual void OnRemoveItem(TItem item, int index)
		{
			_onRemoveItem?.Invoke(item, index);
			RemovedItemEvent?.Invoke(item, index, this);
		}

		protected virtual bool IsValidAddCheck(TItem item, string operationName) => true;
		protected virtual bool IsValidRemoveCheck(TItem item, string operationName) => true;

		#endregion
	}

	public interface IReadOnlyRaCollection<TItem> : IReadOnlyList<TItem>
	{
		public event ItemSourceHandler<TItem> AddedItemEvent;
		public event ItemSourceHandler<TItem> RemovedItemEvent;

		bool Contains(TItem item);
	}
}