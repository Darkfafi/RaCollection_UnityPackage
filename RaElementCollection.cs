using System;
using System.Collections.Generic;
using System.Linq;

namespace RaCollection
{
	public interface IReadOnlyRaElementCollection<TElement> : IReadOnlyRaCollection<TElement>
		where TElement : IRaCollectionElement
	{
		bool Contains(string id);
		bool TryGetItem(string id, out TElement element);
		bool TryGetItem<T>(string id, out T element) where T : TElement;
		bool TryFindItem<T>(string id, out T element);
		string[] GetAllIds();
	}

	[Serializable]
	public class RaElementCollection<TElement> : RaCollection<TElement>, IReadOnlyRaElementCollection<TElement>
		where TElement : IRaCollectionElement
	{
		public static readonly IReadOnlyRaElementCollection<TElement> EmptyReadOnly = new RaElementCollection<TElement>();

		private Dictionary<string, TElement> _idToElementsMap = new Dictionary<string, TElement>();

		#region Helper

		public bool Contains(string id) => _idToElementsMap.ContainsKey(id);

		public bool TryGetItem(string id, out TElement element)
		{
			return _idToElementsMap.TryGetValue(id, out element);
		}

		public bool TryGetItem<T>(string id, out T element)
			where T : TElement
		{
			return TryFindItem(id, out element);
		}

		public bool TryFindItem<T>(string id, out T element)
		{
			if(_idToElementsMap.TryGetValue(id, out TElement rawElement) &&
				rawElement is T castedElement)
			{
				element = castedElement;
				return true;
			}

			element = default;
			return false;
		}

		public string[] GetAllIds() => _idToElementsMap.Keys.ToArray();

		#endregion

		#region Core

		public void Trim(string[] ids)
		{
			HashSet<string> idsToKeep = new HashSet<string>(ids);
			string[] currentIds = GetAllIds();

			for (int i = currentIds.Length - 1; i >= 0; i--)
			{
				var id = currentIds[i];
				if (!idsToKeep.Contains(id))
				{
					Remove(id);
				}
			}
		}

		public void AddRange(IList<TElement> elements, bool trim)
		{
			string[] ids = new string[elements.Count];
			for (int i = 0, c = elements.Count; i < c; i++)
			{
				TElement element = elements[i];
				Add(element);
				ids[i] = element.Id;
			}

			if (trim)
			{
				Trim(ids);
			}
		}

		public bool Remove(string id)
		{
			return Remove(id, out _);
		}


		public bool Remove(string id, out TElement item)
		{
			if (_idToElementsMap.TryGetValue(id, out item))
			{
				return Remove(item);
			}
			return false;
		}

		public override void Dispose()
		{
			_idToElementsMap.Clear();
			_idToElementsMap = null;
			base.Dispose();
		}

		#endregion

		public RaElementCollection(ItemIndexHandler<TElement> onAddItem, ItemIndexHandler<TElement> onRemoveItem)
			: base(onAddItem, onRemoveItem)
		{
		}

		public RaElementCollection()
			: base()
		{
		
		}

		public RaElementCollection(IList<TElement> elements, ItemIndexHandler<TElement> onAddItem = null, ItemIndexHandler<TElement> onRemoveItem = null)
			: base(elements, onAddItem, onRemoveItem)
		{

		}

		#region Core

		protected override void OnAddItem(TElement item, int index)
		{
			_idToElementsMap[item.Id] = item;
			base.OnAddItem(item, index);
		}

		protected override void OnRemoveItem(TElement item, int index)
		{
			_idToElementsMap.Remove(item.Id);
			base.OnRemoveItem(item, index);
		}

		protected override bool IsValidAddCheck(TElement item, string operationName)
		{
			return base.IsValidAddCheck(item, operationName) &&
					IsNotNullCheck(item, operationName) &&
					IsIDNotNullCheck(item, operationName) &&
					DoesNotContainCheck(item, operationName);
		}

		protected override bool IsValidRemoveCheck(TElement item, string operationName)
		{
			return base.IsValidRemoveCheck(item, operationName) &&
					IsNotNullCheck(item, operationName) &&
					IsIDNotNullCheck(item, operationName);
		}

		private bool DoesNotContainCheck(TElement item, string operationName)
		{
			if(_idToElementsMap.ContainsKey(item.Id))
			{
				throw new InvalidOperationException($"Can't run operation '{operationName}' because item {item.Id} already exists in the collection");
			}

			return true;
		}

		private bool IsNotNullCheck(TElement item, string operationName)
		{
			if(item == null)
			{
				throw new NullReferenceException($"Can't run operation '{operationName}' with null item");
			}

			return true;
		}

		private bool IsIDNotNullCheck(TElement item, string operationName)
		{
			if(string.IsNullOrEmpty(item.Id))
			{
				throw new NullReferenceException($"Can't run operation '{operationName}' because {item} has empty {nameof(item.Id)}");
			}

			return true;
		}

		#endregion
	}

	public interface IRaCollectionElement
	{
		public string Id
		{
			get;
		}
	}
}