using System;

namespace System.Collections.Specialized
{
	/// <summary>Notifies listeners of dynamic changes, such as when an item is added and removed or the whole list is cleared.</summary>
	public interface INotifyCollectionChanged
	{
		/// <summary>Occurs when the collection changes.</summary>
		event NotifyCollectionChangedEventHandler CollectionChanged;
	}
}
