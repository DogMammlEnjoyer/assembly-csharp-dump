using System;
using System.Collections;

namespace System.ComponentModel
{
	/// <summary>Provides the features required to support both complex and simple scenarios when binding to a data source.</summary>
	public interface IBindingList : IList, ICollection, IEnumerable
	{
		/// <summary>Gets whether you can add items to the list using <see cref="M:System.ComponentModel.IBindingList.AddNew" />.</summary>
		/// <returns>
		///   <see langword="true" /> if you can add items to the list using <see cref="M:System.ComponentModel.IBindingList.AddNew" />; otherwise, <see langword="false" />.</returns>
		bool AllowNew { get; }

		/// <summary>Adds a new item to the list.</summary>
		/// <returns>The item added to the list.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.AllowNew" /> is <see langword="false" />.</exception>
		object AddNew();

		/// <summary>Gets whether you can update items in the list.</summary>
		/// <returns>
		///   <see langword="true" /> if you can update the items in the list; otherwise, <see langword="false" />.</returns>
		bool AllowEdit { get; }

		/// <summary>Gets whether you can remove items from the list, using <see cref="M:System.Collections.IList.Remove(System.Object)" /> or <see cref="M:System.Collections.IList.RemoveAt(System.Int32)" />.</summary>
		/// <returns>
		///   <see langword="true" /> if you can remove items from the list; otherwise, <see langword="false" />.</returns>
		bool AllowRemove { get; }

		/// <summary>Gets whether a <see cref="E:System.ComponentModel.IBindingList.ListChanged" /> event is raised when the list changes or an item in the list changes.</summary>
		/// <returns>
		///   <see langword="true" /> if a <see cref="E:System.ComponentModel.IBindingList.ListChanged" /> event is raised when the list changes or when an item changes; otherwise, <see langword="false" />.</returns>
		bool SupportsChangeNotification { get; }

		/// <summary>Gets whether the list supports searching using the <see cref="M:System.ComponentModel.IBindingList.Find(System.ComponentModel.PropertyDescriptor,System.Object)" /> method.</summary>
		/// <returns>
		///   <see langword="true" /> if the list supports searching using the <see cref="M:System.ComponentModel.IBindingList.Find(System.ComponentModel.PropertyDescriptor,System.Object)" /> method; otherwise, <see langword="false" />.</returns>
		bool SupportsSearching { get; }

		/// <summary>Gets whether the list supports sorting.</summary>
		/// <returns>
		///   <see langword="true" /> if the list supports sorting; otherwise, <see langword="false" />.</returns>
		bool SupportsSorting { get; }

		/// <summary>Gets whether the items in the list are sorted.</summary>
		/// <returns>
		///   <see langword="true" /> if <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" /> has been called and <see cref="M:System.ComponentModel.IBindingList.RemoveSort" /> has not been called; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" /> is <see langword="false" />.</exception>
		bool IsSorted { get; }

		/// <summary>Gets the <see cref="T:System.ComponentModel.PropertyDescriptor" /> that is being used for sorting.</summary>
		/// <returns>The <see cref="T:System.ComponentModel.PropertyDescriptor" /> that is being used for sorting.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" /> is <see langword="false" />.</exception>
		PropertyDescriptor SortProperty { get; }

		/// <summary>Gets the direction of the sort.</summary>
		/// <returns>One of the <see cref="T:System.ComponentModel.ListSortDirection" /> values.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" /> is <see langword="false" />.</exception>
		ListSortDirection SortDirection { get; }

		/// <summary>Occurs when the list changes or an item in the list changes.</summary>
		event ListChangedEventHandler ListChanged;

		/// <summary>Adds the <see cref="T:System.ComponentModel.PropertyDescriptor" /> to the indexes used for searching.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to add to the indexes used for searching.</param>
		void AddIndex(PropertyDescriptor property);

		/// <summary>Sorts the list based on a <see cref="T:System.ComponentModel.PropertyDescriptor" /> and a <see cref="T:System.ComponentModel.ListSortDirection" />.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to sort by.</param>
		/// <param name="direction">One of the <see cref="T:System.ComponentModel.ListSortDirection" /> values.</param>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" /> is <see langword="false" />.</exception>
		void ApplySort(PropertyDescriptor property, ListSortDirection direction);

		/// <summary>Returns the index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor" />.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to search on.</param>
		/// <param name="key">The value of the <paramref name="property" /> parameter to search for.</param>
		/// <returns>The index of the row that has the given <see cref="T:System.ComponentModel.PropertyDescriptor" />.</returns>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSearching" /> is <see langword="false" />.</exception>
		int Find(PropertyDescriptor property, object key);

		/// <summary>Removes the <see cref="T:System.ComponentModel.PropertyDescriptor" /> from the indexes used for searching.</summary>
		/// <param name="property">The <see cref="T:System.ComponentModel.PropertyDescriptor" /> to remove from the indexes used for searching.</param>
		void RemoveIndex(PropertyDescriptor property);

		/// <summary>Removes any sort applied using <see cref="M:System.ComponentModel.IBindingList.ApplySort(System.ComponentModel.PropertyDescriptor,System.ComponentModel.ListSortDirection)" />.</summary>
		/// <exception cref="T:System.NotSupportedException">
		///   <see cref="P:System.ComponentModel.IBindingList.SupportsSorting" /> is <see langword="false" />.</exception>
		void RemoveSort();
	}
}
