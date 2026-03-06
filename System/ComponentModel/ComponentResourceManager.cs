using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;

namespace System.ComponentModel
{
	/// <summary>Provides simple functionality for enumerating resources for a component or object. The <see cref="T:System.ComponentModel.ComponentResourceManager" /> class is a <see cref="T:System.Resources.ResourceManager" />.</summary>
	public class ComponentResourceManager : ResourceManager
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.ComponentResourceManager" /> class with default values.</summary>
		public ComponentResourceManager()
		{
		}

		/// <summary>Creates a <see cref="T:System.ComponentModel.ComponentResourceManager" /> that looks up resources in satellite assemblies based on information from the specified <see cref="T:System.Type" />.</summary>
		/// <param name="t">A <see cref="T:System.Type" /> from which the <see cref="T:System.ComponentModel.ComponentResourceManager" /> derives all information for finding resource files.</param>
		public ComponentResourceManager(Type t) : base(t)
		{
		}

		private CultureInfo NeutralResourcesCulture
		{
			get
			{
				if (this._neutralResourcesCulture == null && this.MainAssembly != null)
				{
					this._neutralResourcesCulture = ResourceManager.GetNeutralResourcesLanguage(this.MainAssembly);
				}
				return this._neutralResourcesCulture;
			}
		}

		/// <summary>Applies a resource's value to the corresponding property of the object.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> that contains the property value to be applied.</param>
		/// <param name="objectName">A <see cref="T:System.String" /> that contains the name of the object to look up in the resources.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> or <paramref name="objectName" /> is <see langword="null" />.</exception>
		public void ApplyResources(object value, string objectName)
		{
			this.ApplyResources(value, objectName, null);
		}

		/// <summary>Applies a resource's value to the corresponding property of the object.</summary>
		/// <param name="value">An <see cref="T:System.Object" /> that contains the property value to be applied.</param>
		/// <param name="objectName">A <see cref="T:System.String" /> that contains the name of the object to look up in the resources.</param>
		/// <param name="culture">The culture for which to apply resources.</param>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> or <paramref name="objectName" /> is <see langword="null" />.</exception>
		public virtual void ApplyResources(object value, string objectName, CultureInfo culture)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (objectName == null)
			{
				throw new ArgumentNullException("objectName");
			}
			if (culture == null)
			{
				culture = CultureInfo.CurrentUICulture;
			}
			SortedList<string, object> sortedList;
			if (this._resourceSets == null)
			{
				this._resourceSets = new Hashtable();
				ResourceSet resourceSet;
				sortedList = this.FillResources(culture, out resourceSet);
				this._resourceSets[culture] = sortedList;
			}
			else
			{
				sortedList = (SortedList<string, object>)this._resourceSets[culture];
				if (sortedList == null || sortedList.Comparer.Equals(StringComparer.OrdinalIgnoreCase) != this.IgnoreCase)
				{
					ResourceSet resourceSet2;
					sortedList = this.FillResources(culture, out resourceSet2);
					this._resourceSets[culture] = sortedList;
				}
			}
			BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty;
			if (this.IgnoreCase)
			{
				bindingFlags |= BindingFlags.IgnoreCase;
			}
			bool flag = false;
			if (value is IComponent)
			{
				ISite site = ((IComponent)value).Site;
				if (site != null && site.DesignMode)
				{
					flag = true;
				}
			}
			foreach (KeyValuePair<string, object> keyValuePair in sortedList)
			{
				string key = keyValuePair.Key;
				if (this.IgnoreCase)
				{
					if (string.Compare(key, 0, objectName, 0, objectName.Length, StringComparison.OrdinalIgnoreCase) != 0)
					{
						continue;
					}
				}
				else if (string.CompareOrdinal(key, 0, objectName, 0, objectName.Length) != 0)
				{
					continue;
				}
				int length = objectName.Length;
				if (key.Length > length && (key[length] == '.' || key[length] == '-'))
				{
					string name = key.Substring(length + 1);
					if (flag)
					{
						PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(value).Find(name, this.IgnoreCase);
						if (propertyDescriptor != null && !propertyDescriptor.IsReadOnly && (keyValuePair.Value == null || propertyDescriptor.PropertyType.IsInstanceOfType(keyValuePair.Value)))
						{
							propertyDescriptor.SetValue(value, keyValuePair.Value);
						}
					}
					else
					{
						PropertyInfo propertyInfo = null;
						try
						{
							propertyInfo = value.GetType().GetProperty(name, bindingFlags);
						}
						catch (AmbiguousMatchException)
						{
							Type type = value.GetType();
							do
							{
								propertyInfo = type.GetProperty(name, bindingFlags | BindingFlags.DeclaredOnly);
								type = type.BaseType;
							}
							while (propertyInfo == null && type != null && type != typeof(object));
						}
						if (propertyInfo != null && propertyInfo.CanWrite && (keyValuePair.Value == null || propertyInfo.PropertyType.IsInstanceOfType(keyValuePair.Value)))
						{
							propertyInfo.SetValue(value, keyValuePair.Value, null);
						}
					}
				}
			}
		}

		private SortedList<string, object> FillResources(CultureInfo culture, out ResourceSet resourceSet)
		{
			ResourceSet resourceSet2 = null;
			SortedList<string, object> sortedList;
			if (!culture.Equals(CultureInfo.InvariantCulture) && !culture.Equals(this.NeutralResourcesCulture))
			{
				sortedList = this.FillResources(culture.Parent, out resourceSet2);
			}
			else if (this.IgnoreCase)
			{
				sortedList = new SortedList<string, object>(StringComparer.OrdinalIgnoreCase);
			}
			else
			{
				sortedList = new SortedList<string, object>(StringComparer.Ordinal);
			}
			resourceSet = this.GetResourceSet(culture, true, true);
			if (resourceSet != null && resourceSet != resourceSet2)
			{
				foreach (object obj in resourceSet)
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)obj;
					sortedList[(string)dictionaryEntry.Key] = dictionaryEntry.Value;
				}
			}
			return sortedList;
		}

		private Hashtable _resourceSets;

		private CultureInfo _neutralResourcesCulture;
	}
}
