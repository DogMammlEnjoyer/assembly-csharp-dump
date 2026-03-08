using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public class IndexedCollectionPropertyBag<TList, TElement> : PropertyBag<TList>, IListPropertyBag<TList, TElement>, ICollectionPropertyBag<TList, TElement>, IPropertyBag<TList>, IPropertyBag, ICollectionPropertyBagAccept<TList>, IListPropertyBagAccept<TList>, IListPropertyAccept<TList>, IIndexedProperties<TList>, IConstructorWithCount<TList>, IConstructor, IIndexedCollectionPropertyBagEnumerator<TList> where TList : IList<TElement>
	{
		public override PropertyCollection<TList> GetProperties()
		{
			return PropertyCollection<TList>.Empty;
		}

		public override PropertyCollection<TList> GetProperties(ref TList container)
		{
			return new PropertyCollection<TList>(new IndexedCollectionPropertyBagEnumerable<TList>(this, container));
		}

		public bool TryGetProperty(ref TList container, int index, out IProperty<TList> property)
		{
			bool flag = index >= container.Count;
			bool result;
			if (flag)
			{
				property = null;
				result = false;
			}
			else
			{
				property = new IndexedCollectionPropertyBag<TList, TElement>.ListElementProperty
				{
					m_Index = index,
					m_IsReadOnly = false
				};
				result = true;
			}
			return result;
		}

		void ICollectionPropertyBagAccept<!0>.Accept(ICollectionPropertyBagVisitor visitor, ref TList container)
		{
			visitor.Visit<TList, TElement>(this, ref container);
		}

		void IListPropertyBagAccept<!0>.Accept(IListPropertyBagVisitor visitor, ref TList list)
		{
			visitor.Visit<TList, TElement>(this, ref list);
		}

		void IListPropertyAccept<!0>.Accept<TContainer>(IListPropertyVisitor visitor, Property<TContainer, TList> property, ref TContainer container, ref TList list)
		{
			using (new AttributesScope(this.m_Property, property))
			{
				visitor.Visit<TContainer, TList, TElement>(property, ref container, ref list);
			}
		}

		TList IConstructorWithCount<!0>.InstantiateWithCount(int count)
		{
			return this.InstantiateWithCount(count);
		}

		protected virtual TList InstantiateWithCount(int count)
		{
			return default(TList);
		}

		int IIndexedCollectionPropertyBagEnumerator<!0>.GetCount(ref TList container)
		{
			return container.Count;
		}

		IProperty<TList> IIndexedCollectionPropertyBagEnumerator<!0>.GetSharedProperty()
		{
			return this.m_Property;
		}

		IndexedCollectionSharedPropertyState IIndexedCollectionPropertyBagEnumerator<!0>.GetSharedPropertyState()
		{
			return new IndexedCollectionSharedPropertyState
			{
				Index = this.m_Property.m_Index,
				IsReadOnly = this.m_Property.IsReadOnly
			};
		}

		void IIndexedCollectionPropertyBagEnumerator<!0>.SetSharedPropertyState(IndexedCollectionSharedPropertyState state)
		{
			this.m_Property.m_Index = state.Index;
			this.m_Property.m_IsReadOnly = state.IsReadOnly;
		}

		private readonly IndexedCollectionPropertyBag<TList, TElement>.ListElementProperty m_Property = new IndexedCollectionPropertyBag<TList, TElement>.ListElementProperty();

		private class ListElementProperty : Property<TList, TElement>, IListElementProperty, ICollectionElementProperty
		{
			public int Index
			{
				get
				{
					return this.m_Index;
				}
			}

			public override string Name
			{
				get
				{
					return this.Index.ToString();
				}
			}

			public override bool IsReadOnly
			{
				get
				{
					return this.m_IsReadOnly;
				}
			}

			public override TElement GetValue(ref TList container)
			{
				return container[this.m_Index];
			}

			public override void SetValue(ref TList container, TElement value)
			{
				container[this.m_Index] = value;
			}

			internal int m_Index;

			internal bool m_IsReadOnly;
		}
	}
}
