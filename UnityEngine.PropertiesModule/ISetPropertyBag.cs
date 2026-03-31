using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ISetPropertyBag<TSet, TElement> : ICollectionPropertyBag<TSet, TElement>, IPropertyBag<TSet>, IPropertyBag, ICollectionPropertyBagAccept<!0>, ISetPropertyBagAccept<TSet>, ISetPropertyAccept<TSet>, IKeyedProperties<TSet, object> where TSet : ISet<TElement>
	{
	}
}
