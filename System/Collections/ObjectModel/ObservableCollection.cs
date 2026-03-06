using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace System.Collections.ObjectModel
{
	/// <summary>Represents a dynamic data collection that provides notifications when items get added, removed, or when the whole list is refreshed.</summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	[DebuggerDisplay("Count = {Count}")]
	[Serializable]
	public class ObservableCollection<T> : Collection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> class.</summary>
		public ObservableCollection()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> class that contains elements copied from the specified collection.</summary>
		/// <param name="collection">The collection from which the elements are copied.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="collection" /> parameter cannot be <see langword="null" />.</exception>
		public ObservableCollection(IEnumerable<T> collection) : base(ObservableCollection<T>.CreateCopy(collection, "collection"))
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> class that contains elements copied from the specified list.</summary>
		/// <param name="list">The list from which the elements are copied.</param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="list" /> parameter cannot be <see langword="null" />.</exception>
		public ObservableCollection(List<T> list) : base(ObservableCollection<T>.CreateCopy(list, "list"))
		{
		}

		private static List<T> CreateCopy(IEnumerable<T> collection, string paramName)
		{
			if (collection == null)
			{
				throw new ArgumentNullException(paramName);
			}
			return new List<T>(collection);
		}

		/// <summary>Moves the item at the specified index to a new location in the collection.</summary>
		/// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
		/// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
		public void Move(int oldIndex, int newIndex)
		{
			this.MoveItem(oldIndex, newIndex);
		}

		/// <summary>Occurs when a property value changes.</summary>
		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged
		{
			add
			{
				this.PropertyChanged += value;
			}
			remove
			{
				this.PropertyChanged -= value;
			}
		}

		/// <summary>Occurs when an item is added, removed, changed, moved, or the entire list is refreshed.</summary>
		[NonSerialized]
		public virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>Removes all items from the collection.</summary>
		protected override void ClearItems()
		{
			this.CheckReentrancy();
			base.ClearItems();
			this.OnCountPropertyChanged();
			this.OnIndexerPropertyChanged();
			this.OnCollectionReset();
		}

		/// <summary>Removes the item at the specified index of the collection.</summary>
		/// <param name="index">The zero-based index of the element to remove.</param>
		protected override void RemoveItem(int index)
		{
			this.CheckReentrancy();
			T t = base[index];
			base.RemoveItem(index);
			this.OnCountPropertyChanged();
			this.OnIndexerPropertyChanged();
			this.OnCollectionChanged(NotifyCollectionChangedAction.Remove, t, index);
		}

		/// <summary>Inserts an item into the collection at the specified index.</summary>
		/// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
		/// <param name="item">The object to insert.</param>
		protected override void InsertItem(int index, T item)
		{
			this.CheckReentrancy();
			base.InsertItem(index, item);
			this.OnCountPropertyChanged();
			this.OnIndexerPropertyChanged();
			this.OnCollectionChanged(NotifyCollectionChangedAction.Add, item, index);
		}

		/// <summary>Replaces the element at the specified index.</summary>
		/// <param name="index">The zero-based index of the element to replace.</param>
		/// <param name="item">The new value for the element at the specified index.</param>
		protected override void SetItem(int index, T item)
		{
			this.CheckReentrancy();
			T t = base[index];
			base.SetItem(index, item);
			this.OnIndexerPropertyChanged();
			this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, t, item, index);
		}

		/// <summary>Moves the item at the specified index to a new location in the collection.</summary>
		/// <param name="oldIndex">The zero-based index specifying the location of the item to be moved.</param>
		/// <param name="newIndex">The zero-based index specifying the new location of the item.</param>
		protected virtual void MoveItem(int oldIndex, int newIndex)
		{
			this.CheckReentrancy();
			T t = base[oldIndex];
			base.RemoveItem(oldIndex);
			base.InsertItem(newIndex, t);
			this.OnIndexerPropertyChanged();
			this.OnCollectionChanged(NotifyCollectionChangedAction.Move, t, newIndex, oldIndex);
		}

		/// <summary>Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.PropertyChanged" /> event with the provided arguments.</summary>
		/// <param name="e">Arguments of the event being raised.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
			if (propertyChanged == null)
			{
				return;
			}
			propertyChanged(this, e);
		}

		/// <summary>Occurs when a property value changes.</summary>
		[NonSerialized]
		protected virtual event PropertyChangedEventHandler PropertyChanged;

		/// <summary>Raises the <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event with the provided arguments.</summary>
		/// <param name="e">Arguments of the event being raised.</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
		{
			NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
			if (collectionChanged != null)
			{
				this._blockReentrancyCount++;
				try
				{
					collectionChanged(this, e);
				}
				finally
				{
					this._blockReentrancyCount--;
				}
			}
		}

		/// <summary>Disallows reentrant attempts to change this collection.</summary>
		/// <returns>An <see cref="T:System.IDisposable" /> object that can be used to dispose of the object.</returns>
		protected IDisposable BlockReentrancy()
		{
			this._blockReentrancyCount++;
			return this.EnsureMonitorInitialized();
		}

		/// <summary>Checks for reentrant attempts to change this collection.</summary>
		/// <exception cref="T:System.InvalidOperationException">If there was a call to <see cref="M:System.Collections.ObjectModel.ObservableCollection`1.BlockReentrancy" /> of which the <see cref="T:System.IDisposable" /> return value has not yet been disposed of. Typically, this means when there are additional attempts to change this collection during a <see cref="E:System.Collections.ObjectModel.ObservableCollection`1.CollectionChanged" /> event. However, it depends on when derived classes choose to call <see cref="M:System.Collections.ObjectModel.ObservableCollection`1.BlockReentrancy" />.</exception>
		protected void CheckReentrancy()
		{
			if (this._blockReentrancyCount > 0)
			{
				NotifyCollectionChangedEventHandler collectionChanged = this.CollectionChanged;
				if (collectionChanged != null && collectionChanged.GetInvocationList().Length > 1)
				{
					throw new InvalidOperationException("Cannot change ObservableCollection during a CollectionChanged event.");
				}
			}
		}

		private void OnCountPropertyChanged()
		{
			this.OnPropertyChanged(EventArgsCache.CountPropertyChanged);
		}

		private void OnIndexerPropertyChanged()
		{
			this.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index)
		{
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index));
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, object item, int index, int oldIndex)
		{
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, item, index, oldIndex));
		}

		private void OnCollectionChanged(NotifyCollectionChangedAction action, object oldItem, object newItem, int index)
		{
			this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));
		}

		private void OnCollectionReset()
		{
			this.OnCollectionChanged(EventArgsCache.ResetCollectionChanged);
		}

		private ObservableCollection<T>.SimpleMonitor EnsureMonitorInitialized()
		{
			ObservableCollection<T>.SimpleMonitor result;
			if ((result = this._monitor) == null)
			{
				result = (this._monitor = new ObservableCollection<T>.SimpleMonitor(this));
			}
			return result;
		}

		[OnSerializing]
		private void OnSerializing(StreamingContext context)
		{
			this.EnsureMonitorInitialized();
			this._monitor._busyCount = this._blockReentrancyCount;
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this._monitor != null)
			{
				this._blockReentrancyCount = this._monitor._busyCount;
				this._monitor._collection = this;
			}
		}

		private ObservableCollection<T>.SimpleMonitor _monitor;

		[NonSerialized]
		private int _blockReentrancyCount;

		[Serializable]
		private sealed class SimpleMonitor : IDisposable
		{
			public SimpleMonitor(ObservableCollection<T> collection)
			{
				this._collection = collection;
			}

			public void Dispose()
			{
				this._collection._blockReentrancyCount--;
			}

			internal int _busyCount;

			[NonSerialized]
			internal ObservableCollection<T> _collection;
		}
	}
}
