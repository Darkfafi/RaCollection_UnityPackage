using System;
using System.Collections;
using System.Collections.Generic;

namespace RaCollection
{
	public interface IReadOnlyPriorityCollection<TItem> : IReadOnlyRaCollection<TItem>
	{
		event ItemsHandler<TItem> PriorityItemChangedEvent;
		bool TryGetPriorityItem(out TItem priorityItem);
	}

	public class RaPriorityCollection<TItem> : IReadOnlyPriorityCollection<TItem>, IDisposable
	{
		public event ItemsHandler<TItem> PriorityItemChangedEvent;
		public event ItemIndexHandler<TItem> AddedItemEvent;
		public event ItemIndexHandler<TItem> RemovedItemEvent;

		private RaCollection<Entry> _priorityEntries;
		private RaCollection<TItem> _values;


		private ItemsHandler<TItem> _priorityItemChangedEvent = null;
		private ItemIndexHandler<TItem> _onAddItem = null;
		private ItemIndexHandler<TItem> _onRemoveItem = null;

		public IReadOnlyCollection<TItem> Values => _values;
		public IReadOnlyRaCollection<Entry> Entries => _priorityEntries;
		public int Count => _priorityEntries.Count;

		public RaPriorityCollection(ItemsHandler<TItem> onPriorityItemChanged = null, ItemIndexHandler<TItem> onAddItem = null, ItemIndexHandler<TItem> onRemoveItem = null)
		{
			_priorityItemChangedEvent = onPriorityItemChanged;
			_onAddItem = onAddItem;
			_onRemoveItem = onRemoveItem;

			_values = new RaCollection<TItem>();
			_priorityEntries = new RaCollection<Entry>(OnAddedEntry, OnRemovedEntry);
		}

		private void OnAddedEntry(Entry item, int index)
		{
			TItem prePrioItem = default;

			if(index == 0)
			{
				TryGetPriorityItem(out prePrioItem);
			}

			_values.Insert(index, item.Value);

			_onAddItem?.Invoke(item.Value, index);
			AddedItemEvent?.Invoke(item.Value, index);

			if(index == 0)
			{
				_priorityItemChangedEvent?.Invoke(item.Value, prePrioItem);
				PriorityItemChangedEvent?.Invoke(item.Value, prePrioItem);
			}
		}

		private void OnRemovedEntry(Entry item, int index)
		{
			_values.RemoveAt(index);

			TItem newPrioItem = default;

			if(index == 0)
			{
				TryGetPriorityItem(out newPrioItem);
			}

			_onRemoveItem?.Invoke(item.Value, index);
			RemovedItemEvent?.Invoke(item.Value, index);

			if(index == 0)
			{
				_priorityItemChangedEvent?.Invoke(newPrioItem, item.Value);
				PriorityItemChangedEvent?.Invoke(newPrioItem, item.Value);
			}
		}

		public bool TryGetPriorityItem(out TItem priorityItem)
		{
			if(_values.Count > 0)
			{
				priorityItem = _values[0];
				return true;
			}

			priorityItem = default;
			return false;
		}

		public bool Remove(TItem item)
		{
			for(int i = 0, c = _priorityEntries.Count; i < c; i++)
			{
				Entry entry = _priorityEntries[i];
				if(entry.Value.Equals(item))
				{
					RemoveAt(i);
					return true;
				}
			}
			return false;
		}

		public void RemoveAt(int index)
		{
			_priorityEntries.RemoveAt(index);
		}

		public void Add(int priority, TItem item)
		{
			Entry entry = new Entry(priority, item);
			int count = _priorityEntries.Count;
			for(int i = 0; i < count; i++)
			{
				if(priority > _priorityEntries[i].Priority)
				{
					_priorityEntries.Insert(i, entry);
					return;
				}
			}
			_priorityEntries.Add(entry);
		}

		public TItem this[int index] => _values[index];
		public bool Contains(TItem item) => _values.Contains(item);
		public IEnumerator<TItem> GetEnumerator() => _values.GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => _priorityEntries.GetEnumerator();

		public void Dispose()
		{
			AddedItemEvent = null;
			RemovedItemEvent = null;
			
			_onAddItem = null;
			_onRemoveItem = null;

			_priorityEntries.Dispose();
			_values.Dispose();

			_priorityEntries = null;
			_values = null;
		}

		public struct Entry
		{
			public int Priority;
			public TItem Value;

			public Entry(int priority, TItem value)
			{
				Priority = priority;
				Value = value;
			}
		}
	}
}