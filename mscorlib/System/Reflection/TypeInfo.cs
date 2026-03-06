using System;
using System.Collections.Generic;

namespace System.Reflection
{
	/// <summary>Represents type declarations for class types, interface types, array types, value types, enumeration types, type parameters, generic type definitions, and open or closed constructed generic types.</summary>
	public abstract class TypeInfo : Type, IReflectableType
	{
		/// <summary>Returns a representation of the current type as a <see cref="T:System.Reflection.TypeInfo" /> object.</summary>
		/// <returns>A reference to the current type.</returns>
		TypeInfo IReflectableType.GetTypeInfo()
		{
			return this;
		}

		/// <summary>Returns the current type as a <see cref="T:System.Type" /> object.</summary>
		/// <returns>The current type.</returns>
		public virtual Type AsType()
		{
			return this;
		}

		/// <summary>Gets an array of the generic type parameters of the current instance.</summary>
		/// <returns>An array that contains the current instance's generic type parameters, or an array of <see cref="P:System.Array.Length" /> zero if the current instance has no generic type parameters.</returns>
		public virtual Type[] GenericTypeParameters
		{
			get
			{
				if (!this.IsGenericTypeDefinition)
				{
					return Type.EmptyTypes;
				}
				return this.GetGenericArguments();
			}
		}

		/// <summary>Returns an object that represents the specified public event declared by the current type.</summary>
		/// <param name="name">The name of the event.</param>
		/// <returns>An object that represents the specified event, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual EventInfo GetDeclaredEvent(string name)
		{
			return this.GetEvent(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		/// <summary>Returns an object that represents the specified public field declared by the current type.</summary>
		/// <param name="name">The name of the field.</param>
		/// <returns>An object that represents the specified field, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual FieldInfo GetDeclaredField(string name)
		{
			return this.GetField(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		/// <summary>Returns an object that represents the specified public method declared by the current type.</summary>
		/// <param name="name">The name of the method.</param>
		/// <returns>An object that represents the specified method, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual MethodInfo GetDeclaredMethod(string name)
		{
			return base.GetMethod(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		/// <summary>Returns an object that represents the specified public nested type declared by the current type.</summary>
		/// <param name="name">The name of the nested type.</param>
		/// <returns>An object that represents the specified nested type, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual TypeInfo GetDeclaredNestedType(string name)
		{
			Type nestedType = this.GetNestedType(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			if (nestedType == null)
			{
				return null;
			}
			return nestedType.GetTypeInfo();
		}

		/// <summary>Returns an object that represents the specified public property declared by the current type.</summary>
		/// <param name="name">The name of the property.</param>
		/// <returns>An object that represents the specified property, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual PropertyInfo GetDeclaredProperty(string name)
		{
			return base.GetProperty(name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}

		/// <summary>Returns a collection that contains all public methods declared on the current type that match the specified name.</summary>
		/// <param name="name">The method name to search for.</param>
		/// <returns>A collection that contains methods that match <paramref name="name" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual IEnumerable<MethodInfo> GetDeclaredMethods(string name)
		{
			foreach (MethodInfo methodInfo in this.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				if (methodInfo.Name == name)
				{
					yield return methodInfo;
				}
			}
			MethodInfo[] array = null;
			yield break;
		}

		/// <summary>Gets a collection of the constructors declared by the current type.</summary>
		/// <returns>A collection of the constructors declared by the current type.</returns>
		public virtual IEnumerable<ConstructorInfo> DeclaredConstructors
		{
			get
			{
				return this.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the events defined by the current type.</summary>
		/// <returns>A collection of the events defined by the current type.</returns>
		public virtual IEnumerable<EventInfo> DeclaredEvents
		{
			get
			{
				return this.GetEvents(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the fields defined by the current type.</summary>
		/// <returns>A collection of the fields defined by the current type.</returns>
		public virtual IEnumerable<FieldInfo> DeclaredFields
		{
			get
			{
				return this.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the members defined by the current type.</summary>
		/// <returns>A collection of the members defined by the current type.</returns>
		public virtual IEnumerable<MemberInfo> DeclaredMembers
		{
			get
			{
				return this.GetMembers(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the methods defined by the current type.</summary>
		/// <returns>A collection of the methods defined by the current type.</returns>
		public virtual IEnumerable<MethodInfo> DeclaredMethods
		{
			get
			{
				return this.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the nested types defined by the current type.</summary>
		/// <returns>A collection of nested types defined by the current type.</returns>
		public virtual IEnumerable<TypeInfo> DeclaredNestedTypes
		{
			get
			{
				foreach (Type type in this.GetNestedTypes(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
				{
					yield return type.GetTypeInfo();
				}
				Type[] array = null;
				yield break;
			}
		}

		/// <summary>Gets a collection of the properties defined by the current type.</summary>
		/// <returns>A collection of the properties defined by the current type.</returns>
		public virtual IEnumerable<PropertyInfo> DeclaredProperties
		{
			get
			{
				return this.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			}
		}

		/// <summary>Gets a collection of the interfaces implemented by the current type.</summary>
		/// <returns>A collection of the interfaces implemented by the current type.</returns>
		public virtual IEnumerable<Type> ImplementedInterfaces
		{
			get
			{
				return this.GetInterfaces();
			}
		}

		/// <summary>Returns a value that indicates whether the specified type can be assigned to the current type.</summary>
		/// <param name="typeInfo">The type to check.</param>
		/// <returns>
		///   <see langword="true" /> if the specified type can be assigned to this type; otherwise, <see langword="false" />.</returns>
		public virtual bool IsAssignableFrom(TypeInfo typeInfo)
		{
			if (typeInfo == null)
			{
				return false;
			}
			if (this == typeInfo)
			{
				return true;
			}
			if (typeInfo.IsSubclassOf(this))
			{
				return true;
			}
			if (base.IsInterface)
			{
				return typeInfo.ImplementInterface(this);
			}
			if (this.IsGenericParameter)
			{
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				for (int i = 0; i < genericParameterConstraints.Length; i++)
				{
					if (!genericParameterConstraints[i].IsAssignableFrom(typeInfo))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		private const BindingFlags DeclaredOnlyLookup = BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
	}
}
