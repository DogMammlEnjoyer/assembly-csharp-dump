using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface IListPropertyBag<TList, TElement> : ICollectionPropertyBag<TList, TElement>, IPropertyBag<TList>, IPropertyBag, ICollectionPropertyBagAccept<!0>, IListPropertyBagAccept<!0>, IListPropertyAccept<!0>, IIndexedProperties<TList> where TList : IList<TElement>
	{
	}
}
