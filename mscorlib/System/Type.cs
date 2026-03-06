using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Threading;

namespace System
{
	/// <summary>Represents type declarations: class types, interface types, array types, value types, enumeration types, type parameters, generic type definitions, and open or closed constructed generic types.</summary>
	[ComVisible(true)]
	[ComDefaultInterface(typeof(_Type))]
	[ClassInterface(ClassInterfaceType.None)]
	[Serializable]
	public abstract class Type : MemberInfo, IReflect, _Type
	{
		/// <summary>Returns a value that indicates whether the specified value exists in the current enumeration type.</summary>
		/// <param name="value">The value to be tested.</param>
		/// <returns>
		///   <see langword="true" /> if the specified value is a member of the current enumeration type; otherwise, <see langword="false" />.</returns>
		/// <exception cref="T:System.ArgumentException">The current type is not an enumeration.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">
		///   <paramref name="value" /> is of a type that cannot be the underlying type of an enumeration.</exception>
		public virtual bool IsEnumDefined(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this.IsEnum)
			{
				throw new ArgumentException("Type provided must be an Enum.", "enumType");
			}
			Type type = value.GetType();
			if (type.IsEnum)
			{
				if (!type.IsEquivalentTo(this))
				{
					throw new ArgumentException(SR.Format("Object must be the same type as the enum. The type passed in was '{0}'; the enum type was '{1}'.", type.ToString(), this.ToString()));
				}
				type = type.GetEnumUnderlyingType();
			}
			if (type == typeof(string))
			{
				object[] enumNames = this.GetEnumNames();
				return Array.IndexOf<object>(enumNames, value) >= 0;
			}
			if (!Type.IsIntegerType(type))
			{
				throw new InvalidOperationException("Unknown enum type.");
			}
			Type enumUnderlyingType = this.GetEnumUnderlyingType();
			if (enumUnderlyingType.GetTypeCodeImpl() != type.GetTypeCodeImpl())
			{
				throw new ArgumentException(SR.Format("Enum underlying type and the object must be same type or object must be a String. Type passed in was '{0}'; the enum underlying type was '{1}'.", type.ToString(), enumUnderlyingType.ToString()));
			}
			return Type.BinarySearch(this.GetEnumRawConstantValues(), value) >= 0;
		}

		/// <summary>Returns the name of the constant that has the specified value, for the current enumeration type.</summary>
		/// <param name="value">The value whose name is to be retrieved.</param>
		/// <returns>The name of the member of the current enumeration type that has the specified value, or <see langword="null" /> if no such constant is found.</returns>
		/// <exception cref="T:System.ArgumentException">The current type is not an enumeration.  
		///  -or-  
		///  <paramref name="value" /> is neither of the current type nor does it have the same underlying type as the current type.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="value" /> is <see langword="null" />.</exception>
		public virtual string GetEnumName(object value)
		{
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!this.IsEnum)
			{
				throw new ArgumentException("Type provided must be an Enum.", "enumType");
			}
			Type type = value.GetType();
			if (!type.IsEnum && !Type.IsIntegerType(type))
			{
				throw new ArgumentException("The value passed in must be an enum base or an underlying type for an enum, such as an Int32.", "value");
			}
			int num = Type.BinarySearch(this.GetEnumRawConstantValues(), value);
			if (num >= 0)
			{
				return this.GetEnumNames()[num];
			}
			return null;
		}

		/// <summary>Returns the names of the members of the current enumeration type.</summary>
		/// <returns>An array that contains the names of the members of the enumeration.</returns>
		/// <exception cref="T:System.ArgumentException">The current type is not an enumeration.</exception>
		public virtual string[] GetEnumNames()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException("Type provided must be an Enum.", "enumType");
			}
			string[] result;
			Array array;
			this.GetEnumData(out result, out array);
			return result;
		}

		private Array GetEnumRawConstantValues()
		{
			string[] array;
			Array result;
			this.GetEnumData(out array, out result);
			return result;
		}

		private void GetEnumData(out string[] enumNames, out Array enumValues)
		{
			FieldInfo[] fields = this.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
			object[] array = new object[fields.Length];
			string[] array2 = new string[fields.Length];
			for (int i = 0; i < fields.Length; i++)
			{
				array2[i] = fields[i].Name;
				array[i] = fields[i].GetRawConstantValue();
			}
			IComparer @default = Comparer<object>.Default;
			for (int j = 1; j < array.Length; j++)
			{
				int num = j;
				string text = array2[j];
				object obj = array[j];
				bool flag = false;
				while (@default.Compare(array[num - 1], obj) > 0)
				{
					array2[num] = array2[num - 1];
					array[num] = array[num - 1];
					num--;
					flag = true;
					if (num == 0)
					{
						break;
					}
				}
				if (flag)
				{
					array2[num] = text;
					array[num] = obj;
				}
			}
			enumNames = array2;
			enumValues = array;
		}

		private static int BinarySearch(Array array, object value)
		{
			ulong[] array2 = new ulong[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array2[i] = Enum.ToUInt64(array.GetValue(i));
			}
			ulong value2 = Enum.ToUInt64(value);
			return Array.BinarySearch<ulong>(array2, value2);
		}

		internal static bool IsIntegerType(Type t)
		{
			return t == typeof(int) || t == typeof(short) || t == typeof(ushort) || t == typeof(byte) || t == typeof(sbyte) || t == typeof(uint) || t == typeof(long) || t == typeof(ulong) || t == typeof(char) || t == typeof(bool);
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is serializable.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is serializable; otherwise, <see langword="false" />.</returns>
		public virtual bool IsSerializable
		{
			get
			{
				if ((this.GetAttributeFlagsImpl() & TypeAttributes.Serializable) != TypeAttributes.NotPublic)
				{
					return true;
				}
				Type type = this.UnderlyingSystemType;
				if (type.IsRuntimeImplemented())
				{
					while (!(type == typeof(Delegate)) && !(type == typeof(Enum)))
					{
						type = type.BaseType;
						if (!(type != null))
						{
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object has type parameters that have not been replaced by specific types.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> object is itself a generic type parameter or has type parameters for which specific types have not been supplied; otherwise, <see langword="false" />.</returns>
		public virtual bool ContainsGenericParameters
		{
			get
			{
				if (this.HasElementType)
				{
					return this.GetRootElementType().ContainsGenericParameters;
				}
				if (this.IsGenericParameter)
				{
					return true;
				}
				if (!this.IsGenericType)
				{
					return false;
				}
				Type[] genericArguments = this.GetGenericArguments();
				for (int i = 0; i < genericArguments.Length; i++)
				{
					if (genericArguments[i].ContainsGenericParameters)
					{
						return true;
					}
				}
				return false;
			}
		}

		internal Type GetRootElementType()
		{
			Type type = this;
			while (type.HasElementType)
			{
				type = type.GetElementType();
			}
			return type;
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> can be accessed by code outside the assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Type" /> is a public type or a public nested type such that all the enclosing types are public; otherwise, <see langword="false" />.</returns>
		public bool IsVisible
		{
			get
			{
				if (this.IsGenericParameter)
				{
					return true;
				}
				if (this.HasElementType)
				{
					return this.GetElementType().IsVisible;
				}
				Type type = this;
				while (type.IsNested)
				{
					if (!type.IsNestedPublic)
					{
						return false;
					}
					type = type.DeclaringType;
				}
				if (!type.IsPublic)
				{
					return false;
				}
				if (this.IsGenericType && !this.IsGenericTypeDefinition)
				{
					Type[] genericArguments = this.GetGenericArguments();
					for (int i = 0; i < genericArguments.Length; i++)
					{
						if (!genericArguments[i].IsVisible)
						{
							return false;
						}
					}
				}
				return true;
			}
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects representing a filtered list of interfaces implemented or inherited by the current <see cref="T:System.Type" />.</summary>
		/// <param name="filter">The delegate that compares the interfaces against <paramref name="filterCriteria" />.</param>
		/// <param name="filterCriteria">The search criteria that determines whether an interface should be included in the returned array.</param>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing a filtered list of the interfaces implemented or inherited by the current <see cref="T:System.Type" />, or an empty array of type <see cref="T:System.Type" /> if no interfaces matching the filter are implemented or inherited by the current <see cref="T:System.Type" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="filter" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A static initializer is invoked and throws an exception.</exception>
		public virtual Type[] FindInterfaces(TypeFilter filter, object filterCriteria)
		{
			if (filter == null)
			{
				throw new ArgumentNullException("filter");
			}
			Type[] interfaces = this.GetInterfaces();
			int num = 0;
			for (int i = 0; i < interfaces.Length; i++)
			{
				if (!filter(interfaces[i], filterCriteria))
				{
					interfaces[i] = null;
				}
				else
				{
					num++;
				}
			}
			if (num == interfaces.Length)
			{
				return interfaces;
			}
			Type[] array = new Type[num];
			num = 0;
			for (int j = 0; j < interfaces.Length; j++)
			{
				if (interfaces[j] != null)
				{
					array[num++] = interfaces[j];
				}
			}
			return array;
		}

		/// <summary>Returns a filtered array of <see cref="T:System.Reflection.MemberInfo" /> objects of the specified member type.</summary>
		/// <param name="memberType">An object that indicates the type of member to search for.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="filter">The delegate that does the comparisons, returning <see langword="true" /> if the member currently being inspected matches the <paramref name="filterCriteria" /> and <see langword="false" /> otherwise. You can use the <see langword="FilterAttribute" />, <see langword="FilterName" />, and <see langword="FilterNameIgnoreCase" /> delegates supplied by this class. The first uses the fields of <see langword="FieldAttributes" />, <see langword="MethodAttributes" />, and <see langword="MethodImplAttributes" /> as search criteria, and the other two delegates use <see langword="String" /> objects as the search criteria.</param>
		/// <param name="filterCriteria">The search criteria that determines whether a member is returned in the array of <see langword="MemberInfo" /> objects.  
		///  The fields of <see langword="FieldAttributes" />, <see langword="MethodAttributes" />, and <see langword="MethodImplAttributes" /> can be used in conjunction with the <see langword="FilterAttribute" /> delegate supplied by this class.</param>
		/// <returns>A filtered array of <see cref="T:System.Reflection.MemberInfo" /> objects of the specified member type.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if the current <see cref="T:System.Type" /> does not have members of type <paramref name="memberType" /> that match the filter criteria.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="filter" /> is <see langword="null" />.</exception>
		public virtual MemberInfo[] FindMembers(MemberTypes memberType, BindingFlags bindingAttr, MemberFilter filter, object filterCriteria)
		{
			MethodInfo[] array = null;
			ConstructorInfo[] array2 = null;
			FieldInfo[] array3 = null;
			PropertyInfo[] array4 = null;
			EventInfo[] array5 = null;
			Type[] array6 = null;
			int num = 0;
			if ((memberType & MemberTypes.Method) != (MemberTypes)0)
			{
				array = this.GetMethods(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array.Length; i++)
					{
						if (!filter(array[i], filterCriteria))
						{
							array[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array.Length;
				}
			}
			if ((memberType & MemberTypes.Constructor) != (MemberTypes)0)
			{
				array2 = this.GetConstructors(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array2.Length; i++)
					{
						if (!filter(array2[i], filterCriteria))
						{
							array2[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array2.Length;
				}
			}
			if ((memberType & MemberTypes.Field) != (MemberTypes)0)
			{
				array3 = this.GetFields(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array3.Length; i++)
					{
						if (!filter(array3[i], filterCriteria))
						{
							array3[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array3.Length;
				}
			}
			if ((memberType & MemberTypes.Property) != (MemberTypes)0)
			{
				array4 = this.GetProperties(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array4.Length; i++)
					{
						if (!filter(array4[i], filterCriteria))
						{
							array4[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array4.Length;
				}
			}
			if ((memberType & MemberTypes.Event) != (MemberTypes)0)
			{
				array5 = this.GetEvents(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array5.Length; i++)
					{
						if (!filter(array5[i], filterCriteria))
						{
							array5[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array5.Length;
				}
			}
			if ((memberType & MemberTypes.NestedType) != (MemberTypes)0)
			{
				array6 = this.GetNestedTypes(bindingAttr);
				if (filter != null)
				{
					for (int i = 0; i < array6.Length; i++)
					{
						if (!filter(array6[i], filterCriteria))
						{
							array6[i] = null;
						}
						else
						{
							num++;
						}
					}
				}
				else
				{
					num += array6.Length;
				}
			}
			MemberInfo[] array7 = new MemberInfo[num];
			num = 0;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i] != null)
					{
						array7[num++] = array[i];
					}
				}
			}
			if (array2 != null)
			{
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] != null)
					{
						array7[num++] = array2[i];
					}
				}
			}
			if (array3 != null)
			{
				for (int i = 0; i < array3.Length; i++)
				{
					if (array3[i] != null)
					{
						array7[num++] = array3[i];
					}
				}
			}
			if (array4 != null)
			{
				for (int i = 0; i < array4.Length; i++)
				{
					if (array4[i] != null)
					{
						array7[num++] = array4[i];
					}
				}
			}
			if (array5 != null)
			{
				for (int i = 0; i < array5.Length; i++)
				{
					if (array5[i] != null)
					{
						array7[num++] = array5[i];
					}
				}
			}
			if (array6 != null)
			{
				for (int i = 0; i < array6.Length; i++)
				{
					if (array6[i] != null)
					{
						array7[num++] = array6[i];
					}
				}
			}
			return array7;
		}

		/// <summary>Determines whether the current <see cref="T:System.Type" /> derives from the specified <see cref="T:System.Type" />.</summary>
		/// <param name="c">The type to compare with the current type.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see langword="Type" /> derives from <paramref name="c" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if <paramref name="c" /> and the current <see langword="Type" /> are equal.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="c" /> is <see langword="null" />.</exception>
		[ComVisible(true)]
		public virtual bool IsSubclassOf(Type c)
		{
			Type type = this;
			if (type == c)
			{
				return false;
			}
			while (type != null)
			{
				if (type == c)
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		/// <summary>Determines whether an instance of a specified type can be assigned to an instance of the current type.</summary>
		/// <param name="c">The type to compare with the current type.</param>
		/// <returns>
		///   <see langword="true" /> if any of the following conditions is true:  
		///
		/// <paramref name="c" /> and the current instance represent the same type.  
		///
		/// <paramref name="c" /> is derived either directly or indirectly from the current instance. <paramref name="c" /> is derived directly from the current instance if it inherits from the current instance; <paramref name="c" /> is derived indirectly from the current instance if it inherits from a succession of one or more classes that inherit from the current instance.  
		///
		/// The current instance is an interface that <paramref name="c" /> implements.  
		///
		/// <paramref name="c" /> is a generic type parameter, and the current instance represents one of the constraints of <paramref name="c" />.  
		///  In the following example, the current instance is a <see cref="T:System.Type" /> object that represents the <see cref="T:System.IO.Stream" /> class. GenericWithConstraint is a generic type whose generic type parameter must be of type    <see cref="T:System.IO.Stream" />. Passing its generic type parameter to the <see cref="M:System.Type.IsAssignableFrom(System.Type)" /> indicates that  an instance of the generic type parameter can be assigned to an <see cref="T:System.IO.Stream" /> object.  
		/// using System;
		/// using System.IO;
		///
		/// public class Example
		/// {
		///    public static void Main()
		///    {
		/// Type t = typeof(Stream);
		/// Type genericT = typeof(GenericWithConstraint&lt;&gt;);
		/// Type genericParam = genericT.GetGenericArguments()[0];
		/// Console.WriteLine(t.IsAssignableFrom(genericParam));  
		/// // Displays True.
		///    }
		/// }
		///
		/// public class GenericWithConstraint&lt;T&gt; where T : Stream
		/// {}
		/// Imports System.IO
		///
		/// Module Example
		///    Public Sub Main()
		/// Dim t As Type = GetType(Stream)
		/// Dim genericT As Type = GetType(GenericWithConstraint(Of ))
		/// Dim genericParam As Type = genericT.GetGenericArguments()(0)
		/// Console.WriteLine(t.IsAssignableFrom(genericParam))  
		/// ' Displays True.
		///    End Sub
		/// End Module
		///
		/// Public Class GenericWithConstraint(Of T As Stream)
		/// End Class
		///
		/// <paramref name="c" /> represents a value type, and the current instance represents Nullable&lt;c&gt; (Nullable(Of c) in Visual Basic).  
		///
		///
		///  <see langword="false" /> if none of these conditions are true, or if <paramref name="c" /> is <see langword="null" />.</returns>
		public virtual bool IsAssignableFrom(Type c)
		{
			if (c == null)
			{
				return false;
			}
			if (this == c)
			{
				return true;
			}
			Type underlyingSystemType = this.UnderlyingSystemType;
			if (underlyingSystemType.IsRuntimeImplemented())
			{
				return underlyingSystemType.IsAssignableFrom(c);
			}
			if (c.IsSubclassOf(this))
			{
				return true;
			}
			if (this.IsInterface)
			{
				return c.ImplementInterface(this);
			}
			if (this.IsGenericParameter)
			{
				Type[] genericParameterConstraints = this.GetGenericParameterConstraints();
				for (int i = 0; i < genericParameterConstraints.Length; i++)
				{
					if (!genericParameterConstraints[i].IsAssignableFrom(c))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		internal bool ImplementInterface(Type ifaceType)
		{
			Type type = this;
			while (type != null)
			{
				Type[] interfaces = type.GetInterfaces();
				if (interfaces != null)
				{
					for (int i = 0; i < interfaces.Length; i++)
					{
						if (interfaces[i] == ifaceType || (interfaces[i] != null && interfaces[i].ImplementInterface(ifaceType)))
						{
							return true;
						}
					}
				}
				type = type.BaseType;
			}
			return false;
		}

		private static bool FilterAttributeImpl(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null)
			{
				throw new InvalidFilterCriteriaException("An Int32 must be provided for the filter criteria.");
			}
			MemberTypes memberType = m.MemberType;
			if (memberType != MemberTypes.Constructor)
			{
				if (memberType == MemberTypes.Field)
				{
					FieldAttributes fieldAttributes = FieldAttributes.PrivateScope;
					try
					{
						fieldAttributes = (FieldAttributes)((int)filterCriteria);
					}
					catch
					{
						throw new InvalidFilterCriteriaException("An Int32 must be provided for the filter criteria.");
					}
					FieldAttributes attributes = ((FieldInfo)m).Attributes;
					return ((fieldAttributes & FieldAttributes.FieldAccessMask) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.FieldAccessMask) == (fieldAttributes & FieldAttributes.FieldAccessMask)) && ((fieldAttributes & FieldAttributes.Static) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.Static) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.InitOnly) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.InitOnly) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.Literal) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.Literal) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.NotSerialized) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.NotSerialized) != FieldAttributes.PrivateScope) && ((fieldAttributes & FieldAttributes.PinvokeImpl) == FieldAttributes.PrivateScope || (attributes & FieldAttributes.PinvokeImpl) != FieldAttributes.PrivateScope);
				}
				if (memberType != MemberTypes.Method)
				{
					return false;
				}
			}
			MethodAttributes methodAttributes = MethodAttributes.PrivateScope;
			try
			{
				methodAttributes = (MethodAttributes)((int)filterCriteria);
			}
			catch
			{
				throw new InvalidFilterCriteriaException("An Int32 must be provided for the filter criteria.");
			}
			MethodAttributes attributes2;
			if (m.MemberType == MemberTypes.Method)
			{
				attributes2 = ((MethodInfo)m).Attributes;
			}
			else
			{
				attributes2 = ((ConstructorInfo)m).Attributes;
			}
			return ((methodAttributes & MethodAttributes.MemberAccessMask) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.MemberAccessMask) == (methodAttributes & MethodAttributes.MemberAccessMask)) && ((methodAttributes & MethodAttributes.Static) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.Static) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Final) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.Final) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Virtual) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.Virtual) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.Abstract) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.Abstract) != MethodAttributes.PrivateScope) && ((methodAttributes & MethodAttributes.SpecialName) == MethodAttributes.PrivateScope || (attributes2 & MethodAttributes.SpecialName) != MethodAttributes.PrivateScope);
		}

		private static bool FilterNameImpl(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException("A String must be provided for the filter criteria.");
			}
			string text = (string)filterCriteria;
			text = text.Trim();
			string text2 = m.Name;
			if (m.MemberType == MemberTypes.NestedType)
			{
				text2 = text2.Substring(text2.LastIndexOf('+') + 1);
			}
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				return text2.StartsWith(text, StringComparison.Ordinal);
			}
			return text2.Equals(text);
		}

		private static bool FilterNameIgnoreCaseImpl(MemberInfo m, object filterCriteria)
		{
			if (filterCriteria == null || !(filterCriteria is string))
			{
				throw new InvalidFilterCriteriaException("A String must be provided for the filter criteria.");
			}
			string text = (string)filterCriteria;
			text = text.Trim();
			string text2 = m.Name;
			if (m.MemberType == MemberTypes.NestedType)
			{
				text2 = text2.Substring(text2.LastIndexOf('+') + 1);
			}
			if (text.Length > 0 && text[text.Length - 1] == '*')
			{
				text = text.Substring(0, text.Length - 1);
				return string.Compare(text2, 0, text, 0, text.Length, StringComparison.OrdinalIgnoreCase) == 0;
			}
			return string.Compare(text, text2, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>Gets a <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</summary>
		/// <returns>A <see cref="T:System.Reflection.MemberTypes" /> value indicating that this member is a type or a nested type.</returns>
		public override MemberTypes MemberType
		{
			get
			{
				return MemberTypes.TypeInfo;
			}
		}

		/// <summary>Gets the current <see cref="T:System.Type" />.</summary>
		/// <returns>The current <see cref="T:System.Type" />.</returns>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		public new Type GetType()
		{
			return base.GetType();
		}

		/// <summary>Gets the namespace of the <see cref="T:System.Type" />.</summary>
		/// <returns>The namespace of the <see cref="T:System.Type" />; <see langword="null" /> if the current instance has no namespace or represents a generic parameter.</returns>
		public abstract string Namespace { get; }

		/// <summary>Gets the assembly-qualified name of the type, which includes the name of the assembly from which this <see cref="T:System.Type" /> object was loaded.</summary>
		/// <returns>The assembly-qualified name of the <see cref="T:System.Type" />, which includes the name of the assembly from which the <see cref="T:System.Type" /> was loaded, or <see langword="null" /> if the current instance represents a generic type parameter.</returns>
		public abstract string AssemblyQualifiedName { get; }

		/// <summary>Gets the fully qualified name of the type, including its namespace but not its assembly.</summary>
		/// <returns>The fully qualified name of the type, including its namespace but not its assembly; or <see langword="null" /> if the current instance represents a generic type parameter, an array type, pointer type, or <see langword="byref" /> type based on a type parameter, or a generic type that is not a generic type definition but contains unresolved type parameters.</returns>
		public abstract string FullName { get; }

		/// <summary>Gets the <see cref="T:System.Reflection.Assembly" /> in which the type is declared. For generic types, gets the <see cref="T:System.Reflection.Assembly" /> in which the generic type is defined.</summary>
		/// <returns>An <see cref="T:System.Reflection.Assembly" /> instance that describes the assembly containing the current type. For generic types, the instance describes the assembly that contains the generic type definition, not the assembly that creates and uses a particular constructed type.</returns>
		public abstract Assembly Assembly { get; }

		/// <summary>Gets the module (the DLL) in which the current <see cref="T:System.Type" /> is defined.</summary>
		/// <returns>The module in which the current <see cref="T:System.Type" /> is defined.</returns>
		public new abstract Module Module { get; }

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> object represents a type whose definition is nested inside the definition of another type.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested inside another type; otherwise, <see langword="false" />.</returns>
		public bool IsNested
		{
			get
			{
				return this.DeclaringType != null;
			}
		}

		/// <summary>Gets the type that declares the current nested type or generic type parameter.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing the enclosing type, if the current type is a nested type; or the generic type definition, if the current type is a type parameter of a generic type; or the type that declares the generic method, if the current type is a type parameter of a generic method; otherwise, <see langword="null" />.</returns>
		public override Type DeclaringType
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets a <see cref="T:System.Reflection.MethodBase" /> that represents the declaring method, if the current <see cref="T:System.Type" /> represents a type parameter of a generic method.</summary>
		/// <returns>If the current <see cref="T:System.Type" /> represents a type parameter of a generic method, a <see cref="T:System.Reflection.MethodBase" /> that represents declaring method; otherwise, <see langword="null" />.</returns>
		public virtual MethodBase DeclaringMethod
		{
			get
			{
				return null;
			}
		}

		/// <summary>Gets the class object that was used to obtain this member.</summary>
		/// <returns>The <see langword="Type" /> object through which this <see cref="T:System.Type" /> object was obtained.</returns>
		public override Type ReflectedType
		{
			get
			{
				return null;
			}
		}

		/// <summary>Indicates the type provided by the common language runtime that represents this type.</summary>
		/// <returns>The underlying system type for the <see cref="T:System.Type" />.</returns>
		public abstract Type UnderlyingSystemType { get; }

		public virtual bool IsTypeDefinition
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value that indicates whether the type is an array.</summary>
		/// <returns>
		///   <see langword="true" /> if the current type is an array; otherwise, <see langword="false" />.</returns>
		public bool IsArray
		{
			get
			{
				return this.IsArrayImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.IsArray" /> property and determines whether the <see cref="T:System.Type" /> is an array.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is an array; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsArrayImpl();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is passed by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is passed by reference; otherwise, <see langword="false" />.</returns>
		public bool IsByRef
		{
			get
			{
				return this.IsByRefImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.IsByRef" /> property and determines whether the <see cref="T:System.Type" /> is passed by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is passed by reference; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsByRefImpl();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a pointer.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a pointer; otherwise, <see langword="false" />.</returns>
		public bool IsPointer
		{
			get
			{
				return this.IsPointerImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.IsPointer" /> property and determines whether the <see cref="T:System.Type" /> is a pointer.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a pointer; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsPointerImpl();

		/// <summary>Gets a value that indicates whether this object represents a constructed generic type. You can create instances of a constructed generic type.</summary>
		/// <returns>
		///   <see langword="true" /> if this object represents a constructed generic type; otherwise, <see langword="false" />.</returns>
		public virtual bool IsConstructedGenericType
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or method.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> object represents a type parameter of a generic type definition or generic method definition; otherwise, <see langword="false" />.</returns>
		public virtual bool IsGenericParameter
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsGenericTypeParameter
		{
			get
			{
				return this.IsGenericParameter && this.DeclaringMethod == null;
			}
		}

		public virtual bool IsGenericMethodParameter
		{
			get
			{
				return this.IsGenericParameter && this.DeclaringMethod != null;
			}
		}

		/// <summary>Gets a value indicating whether the current type is a generic type.</summary>
		/// <returns>
		///   <see langword="true" /> if the current type is a generic type; otherwise, <see langword="false" />.</returns>
		public virtual bool IsGenericType
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents a generic type definition, from which other generic types can be constructed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> object represents a generic type definition; otherwise, <see langword="false" />.</returns>
		public virtual bool IsGenericTypeDefinition
		{
			get
			{
				return false;
			}
		}

		public virtual bool IsSZArray
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		public virtual bool IsVariableBoundArray
		{
			get
			{
				return this.IsArray && !this.IsSZArray;
			}
		}

		public virtual bool IsByRefLike
		{
			get
			{
				throw new NotSupportedException("Derived classes must provide an implementation.");
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> encompasses or refers to another type; that is, whether the current <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference; otherwise, <see langword="false" />.</returns>
		public bool HasElementType
		{
			get
			{
				return this.HasElementTypeImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.HasElementType" /> property and determines whether the current <see cref="T:System.Type" /> encompasses or refers to another type; that is, whether the current <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is an array, a pointer, or is passed by reference; otherwise, <see langword="false" />.</returns>
		protected abstract bool HasElementTypeImpl();

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer or reference type.</summary>
		/// <returns>The <see cref="T:System.Type" /> of the object encompassed or referred to by the current array, pointer, or reference type, or <see langword="null" /> if the current <see cref="T:System.Type" /> is not an array or a pointer, or is not passed by reference, or represents a generic type or a type parameter in the definition of a generic type or generic method.</returns>
		public abstract Type GetElementType();

		/// <summary>Gets the number of dimensions in an array.</summary>
		/// <returns>An integer that contains the number of dimensions in the current type.</returns>
		/// <exception cref="T:System.NotSupportedException">The functionality of this method is unsupported in the base class and must be implemented in a derived class instead.</exception>
		/// <exception cref="T:System.ArgumentException">The current type is not an array.</exception>
		public virtual int GetArrayRank()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents a generic type definition from which the current generic type can be constructed.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing a generic type from which the current type can be constructed.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type is not a generic type.  That is, <see cref="P:System.Type.IsGenericType" /> returns <see langword="false" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		public virtual Type GetGenericTypeDefinition()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Gets an array of the generic type arguments for this type.</summary>
		/// <returns>An array of the generic type arguments for this type.</returns>
		public virtual Type[] GenericTypeArguments
		{
			get
			{
				if (!this.IsGenericType || this.IsGenericTypeDefinition)
				{
					return Array.Empty<Type>();
				}
				return this.GetGenericArguments();
			}
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the type arguments of a closed generic type or the type parameters of a generic type definition.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that represent the type arguments of a generic type. Returns an empty array if the current type is not a generic type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		public virtual Type[] GetGenericArguments()
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Gets the position of the type parameter in the type parameter list of the generic type or method that declared the parameter, when the <see cref="T:System.Type" /> object represents a type parameter of a generic type or a generic method.</summary>
		/// <returns>The position of a type parameter in the type parameter list of the generic type or method that defines the parameter. Position numbers begin at 0.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type does not represent a type parameter. That is, <see cref="P:System.Type.IsGenericParameter" /> returns <see langword="false" />.</exception>
		public virtual int GenericParameterPosition
		{
			get
			{
				throw new InvalidOperationException("Method may only be called on a Type for which Type.IsGenericParameter is true.");
			}
		}

		/// <summary>Gets a combination of <see cref="T:System.Reflection.GenericParameterAttributes" /> flags that describe the covariance and special constraints of the current generic type parameter.</summary>
		/// <returns>A bitwise combination of <see cref="T:System.Reflection.GenericParameterAttributes" /> values that describes the covariance and special constraints of the current generic type parameter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type" /> object is not a generic type parameter. That is, the <see cref="P:System.Type.IsGenericParameter" /> property returns <see langword="false" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		public virtual GenericParameterAttributes GenericParameterAttributes
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>Returns an array of <see cref="T:System.Type" /> objects that represent the constraints on the current generic type parameter.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects that represent the constraints on the current generic type parameter.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type" /> object is not a generic type parameter. That is, the <see cref="P:System.Type.IsGenericParameter" /> property returns <see langword="false" />.</exception>
		public virtual Type[] GetGenericParameterConstraints()
		{
			if (!this.IsGenericParameter)
			{
				throw new InvalidOperationException("Method may only be called on a Type for which Type.IsGenericParameter is true.");
			}
			throw new InvalidOperationException();
		}

		/// <summary>Gets the attributes associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>A <see cref="T:System.Reflection.TypeAttributes" /> object representing the attribute set of the <see cref="T:System.Type" />, unless the <see cref="T:System.Type" /> represents a generic type parameter, in which case the value is unspecified.</returns>
		public TypeAttributes Attributes
		{
			get
			{
				return this.GetAttributeFlagsImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.Attributes" /> property and gets a bitmask indicating the attributes associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>A <see cref="T:System.Reflection.TypeAttributes" /> object representing the attribute set of the <see cref="T:System.Type" />.</returns>
		protected abstract TypeAttributes GetAttributeFlagsImpl();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is abstract and must be overridden.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is abstract; otherwise, <see langword="false" />.</returns>
		public bool IsAbstract
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Abstract) > TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" /> attribute applied, indicating that it was imported from a COM type library.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> has a <see cref="T:System.Runtime.InteropServices.ComImportAttribute" />; otherwise, <see langword="false" />.</returns>
		public bool IsImport
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Import) > TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared sealed.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is declared sealed; otherwise, <see langword="false" />.</returns>
		public bool IsSealed
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.Sealed) > TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the type has a name that requires special handling.</summary>
		/// <returns>
		///   <see langword="true" /> if the type has a name that requires special handling; otherwise, <see langword="false" />.</returns>
		public bool IsSpecialName
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.SpecialName) > TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a class or a delegate; that is, not a value type or interface.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a class; otherwise, <see langword="false" />.</returns>
		public bool IsClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.NotPublic && !this.IsValueType;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own assembly; otherwise, <see langword="false" />.</returns>
		public bool IsNestedAssembly
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedAssembly;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to both its own family and its own assembly; otherwise, <see langword="false" />.</returns>
		public bool IsNestedFamANDAssem
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamANDAssem;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only within its own family.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only within its own family; otherwise, <see langword="false" />.</returns>
		public bool IsNestedFamily
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedFamily;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and visible only to classes that belong to either its own family or to its own assembly.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested and visible only to classes that belong to its own family or to its own assembly; otherwise, <see langword="false" />.</returns>
		public bool IsNestedFamORAssem
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.VisibilityMask;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is nested and declared private.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is nested and declared private; otherwise, <see langword="false" />.</returns>
		public bool IsNestedPrivate
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPrivate;
			}
		}

		/// <summary>Gets a value indicating whether a class is nested and declared public.</summary>
		/// <returns>
		///   <see langword="true" /> if the class is nested and declared public; otherwise, <see langword="false" />.</returns>
		public bool IsNestedPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NestedPublic;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is not declared public.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is not declared public and is not a nested type; otherwise, <see langword="false" />.</returns>
		public bool IsNotPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is declared public.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is declared public and is not a nested type; otherwise, <see langword="false" />.</returns>
		public bool IsPublic
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.VisibilityMask) == TypeAttributes.Public;
			}
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out automatically by the common language runtime.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.AutoLayout" />; otherwise, <see langword="false" />.</returns>
		public bool IsAutoLayout
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out at explicitly specified offsets.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.ExplicitLayout" />; otherwise, <see langword="false" />.</returns>
		public bool IsExplicitLayout
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.ExplicitLayout;
			}
		}

		/// <summary>Gets a value indicating whether the fields of the current type are laid out sequentially, in the order that they were defined or emitted to the metadata.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="P:System.Type.Attributes" /> property of the current type includes <see cref="F:System.Reflection.TypeAttributes.SequentialLayout" />; otherwise, <see langword="false" />.</returns>
		public bool IsLayoutSequential
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.LayoutMask) == TypeAttributes.SequentialLayout;
			}
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the string format attribute <see langword="AnsiClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.</returns>
		public bool IsAnsiClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.NotPublic;
			}
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the string format attribute <see langword="AutoClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.</returns>
		public bool IsAutoClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.AutoClass;
			}
		}

		/// <summary>Gets a value indicating whether the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />.</summary>
		/// <returns>
		///   <see langword="true" /> if the string format attribute <see langword="UnicodeClass" /> is selected for the <see cref="T:System.Type" />; otherwise, <see langword="false" />.</returns>
		public bool IsUnicodeClass
		{
			get
			{
				return (this.GetAttributeFlagsImpl() & TypeAttributes.StringFormatMask) == TypeAttributes.UnicodeClass;
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a COM object.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a COM object; otherwise, <see langword="false" />.</returns>
		public bool IsCOMObject
		{
			get
			{
				return this.IsCOMObjectImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.IsCOMObject" /> property and determines whether the <see cref="T:System.Type" /> is a COM object.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a COM object; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsCOMObjectImpl();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> can be hosted in a context.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> can be hosted in a context; otherwise, <see langword="false" />.</returns>
		public bool IsContextful
		{
			get
			{
				return this.IsContextfulImpl();
			}
		}

		/// <summary>Implements the <see cref="P:System.Type.IsContextful" /> property and determines whether the <see cref="T:System.Type" /> can be hosted in a context.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> can be hosted in a context; otherwise, <see langword="false" />.</returns>
		protected virtual bool IsContextfulImpl()
		{
			return typeof(ContextBoundObject).IsAssignableFrom(this);
		}

		public virtual bool IsCollectible
		{
			get
			{
				return true;
			}
		}

		/// <summary>Gets a value indicating whether the current <see cref="T:System.Type" /> represents an enumeration.</summary>
		/// <returns>
		///   <see langword="true" /> if the current <see cref="T:System.Type" /> represents an enumeration; otherwise, <see langword="false" />.</returns>
		public virtual bool IsEnum
		{
			get
			{
				return this.IsSubclassOf(typeof(Enum));
			}
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is marshaled by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is marshaled by reference; otherwise, <see langword="false" />.</returns>
		public bool IsMarshalByRef
		{
			get
			{
				return this.IsMarshalByRefImpl();
			}
		}

		/// <summary>Implements the <see cref="P:System.Type.IsMarshalByRef" /> property and determines whether the <see cref="T:System.Type" /> is marshaled by reference.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is marshaled by reference; otherwise, <see langword="false" />.</returns>
		protected virtual bool IsMarshalByRefImpl()
		{
			return typeof(MarshalByRefObject).IsAssignableFrom(this);
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is one of the primitive types.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is one of the primitive types; otherwise, <see langword="false" />.</returns>
		public bool IsPrimitive
		{
			get
			{
				return this.IsPrimitiveImpl();
			}
		}

		/// <summary>When overridden in a derived class, implements the <see cref="P:System.Type.IsPrimitive" /> property and determines whether the <see cref="T:System.Type" /> is one of the primitive types.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is one of the primitive types; otherwise, <see langword="false" />.</returns>
		protected abstract bool IsPrimitiveImpl();

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is a value type.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a value type; otherwise, <see langword="false" />.</returns>
		public bool IsValueType
		{
			get
			{
				return this.IsValueTypeImpl();
			}
		}

		/// <summary>Implements the <see cref="P:System.Type.IsValueType" /> property and determines whether the <see cref="T:System.Type" /> is a value type; that is, not a class or an interface.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is a value type; otherwise, <see langword="false" />.</returns>
		protected virtual bool IsValueTypeImpl()
		{
			return this.IsSubclassOf(typeof(ValueType));
		}

		public virtual bool IsSignatureType
		{
			get
			{
				return false;
			}
		}

		/// <summary>Gets a value that indicates whether the current type is security-critical or security-safe-critical at the current trust level, and therefore can perform critical operations.</summary>
		/// <returns>
		///   <see langword="true" /> if the current type is security-critical or security-safe-critical at the current trust level; <see langword="false" /> if it is transparent.</returns>
		public virtual bool IsSecurityCritical
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value that indicates whether the current type is security-safe-critical at the current trust level; that is, whether it can perform critical operations and can be accessed by transparent code.</summary>
		/// <returns>
		///   <see langword="true" /> if the current type is security-safe-critical at the current trust level; <see langword="false" /> if it is security-critical or transparent.</returns>
		public virtual bool IsSecuritySafeCritical
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a value that indicates whether the current type is transparent at the current trust level, and therefore cannot perform critical operations.</summary>
		/// <returns>
		///   <see langword="true" /> if the type is security-transparent at the current trust level; otherwise, <see langword="false" />.</returns>
		public virtual bool IsSecurityTransparent
		{
			get
			{
				throw NotImplemented.ByDesign;
			}
		}

		/// <summary>Gets a <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute" /> that describes the layout of the current type.</summary>
		/// <returns>Gets a <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute" /> that describes the gross layout features of the current type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		public virtual StructLayoutAttribute StructLayoutAttribute
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>Gets the initializer for the type.</summary>
		/// <returns>An object that contains the name of the class constructor for the <see cref="T:System.Type" />.</returns>
		public ConstructorInfo TypeInitializer
		{
			get
			{
				return this.GetConstructorImpl(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, null, CallingConventions.Any, Type.EmptyTypes, null);
			}
		}

		/// <summary>Searches for a public instance constructor whose parameters match the types in the specified array.</summary>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the desired constructor.  
		///  -or-  
		///  An empty array of <see cref="T:System.Type" /> objects, to get a constructor that takes no parameters. Such an empty array is provided by the <see langword="static" /> field <see cref="F:System.Type.EmptyTypes" />.</param>
		/// <returns>An object representing the public instance constructor whose parameters match the types in the parameter type array, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.</exception>
		[ComVisible(true)]
		public ConstructorInfo GetConstructor(Type[] types)
		{
			return this.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, types, null);
		}

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the constructor to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.  
		///  -or-  
		///  <see cref="F:System.Type.EmptyTypes" />.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the parameter type array. The default binder does not process this parameter.</param>
		/// <returns>A <see cref="T:System.Reflection.ConstructorInfo" /> object representing the constructor that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetConstructor(bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		/// <summary>Searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and the stack is cleaned up.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the constructor to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>An object representing the constructor that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		[ComVisible(true)]
		public ConstructorInfo GetConstructor(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetConstructorImpl(bindingAttr, binder, callConvention, types, modifiers);
		}

		/// <summary>When overridden in a derived class, searches for a constructor whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and the stack is cleaned up.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the constructor to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a constructor that takes no parameters.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>A <see cref="T:System.Reflection.ConstructorInfo" /> object representing the constructor that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		/// <exception cref="T:System.NotSupportedException">The current type is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> or <see cref="T:System.Reflection.Emit.GenericTypeParameterBuilder" />.</exception>
		protected abstract ConstructorInfo GetConstructorImpl(BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		/// <summary>Returns all the public constructors defined for the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.ConstructorInfo" /> objects representing all the public instance constructors defined for the current <see cref="T:System.Type" />, but not including the type initializer (static constructor). If no public instance constructors are defined for the current <see cref="T:System.Type" />, or if the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or generic method, an empty array of type <see cref="T:System.Reflection.ConstructorInfo" /> is returned.</returns>
		[ComVisible(true)]
		public ConstructorInfo[] GetConstructors()
		{
			return this.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the constructors defined for the current <see cref="T:System.Type" />, using the specified <see langword="BindingFlags" />.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Reflection.ConstructorInfo" /> objects representing all constructors defined for the current <see cref="T:System.Type" /> that match the specified binding constraints, including the type initializer if it is defined. Returns an empty array of type <see cref="T:System.Reflection.ConstructorInfo" /> if no constructors are defined for the current <see cref="T:System.Type" />, if none of the defined constructors match the binding constraints, or if the current <see cref="T:System.Type" /> represents a type parameter in the definition of a generic type or generic method.</returns>
		[ComVisible(true)]
		public abstract ConstructorInfo[] GetConstructors(BindingFlags bindingAttr);

		/// <summary>Returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified public event.</summary>
		/// <param name="name">The string containing the name of an event that is declared or inherited by the current <see cref="T:System.Type" />.</param>
		/// <returns>The object representing the specified public event that is declared or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public EventInfo GetEvent(string name)
		{
			return this.GetEvent(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, returns the <see cref="T:System.Reflection.EventInfo" /> object representing the specified event, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of an event which is declared or inherited by the current <see cref="T:System.Type" />.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>The object representing the specified event that is declared or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public abstract EventInfo GetEvent(string name, BindingFlags bindingAttr);

		/// <summary>Returns all the public events that are declared or inherited by the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all the public events which are declared or inherited by the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have public events.</returns>
		public virtual EventInfo[] GetEvents()
		{
			return this.GetEvents(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for events that are declared or inherited by the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Reflection.EventInfo" /> objects representing all events that are declared or inherited by the current <see cref="T:System.Type" /> that match the specified binding constraints.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.EventInfo" />, if the current <see cref="T:System.Type" /> does not have events, or if none of the events match the binding constraints.</returns>
		public abstract EventInfo[] GetEvents(BindingFlags bindingAttr);

		/// <summary>Searches for the public field with the specified name.</summary>
		/// <param name="name">The string containing the name of the data field to get.</param>
		/// <returns>An object representing the public field with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">This <see cref="T:System.Type" /> object is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> whose <see cref="M:System.Reflection.Emit.TypeBuilder.CreateType" /> method has not yet been called.</exception>
		public FieldInfo GetField(string name)
		{
			return this.GetField(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Searches for the specified field, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the data field to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An object representing the field that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public abstract FieldInfo GetField(string name, BindingFlags bindingAttr);

		/// <summary>Returns all the public fields of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all the public fields defined for the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no public fields are defined for the current <see cref="T:System.Type" />.</returns>
		public FieldInfo[] GetFields()
		{
			return this.GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the fields defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Reflection.FieldInfo" /> objects representing all fields defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.FieldInfo" />, if no fields are defined for the current <see cref="T:System.Type" />, or if none of the defined fields match the binding constraints.</returns>
		public abstract FieldInfo[] GetFields(BindingFlags bindingAttr);

		/// <summary>Searches for the public members with the specified name.</summary>
		/// <param name="name">The string containing the name of the public members to get.</param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public MemberInfo[] GetMember(string name)
		{
			return this.GetMember(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Searches for the specified members, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the members to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return an empty array.</param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public virtual MemberInfo[] GetMember(string name, BindingFlags bindingAttr)
		{
			return this.GetMember(name, MemberTypes.All, bindingAttr);
		}

		/// <summary>Searches for the specified members of the specified member type, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the members to get.</param>
		/// <param name="type">The value to search for.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return an empty array.</param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing the public members with the specified name, if found; otherwise, an empty array.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.NotSupportedException">A derived class must provide an implementation.</exception>
		public virtual MemberInfo[] GetMember(string name, MemberTypes type, BindingFlags bindingAttr)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Returns all the public members of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all the public members of the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if the current <see cref="T:System.Type" /> does not have public members.</returns>
		public MemberInfo[] GetMembers()
		{
			return this.GetMembers(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the members defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero (<see cref="F:System.Reflection.BindingFlags.Default" />), to return an empty array.</param>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all members defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if no members are defined for the current <see cref="T:System.Type" />, or if none of the defined members match the binding constraints.</returns>
		public abstract MemberInfo[] GetMembers(BindingFlags bindingAttr);

		/// <summary>Searches for the public method with the specified name.</summary>
		/// <param name="name">The string containing the name of the public method to get.</param>
		/// <returns>An object that represents the public method with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public MethodInfo GetMethod(string name)
		{
			return this.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Searches for the specified method, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the method to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetMethodImpl(name, bindingAttr, null, CallingConventions.Any, null, null);
		}

		/// <summary>Searches for the specified public method whose parameters match the specified argument types.</summary>
		/// <param name="name">The string containing the name of the public method to get.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.  
		///  -or-  
		///  An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters.</param>
		/// <returns>An object representing the public method whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and specified parameters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.</exception>
		public MethodInfo GetMethod(string name, Type[] types)
		{
			return this.GetMethod(name, types, null);
		}

		/// <summary>Searches for the specified public method whose parameters match the specified argument types and modifiers.</summary>
		/// <param name="name">The string containing the name of the public method to get.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.  
		///  -or-  
		///  An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the public method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and specified parameters.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		public MethodInfo GetMethod(string name, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, types, modifiers);
		}

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the method to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.  
		///  -or-  
		///  An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		/// <summary>Searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="name">The string containing the name of the method to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and how the stack is cleaned up.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.  
		///  -or-  
		///  An empty array of <see cref="T:System.Type" /> objects (as provided by the <see cref="F:System.Type.EmptyTypes" /> field) to get a method that takes no parameters.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. To be only used when calling through COM interop, and only parameters that are passed by reference are handled. The default binder does not process this parameter.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.</exception>
		public MethodInfo GetMethod(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, bindingAttr, binder, callConvention, types, modifiers);
		}

		/// <summary>When overridden in a derived class, searches for the specified method whose parameters match the specified argument types and modifiers, using the specified binding constraints and the specified calling convention.</summary>
		/// <param name="name">The string containing the name of the method to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="callConvention">The object that specifies the set of rules to use regarding the order and layout of arguments, how the return value is passed, what registers are used for arguments, and what process cleans up the stack.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the method to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a method that takes no parameters.  
		///  -or-  
		///  <see langword="null" />. If <paramref name="types" /> is <see langword="null" />, arguments are not matched.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>An object representing the method that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		/// <exception cref="T:System.NotSupportedException">The current type is a <see cref="T:System.Reflection.Emit.TypeBuilder" /> or <see cref="T:System.Reflection.Emit.GenericTypeParameterBuilder" />.</exception>
		protected abstract MethodInfo GetMethodImpl(string name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers);

		public MethodInfo GetMethod(string name, int genericParameterCount, Type[] types)
		{
			return this.GetMethod(name, genericParameterCount, types, null);
		}

		public MethodInfo GetMethod(string name, int genericParameterCount, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, genericParameterCount, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, types, modifiers);
		}

		public MethodInfo GetMethod(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetMethod(name, genericParameterCount, bindingAttr, binder, CallingConventions.Any, types, modifiers);
		}

		public MethodInfo GetMethod(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (genericParameterCount < 0)
			{
				throw new ArgumentException("Non-negative number required.", "genericParameterCount");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			for (int i = 0; i < types.Length; i++)
			{
				if (types[i] == null)
				{
					throw new ArgumentNullException("types");
				}
			}
			return this.GetMethodImpl(name, genericParameterCount, bindingAttr, binder, callConvention, types, modifiers);
		}

		protected virtual MethodInfo GetMethodImpl(string name, int genericParameterCount, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
		{
			throw new NotSupportedException();
		}

		/// <summary>Returns all the public methods of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all the public methods defined for the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no public methods are defined for the current <see cref="T:System.Type" />.</returns>
		public MethodInfo[] GetMethods()
		{
			return this.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the methods defined for the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Reflection.MethodInfo" /> objects representing all methods defined for the current <see cref="T:System.Type" /> that match the specified binding constraints.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MethodInfo" />, if no methods are defined for the current <see cref="T:System.Type" />, or if none of the defined methods match the binding constraints.</returns>
		public abstract MethodInfo[] GetMethods(BindingFlags bindingAttr);

		/// <summary>Searches for the public nested type with the specified name.</summary>
		/// <param name="name">The string containing the name of the nested type to get.</param>
		/// <returns>An object representing the public nested type with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public Type GetNestedType(string name)
		{
			return this.GetNestedType(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the specified nested type, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the nested type to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An object representing the nested type that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public abstract Type GetNestedType(string name, BindingFlags bindingAttr);

		/// <summary>Returns the public types nested in the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing the public types nested in the current <see cref="T:System.Type" /> (the search is not recursive), or an empty array of type <see cref="T:System.Type" /> if no public types are nested in the current <see cref="T:System.Type" />.</returns>
		public Type[] GetNestedTypes()
		{
			return this.GetNestedTypes(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the types nested in the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing all the types nested in the current <see cref="T:System.Type" /> that match the specified binding constraints (the search is not recursive), or an empty array of type <see cref="T:System.Type" />, if no nested types are found that match the binding constraints.</returns>
		public abstract Type[] GetNestedTypes(BindingFlags bindingAttr);

		/// <summary>Searches for the public property with the specified name.</summary>
		/// <param name="name">The string containing the name of the public property to get.</param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name)
		{
			return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>Searches for the specified property, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the property to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			return this.GetPropertyImpl(name, bindingAttr, null, null, null, null);
		}

		/// <summary>Searches for the public property with the specified name and return type.</summary>
		/// <param name="name">The string containing the name of the public property to get.</param>
		/// <param name="returnType">The return type of the property.</param>
		/// <returns>An object representing the public property with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />, or <paramref name="returnType" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, Type returnType)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (returnType == null)
			{
				throw new ArgumentNullException("returnType");
			}
			return this.GetPropertyImpl(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, null, null);
		}

		/// <summary>Searches for the specified public property whose parameters match the specified argument types.</summary>
		/// <param name="name">The string containing the name of the public property to get.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed.</param>
		/// <returns>An object representing the public property whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.</exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, Type[] types)
		{
			return this.GetProperty(name, null, types);
		}

		/// <summary>Searches for the specified public property whose parameters match the specified argument types.</summary>
		/// <param name="name">The string containing the name of the public property to get.</param>
		/// <param name="returnType">The return type of the property.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed.</param>
		/// <returns>An object representing the public property whose parameters match the specified argument types, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.</exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types)
		{
			return this.GetProperty(name, returnType, types, null);
		}

		/// <summary>Searches for the specified public property whose parameters match the specified argument types and modifiers.</summary>
		/// <param name="name">The string containing the name of the public property to get.</param>
		/// <param name="returnType">The return type of the property.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>An object representing the public property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified argument types and modifiers.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			return this.GetProperty(name, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public, null, returnType, types, modifiers);
		}

		/// <summary>Searches for the specified property whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the property to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="returnType">The return type of the property.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		/// <exception cref="T:System.NullReferenceException">An element of <paramref name="types" /> is <see langword="null" />.</exception>
		public PropertyInfo GetProperty(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name");
			}
			if (types == null)
			{
				throw new ArgumentNullException("types");
			}
			return this.GetPropertyImpl(name, bindingAttr, binder, returnType, types, modifiers);
		}

		/// <summary>When overridden in a derived class, searches for the specified property whose parameters match the specified argument types and modifiers, using the specified binding constraints.</summary>
		/// <param name="name">The string containing the name of the property to get.</param>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded member, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />.</param>
		/// <param name="returnType">The return type of the property.</param>
		/// <param name="types">An array of <see cref="T:System.Type" /> objects representing the number, order, and type of the parameters for the indexed property to get.  
		///  -or-  
		///  An empty array of the type <see cref="T:System.Type" /> (that is, Type[] types = new Type[0]) to get a property that is not indexed.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="types" /> array. The default binder does not process this parameter.</param>
		/// <returns>An object representing the property that matches the specified requirements, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one property is found with the specified name and matching the specified binding constraints.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.  
		/// -or-  
		/// <paramref name="types" /> is <see langword="null" />.  
		/// -or-  
		/// One of the elements in <paramref name="types" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="types" /> is multidimensional.  
		/// -or-  
		/// <paramref name="modifiers" /> is multidimensional.  
		/// -or-  
		/// <paramref name="types" /> and <paramref name="modifiers" /> do not have the same length.</exception>
		/// <exception cref="T:System.NotSupportedException">The current type is a <see cref="T:System.Reflection.Emit.TypeBuilder" />, <see cref="T:System.Reflection.Emit.EnumBuilder" />, or <see cref="T:System.Reflection.Emit.GenericTypeParameterBuilder" />.</exception>
		protected abstract PropertyInfo GetPropertyImpl(string name, BindingFlags bindingAttr, Binder binder, Type returnType, Type[] types, ParameterModifier[] modifiers);

		/// <summary>Returns all the public properties of the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.PropertyInfo" /> objects representing all public properties of the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.PropertyInfo" />, if the current <see cref="T:System.Type" /> does not have public properties.</returns>
		public PropertyInfo[] GetProperties()
		{
			return this.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public);
		}

		/// <summary>When overridden in a derived class, searches for the properties of the current <see cref="T:System.Type" />, using the specified binding constraints.</summary>
		/// <param name="bindingAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted.  
		///  -or-  
		///  Zero, to return <see langword="null" />.</param>
		/// <returns>An array of <see cref="T:System.Reflection.PropertyInfo" /> objects representing all properties of the current <see cref="T:System.Type" /> that match the specified binding constraints.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.PropertyInfo" />, if the current <see cref="T:System.Type" /> does not have properties, or if none of the properties match the binding constraints.</returns>
		public abstract PropertyInfo[] GetProperties(BindingFlags bindingAttr);

		/// <summary>Searches for the members defined for the current <see cref="T:System.Type" /> whose <see cref="T:System.Reflection.DefaultMemberAttribute" /> is set.</summary>
		/// <returns>An array of <see cref="T:System.Reflection.MemberInfo" /> objects representing all default members of the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Reflection.MemberInfo" />, if the current <see cref="T:System.Type" /> does not have default members.</returns>
		public virtual MemberInfo[] GetDefaultMembers()
		{
			throw NotImplemented.ByDesign;
		}

		/// <summary>Gets the handle for the current <see cref="T:System.Type" />.</summary>
		/// <returns>The handle for the current <see cref="T:System.Type" />.</returns>
		/// <exception cref="T:System.NotSupportedException">The .NET Compact Framework does not currently support this property.</exception>
		public virtual RuntimeTypeHandle TypeHandle
		{
			get
			{
				throw new NotSupportedException();
			}
		}

		/// <summary>Gets the handle for the <see cref="T:System.Type" /> of a specified object.</summary>
		/// <param name="o">The object for which to get the type handle.</param>
		/// <returns>The handle for the <see cref="T:System.Type" /> of the specified <see cref="T:System.Object" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="o" /> is <see langword="null" />.</exception>
		public static RuntimeTypeHandle GetTypeHandle(object o)
		{
			if (o == null)
			{
				throw new ArgumentNullException(null, "Invalid handle.");
			}
			return o.GetType().TypeHandle;
		}

		/// <summary>Gets the types of the objects in the specified array.</summary>
		/// <param name="args">An array of objects whose types to determine.</param>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing the types of the corresponding elements in <paramref name="args" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="args" /> is <see langword="null" />.  
		/// -or-  
		/// One or more of the elements in <paramref name="args" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">The class initializers are invoked and at least one throws an exception.</exception>
		public static Type[] GetTypeArray(object[] args)
		{
			if (args == null)
			{
				throw new ArgumentNullException("args");
			}
			Type[] array = new Type[args.Length];
			for (int i = 0; i < array.Length; i++)
			{
				if (args[i] == null)
				{
					throw new ArgumentNullException();
				}
				array[i] = args[i].GetType();
			}
			return array;
		}

		/// <summary>Gets the underlying type code of the specified <see cref="T:System.Type" />.</summary>
		/// <param name="type">The type whose underlying type code to get.</param>
		/// <returns>The code of the underlying type, or <see cref="F:System.TypeCode.Empty" /> if <paramref name="type" /> is <see langword="null" />.</returns>
		public static TypeCode GetTypeCode(Type type)
		{
			if (type == null)
			{
				return TypeCode.Empty;
			}
			return type.GetTypeCodeImpl();
		}

		/// <summary>Returns the underlying type code of this <see cref="T:System.Type" /> instance.</summary>
		/// <returns>The type code of the underlying type.</returns>
		protected virtual TypeCode GetTypeCodeImpl()
		{
			if (this != this.UnderlyingSystemType && this.UnderlyingSystemType != null)
			{
				return Type.GetTypeCode(this.UnderlyingSystemType);
			}
			return TypeCode.Object;
		}

		/// <summary>Gets the GUID associated with the <see cref="T:System.Type" />.</summary>
		/// <returns>The GUID associated with the <see cref="T:System.Type" />.</returns>
		public abstract Guid GUID { get; }

		/// <summary>Gets the type associated with the specified class identifier (CLSID).</summary>
		/// <param name="clsid">The CLSID of the type to get.</param>
		/// <returns>
		///   <see langword="System.__ComObject" /> regardless of whether the CLSID is valid.</returns>
		public static Type GetTypeFromCLSID(Guid clsid)
		{
			return Type.GetTypeFromCLSID(clsid, null, false);
		}

		/// <summary>Gets the type associated with the specified class identifier (CLSID), specifying whether to throw an exception if an error occurs while loading the type.</summary>
		/// <param name="clsid">The CLSID of the type to get.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw any exception that occurs.  
		/// -or-  
		/// <see langword="false" /> to ignore any exception that occurs.</param>
		/// <returns>
		///   <see langword="System.__ComObject" /> regardless of whether the CLSID is valid.</returns>
		public static Type GetTypeFromCLSID(Guid clsid, bool throwOnError)
		{
			return Type.GetTypeFromCLSID(clsid, null, throwOnError);
		}

		/// <summary>Gets the type associated with the specified class identifier (CLSID) from the specified server.</summary>
		/// <param name="clsid">The CLSID of the type to get.</param>
		/// <param name="server">The server from which to load the type. If the server name is <see langword="null" />, this method automatically reverts to the local machine.</param>
		/// <returns>
		///   <see langword="System.__ComObject" /> regardless of whether the CLSID is valid.</returns>
		public static Type GetTypeFromCLSID(Guid clsid, string server)
		{
			return Type.GetTypeFromCLSID(clsid, server, false);
		}

		/// <summary>Gets the type associated with the specified program identifier (ProgID), returning null if an error is encountered while loading the <see cref="T:System.Type" />.</summary>
		/// <param name="progID">The ProgID of the type to get.</param>
		/// <returns>The type associated with the specified ProgID, if <paramref name="progID" /> is a valid entry in the registry and a type is associated with it; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="progID" /> is <see langword="null" />.</exception>
		public static Type GetTypeFromProgID(string progID)
		{
			return Type.GetTypeFromProgID(progID, null, false);
		}

		/// <summary>Gets the type associated with the specified program identifier (ProgID), specifying whether to throw an exception if an error occurs while loading the type.</summary>
		/// <param name="progID">The ProgID of the type to get.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw any exception that occurs.  
		/// -or-  
		/// <see langword="false" /> to ignore any exception that occurs.</param>
		/// <returns>The type associated with the specified program identifier (ProgID), if <paramref name="progID" /> is a valid entry in the registry and a type is associated with it; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="progID" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">The specified ProgID is not registered.</exception>
		public static Type GetTypeFromProgID(string progID, bool throwOnError)
		{
			return Type.GetTypeFromProgID(progID, null, throwOnError);
		}

		/// <summary>Gets the type associated with the specified program identifier (progID) from the specified server, returning null if an error is encountered while loading the type.</summary>
		/// <param name="progID">The progID of the type to get.</param>
		/// <param name="server">The server from which to load the type. If the server name is <see langword="null" />, this method automatically reverts to the local machine.</param>
		/// <returns>The type associated with the specified program identifier (progID), if <paramref name="progID" /> is a valid entry in the registry and a type is associated with it; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="prodID" /> is <see langword="null" />.</exception>
		public static Type GetTypeFromProgID(string progID, string server)
		{
			return Type.GetTypeFromProgID(progID, server, false);
		}

		/// <summary>Gets the type from which the current <see cref="T:System.Type" /> directly inherits.</summary>
		/// <returns>The <see cref="T:System.Type" /> from which the current <see cref="T:System.Type" /> directly inherits, or <see langword="null" /> if the current <see langword="Type" /> represents the <see cref="T:System.Object" /> class or an interface.</returns>
		public abstract Type BaseType { get; }

		/// <summary>Invokes the specified member, using the specified binding constraints and matching the specified argument list.</summary>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.  
		///  -or-  
		///  An empty string ("") to invoke the default member.  
		///  -or-  
		///  For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member.</param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke.</param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="invokeAttr" /> does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute.  
		/// -or-  
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.  
		/// -or-  
		/// This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.  
		/// -or-  
		/// One of the named parameter arrays contains a string that is <see langword="null" />.</exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer.</exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found.</exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.  
		///  -or-  
		///  The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />.</exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria.</exception>
		/// <exception cref="T:System.NotSupportedException">The .NET Compact Framework does not currently support this method.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args)
		{
			return this.InvokeMember(name, invokeAttr, binder, target, args, null, null, null);
		}

		/// <summary>Invokes the specified member, using the specified binding constraints and matching the specified argument list and culture.</summary>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.  
		///  -or-  
		///  An empty string ("") to invoke the default member.  
		///  -or-  
		///  For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member.</param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke.</param>
		/// <param name="culture">The object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric <see cref="T:System.String" /> to a <see cref="T:System.Double" />.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic) to use the current thread's <see cref="T:System.Globalization.CultureInfo" />.</param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="invokeAttr" /> does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute.  
		/// -or-  
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.  
		/// -or-  
		/// This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.  
		/// -or-  
		/// One of the named parameter arrays contains a string that is <see langword="null" />.</exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer.</exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found.</exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.  
		///  -or-  
		///  The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />.</exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		[DebuggerHidden]
		[DebuggerStepThrough]
		public object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, CultureInfo culture)
		{
			return this.InvokeMember(name, invokeAttr, binder, target, args, null, culture, null);
		}

		/// <summary>When overridden in a derived class, invokes the specified member, using the specified binding constraints and matching the specified argument list, modifiers and culture.</summary>
		/// <param name="name">The string containing the name of the constructor, method, property, or field member to invoke.  
		///  -or-  
		///  An empty string ("") to invoke the default member.  
		///  -or-  
		///  For <see langword="IDispatch" /> members, a string representing the DispID, for example "[DispID=3]".</param>
		/// <param name="invokeAttr">A bitmask comprised of one or more <see cref="T:System.Reflection.BindingFlags" /> that specify how the search is conducted. The access can be one of the <see langword="BindingFlags" /> such as <see langword="Public" />, <see langword="NonPublic" />, <see langword="Private" />, <see langword="InvokeMethod" />, <see langword="GetField" />, and so on. The type of lookup need not be specified. If the type of lookup is omitted, <see langword="BindingFlags.Public" /> | <see langword="BindingFlags.Instance" /> | <see langword="BindingFlags.Static" /> are used.</param>
		/// <param name="binder">An object that defines a set of properties and enables binding, which can involve selection of an overloaded method, coercion of argument types, and invocation of a member through reflection.  
		///  -or-  
		///  A null reference (Nothing in Visual Basic), to use the <see cref="P:System.Type.DefaultBinder" />. Note that explicitly defining a <see cref="T:System.Reflection.Binder" /> object may be required for successfully invoking method overloads with variable arguments.</param>
		/// <param name="target">The object on which to invoke the specified member.</param>
		/// <param name="args">An array containing the arguments to pass to the member to invoke.</param>
		/// <param name="modifiers">An array of <see cref="T:System.Reflection.ParameterModifier" /> objects representing the attributes associated with the corresponding element in the <paramref name="args" /> array. A parameter's associated attributes are stored in the member's signature.  
		///  The default binder processes this parameter only when calling a COM component.</param>
		/// <param name="culture">The <see cref="T:System.Globalization.CultureInfo" /> object representing the globalization locale to use, which may be necessary for locale-specific conversions, such as converting a numeric String to a Double.  
		///  -or-  
		///  A null reference (<see langword="Nothing" /> in Visual Basic) to use the current thread's <see cref="T:System.Globalization.CultureInfo" />.</param>
		/// <param name="namedParameters">An array containing the names of the parameters to which the values in the <paramref name="args" /> array are passed.</param>
		/// <returns>An object representing the return value of the invoked member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="invokeAttr" /> does not contain <see langword="CreateInstance" /> and <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="args" /> and <paramref name="modifiers" /> do not have the same length.  
		/// -or-  
		/// <paramref name="invokeAttr" /> is not a valid <see cref="T:System.Reflection.BindingFlags" /> attribute.  
		/// -or-  
		/// <paramref name="invokeAttr" /> does not contain one of the following binding flags: <see langword="InvokeMethod" />, <see langword="CreateInstance" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="CreateInstance" /> combined with <see langword="InvokeMethod" />, <see langword="GetField" />, <see langword="SetField" />, <see langword="GetProperty" />, or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetField" /> and <see langword="SetField" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains both <see langword="GetProperty" /> and <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="InvokeMethod" /> combined with <see langword="SetField" /> or <see langword="SetProperty" />.  
		/// -or-  
		/// <paramref name="invokeAttr" /> contains <see langword="SetField" /> and <paramref name="args" /> has more than one element.  
		/// -or-  
		/// The named parameter array is larger than the argument array.  
		/// -or-  
		/// This method is called on a COM object and one of the following binding flags was not passed in: <see langword="BindingFlags.InvokeMethod" />, <see langword="BindingFlags.GetProperty" />, <see langword="BindingFlags.SetProperty" />, <see langword="BindingFlags.PutDispProperty" />, or <see langword="BindingFlags.PutRefDispProperty" />.  
		/// -or-  
		/// One of the named parameter arrays contains a string that is <see langword="null" />.</exception>
		/// <exception cref="T:System.MethodAccessException">The specified member is a class initializer.</exception>
		/// <exception cref="T:System.MissingFieldException">The field or property cannot be found.</exception>
		/// <exception cref="T:System.MissingMethodException">No method can be found that matches the arguments in <paramref name="args" />.  
		///  -or-  
		///  No member can be found that has the argument names supplied in <paramref name="namedParameters" />.  
		///  -or-  
		///  The current <see cref="T:System.Type" /> object represents a type that contains open type parameters, that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />.</exception>
		/// <exception cref="T:System.Reflection.TargetException">The specified member cannot be invoked on <paramref name="target" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">More than one method matches the binding criteria.</exception>
		/// <exception cref="T:System.InvalidOperationException">The method represented by <paramref name="name" /> has one or more unspecified generic type parameters. That is, the method's <see cref="P:System.Reflection.MethodInfo.ContainsGenericParameters" /> property returns <see langword="true" />.</exception>
		public abstract object InvokeMember(string name, BindingFlags invokeAttr, Binder binder, object target, object[] args, ParameterModifier[] modifiers, CultureInfo culture, string[] namedParameters);

		/// <summary>Searches for the interface with the specified name.</summary>
		/// <param name="name">The string containing the name of the interface to get. For generic interfaces, this is the mangled name.</param>
		/// <returns>An object representing the interface with the specified name, implemented or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">The current <see cref="T:System.Type" /> represents a type that implements the same generic interface with different type arguments.</exception>
		public Type GetInterface(string name)
		{
			return this.GetInterface(name, false);
		}

		/// <summary>When overridden in a derived class, searches for the specified interface, specifying whether to do a case-insensitive search for the interface name.</summary>
		/// <param name="name">The string containing the name of the interface to get. For generic interfaces, this is the mangled name.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> to ignore the case of that part of <paramref name="name" /> that specifies the simple interface name (the part that specifies the namespace must be correctly cased).  
		/// -or-  
		/// <see langword="false" /> to perform a case-sensitive search for all parts of <paramref name="name" />.</param>
		/// <returns>An object representing the interface with the specified name, implemented or inherited by the current <see cref="T:System.Type" />, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="name" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.AmbiguousMatchException">The current <see cref="T:System.Type" /> represents a type that implements the same generic interface with different type arguments.</exception>
		public abstract Type GetInterface(string name, bool ignoreCase);

		/// <summary>When overridden in a derived class, gets all the interfaces implemented or inherited by the current <see cref="T:System.Type" />.</summary>
		/// <returns>An array of <see cref="T:System.Type" /> objects representing all the interfaces implemented or inherited by the current <see cref="T:System.Type" />.  
		///  -or-  
		///  An empty array of type <see cref="T:System.Type" />, if no interfaces are implemented or inherited by the current <see cref="T:System.Type" />.</returns>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A static initializer is invoked and throws an exception.</exception>
		public abstract Type[] GetInterfaces();

		/// <summary>Returns an interface mapping for the specified interface type.</summary>
		/// <param name="interfaceType">The interface type to retrieve a mapping for.</param>
		/// <returns>An object that represents the interface mapping for <paramref name="interfaceType" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="interfaceType" /> is not implemented by the current type.  
		/// -or-  
		/// The <paramref name="interfaceType" /> argument does not refer to an interface.  
		/// -or-  
		/// The current instance or <paramref name="interfaceType" /> argument is an open generic type; that is, the <see cref="P:System.Type.ContainsGenericParameters" /> property returns <see langword="true" />.
		/// -or-
		///  <paramref name="interfaceType" /> is a generic interface, and the current type is an array type.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="interfaceType" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.InvalidOperationException">The current <see cref="T:System.Type" /> represents a generic type parameter; that is, <see cref="P:System.Type.IsGenericParameter" /> is <see langword="true" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		[ComVisible(true)]
		public virtual InterfaceMapping GetInterfaceMap(Type interfaceType)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Determines whether the specified object is an instance of the current <see cref="T:System.Type" />.</summary>
		/// <param name="o">The object to compare with the current type.</param>
		/// <returns>
		///   <see langword="true" /> if the current <see langword="Type" /> is in the inheritance hierarchy of the object represented by <paramref name="o" />, or if the current <see langword="Type" /> is an interface that <paramref name="o" /> implements. <see langword="false" /> if neither of these conditions is the case, if <paramref name="o" /> is <see langword="null" />, or if the current <see langword="Type" /> is an open generic type (that is, <see cref="P:System.Type.ContainsGenericParameters" /> returns <see langword="true" />).</returns>
		public virtual bool IsInstanceOfType(object o)
		{
			return o != null && this.IsAssignableFrom(o.GetType());
		}

		/// <summary>Determines whether two COM types have the same identity and are eligible for type equivalence.</summary>
		/// <param name="other">The COM type that is tested for equivalence with the current type.</param>
		/// <returns>
		///   <see langword="true" /> if the COM types are equivalent; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if one type is in an assembly that is loaded for execution, and the other is in an assembly that is loaded into the reflection-only context.</returns>
		public virtual bool IsEquivalentTo(Type other)
		{
			return this == other;
		}

		/// <summary>Returns the underlying type of the current enumeration type.</summary>
		/// <returns>The underlying type of the current enumeration.</returns>
		/// <exception cref="T:System.ArgumentException">The current type is not an enumeration.  
		///  -or-  
		///  The enumeration type is not valid, because it contains more than one instance field.</exception>
		public virtual Type GetEnumUnderlyingType()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException("Type provided must be an Enum.", "enumType");
			}
			FieldInfo[] fields = this.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
			if (fields == null || fields.Length != 1)
			{
				throw new ArgumentException("The Enum type should contain one and only one instance field.", "enumType");
			}
			return fields[0].FieldType;
		}

		/// <summary>Returns an array of the values of the constants in the current enumeration type.</summary>
		/// <returns>An array that contains the values. The elements of the array are sorted by the binary values (that is, the unsigned values) of the enumeration constants.</returns>
		/// <exception cref="T:System.ArgumentException">The current type is not an enumeration.</exception>
		public virtual Array GetEnumValues()
		{
			if (!this.IsEnum)
			{
				throw new ArgumentException("Type provided must be an Enum.", "enumType");
			}
			throw NotImplemented.ByDesign;
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object representing a one-dimensional array of the current type, with a lower bound of zero.</summary>
		/// <returns>A <see cref="T:System.Type" /> object representing a one-dimensional array of the current type, with a lower bound of zero.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.  
		///  -or-  
		///  The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public virtual Type MakeArrayType()
		{
			throw new NotSupportedException();
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object representing an array of the current type, with the specified number of dimensions.</summary>
		/// <param name="rank">The number of dimensions for the array. This number must be less than or equal to 32.</param>
		/// <returns>An object representing an array of the current type, with the specified number of dimensions.</returns>
		/// <exception cref="T:System.IndexOutOfRangeException">
		///   <paramref name="rank" /> is invalid. For example, 0 or negative.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.  
		///  -or-  
		///  The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.  
		///  -or-  
		///  <paramref name="rank" /> is greater than 32.</exception>
		public virtual Type MakeArrayType(int rank)
		{
			throw new NotSupportedException();
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents the current type when passed as a <see langword="ref" /> parameter (<see langword="ByRef" /> parameter in Visual Basic).</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents the current type when passed as a <see langword="ref" /> parameter (<see langword="ByRef" /> parameter in Visual Basic).</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.  
		///  -or-  
		///  The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public virtual Type MakeByRefType()
		{
			throw new NotSupportedException();
		}

		/// <summary>Substitutes the elements of an array of types for the type parameters of the current generic type definition and returns a <see cref="T:System.Type" /> object representing the resulting constructed type.</summary>
		/// <param name="typeArguments">An array of types to be substituted for the type parameters of the current generic type.</param>
		/// <returns>A <see cref="T:System.Type" /> representing the constructed type formed by substituting the elements of <paramref name="typeArguments" /> for the type parameters of the current generic type.</returns>
		/// <exception cref="T:System.InvalidOperationException">The current type does not represent a generic type definition. That is, <see cref="P:System.Type.IsGenericTypeDefinition" /> returns <see langword="false" />.</exception>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeArguments" /> is <see langword="null" />.  
		/// -or-  
		/// Any element of <paramref name="typeArguments" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.ArgumentException">The number of elements in <paramref name="typeArguments" /> is not the same as the number of type parameters in the current generic type definition.  
		///  -or-  
		///  Any element of <paramref name="typeArguments" /> does not satisfy the constraints specified for the corresponding type parameter of the current generic type.  
		///  -or-  
		///  <paramref name="typeArguments" /> contains an element that is a pointer type (<see cref="P:System.Type.IsPointer" /> returns <see langword="true" />), a by-ref type (<see cref="P:System.Type.IsByRef" /> returns <see langword="true" />), or <see cref="T:System.Void" />.</exception>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class. Derived classes must provide an implementation.</exception>
		public virtual Type MakeGenericType(params Type[] typeArguments)
		{
			throw new NotSupportedException("Derived classes must provide an implementation.");
		}

		/// <summary>Returns a <see cref="T:System.Type" /> object that represents a pointer to the current type.</summary>
		/// <returns>A <see cref="T:System.Type" /> object that represents a pointer to the current type.</returns>
		/// <exception cref="T:System.NotSupportedException">The invoked method is not supported in the base class.</exception>
		/// <exception cref="T:System.TypeLoadException">The current type is <see cref="T:System.TypedReference" />.  
		///  -or-  
		///  The current type is a <see langword="ByRef" /> type. That is, <see cref="P:System.Type.IsByRef" /> returns <see langword="true" />.</exception>
		public virtual Type MakePointerType()
		{
			throw new NotSupportedException();
		}

		public static Type MakeGenericSignatureType(Type genericTypeDefinition, params Type[] typeArguments)
		{
			return new SignatureConstructedGenericType(genericTypeDefinition, typeArguments);
		}

		public static Type MakeGenericMethodParameter(int position)
		{
			if (position < 0)
			{
				throw new ArgumentException("Non-negative number required.", "position");
			}
			return new SignatureGenericMethodParameterType(position);
		}

		/// <summary>Returns a <see langword="String" /> representing the name of the current <see langword="Type" />.</summary>
		/// <returns>A <see cref="T:System.String" /> representing the name of the current <see cref="T:System.Type" />.</returns>
		public override string ToString()
		{
			return "Type: " + this.Name;
		}

		/// <summary>Determines if the underlying system type of the current <see cref="T:System.Type" /> object is the same as the underlying system type of the specified <see cref="T:System.Object" />.</summary>
		/// <param name="o">The object whose underlying system type is to be compared with the underlying system type of the current <see cref="T:System.Type" />. For the comparison to succeed, <paramref name="o" /> must be able to be cast or converted to an object of type   <see cref="T:System.Type" />.</param>
		/// <returns>
		///   <see langword="true" /> if the underlying system type of <paramref name="o" /> is the same as the underlying system type of the current <see cref="T:System.Type" />; otherwise, <see langword="false" />. This method also returns <see langword="false" /> if: .  
		///
		/// <paramref name="o" /> is <see langword="null" />.  
		///
		/// <paramref name="o" /> cannot be cast or converted to a <see cref="T:System.Type" /> object.</returns>
		public override bool Equals(object o)
		{
			return o != null && this.Equals(o as Type);
		}

		/// <summary>Returns the hash code for this instance.</summary>
		/// <returns>The hash code for this instance.</returns>
		public override int GetHashCode()
		{
			Type underlyingSystemType = this.UnderlyingSystemType;
			if (underlyingSystemType != this)
			{
				return underlyingSystemType.GetHashCode();
			}
			return base.GetHashCode();
		}

		/// <summary>Determines if the underlying system type of the current <see cref="T:System.Type" /> is the same as the underlying system type of the specified <see cref="T:System.Type" />.</summary>
		/// <param name="o">The object whose underlying system type is to be compared with the underlying system type of the current <see cref="T:System.Type" />.</param>
		/// <returns>
		///   <see langword="true" /> if the underlying system type of <paramref name="o" /> is the same as the underlying system type of the current <see cref="T:System.Type" />; otherwise, <see langword="false" />.</returns>
		public virtual bool Equals(Type o)
		{
			return !(o == null) && this.UnderlyingSystemType == o.UnderlyingSystemType;
		}

		/// <summary>Gets a reference to the default binder, which implements internal rules for selecting the appropriate members to be called by <see cref="M:System.Type.InvokeMember(System.String,System.Reflection.BindingFlags,System.Reflection.Binder,System.Object,System.Object[],System.Reflection.ParameterModifier[],System.Globalization.CultureInfo,System.String[])" />.</summary>
		/// <returns>A reference to the default binder used by the system.</returns>
		public static Binder DefaultBinder
		{
			get
			{
				if (Type.s_defaultBinder == null)
				{
					DefaultBinder value = new DefaultBinder();
					Interlocked.CompareExchange<Binder>(ref Type.s_defaultBinder, value, null);
				}
				return Type.s_defaultBinder;
			}
		}

		/// <summary>Maps a set of names to a corresponding set of dispatch identifiers.</summary>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="rgszNames">Passed-in array of names to be mapped.</param>
		/// <param name="cNames">Count of the names to be mapped.</param>
		/// <param name="lcid">The locale context in which to interpret the names.</param>
		/// <param name="rgDispId">Caller-allocated array which receives the IDs corresponding to the names.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Type.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
		{
			throw new NotImplementedException();
		}

		/// <summary>Retrieves the type information for an object, which can then be used to get the type information for an interface.</summary>
		/// <param name="iTInfo">The type information to return.</param>
		/// <param name="lcid">The locale identifier for the type information.</param>
		/// <param name="ppTInfo">A pointer to the requested type information object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Type.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Retrieves the number of type information interfaces that an object provides (either 0 or 1).</summary>
		/// <param name="pcTInfo">Points to a location that receives the number of type information interfaces provided by the object.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Type.GetTypeInfoCount(out uint pcTInfo)
		{
			throw new NotImplementedException();
		}

		/// <summary>Provides access to properties and methods exposed by an object.</summary>
		/// <param name="dispIdMember">Identifies the member.</param>
		/// <param name="riid">Reserved for future use. Must be IID_NULL.</param>
		/// <param name="lcid">The locale context in which to interpret arguments.</param>
		/// <param name="wFlags">Flags describing the context of the call.</param>
		/// <param name="pDispParams">Pointer to a structure containing an array of arguments, an array of argument DISPIDs for named arguments, and counts for the number of elements in the arrays.</param>
		/// <param name="pVarResult">Pointer to the location where the result is to be stored.</param>
		/// <param name="pExcepInfo">Pointer to a structure that contains exception information.</param>
		/// <param name="puArgErr">The index of the first argument that has an error.</param>
		/// <exception cref="T:System.NotImplementedException">Late-bound access using the COM IDispatch interface is not supported.</exception>
		void _Type.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
		{
			throw new NotImplementedException();
		}

		internal virtual Type InternalResolve()
		{
			return this.UnderlyingSystemType;
		}

		internal virtual Type RuntimeResolve()
		{
			throw new NotImplementedException();
		}

		internal virtual bool IsUserType
		{
			get
			{
				return true;
			}
		}

		internal virtual MethodInfo GetMethod(MethodInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual ConstructorInfo GetConstructor(ConstructorInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		internal virtual FieldInfo GetField(FieldInfo fromNoninstanciated)
		{
			throw new InvalidOperationException("can only be called in generic type");
		}

		/// <summary>Gets the type referenced by the specified type handle.</summary>
		/// <param name="handle">The object that refers to the type.</param>
		/// <returns>The type referenced by the specified <see cref="T:System.RuntimeTypeHandle" />, or <see langword="null" /> if the <see cref="P:System.RuntimeTypeHandle.Value" /> property of <paramref name="handle" /> is <see langword="null" />.</returns>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
		{
			if (handle.Value == IntPtr.Zero)
			{
				return null;
			}
			return Type.internal_from_handle(handle.Value);
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern Type internal_from_handle(IntPtr handle);

		internal virtual RuntimeTypeHandle GetTypeHandleInternal()
		{
			return this.TypeHandle;
		}

		internal virtual bool IsWindowsRuntimeObjectImpl()
		{
			throw new NotImplementedException();
		}

		internal virtual bool IsExportedToWindowsRuntimeImpl()
		{
			throw new NotImplementedException();
		}

		internal bool IsWindowsRuntimeObject
		{
			get
			{
				return this.IsWindowsRuntimeObjectImpl();
			}
		}

		internal bool IsExportedToWindowsRuntime
		{
			get
			{
				return this.IsExportedToWindowsRuntimeImpl();
			}
		}

		internal virtual bool HasProxyAttributeImpl()
		{
			return false;
		}

		internal virtual bool IsSzArray
		{
			get
			{
				return false;
			}
		}

		internal string FormatTypeName()
		{
			return this.FormatTypeName(false);
		}

		internal virtual string FormatTypeName(bool serialization)
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets a value indicating whether the <see cref="T:System.Type" /> is an interface; that is, not a class or a value type.</summary>
		/// <returns>
		///   <see langword="true" /> if the <see cref="T:System.Type" /> is an interface; otherwise, <see langword="false" />.</returns>
		public bool IsInterface
		{
			[SecuritySafeCritical]
			get
			{
				RuntimeType runtimeType = this as RuntimeType;
				if (runtimeType != null)
				{
					return RuntimeTypeHandle.IsInterface(runtimeType);
				}
				return (this.GetAttributeFlagsImpl() & TypeAttributes.ClassSemanticsMask) == TypeAttributes.ClassSemanticsMask;
			}
		}

		/// <summary>Gets the <see cref="T:System.Type" /> with the specified name, specifying whether to throw an exception if the type is not found and whether to perform a case-sensitive search.</summary>
		/// <param name="typeName">The assembly-qualified name of the type to get. See <see cref="P:System.Type.AssemblyQualifiedName" />. If the type is in the currently executing assembly or in Mscorlib.dll, it is sufficient to supply the type name qualified by its namespace.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />. Specifying <see langword="false" /> also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> to perform a case-insensitive search for <paramref name="typeName" />, <see langword="false" /> to perform a case-sensitive search for <paramref name="typeName" />.</param>
		/// <returns>The type with the specified name. If the type is not found, the <paramref name="throwOnError" /> parameter specifies whether <see langword="null" /> is returned or an exception is thrown. In some cases, an exception is thrown regardless of the value of <paramref name="throwOnError" />. See the Exceptions section.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the type is not found.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid characters, such as an embedded tab.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> is an empty string.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> represents an array type with an invalid size.  
		/// -or-  
		/// <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid syntax. For example, "MyType[,*,]".  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the assembly or one of its dependencies was not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded, and the assembly was compiled with a later version.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName, bool throwOnError, bool ignoreCase)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, throwOnError, ignoreCase, false, ref stackCrawlMark);
		}

		/// <summary>Gets the <see cref="T:System.Type" /> with the specified name, performing a case-sensitive search and specifying whether to throw an exception if the type is not found.</summary>
		/// <param name="typeName">The assembly-qualified name of the type to get. See <see cref="P:System.Type.AssemblyQualifiedName" />. If the type is in the currently executing assembly or in Mscorlib.dll, it is sufficient to supply the type name qualified by its namespace.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />. Specifying <see langword="false" /> also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
		/// <returns>The type with the specified name. If the type is not found, the <paramref name="throwOnError" /> parameter specifies whether <see langword="null" /> is returned or an exception is thrown. In some cases, an exception is thrown regardless of the value of <paramref name="throwOnError" />. See the Exceptions section.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the type is not found.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid characters, such as an embedded tab.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> is an empty string.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> represents an array type with an invalid size.  
		/// -or-  
		/// <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid syntax. For example, "MyType[,*,]".  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the assembly or one of its dependencies was not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.IO.IOException" />, instead.  
		///
		///
		///
		///
		///  The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded, and the assembly was compiled with a later version.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName, bool throwOnError)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, throwOnError, false, false, ref stackCrawlMark);
		}

		/// <summary>Gets the <see cref="T:System.Type" /> with the specified name, performing a case-sensitive search.</summary>
		/// <param name="typeName">The assembly-qualified name of the type to get. See <see cref="P:System.Type.AssemblyQualifiedName" />. If the type is in the currently executing assembly or in Mscorlib.dll, it is sufficient to supply the type name qualified by its namespace.</param>
		/// <returns>The type with the specified name, if found; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.IO.FileLoadException">In the .NET for Windows Store apps or the Portable Class Library, catch the base class exception, <see cref="T:System.IO.IOException" />, instead.  
		///
		///
		///
		///
		///  The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  Version 2.0 or later of the common language runtime is currently loaded, and the assembly was compiled with a later version.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, false, false, false, ref stackCrawlMark);
		}

		/// <summary>Gets the type with the specified name, optionally providing custom methods to resolve the assembly and the type.</summary>
		/// <param name="typeName">The name of the type to get. If the <paramref name="typeResolver" /> parameter is provided, the type name can be any string that <paramref name="typeResolver" /> is capable of resolving. If the <paramref name="assemblyResolver" /> parameter is provided or if standard type resolution is used, <paramref name="typeName" /> must be an assembly-qualified name (see <see cref="P:System.Type.AssemblyQualifiedName" />), unless the type is in the currently executing assembly or in Mscorlib.dll, in which case it is sufficient to supply the type name qualified by its namespace.</param>
		/// <param name="assemblyResolver">A method that locates and returns the assembly that is specified in <paramref name="typeName" />. The assembly name is passed to <paramref name="assemblyResolver" /> as an <see cref="T:System.Reflection.AssemblyName" /> object. If <paramref name="typeName" /> does not contain the name of an assembly, <paramref name="assemblyResolver" /> is not called. If <paramref name="assemblyResolver" /> is not supplied, standard assembly resolution is performed.  
		///  Caution   Do not pass methods from unknown or untrusted callers. Doing so could result in elevation of privilege for malicious code. Use only methods that you provide or that you are familiar with.</param>
		/// <param name="typeResolver">A method that locates and returns the type that is specified by <paramref name="typeName" /> from the assembly that is returned by <paramref name="assemblyResolver" /> or by standard assembly resolution. If no assembly is provided, the <paramref name="typeResolver" /> method can provide one. The method also takes a parameter that specifies whether to perform a case-insensitive search; <see langword="false" /> is passed to that parameter.  
		///  Caution   Do not pass methods from unknown or untrusted callers.</param>
		/// <returns>The type with the specified name, or <see langword="null" /> if the type is not found.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.ArgumentException">An error occurs when <paramref name="typeName" /> is parsed into a type name and an assembly name (for example, when the simple type name includes an unescaped special character).  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.  
		///  -or-  
		///  <paramref name="typeName" /> contains an invalid assembly name.  
		///  -or-  
		///  <paramref name="typeName" /> is a valid assembly name without a type name.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  The assembly was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return TypeNameParser.GetType(typeName, assemblyResolver, typeResolver, false, false, ref stackCrawlMark);
		}

		/// <summary>Gets the type with the specified name, specifying whether to throw an exception if the type is not found, and optionally providing custom methods to resolve the assembly and the type.</summary>
		/// <param name="typeName">The name of the type to get. If the <paramref name="typeResolver" /> parameter is provided, the type name can be any string that <paramref name="typeResolver" /> is capable of resolving. If the <paramref name="assemblyResolver" /> parameter is provided or if standard type resolution is used, <paramref name="typeName" /> must be an assembly-qualified name (see <see cref="P:System.Type.AssemblyQualifiedName" />), unless the type is in the currently executing assembly or in Mscorlib.dll, in which case it is sufficient to supply the type name qualified by its namespace.</param>
		/// <param name="assemblyResolver">A method that locates and returns the assembly that is specified in <paramref name="typeName" />. The assembly name is passed to <paramref name="assemblyResolver" /> as an <see cref="T:System.Reflection.AssemblyName" /> object. If <paramref name="typeName" /> does not contain the name of an assembly, <paramref name="assemblyResolver" /> is not called. If <paramref name="assemblyResolver" /> is not supplied, standard assembly resolution is performed.  
		///  Caution   Do not pass methods from unknown or untrusted callers. Doing so could result in elevation of privilege for malicious code. Use only methods that you provide or that you are familiar with.</param>
		/// <param name="typeResolver">A method that locates and returns the type that is specified by <paramref name="typeName" /> from the assembly that is returned by <paramref name="assemblyResolver" /> or by standard assembly resolution. If no assembly is provided, the method can provide one. The method also takes a parameter that specifies whether to perform a case-insensitive search; <see langword="false" /> is passed to that parameter.  
		///  Caution   Do not pass methods from unknown or untrusted callers.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />. Specifying <see langword="false" /> also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
		/// <returns>The type with the specified name. If the type is not found, the <paramref name="throwOnError" /> parameter specifies whether <see langword="null" /> is returned or an exception is thrown. In some cases, an exception is thrown regardless of the value of <paramref name="throwOnError" />. See the Exceptions section.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the type is not found.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid characters, such as an embedded tab.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> is an empty string.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> represents an array type with an invalid size.  
		/// -or-  
		/// <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.ArgumentException">An error occurs when <paramref name="typeName" /> is parsed into a type name and an assembly name (for example, when the simple type name includes an unescaped special character).  
		///  -or-  
		///  <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid syntax (for example, "MyType[,*,]").  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the assembly or one of its dependencies was not found.  
		/// -or-  
		/// <paramref name="typeName" /> contains an invalid assembly name.  
		/// -or-  
		/// <paramref name="typeName" /> is a valid assembly name without a type name.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  The assembly was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return TypeNameParser.GetType(typeName, assemblyResolver, typeResolver, throwOnError, false, ref stackCrawlMark);
		}

		/// <summary>Gets the type with the specified name, specifying whether to perform a case-sensitive search and whether to throw an exception if the type is not found, and optionally providing custom methods to resolve the assembly and the type.</summary>
		/// <param name="typeName">The name of the type to get. If the <paramref name="typeResolver" /> parameter is provided, the type name can be any string that <paramref name="typeResolver" /> is capable of resolving. If the <paramref name="assemblyResolver" /> parameter is provided or if standard type resolution is used, <paramref name="typeName" /> must be an assembly-qualified name (see <see cref="P:System.Type.AssemblyQualifiedName" />), unless the type is in the currently executing assembly or in Mscorlib.dll, in which case it is sufficient to supply the type name qualified by its namespace.</param>
		/// <param name="assemblyResolver">A method that locates and returns the assembly that is specified in <paramref name="typeName" />. The assembly name is passed to <paramref name="assemblyResolver" /> as an <see cref="T:System.Reflection.AssemblyName" /> object. If <paramref name="typeName" /> does not contain the name of an assembly, <paramref name="assemblyResolver" /> is not called. If <paramref name="assemblyResolver" /> is not supplied, standard assembly resolution is performed.  
		///  Caution   Do not pass methods from unknown or untrusted callers. Doing so could result in elevation of privilege for malicious code. Use only methods that you provide or that you are familiar with.</param>
		/// <param name="typeResolver">A method that locates and returns the type that is specified by <paramref name="typeName" /> from the assembly that is returned by <paramref name="assemblyResolver" /> or by standard assembly resolution. If no assembly is provided, the method can provide one. The method also takes a parameter that specifies whether to perform a case-insensitive search; the value of <paramref name="ignoreCase" /> is passed to that parameter.  
		///  Caution   Do not pass methods from unknown or untrusted callers.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw an exception if the type cannot be found; <see langword="false" /> to return <see langword="null" />. Specifying <see langword="false" /> also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> to perform a case-insensitive search for <paramref name="typeName" />, <see langword="false" /> to perform a case-sensitive search for <paramref name="typeName" />.</param>
		/// <returns>The type with the specified name. If the type is not found, the <paramref name="throwOnError" /> parameter specifies whether <see langword="null" /> is returned or an exception is thrown. In some cases, an exception is thrown regardless of the value of <paramref name="throwOnError" />. See the Exceptions section.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the type is not found.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid characters, such as an embedded tab.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> is an empty string.  
		/// -or-  
		/// <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> represents an array type with an invalid size.  
		/// -or-  
		/// <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" />.</exception>
		/// <exception cref="T:System.ArgumentException">An error occurs when <paramref name="typeName" /> is parsed into a type name and an assembly name (for example, when the simple type name includes an unescaped special character).  
		///  -or-  
		///  <paramref name="throwOnError" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid syntax (for example, "MyType[,*,]").  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		///  -or-  
		///  <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="throwOnError" /> is <see langword="true" /> and the assembly or one of its dependencies was not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.  
		///  -or-  
		///  <paramref name="typeName" /> contains an invalid assembly name.  
		///  -or-  
		///  <paramref name="typeName" /> is a valid assembly name without a type name.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  The assembly was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type GetType(string typeName, Func<AssemblyName, Assembly> assemblyResolver, Func<Assembly, string, bool, Type> typeResolver, bool throwOnError, bool ignoreCase)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return TypeNameParser.GetType(typeName, assemblyResolver, typeResolver, throwOnError, ignoreCase, ref stackCrawlMark);
		}

		/// <summary>Indicates whether two <see cref="T:System.Type" /> objects are equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator ==(Type left, Type right)
		{
			return left == right;
		}

		/// <summary>Indicates whether two <see cref="T:System.Type" /> objects are not equal.</summary>
		/// <param name="left">The first object to compare.</param>
		/// <param name="right">The second object to compare.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="left" /> is not equal to <paramref name="right" />; otherwise, <see langword="false" />.</returns>
		public static bool operator !=(Type left, Type right)
		{
			return left != right;
		}

		/// <summary>Gets the <see cref="T:System.Type" /> with the specified name, specifying whether to perform a case-sensitive search and whether to throw an exception if the type is not found. The type is loaded for reflection only, not for execution.</summary>
		/// <param name="typeName">The assembly-qualified name of the <see cref="T:System.Type" /> to get.</param>
		/// <param name="throwIfNotFound">
		///   <see langword="true" /> to throw a <see cref="T:System.TypeLoadException" /> if the type cannot be found; <see langword="false" /> to return <see langword="null" /> if the type cannot be found. Specifying <see langword="false" /> also suppresses some other exception conditions, but not all of them. See the Exceptions section.</param>
		/// <param name="ignoreCase">
		///   <see langword="true" /> to perform a case-insensitive search for <paramref name="typeName" />; <see langword="false" /> to perform a case-sensitive search for <paramref name="typeName" />.</param>
		/// <returns>The type with the specified name, if found; otherwise, <see langword="null" />. If the type is not found, the <paramref name="throwIfNotFound" /> parameter specifies whether <see langword="null" /> is returned or an exception is thrown. In some cases, an exception is thrown regardless of the value of <paramref name="throwIfNotFound" />. See the Exceptions section.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="typeName" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Reflection.TargetInvocationException">A class initializer is invoked and throws an exception.</exception>
		/// <exception cref="T:System.TypeLoadException">
		///   <paramref name="throwIfNotFound" /> is <see langword="true" /> and the type is not found.  
		/// -or-  
		/// <paramref name="throwIfNotFound" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid characters, such as an embedded tab.  
		/// -or-  
		/// <paramref name="throwIfNotFound" /> is <see langword="true" /> and <paramref name="typeName" /> is an empty string.  
		/// -or-  
		/// <paramref name="throwIfNotFound" /> is <see langword="true" /> and <paramref name="typeName" /> represents an array type with an invalid size.  
		/// -or-  
		/// <paramref name="typeName" /> represents an array of <see cref="T:System.TypedReference" /> objects.</exception>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="typeName" /> does not include the assembly name.  
		/// -or-  
		/// <paramref name="throwIfNotFound" /> is <see langword="true" /> and <paramref name="typeName" /> contains invalid syntax; for example, "MyType[,*,]".  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has a pointer type, a <see langword="ByRef" /> type, or <see cref="T:System.Void" /> as one of its type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type that has an incorrect number of type arguments.  
		/// -or-  
		/// <paramref name="typeName" /> represents a generic type, and one of its type arguments does not satisfy the constraints for the corresponding type parameter.</exception>
		/// <exception cref="T:System.IO.FileNotFoundException">
		///   <paramref name="throwIfNotFound" /> is <see langword="true" /> and the assembly or one of its dependencies was not found.</exception>
		/// <exception cref="T:System.IO.FileLoadException">The assembly or one of its dependencies was found, but could not be loaded.</exception>
		/// <exception cref="T:System.BadImageFormatException">The assembly or one of its dependencies is not valid.  
		///  -or-  
		///  The assembly was compiled with a later version of the common language runtime than the version that is currently loaded.</exception>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static Type ReflectionOnlyGetType(string typeName, bool throwIfNotFound, bool ignoreCase)
		{
			StackCrawlMark stackCrawlMark = StackCrawlMark.LookForMyCaller;
			return RuntimeType.GetType(typeName, throwIfNotFound, ignoreCase, true, ref stackCrawlMark);
		}

		/// <summary>Gets the type associated with the specified class identifier (CLSID) from the specified server, specifying whether to throw an exception if an error occurs while loading the type.</summary>
		/// <param name="clsid">The CLSID of the type to get.</param>
		/// <param name="server">The server from which to load the type. If the server name is <see langword="null" />, this method automatically reverts to the local machine.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw any exception that occurs.  
		/// -or-  
		/// <see langword="false" /> to ignore any exception that occurs.</param>
		/// <returns>
		///   <see langword="System.__ComObject" /> regardless of whether the CLSID is valid.</returns>
		public static Type GetTypeFromCLSID(Guid clsid, string server, bool throwOnError)
		{
			return RuntimeType.GetTypeFromCLSIDImpl(clsid, server, throwOnError);
		}

		/// <summary>Gets the type associated with the specified program identifier (progID) from the specified server, specifying whether to throw an exception if an error occurs while loading the type.</summary>
		/// <param name="progID">The progID of the <see cref="T:System.Type" /> to get.</param>
		/// <param name="server">The server from which to load the type. If the server name is <see langword="null" />, this method automatically reverts to the local machine.</param>
		/// <param name="throwOnError">
		///   <see langword="true" /> to throw any exception that occurs.  
		/// -or-  
		/// <see langword="false" /> to ignore any exception that occurs.</param>
		/// <returns>The type associated with the specified program identifier (progID), if <paramref name="progID" /> is a valid entry in the registry and a type is associated with it; otherwise, <see langword="null" />.</returns>
		/// <exception cref="T:System.ArgumentException">
		///   <paramref name="progID" /> is <see langword="null" />.</exception>
		/// <exception cref="T:System.Runtime.InteropServices.COMException">The specified progID is not registered.</exception>
		public static Type GetTypeFromProgID(string progID, string server, bool throwOnError)
		{
			return RuntimeType.GetTypeFromProgIDImpl(progID, server, throwOnError);
		}

		internal string FullNameOrDefault
		{
			get
			{
				if (this.InternalNameIfAvailable == null)
				{
					return "UnknownType";
				}
				string result;
				try
				{
					result = this.FullName;
				}
				catch (MissingMetadataException)
				{
					result = "UnknownType";
				}
				return result;
			}
		}

		internal bool IsRuntimeImplemented()
		{
			return this.UnderlyingSystemType is RuntimeType;
		}

		internal virtual string InternalGetNameIfAvailable(ref Type rootCauseForFailure)
		{
			return this.Name;
		}

		internal string InternalNameIfAvailable
		{
			get
			{
				Type type = null;
				return this.InternalGetNameIfAvailable(ref type);
			}
		}

		internal string NameOrDefault
		{
			get
			{
				return this.InternalNameIfAvailable ?? "UnknownType";
			}
		}

		private static volatile Binder s_defaultBinder;

		/// <summary>Separates names in the namespace of the <see cref="T:System.Type" />. This field is read-only.</summary>
		public static readonly char Delimiter = '.';

		/// <summary>Represents an empty array of type <see cref="T:System.Type" />. This field is read-only.</summary>
		public static readonly Type[] EmptyTypes = Array.Empty<Type>();

		/// <summary>Represents a missing value in the <see cref="T:System.Type" /> information. This field is read-only.</summary>
		public static readonly object Missing = System.Reflection.Missing.Value;

		/// <summary>Represents the member filter used on attributes. This field is read-only.</summary>
		public static readonly MemberFilter FilterAttribute = new MemberFilter(Type.FilterAttributeImpl);

		/// <summary>Represents the case-sensitive member filter used on names. This field is read-only.</summary>
		public static readonly MemberFilter FilterName = new MemberFilter(Type.FilterNameImpl);

		/// <summary>Represents the case-insensitive member filter used on names. This field is read-only.</summary>
		public static readonly MemberFilter FilterNameIgnoreCase = new MemberFilter(Type.FilterNameIgnoreCaseImpl);

		private const BindingFlags DefaultLookup = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;

		internal RuntimeTypeHandle _impl;

		internal const string DefaultTypeNameWhenMissingMetadata = "UnknownType";
	}
}
