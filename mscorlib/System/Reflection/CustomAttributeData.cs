using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace System.Reflection
{
	/// <summary>Provides access to custom attribute data for assemblies, modules, types, members and parameters that are loaded into the reflection-only context.</summary>
	[ComVisible(true)]
	[Serializable]
	public class CustomAttributeData
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.Reflection.CustomAttributeData" /> class.</summary>
		protected CustomAttributeData()
		{
		}

		internal CustomAttributeData(ConstructorInfo ctorInfo, Assembly assembly, IntPtr data, uint data_length)
		{
			this.ctorInfo = ctorInfo;
			this.lazyData = new CustomAttributeData.LazyCAttrData();
			this.lazyData.assembly = assembly;
			this.lazyData.data = data;
			this.lazyData.data_length = data_length;
		}

		internal CustomAttributeData(ConstructorInfo ctorInfo) : this(ctorInfo, Array.Empty<CustomAttributeTypedArgument>(), Array.Empty<CustomAttributeNamedArgument>())
		{
		}

		internal CustomAttributeData(ConstructorInfo ctorInfo, IList<CustomAttributeTypedArgument> ctorArgs, IList<CustomAttributeNamedArgument> namedArgs)
		{
			this.ctorInfo = ctorInfo;
			this.ctorArgs = ctorArgs;
			this.namedArgs = namedArgs;
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void ResolveArgumentsInternal(ConstructorInfo ctor, Assembly assembly, IntPtr data, uint data_length, out object[] ctorArgs, out object[] namedArgs);

		private void ResolveArguments()
		{
			if (this.lazyData == null)
			{
				return;
			}
			object[] array;
			object[] array2;
			CustomAttributeData.ResolveArgumentsInternal(this.ctorInfo, this.lazyData.assembly, this.lazyData.data, this.lazyData.data_length, out array, out array2);
			this.ctorArgs = Array.AsReadOnly<CustomAttributeTypedArgument>((array != null) ? CustomAttributeData.UnboxValues<CustomAttributeTypedArgument>(array) : Array.Empty<CustomAttributeTypedArgument>());
			this.namedArgs = Array.AsReadOnly<CustomAttributeNamedArgument>((array2 != null) ? CustomAttributeData.UnboxValues<CustomAttributeNamedArgument>(array2) : Array.Empty<CustomAttributeNamedArgument>());
			this.lazyData = null;
		}

		/// <summary>Gets a <see cref="T:System.Reflection.ConstructorInfo" /> object that represents the constructor that would have initialized the custom attribute.</summary>
		/// <returns>An object that represents the constructor that would have initialized the custom attribute represented by the current instance of the <see cref="T:System.Reflection.CustomAttributeData" /> class.</returns>
		[ComVisible(true)]
		public virtual ConstructorInfo Constructor
		{
			get
			{
				return this.ctorInfo;
			}
		}

		/// <summary>Gets the list of positional arguments specified for the attribute instance represented by the <see cref="T:System.Reflection.CustomAttributeData" /> object.</summary>
		/// <returns>A collection of structures that represent the positional arguments specified for the custom attribute instance.</returns>
		[ComVisible(true)]
		public virtual IList<CustomAttributeTypedArgument> ConstructorArguments
		{
			get
			{
				this.ResolveArguments();
				return this.ctorArgs;
			}
		}

		/// <summary>Gets the list of named arguments specified for the attribute instance represented by the <see cref="T:System.Reflection.CustomAttributeData" /> object.</summary>
		/// <returns>A collection of structures that represent the named arguments specified for the custom attribute instance.</returns>
		public virtual IList<CustomAttributeNamedArgument> NamedArguments
		{
			get
			{
				this.ResolveArguments();
				return this.namedArgs;
			}
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target assembly.</summary>
		/// <param name="target">The assembly whose custom attribute data is to be retrieved.</param>
		/// <returns>A list of objects that represent data about the attributes that have been applied to the target assembly.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is <see langword="null" />.</exception>
		public static IList<CustomAttributeData> GetCustomAttributes(Assembly target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target, false);
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target member.</summary>
		/// <param name="target">The member whose attribute data is to be retrieved.</param>
		/// <returns>A list of objects that represent data about the attributes that have been applied to the target member.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is <see langword="null" />.</exception>
		public static IList<CustomAttributeData> GetCustomAttributes(MemberInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target, false);
		}

		internal static IList<CustomAttributeData> GetCustomAttributesInternal(RuntimeType target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target, false);
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target module.</summary>
		/// <param name="target">The module whose custom attribute data is to be retrieved.</param>
		/// <returns>A list of objects that represent data about the attributes that have been applied to the target module.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is <see langword="null" />.</exception>
		public static IList<CustomAttributeData> GetCustomAttributes(Module target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target, false);
		}

		/// <summary>Returns a list of <see cref="T:System.Reflection.CustomAttributeData" /> objects representing data about the attributes that have been applied to the target parameter.</summary>
		/// <param name="target">The parameter whose attribute data is to be retrieved.</param>
		/// <returns>A list of objects that represent data about the attributes that have been applied to the target parameter.</returns>
		/// <exception cref="T:System.ArgumentNullException">
		///   <paramref name="target" /> is <see langword="null" />.</exception>
		public static IList<CustomAttributeData> GetCustomAttributes(ParameterInfo target)
		{
			return MonoCustomAttrs.GetCustomAttributesData(target, false);
		}

		/// <summary>Gets the type of the attribute.</summary>
		/// <returns>The type of the attribute.</returns>
		public Type AttributeType
		{
			get
			{
				return this.ctorInfo.DeclaringType;
			}
		}

		/// <summary>Returns a string representation of the custom attribute.</summary>
		/// <returns>A string value that represents the custom attribute.</returns>
		public override string ToString()
		{
			this.ResolveArguments();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("[" + this.ctorInfo.DeclaringType.FullName + "(");
			for (int i = 0; i < this.ctorArgs.Count; i++)
			{
				stringBuilder.Append(this.ctorArgs[i].ToString());
				if (i + 1 < this.ctorArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			if (this.namedArgs.Count > 0)
			{
				stringBuilder.Append(", ");
			}
			for (int j = 0; j < this.namedArgs.Count; j++)
			{
				stringBuilder.Append(this.namedArgs[j].ToString());
				if (j + 1 < this.namedArgs.Count)
				{
					stringBuilder.Append(", ");
				}
			}
			stringBuilder.AppendFormat(")]", Array.Empty<object>());
			return stringBuilder.ToString();
		}

		private static T[] UnboxValues<T>(object[] values)
		{
			T[] array = new T[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				array[i] = (T)((object)values[i]);
			}
			return array;
		}

		/// <summary>Returns a value that indicates whether this instance is equal to a specified object.</summary>
		/// <param name="obj">An object to compare with this instance, or <see langword="null" />.</param>
		/// <returns>
		///   <see langword="true" /> if <paramref name="obj" /> is equal to the current instance; otherwise, <see langword="false" />.</returns>
		public override bool Equals(object obj)
		{
			CustomAttributeData customAttributeData = obj as CustomAttributeData;
			if (customAttributeData == null || customAttributeData.ctorInfo != this.ctorInfo || customAttributeData.ctorArgs.Count != this.ctorArgs.Count || customAttributeData.namedArgs.Count != this.namedArgs.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ctorArgs.Count; i++)
			{
				if (this.ctorArgs[i].Equals(customAttributeData.ctorArgs[i]))
				{
					return false;
				}
			}
			for (int j = 0; j < this.namedArgs.Count; j++)
			{
				bool flag = false;
				for (int k = 0; k < customAttributeData.namedArgs.Count; k++)
				{
					if (this.namedArgs[j].Equals(customAttributeData.namedArgs[k]))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>Serves as a hash function for a particular type.</summary>
		/// <returns>A hash code for the current <see cref="T:System.Object" />.</returns>
		public override int GetHashCode()
		{
			int num = (this.ctorInfo == null) ? 13 : (this.ctorInfo.GetHashCode() << 16);
			if (this.ctorArgs != null)
			{
				for (int i = 0; i < this.ctorArgs.Count; i++)
				{
					num += (num ^ 7 + this.ctorArgs[i].GetHashCode() << i * 4);
				}
			}
			if (this.namedArgs != null)
			{
				for (int j = 0; j < this.namedArgs.Count; j++)
				{
					num += this.namedArgs[j].GetHashCode() << 5;
				}
			}
			return num;
		}

		private ConstructorInfo ctorInfo;

		private IList<CustomAttributeTypedArgument> ctorArgs;

		private IList<CustomAttributeNamedArgument> namedArgs;

		private CustomAttributeData.LazyCAttrData lazyData;

		private class LazyCAttrData
		{
			internal Assembly assembly;

			internal IntPtr data;

			internal uint data_length;
		}
	}
}
