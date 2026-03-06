using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.InteropServices
{
	/// <summary>Lets you control the physical layout of the data fields of a class or structure in memory.</summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
	[ComVisible(true)]
	public sealed class StructLayoutAttribute : Attribute
	{
		[SecurityCritical]
		internal static StructLayoutAttribute GetCustomAttribute(RuntimeType type)
		{
			if (!StructLayoutAttribute.IsDefined(type))
			{
				return null;
			}
			int num = 0;
			int size = 0;
			LayoutKind layoutKind = LayoutKind.Auto;
			TypeAttributes typeAttributes = type.Attributes & TypeAttributes.LayoutMask;
			if (typeAttributes != TypeAttributes.NotPublic)
			{
				if (typeAttributes != TypeAttributes.SequentialLayout)
				{
					if (typeAttributes == TypeAttributes.ExplicitLayout)
					{
						layoutKind = LayoutKind.Explicit;
					}
				}
				else
				{
					layoutKind = LayoutKind.Sequential;
				}
			}
			else
			{
				layoutKind = LayoutKind.Auto;
			}
			CharSet charSet = CharSet.None;
			typeAttributes = (type.Attributes & TypeAttributes.StringFormatMask);
			if (typeAttributes != TypeAttributes.NotPublic)
			{
				if (typeAttributes != TypeAttributes.UnicodeClass)
				{
					if (typeAttributes == TypeAttributes.AutoClass)
					{
						charSet = CharSet.Auto;
					}
				}
				else
				{
					charSet = CharSet.Unicode;
				}
			}
			else
			{
				charSet = CharSet.Ansi;
			}
			type.GetPacking(out num, out size);
			if (num == 0)
			{
				num = 8;
			}
			return new StructLayoutAttribute(layoutKind, num, size, charSet);
		}

		internal static bool IsDefined(RuntimeType type)
		{
			return !type.IsInterface && !type.HasElementType && !type.IsGenericParameter;
		}

		internal StructLayoutAttribute(LayoutKind layoutKind, int pack, int size, CharSet charSet)
		{
			this._val = layoutKind;
			this.Pack = pack;
			this.Size = size;
			this.CharSet = charSet;
		}

		/// <summary>Initalizes a new instance of the <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute" /> class with the specified <see cref="T:System.Runtime.InteropServices.LayoutKind" /> enumeration member.</summary>
		/// <param name="layoutKind">One of the enumeration values that specifes how the class or structure should be arranged.</param>
		public StructLayoutAttribute(LayoutKind layoutKind)
		{
			this._val = layoutKind;
		}

		/// <summary>Initalizes a new instance of the <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute" /> class with the specified <see cref="T:System.Runtime.InteropServices.LayoutKind" /> enumeration member.</summary>
		/// <param name="layoutKind">A 16-bit integer that represents one of the <see cref="T:System.Runtime.InteropServices.LayoutKind" /> values that specifes how the class or structure should be arranged.</param>
		public StructLayoutAttribute(short layoutKind)
		{
			this._val = (LayoutKind)layoutKind;
		}

		/// <summary>Gets the <see cref="T:System.Runtime.InteropServices.LayoutKind" /> value that specifies how the class or structure is arranged.</summary>
		/// <returns>One of the enumeration values that specifies how the class or structure is arranged.</returns>
		public LayoutKind Value
		{
			get
			{
				return this._val;
			}
		}

		private const int DEFAULT_PACKING_SIZE = 8;

		internal LayoutKind _val;

		/// <summary>Controls the alignment of data fields of a class or structure in memory.</summary>
		public int Pack;

		/// <summary>Indicates the absolute size of the class or structure.</summary>
		public int Size;

		/// <summary>Indicates whether string data fields within the class should be marshaled as <see langword="LPWSTR" /> or <see langword="LPSTR" /> by default.</summary>
		public CharSet CharSet;
	}
}
