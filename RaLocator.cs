using System;
using System.Collections.Generic;

namespace RaCollection
{
	public interface IRaLocator
	{
		T GetValue<T>(string key);
		T GetValue<T>(Predicate<T> predicate = null);
		List<T> GetValues<T>(Predicate<T> predicate = null);
		bool TryGetValue<T>(string key, out T value);
		bool TryGetValue<T>(out T value, Predicate<T> predicate = null);
	}

	public class RaLocator : IRaLocator, IDisposable
	{
		public delegate void Handler(Entry entry);

		private RaElementCollection<Entry> _entries = new RaElementCollection<Entry>();
		private Handler _onRegister = null;
		private Handler _onUnregister = null;

		public IReadOnlyRaElementCollection<Entry> Entries => _entries;

		public RaLocator(Handler onRegister, Handler onUnregister)
		{
			_entries = new RaElementCollection<Entry>(OnRegister, OnUnregister);
			_onRegister = onRegister;
			_onUnregister = onUnregister;
		}

		public void Register<T>(string key, T value)
		{
			_entries.Add(new Entry(key, value));
		}

		public void Unregister(string key)
		{
			_entries.Remove(key);
		}

		public T GetValue<T>(string key)
		{
			if(TryGetValue(key, out T value))
			{
				return value;
			}
			return default;
		}

		public T GetValue<T>(Predicate<T> predicate = null)
		{
			if(TryGetValue(out T value, predicate))
			{
				return value;
			}
			return default;
		}

		public List<T> GetValues<T>(Predicate<T> predicate = null)
		{
			List<T> returnValue = new List<T>();
			_entries.ForEach((x, i) =>
			{
				if(x is T castedValue && (predicate == null || predicate(castedValue)))
				{
					returnValue.Add(castedValue);
				}
			});
			return returnValue;
		}

		public bool TryGetValue<T>(string key, out T value)
		{
			if(_entries.TryGetItem(key, out Entry entry) && entry.Value is T castedValue)
			{
				value = castedValue;
				return true;
			}
			value = default;
			return false;
		}

		public bool HasValue<T>(Predicate<T> predicate = null) => TryGetValue(out _, predicate);

		public bool TryGetValue<T>(out T value, Predicate<T> predicate = null)
		{
			if(_entries.TryGetItem(out Entry entry, x => x.Value is T castedValue && (predicate == null || predicate(castedValue))))
			{
				value = (T)entry.Value;
				return true;
			}
			value = default;
			return false;
		}

		public void Sort(Comparison<Entry> comparison)
		{
			_entries.Sort(comparison);
		}

		public void Clear()
		{
			_entries.Clear();
		}

		public void Dispose()
		{
			_onRegister = null;
			_onUnregister = null;
			_entries.Dispose();
			_entries = null;
		}

		private void OnRegister(Entry item, int index)
		{
			_onRegister?.Invoke(item);
		}

		private void OnUnregister(Entry item, int index)
		{
			_onUnregister?.Invoke(item);
		}

		public struct Entry : IRaCollectionElement
		{
			public string Id
			{
				get; private set;
			}

			public object Value
			{
				get; private set;
			}

			public Entry(string id, object value)
			{
				Id = id;
				Value = value;
			}
		}
	}
}