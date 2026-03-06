using System;
using System.Collections.Generic;
using UnityEngine.XR.Interaction.Toolkit.Filtering;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class ExposedRegistrationList<T> : SmallRegistrationList<T>, IXRFilterList<T> where T : class
	{
		public int count
		{
			get
			{
				return base.flushedCount;
			}
		}

		public void Add(T item)
		{
			if (item != null)
			{
				Object @object = item as Object;
				if (@object == null || !(@object == null))
				{
					this.Register(item);
					return;
				}
			}
			throw new ArgumentNullException("item");
		}

		public bool Remove(T item)
		{
			return this.Unregister(item);
		}

		public void MoveTo(T item, int newIndex)
		{
			base.MoveItemImmediately(item, newIndex);
		}

		public void Clear()
		{
			base.UnregisterAll();
		}

		public void GetAll(List<T> results)
		{
			this.GetRegisteredItems(results);
		}

		public T GetAt(int index)
		{
			return this.GetRegisteredItemAt(index);
		}

		public void RegisterReferences<TObject>(List<TObject> references, Object context = null) where TObject : Object
		{
			foreach (TObject tobject in references)
			{
				if (tobject != null)
				{
					T t = tobject as T;
					if (t != null)
					{
						this.Add(t);
						continue;
					}
				}
				if (context != null)
				{
					Debug.LogError(string.Format("Trying to add the invalid item {0} into {1}, in {2}. {3} does not implement {4}.", new object[]
					{
						tobject,
						typeof(IXRFilterList<T>).Name,
						context,
						tobject,
						typeof(T).Name
					}), context);
				}
				else
				{
					Debug.LogError(string.Format("Trying to add the invalid item {0} into {1}. {2} does not implement {3}.", new object[]
					{
						tobject,
						typeof(IXRFilterList<T>).Name,
						tobject,
						typeof(T).Name
					}));
				}
			}
		}
	}
}
