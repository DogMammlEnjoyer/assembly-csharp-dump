using System;
using System.Collections.Generic;

namespace Unity.Properties
{
	public interface ICollectionPropertyBag<TCollection, TElement> : IPropertyBag<TCollection>, IPropertyBag, ICollectionPropertyBagAccept<!0> where TCollection : ICollection<TElement>
	{
	}
}
