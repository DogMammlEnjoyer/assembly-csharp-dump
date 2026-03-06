using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;

namespace System.Collections.ObjectModel
{
	/// <summary>Represents a read-only <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" />.</summary>
	/// <typeparam name="T">The type of elements in the collection.</typeparam>
	[DebuggerDisplay("Count = {Count}")]
	[DebuggerTypeProxy(typeof(CollectionDebugView<>))]
	[Serializable]
	public class ReadOnlyObservableCollection<T> : ReadOnlyCollection<T>, INotifyCollectionChanged, INotifyPropertyChanged
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Collections.ObjectModel.ReadOnlyObservableCollection`1" /> class that serves as a wrapper around the specified <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" />.</summary>
		/// <param name="list">The <see cref="T:System.Collections.ObjectModel.ObservableCollection`1" /> with which to create this instance of the <see cref="T:System.Collections.ObjectModel.ReadOnlyObservableCollection`1" /> class.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="list" /> is <see langword="null" />.</exception>
		public ReadOnlyObservableCollection(ObservableCollection<T> list) : base(list)
		{
			((INotifyCollectionChanged)base.Items).CollectionChanged += this.HandleCollectionChanged;
			((INotifyPropertyChanged)base.Items).PropertyChanged += this.HandlePropertyChanged;
		}

		/// <summary>Occurs when the collection changes.</summary>
		event NotifyCollectionChangedEventHandler INotifyCollectionChanged.CollectionChanged
		{
			add
			{
				this.CollectionChanged += value;
			}
			remove
			{
				this.CollectionChanged -= value;
			}
		}

		/// <summary>Occurs when an item is added or removed.</summary>
		[NonSerialized]
		protected virtual event NotifyCollectionChangedEventHandler CollectionChanged;

		/// <summary>Raises the <see cref="E:System.Collections.ObjectModel.ReadOnlyObservableCollection`1.CollectionChanged" /> event using the provided arguments.</summary>
		/// <param name="args">Arguments of the event being raised.</param>
		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (this.CollectionChanged != null)
			{
				this.CollectionChanged(this, args);
			}
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

		/// <summary>Occurs when a property value changes.</summary>
		[NonSerialized]
		protected virtual event PropertyChangedEventHandler PropertyChanged;

		/// <summary>Raises the <see cref="E:System.Collections.ObjectModel.ReadOnlyObservableCollection`1.PropertyChanged" /> event using the provided arguments.</summary>
		/// <param name="args">Arguments of the event being raised.</param>
		protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, args);
			}
		}

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			this.OnCollectionChanged(e);
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.OnPropertyChanged(e);
		}
	}
}
