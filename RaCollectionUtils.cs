using System;
using System.Collections.Generic;

namespace RaCollection
{
	public static class RaCollectionUtils
	{
		// List
		public static bool HasItems<TItem>(this IList<TItem> self)
		{
			return self != null && self.Count > 0;
		}

		public static bool IsInBounds<TItem>(this IList<TItem> self, int index)
		{
			return self != null && index >= 0 && index < self.Count;
		}

		public static void ForEach<TItem>(this IList<TItem> self, ItemHandler<TItem> action)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				action(self[i]);
			}
		}

		public static void ForEach<TItem>(this IList<TItem> self, ItemIndexHandler<TItem> action)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				action(self[i], i);
			}
		}

		public static void ForEachReverse<TItem>(this IList<TItem> self, ItemHandler<TItem> action)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				action(self[i]);
			}
		}

		public static void ForEachReverse<TItem>(this IList<TItem> self, ItemIndexHandler<TItem> action)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				action(self[i], i);
			}
		}

		public static bool TryGetItem<TItem>(this IList<TItem> self, out TItem item, Predicate<TItem> predicate = null)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				item = self[i];
				if (predicate == null || predicate(item))
				{
					return true;
				}
			}

			item = default;
			return false;
		}

		public static bool TryGetItem<TItem, T>(this IList<TItem> self, out T item, Predicate<T> predicate = null)
			where T : TItem
		{
			return TryFindItem(self, out item, predicate);
		}

		public static bool TryFindItem<TItem, T>(this IList<TItem> self, out T item, Predicate<T> predicate = null)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem rawItem = self[i];
				if (rawItem is T castedItem && (predicate == null || predicate(castedItem)))
				{
					item = castedItem;
					return true;
				}
			}

			item = default;
			return false;
		}

		public static List<TItem> CutItems<TItem>(this List<TItem> self, Predicate<TItem> predicate)
		{
			List<TItem> returnValue = new List<TItem>();

			for (int i = self.Count - 1; i >= 0; i--)
			{
				TItem item = self[i];
				if (predicate == null || predicate(item))
				{
					self.RemoveAt(i);
					returnValue.Insert(0, item);
				}
			}

			return returnValue;
		}

		public static List<TItem> GetItems<TItem>(this IList<TItem> self, Predicate<TItem> predicate)
		{
			List<TItem> returnValue = new List<TItem>();

			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem item = self[i];
				if (predicate == null || predicate(item))
				{
					returnValue.Add(item);
				}
			}

			return returnValue;
		}

		public static List<T> GetItems<TItem, T>(this IList<TItem> self, Predicate<T> predicate)
			where T : TItem
		{
			return FindItems(self, predicate);
		}

		public static List<T> FindItems<TItem, T>(this IList<TItem> self, Predicate<T> predicate)
		{
			List<T> returnValue = new List<T>();

			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem rawItem = self[i];
				if (rawItem is T castedItem && (predicate == null || predicate(castedItem)))
				{
					returnValue.Add(castedItem);
				}
			}

			return returnValue;
		}

		// ReadOnly List
		public static bool HasItemsReadOnly<TItem>(this IReadOnlyList<TItem> self)
		{
			return self != null && self.Count > 0;
		}

		public static bool IsInBoundsReadOnly<TItem>(this IReadOnlyList<TItem> self, int index)
		{
			return self != null && index >= 0 && index < self.Count;
		}

		public static void ForEachReadOnly<TItem>(this IReadOnlyList<TItem> self, ItemHandler<TItem> action)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				action(self[i]);
			}
		}

		public static void ForEachReadOnly<TItem>(this IReadOnlyList<TItem> self, ItemIndexHandler<TItem> action)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				action(self[i], i);
			}
		}

		public static void ForEachReverseReadOnly<TItem>(this IReadOnlyList<TItem> self, ItemHandler<TItem> action)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				action(self[i]);
			}
		}

		public static void ForEachReverseReadOnly<TItem>(this IReadOnlyList<TItem> self, ItemIndexHandler<TItem> action)
		{
			for (int i = self.Count - 1; i >= 0; i--)
			{
				action(self[i], i);
			}
		}

		public static bool TryGetItemReadOnly<TItem>(this IReadOnlyList<TItem> self, out TItem item, Predicate<TItem> predicate = null)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				item = self[i];
				if (predicate == null || predicate(item))
				{
					return true;
				}
			}

			item = default;
			return false;
		}

		public static bool TryGetItemReadOnly<TItem, T>(this IReadOnlyList<TItem> self, out T item, Predicate<T> predicate = null)
			where T : TItem
		{
			return TryFindItemReadOnly(self, out item, predicate);
		}

		public static bool TryFindItemReadOnly<TItem, T>(this IReadOnlyList<TItem> self, out T item, Predicate<T> predicate = null)
		{
			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem rawItem = self[i];
				if (rawItem is T castedItem && (predicate == null || predicate(castedItem)))
				{
					item = castedItem;
					return true;
				}
			}

			item = default;
			return false;
		}

		public static List<TItem> GetItemsReadOnly<TItem>(this IReadOnlyList<TItem> self, Predicate<TItem> predicate)
		{
			List<TItem> returnValue = new List<TItem>();

			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem item = self[i];
				if (predicate == null || predicate(item))
				{
					returnValue.Add(item);
				}
			}

			return returnValue;
		}

		public static List<T> GetItemsReadOnly<TItem, T>(this IReadOnlyList<TItem> self, Predicate<T> predicate)
		   where T : TItem
		{
			return FindItemsReadOnly(self, predicate);
		}

		public static List<T> FindItemsReadOnly<TItem, T>(this IReadOnlyList<TItem> self, Predicate<T> predicate)
		{
			List<T> returnValue = new List<T>();

			for (int i = 0, c = self.Count; i < c; i++)
			{
				TItem rawItem = self[i];
				if (rawItem is T castedItem && (predicate == null || predicate(castedItem)))
				{
					returnValue.Add(castedItem);
				}
			}

			return returnValue;
		}
	}
}