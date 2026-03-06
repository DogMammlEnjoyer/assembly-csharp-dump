using System;

namespace System.Collections.Specialized
{
	/// <summary>Describes the action that caused a <see cref="E:System.Collections.Specialized.INotifyCollectionChanged.CollectionChanged" /> event.</summary>
	public enum NotifyCollectionChangedAction
	{
		/// <summary>An item was added to the collection.</summary>
		Add,
		/// <summary>An item was removed from the collection.</summary>
		Remove,
		/// <summary>An item was replaced in the collection.</summary>
		Replace,
		/// <summary>An item was moved within the collection.</summary>
		Move,
		/// <summary>The content of the collection was cleared.</summary>
		Reset
	}
}
