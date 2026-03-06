using System;
using System.Collections;

namespace System.ComponentModel
{
	/// <summary>Provides supplemental metadata to the <see cref="T:System.ComponentModel.TypeDescriptor" />.</summary>
	public abstract class TypeDescriptionProvider
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> class.</summary>
		protected TypeDescriptionProvider()
		{
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.ComponentModel.TypeDescriptionProvider" /> class using a parent type description provider.</summary>
		/// <param name="parent">The parent type description provider.</param>
		protected TypeDescriptionProvider(TypeDescriptionProvider parent)
		{
			this._parent = parent;
		}

		/// <summary>Creates an object that can substitute for another data type.</summary>
		/// <param name="provider">An optional service provider.</param>
		/// <param name="objectType">The type of object to create. This parameter is never <see langword="null" />.</param>
		/// <param name="argTypes">An optional array of types that represent the parameter types to be passed to the object's constructor. This array can be <see langword="null" /> or of zero length.</param>
		/// <param name="args">An optional array of parameter values to pass to the object's constructor.</param>
		/// <returns>The substitute <see cref="T:System.Object" />.</returns>
		public virtual object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
		{
			if (this._parent != null)
			{
				return this._parent.CreateInstance(provider, objectType, argTypes, args);
			}
			if (objectType == null)
			{
				throw new ArgumentNullException("objectType");
			}
			return Activator.CreateInstance(objectType, args);
		}

		/// <summary>Gets a per-object cache, accessed as an <see cref="T:System.Collections.IDictionary" /> of key/value pairs.</summary>
		/// <param name="instance">The object for which to get the cache.</param>
		/// <returns>An <see cref="T:System.Collections.IDictionary" /> if the provided object supports caching; otherwise, <see langword="null" />.</returns>
		public virtual IDictionary GetCache(object instance)
		{
			TypeDescriptionProvider parent = this._parent;
			if (parent == null)
			{
				return null;
			}
			return parent.GetCache(instance);
		}

		/// <summary>Gets an extended custom type descriptor for the given object.</summary>
		/// <param name="instance">The object for which to get the extended type descriptor.</param>
		/// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide extended metadata for the object.</returns>
		public virtual ICustomTypeDescriptor GetExtendedTypeDescriptor(object instance)
		{
			if (this._parent != null)
			{
				return this._parent.GetExtendedTypeDescriptor(instance);
			}
			TypeDescriptionProvider.EmptyCustomTypeDescriptor result;
			if ((result = this._emptyDescriptor) == null)
			{
				result = (this._emptyDescriptor = new TypeDescriptionProvider.EmptyCustomTypeDescriptor());
			}
			return result;
		}

		/// <summary>Gets the extender providers for the specified object.</summary>
		/// <param name="instance">The object to get extender providers for.</param>
		/// <returns>An array of extender providers for <paramref name="instance" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="instance" /> is <see langword="null" />.</exception>
		protected internal virtual IExtenderProvider[] GetExtenderProviders(object instance)
		{
			if (this._parent != null)
			{
				return this._parent.GetExtenderProviders(instance);
			}
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return Array.Empty<IExtenderProvider>();
		}

		/// <summary>Gets the name of the specified component, or <see langword="null" /> if the component has no name.</summary>
		/// <param name="component">The specified component.</param>
		/// <returns>The name of the specified component.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="component" /> is <see langword="null" />.</exception>
		public virtual string GetFullComponentName(object component)
		{
			if (this._parent != null)
			{
				return this._parent.GetFullComponentName(component);
			}
			return this.GetTypeDescriptor(component).GetComponentName();
		}

		/// <summary>Performs normal reflection against a type.</summary>
		/// <param name="objectType">The type of object for which to retrieve the <see cref="T:System.Reflection.IReflect" />.</param>
		/// <returns>The type of reflection for this <paramref name="objectType" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="objectType" /> is <see langword="null" />.</exception>
		public Type GetReflectionType(Type objectType)
		{
			return this.GetReflectionType(objectType, null);
		}

		/// <summary>Performs normal reflection against the given object.</summary>
		/// <param name="instance">An instance of the type (should not be <see langword="null" />).</param>
		/// <returns>The type of reflection for this <paramref name="instance" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="instance" /> is <see langword="null" />.</exception>
		public Type GetReflectionType(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return this.GetReflectionType(instance.GetType(), instance);
		}

		/// <summary>Performs normal reflection against the given object with the given type.</summary>
		/// <param name="objectType">The type of object for which to retrieve the <see cref="T:System.Reflection.IReflect" />.</param>
		/// <param name="instance">An instance of the type. Can be <see langword="null" />.</param>
		/// <returns>The type of reflection for this <paramref name="objectType" />.</returns>
		public virtual Type GetReflectionType(Type objectType, object instance)
		{
			if (this._parent != null)
			{
				return this._parent.GetReflectionType(objectType, instance);
			}
			return objectType;
		}

		/// <summary>Converts a reflection type into a runtime type.</summary>
		/// <param name="reflectionType">The type to convert to its runtime equivalent.</param>
		/// <returns>A <see cref="T:System.Type" /> that represents the runtime equivalent of <paramref name="reflectionType" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="reflectionType" /> is <see langword="null" />.</exception>
		public virtual Type GetRuntimeType(Type reflectionType)
		{
			if (this._parent != null)
			{
				return this._parent.GetRuntimeType(reflectionType);
			}
			if (reflectionType == null)
			{
				throw new ArgumentNullException("reflectionType");
			}
			if (reflectionType.GetType().Assembly == typeof(object).Assembly)
			{
				return reflectionType;
			}
			return reflectionType.UnderlyingSystemType;
		}

		/// <summary>Gets a custom type descriptor for the given type.</summary>
		/// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
		/// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide metadata for the type.</returns>
		public ICustomTypeDescriptor GetTypeDescriptor(Type objectType)
		{
			return this.GetTypeDescriptor(objectType, null);
		}

		/// <summary>Gets a custom type descriptor for the given object.</summary>
		/// <param name="instance">An instance of the type. Can be <see langword="null" /> if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide metadata for the type.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="instance" /> is <see langword="null" />.</exception>
		public ICustomTypeDescriptor GetTypeDescriptor(object instance)
		{
			if (instance == null)
			{
				throw new ArgumentNullException("instance");
			}
			return this.GetTypeDescriptor(instance.GetType(), instance);
		}

		/// <summary>Gets a custom type descriptor for the given type and object.</summary>
		/// <param name="objectType">The type of object for which to retrieve the type descriptor.</param>
		/// <param name="instance">An instance of the type. Can be <see langword="null" /> if no instance was passed to the <see cref="T:System.ComponentModel.TypeDescriptor" />.</param>
		/// <returns>An <see cref="T:System.ComponentModel.ICustomTypeDescriptor" /> that can provide metadata for the type.</returns>
		public virtual ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
		{
			if (this._parent != null)
			{
				return this._parent.GetTypeDescriptor(objectType, instance);
			}
			TypeDescriptionProvider.EmptyCustomTypeDescriptor result;
			if ((result = this._emptyDescriptor) == null)
			{
				result = (this._emptyDescriptor = new TypeDescriptionProvider.EmptyCustomTypeDescriptor());
			}
			return result;
		}

		/// <summary>Gets a value that indicates whether the specified type is compatible with the type description and its chain of type description providers.</summary>
		/// <param name="type">The type to test for compatibility.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="type" /> is compatible with the type description and its chain of type description providers; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="type" /> is <see langword="null" />.</exception>
		public virtual bool IsSupportedType(Type type)
		{
			if (type == null)
			{
				throw new ArgumentNullException("type");
			}
			return this._parent == null || this._parent.IsSupportedType(type);
		}

		private readonly TypeDescriptionProvider _parent;

		private TypeDescriptionProvider.EmptyCustomTypeDescriptor _emptyDescriptor;

		private sealed class EmptyCustomTypeDescriptor : CustomTypeDescriptor
		{
		}
	}
}
