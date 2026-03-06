using System;
using System.Reflection;
using System.Security;

namespace System.Runtime.InteropServices
{
	/// <summary>Indicates the physical position of fields within the unmanaged representation of a class or structure.</summary>
	[ComVisible(true)]
	[AttributeUsage(AttributeTargets.Field, Inherited = false)]
	public sealed class FieldOffsetAttribute : Attribute
	{
		[SecurityCritical]
		internal static Attribute GetCustomAttribute(RuntimeFieldInfo field)
		{
			int fieldOffset;
			if (field.DeclaringType != null && (fieldOffset = field.GetFieldOffset()) >= 0)
			{
				return new FieldOffsetAttribute(fieldOffset);
			}
			return null;
		}

		[SecurityCritical]
		internal static bool IsDefined(RuntimeFieldInfo field)
		{
			return FieldOffsetAttribute.GetCustomAttribute(field) != null;
		}

		/// <summary>Initializes a new instance of the <see cref="T:System.Runtime.InteropServices.FieldOffsetAttribute" /> class with the offset in the structure to the beginning of the field.</summary>
		/// <param name="offset">The offset in bytes from the beginning of the structure to the beginning of the field.</param>
		public FieldOffsetAttribute(int offset)
		{
			this._val = offset;
		}

		/// <summary>Gets the offset from the beginning of the structure to the beginning of the field.</summary>
		/// <returns>The offset from the beginning of the structure to the beginning of the field.</returns>
		public int Value
		{
			get
			{
				return this._val;
			}
		}

		internal int _val;
	}
}
