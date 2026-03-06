using System;
using System.Collections;
using System.Globalization;
using System.Security.Permissions;
using Unity;

namespace System.ComponentModel.Design
{
	/// <summary>Provides a base class for getting and setting option values for a designer.</summary>
	[HostProtection(SecurityAction.LinkDemand, SharedState = true)]
	public abstract class DesignerOptionService : IDesignerOptionService
	{
		/// <summary>Gets the options collection for this service.</summary>
		/// <returns>A <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" /> populated with available designer options.</returns>
		public DesignerOptionService.DesignerOptionCollection Options
		{
			get
			{
				if (this._options == null)
				{
					this._options = new DesignerOptionService.DesignerOptionCollection(this, null, string.Empty, null);
				}
				return this._options;
			}
		}

		/// <summary>Creates a new <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" /> with the given name and adds it to the given parent.</summary>
		/// <param name="parent">The parent designer option collection. All collections have a parent except the root object collection.</param>
		/// <param name="name">The name of this collection.</param>
		/// <param name="value">The object providing properties for this collection. Can be <see langword="null" /> if the collection should not provide any properties.</param>
		/// <returns>A new <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" /> with the given name.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="parent" /> or <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="name" /> is an empty string.</exception>
		protected DesignerOptionService.DesignerOptionCollection CreateOptionCollection(DesignerOptionService.DesignerOptionCollection parent, string name, object value)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (name.Length == 0)
			{
				throw new ArgumentException(SR.GetString("'{1}' is not a valid value for '{0}'.", new object[]
				{
					name.Length.ToString(CultureInfo.CurrentCulture),
					0.ToString(CultureInfo.CurrentCulture)
				}), "name.Length");
			}
			return new DesignerOptionService.DesignerOptionCollection(this, parent, name, value);
		}

		private PropertyDescriptor GetOptionProperty(string pageName, string valueName)
		{
			if (pageName == null)
			{
				throw new ArgumentNullException("pageName");
			}
			if (valueName == null)
			{
				throw new ArgumentNullException("valueName");
			}
			string[] array = pageName.Split(new char[]
			{
				'\\'
			});
			DesignerOptionService.DesignerOptionCollection designerOptionCollection = this.Options;
			foreach (string name in array)
			{
				designerOptionCollection = designerOptionCollection[name];
				if (designerOptionCollection == null)
				{
					return null;
				}
			}
			return designerOptionCollection.Properties[valueName];
		}

		/// <summary>Populates a <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />.</summary>
		/// <param name="options">The collection to populate.</param>
		protected virtual void PopulateOptionCollection(DesignerOptionService.DesignerOptionCollection options)
		{
		}

		/// <summary>Shows the options dialog box for the given object.</summary>
		/// <param name="options">The options collection containing the object to be invoked.</param>
		/// <param name="optionObject">The actual options object.</param>
		/// <returns>
		///   <see langword="true" /> if the dialog box is shown; otherwise, <see langword="false" />.</returns>
		protected virtual bool ShowDialog(DesignerOptionService.DesignerOptionCollection options, object optionObject)
		{
			return false;
		}

		/// <summary>Gets the value of an option defined in this package.</summary>
		/// <param name="pageName">The page to which the option is bound.</param>
		/// <param name="valueName">The name of the option value.</param>
		/// <returns>The value of the option named <paramref name="valueName" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="pageName" /> or <paramref name="valueName" /> is <see langword="null" />.</exception>
		object IDesignerOptionService.GetOptionValue(string pageName, string valueName)
		{
			PropertyDescriptor optionProperty = this.GetOptionProperty(pageName, valueName);
			if (optionProperty != null)
			{
				return optionProperty.GetValue(null);
			}
			return null;
		}

		/// <summary>Sets the value of an option defined in this package.</summary>
		/// <param name="pageName">The page to which the option is bound</param>
		/// <param name="valueName">The name of the option value.</param>
		/// <param name="value">The value of the option.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="pageName" /> or <paramref name="valueName" /> is <see langword="null" />.</exception>
		void IDesignerOptionService.SetOptionValue(string pageName, string valueName, object value)
		{
			PropertyDescriptor optionProperty = this.GetOptionProperty(pageName, valueName);
			if (optionProperty != null)
			{
				optionProperty.SetValue(null, value);
			}
		}

		private DesignerOptionService.DesignerOptionCollection _options;

		/// <summary>Contains a collection of designer options. This class cannot be inherited.</summary>
		[TypeConverter(typeof(DesignerOptionService.DesignerOptionConverter))]
		[Editor("", "System.Drawing.Design.UITypeEditor, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
		public sealed class DesignerOptionCollection : IList, ICollection, IEnumerable
		{
			internal DesignerOptionCollection(DesignerOptionService service, DesignerOptionService.DesignerOptionCollection parent, string name, object value)
			{
				this._service = service;
				this._parent = parent;
				this._name = name;
				this._value = value;
				if (this._parent != null)
				{
					if (this._parent._children == null)
					{
						this._parent._children = new ArrayList(1);
					}
					this._parent._children.Add(this);
				}
			}

			/// <summary>Gets the number of child option collections this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" /> contains.</summary>
			/// <returns>The number of child option collections this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" /> contains.</returns>
			public int Count
			{
				get
				{
					this.EnsurePopulated();
					return this._children.Count;
				}
			}

			/// <summary>Gets the name of this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />.</summary>
			/// <returns>The name of this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />.</returns>
			public string Name
			{
				get
				{
					return this._name;
				}
			}

			/// <summary>Gets the parent collection object.</summary>
			/// <returns>The parent collection object, or <see langword="null" /> if there is no parent.</returns>
			public DesignerOptionService.DesignerOptionCollection Parent
			{
				get
				{
					return this._parent;
				}
			}

			/// <summary>Gets the collection of properties offered by this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />, along with all of its children.</summary>
			/// <returns>The collection of properties offered by this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />, along with all of its children.</returns>
			public PropertyDescriptorCollection Properties
			{
				get
				{
					if (this._properties == null)
					{
						ArrayList arrayList;
						if (this._value != null)
						{
							PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(this._value);
							arrayList = new ArrayList(properties.Count);
							using (IEnumerator enumerator = properties.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									object obj = enumerator.Current;
									PropertyDescriptor property = (PropertyDescriptor)obj;
									arrayList.Add(new DesignerOptionService.DesignerOptionCollection.WrappedPropertyDescriptor(property, this._value));
								}
								goto IL_76;
							}
						}
						arrayList = new ArrayList(1);
						IL_76:
						this.EnsurePopulated();
						foreach (object obj2 in this._children)
						{
							DesignerOptionService.DesignerOptionCollection designerOptionCollection = (DesignerOptionService.DesignerOptionCollection)obj2;
							arrayList.AddRange(designerOptionCollection.Properties);
						}
						PropertyDescriptor[] properties2 = (PropertyDescriptor[])arrayList.ToArray(typeof(PropertyDescriptor));
						this._properties = new PropertyDescriptorCollection(properties2, true);
					}
					return this._properties;
				}
			}

			/// <summary>Gets the child collection at the given index.</summary>
			/// <param name="index">The zero-based index of the child collection to get.</param>
			/// <returns>The child collection at the specified index.</returns>
			public DesignerOptionService.DesignerOptionCollection this[int index]
			{
				get
				{
					this.EnsurePopulated();
					if (index < 0 || index >= this._children.Count)
					{
						throw new IndexOutOfRangeException("index");
					}
					return (DesignerOptionService.DesignerOptionCollection)this._children[index];
				}
			}

			/// <summary>Gets the child collection at the given name.</summary>
			/// <param name="name">The name of the child collection.</param>
			/// <returns>The child collection with the name specified by the <paramref name="name" /> parameter, or <see langword="null" /> if the name is not found.</returns>
			public DesignerOptionService.DesignerOptionCollection this[string name]
			{
				get
				{
					this.EnsurePopulated();
					foreach (object obj in this._children)
					{
						DesignerOptionService.DesignerOptionCollection designerOptionCollection = (DesignerOptionService.DesignerOptionCollection)obj;
						if (string.Compare(designerOptionCollection.Name, name, true, CultureInfo.InvariantCulture) == 0)
						{
							return designerOptionCollection;
						}
					}
					return null;
				}
			}

			/// <summary>Copies the entire collection to a compatible one-dimensional <see cref="T:System.Array" />, starting at the specified index of the target array.</summary>
			/// <param name="array">The one-dimensional <see cref="T:System.Array" /> that is the destination of the elements copied from the collection. The <paramref name="array" /> must have zero-based indexing.</param>
			/// <param name="index">The zero-based index in <paramref name="array" /> at which copying begins.</param>
			public void CopyTo(Array array, int index)
			{
				this.EnsurePopulated();
				this._children.CopyTo(array, index);
			}

			private void EnsurePopulated()
			{
				if (this._children == null)
				{
					this._service.PopulateOptionCollection(this);
					if (this._children == null)
					{
						this._children = new ArrayList(1);
					}
				}
			}

			/// <summary>Returns an <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate this collection.</summary>
			/// <returns>An <see cref="T:System.Collections.IEnumerator" /> that can be used to iterate this collection.</returns>
			public IEnumerator GetEnumerator()
			{
				this.EnsurePopulated();
				return this._children.GetEnumerator();
			}

			/// <summary>Returns the index of the first occurrence of a given value in a range of this collection.</summary>
			/// <param name="value">The object to locate in the collection.</param>
			/// <returns>The index of the first occurrence of value within the entire collection, if found; otherwise, the lower bound of the collection minus 1.</returns>
			public int IndexOf(DesignerOptionService.DesignerOptionCollection value)
			{
				this.EnsurePopulated();
				return this._children.IndexOf(value);
			}

			private static object RecurseFindValue(DesignerOptionService.DesignerOptionCollection options)
			{
				if (options._value != null)
				{
					return options._value;
				}
				foreach (object obj in options)
				{
					object obj2 = DesignerOptionService.DesignerOptionCollection.RecurseFindValue((DesignerOptionService.DesignerOptionCollection)obj);
					if (obj2 != null)
					{
						return obj2;
					}
				}
				return null;
			}

			/// <summary>Displays a dialog box user interface (UI) with which the user can configure the options in this <see cref="T:System.ComponentModel.Design.DesignerOptionService.DesignerOptionCollection" />.</summary>
			/// <returns>
			///   <see langword="true" /> if the dialog box can be displayed; otherwise, <see langword="false" />.</returns>
			public bool ShowDialog()
			{
				object obj = DesignerOptionService.DesignerOptionCollection.RecurseFindValue(this);
				return obj != null && this._service.ShowDialog(this, obj);
			}

			/// <summary>Gets a value indicating whether access to the collection is synchronized and, therefore, thread safe.</summary>
			/// <returns>
			///   <see langword="true" /> if the access to the collection is synchronized; otherwise, <see langword="false" />.</returns>
			bool ICollection.IsSynchronized
			{
				get
				{
					return false;
				}
			}

			/// <summary>Gets an object that can be used to synchronize access to the collection.</summary>
			/// <returns>An object that can be used to synchronize access to the collection.</returns>
			object ICollection.SyncRoot
			{
				get
				{
					return this;
				}
			}

			/// <summary>Gets a value indicating whether the collection has a fixed size.</summary>
			/// <returns>
			///   <see langword="true" /> if the collection has a fixed size; otherwise, <see langword="false" />.</returns>
			bool IList.IsFixedSize
			{
				get
				{
					return true;
				}
			}

			/// <summary>Gets a value indicating whether the collection is read-only.</summary>
			/// <returns>
			///   <see langword="true" /> if the collection is read-only; otherwise, <see langword="false" />.</returns>
			bool IList.IsReadOnly
			{
				get
				{
					return true;
				}
			}

			/// <summary>Gets or sets the element at the specified index.</summary>
			/// <param name="index">The zero-based index of the element to get or set.</param>
			/// <returns>The element at the specified index.</returns>
			object IList.this[int index]
			{
				get
				{
					return this[index];
				}
				set
				{
					throw new NotSupportedException();
				}
			}

			/// <summary>Adds an item to the <see cref="T:System.Collections.IList" />.</summary>
			/// <param name="value">The <see cref="T:System.Object" /> to add to the <see cref="T:System.Collections.IList" />.</param>
			/// <returns>The position into which the new element was inserted.</returns>
			int IList.Add(object value)
			{
				throw new NotSupportedException();
			}

			/// <summary>Removes all items from the collection.</summary>
			void IList.Clear()
			{
				throw new NotSupportedException();
			}

			/// <summary>Determines whether the collection contains a specific value.</summary>
			/// <param name="value">The <see cref="T:System.Object" /> to locate in the collection</param>
			/// <returns>
			///   <see langword="true" /> if the <see cref="T:System.Object" /> is found in the collection; otherwise, <see langword="false" />.</returns>
			bool IList.Contains(object value)
			{
				this.EnsurePopulated();
				return this._children.Contains(value);
			}

			/// <summary>Determines the index of a specific item in the collection.</summary>
			/// <param name="value">The <see cref="T:System.Object" /> to locate in the collection.</param>
			/// <returns>The index of <paramref name="value" /> if found in the list; otherwise, -1.</returns>
			int IList.IndexOf(object value)
			{
				this.EnsurePopulated();
				return this._children.IndexOf(value);
			}

			/// <summary>Inserts an item into the collection at the specified index.</summary>
			/// <param name="index">The zero-based index at which <paramref name="value" /> should be inserted.</param>
			/// <param name="value">The <see cref="T:System.Object" /> to insert into the collection.</param>
			void IList.Insert(int index, object value)
			{
				throw new NotSupportedException();
			}

			/// <summary>Removes the first occurrence of a specific object from the collection.</summary>
			/// <param name="value">The <see cref="T:System.Object" /> to remove from the collection.</param>
			void IList.Remove(object value)
			{
				throw new NotSupportedException();
			}

			/// <summary>Removes the collection item at the specified index.</summary>
			/// <param name="index">The zero-based index of the item to remove.</param>
			void IList.RemoveAt(int index)
			{
				throw new NotSupportedException();
			}

			internal DesignerOptionCollection()
			{
				ThrowStub.ThrowNotSupportedException();
			}

			private DesignerOptionService _service;

			private DesignerOptionService.DesignerOptionCollection _parent;

			private string _name;

			private object _value;

			private ArrayList _children;

			private PropertyDescriptorCollection _properties;

			private sealed class WrappedPropertyDescriptor : PropertyDescriptor
			{
				internal WrappedPropertyDescriptor(PropertyDescriptor property, object target) : base(property.Name, null)
				{
					this.property = property;
					this.target = target;
				}

				public override AttributeCollection Attributes
				{
					get
					{
						return this.property.Attributes;
					}
				}

				public override Type ComponentType
				{
					get
					{
						return this.property.ComponentType;
					}
				}

				public override bool IsReadOnly
				{
					get
					{
						return this.property.IsReadOnly;
					}
				}

				public override Type PropertyType
				{
					get
					{
						return this.property.PropertyType;
					}
				}

				public override bool CanResetValue(object component)
				{
					return this.property.CanResetValue(this.target);
				}

				public override object GetValue(object component)
				{
					return this.property.GetValue(this.target);
				}

				public override void ResetValue(object component)
				{
					this.property.ResetValue(this.target);
				}

				public override void SetValue(object component, object value)
				{
					this.property.SetValue(this.target, value);
				}

				public override bool ShouldSerializeValue(object component)
				{
					return this.property.ShouldSerializeValue(this.target);
				}

				private object target;

				private PropertyDescriptor property;
			}
		}

		internal sealed class DesignerOptionConverter : TypeConverter
		{
			public override bool GetPropertiesSupported(ITypeDescriptorContext cxt)
			{
				return true;
			}

			public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext cxt, object value, Attribute[] attributes)
			{
				PropertyDescriptorCollection propertyDescriptorCollection = new PropertyDescriptorCollection(null);
				DesignerOptionService.DesignerOptionCollection designerOptionCollection = value as DesignerOptionService.DesignerOptionCollection;
				if (designerOptionCollection == null)
				{
					return propertyDescriptorCollection;
				}
				foreach (object obj in designerOptionCollection)
				{
					DesignerOptionService.DesignerOptionCollection option = (DesignerOptionService.DesignerOptionCollection)obj;
					propertyDescriptorCollection.Add(new DesignerOptionService.DesignerOptionConverter.OptionPropertyDescriptor(option));
				}
				foreach (object obj2 in designerOptionCollection.Properties)
				{
					PropertyDescriptor value2 = (PropertyDescriptor)obj2;
					propertyDescriptorCollection.Add(value2);
				}
				return propertyDescriptorCollection;
			}

			public override object ConvertTo(ITypeDescriptorContext cxt, CultureInfo culture, object value, Type destinationType)
			{
				if (destinationType == typeof(string))
				{
					return SR.GetString("(Collection)");
				}
				return base.ConvertTo(cxt, culture, value, destinationType);
			}

			private class OptionPropertyDescriptor : PropertyDescriptor
			{
				internal OptionPropertyDescriptor(DesignerOptionService.DesignerOptionCollection option) : base(option.Name, null)
				{
					this._option = option;
				}

				public override Type ComponentType
				{
					get
					{
						return this._option.GetType();
					}
				}

				public override bool IsReadOnly
				{
					get
					{
						return true;
					}
				}

				public override Type PropertyType
				{
					get
					{
						return this._option.GetType();
					}
				}

				public override bool CanResetValue(object component)
				{
					return false;
				}

				public override object GetValue(object component)
				{
					return this._option;
				}

				public override void ResetValue(object component)
				{
				}

				public override void SetValue(object component, object value)
				{
				}

				public override bool ShouldSerializeValue(object component)
				{
					return false;
				}

				private DesignerOptionService.DesignerOptionCollection _option;
			}
		}
	}
}
