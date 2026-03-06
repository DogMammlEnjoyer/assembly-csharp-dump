using System;

namespace System.Runtime.InteropServices
{
	/// <summary>Identifies how to marshal parameters or fields to unmanaged code.</summary>
	[ComVisible(true)]
	[Serializable]
	public enum UnmanagedType
	{
		/// <summary>A 4-byte Boolean value (<see langword="true" /> != 0, <see langword="false" /> = 0). This is the Win32 BOOL type.</summary>
		Bool = 2,
		/// <summary>A 1-byte signed integer. You can use this member to transform a Boolean value into a 1-byte, C-style <see langword="bool" /> (<see langword="true" /> = 1, <see langword="false" /> = 0).</summary>
		I1,
		/// <summary>A 1-byte unsigned integer.</summary>
		U1,
		/// <summary>A 2-byte signed integer.</summary>
		I2,
		/// <summary>A 2-byte unsigned integer.</summary>
		U2,
		/// <summary>A 4-byte signed integer.</summary>
		I4,
		/// <summary>A 4-byte unsigned integer.</summary>
		U4,
		/// <summary>An 8-byte signed integer.</summary>
		I8,
		/// <summary>An 8-byte unsigned integer.</summary>
		U8,
		/// <summary>A 4-byte floating-point number.</summary>
		R4,
		/// <summary>An 8-byte floating-point number.</summary>
		R8,
		/// <summary>A currency type. Used on a <see cref="T:System.Decimal" /> to marshal the decimal value as a COM currency type instead of as a <see langword="Decimal" />.</summary>
		Currency = 15,
		/// <summary>A Unicode character string that is a length-prefixed double byte. You can use this member, which is the default string in COM, on the <see cref="T:System.String" /> data type.</summary>
		BStr = 19,
		/// <summary>A single byte, null-terminated ANSI character string. You can use this member on the <see cref="T:System.String" /> and <see cref="T:System.Text.StringBuilder" /> data types.</summary>
		LPStr,
		/// <summary>A 2-byte, null-terminated Unicode character string.</summary>
		LPWStr,
		/// <summary>A platform-dependent character string: ANSI on Windows 98, and Unicode on Windows NT and Windows XP. This value is supported only for platform invoke and not for COM interop, because exporting a string of type <see langword="LPTStr" /> is not supported.</summary>
		LPTStr,
		/// <summary>Used for in-line, fixed-length character arrays that appear within a structure. The character type used with <see cref="F:System.Runtime.InteropServices.UnmanagedType.ByValTStr" /> is determined by the <see cref="T:System.Runtime.InteropServices.CharSet" /> argument of the <see cref="T:System.Runtime.InteropServices.StructLayoutAttribute" /> attribute applied to the containing structure. Always use the <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.SizeConst" /> field to indicate the size of the array.</summary>
		ByValTStr,
		/// <summary>A COM <see langword="IUnknown" /> pointer. You can use this member on the <see cref="T:System.Object" /> data type.</summary>
		IUnknown = 25,
		/// <summary>A COM <see langword="IDispatch" /> pointer (<see langword="Object" /> in Microsoft Visual Basic 6.0).</summary>
		IDispatch,
		/// <summary>A VARIANT, which is used to marshal managed formatted classes and value types.</summary>
		Struct,
		/// <summary>A COM interface pointer. The <see cref="T:System.Guid" /> of the interface is obtained from the class metadata. Use this member to specify the exact interface type or the default interface type if you apply it to a class. This member produces the same behavior as <see cref="F:System.Runtime.InteropServices.UnmanagedType.IUnknown" /> when you apply it to the <see cref="T:System.Object" /> data type.</summary>
		Interface,
		/// <summary>A <see langword="SafeArray" />, which is a self-describing array that carries the type, rank, and bounds of the associated array data. You can use this member with the <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.SafeArraySubType" /> field to override the default element type.</summary>
		SafeArray,
		/// <summary>When the <see cref="P:System.Runtime.InteropServices.MarshalAsAttribute.Value" /> property is set to <see langword="ByValArray" />, the <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.SizeConst" /> field must be set to indicate the number of elements in the array. The <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.ArraySubType" /> field can optionally contain the <see cref="T:System.Runtime.InteropServices.UnmanagedType" /> of the array elements when it is necessary to differentiate among string types. You can use this <see cref="T:System.Runtime.InteropServices.UnmanagedType" /> only on an array that whose elements appear as fields in a structure.</summary>
		ByValArray,
		/// <summary>A platform-dependent, signed integer: 4 bytes on 32-bit Windows, 8 bytes on 64-bit Windows.</summary>
		SysInt,
		/// <summary>A platform-dependent, unsigned integer: 4 bytes on 32-bit Windows, 8 bytes on 64-bit Windows.</summary>
		SysUInt,
		/// <summary>A value that enables Visual Basic to change a string in unmanaged code and have the results reflected in managed code. This value is only supported for platform invoke.</summary>
		VBByRefStr = 34,
		/// <summary>An ANSI character string that is a length-prefixed single byte. You can use this member on the <see cref="T:System.String" /> data type.</summary>
		AnsiBStr,
		/// <summary>A length-prefixed, platform-dependent <see langword="char" /> string: ANSI on Windows 98, Unicode on Windows NT. You rarely use this BSTR-like member.</summary>
		TBStr,
		/// <summary>A 2-byte, OLE-defined VARIANT_BOOL type (<see langword="true" /> = -1, <see langword="false" /> = 0).</summary>
		VariantBool,
		/// <summary>An integer that can be used as a C-style function pointer. You can use this member on a <see cref="T:System.Delegate" /> data type or on a type that inherits from a <see cref="T:System.Delegate" />.</summary>
		FunctionPtr,
		/// <summary>A dynamic type that determines the type of an object at run time and marshals the object as that type. This member is valid for platform invoke methods only.</summary>
		AsAny = 40,
		/// <summary>A pointer to the first element of a C-style array. When marshaling from managed to unmanaged code, the length of the array is determined by the length of the managed array. When marshaling from unmanaged to managed code, the length of the array is determined from the <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.SizeConst" /> and <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.SizeParamIndex" /> fields, optionally followed by the unmanaged type of the elements within the array when it is necessary to differentiate among string types.</summary>
		LPArray = 42,
		/// <summary>A pointer to a C-style structure that you use to marshal managed formatted classes. This member is valid for platform invoke methods only.</summary>
		LPStruct,
		/// <summary>Specifies the custom marshaler class when used with the <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.MarshalType" /> or <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.MarshalTypeRef" /> field. The <see cref="F:System.Runtime.InteropServices.MarshalAsAttribute.MarshalCookie" /> field can be used to pass additional information to the custom marshaler. You can use this member on any reference type. This member is valid for parameters and return values only. It cannot be used on fields.</summary>
		CustomMarshaler,
		/// <summary>A native type that is associated with an <see cref="F:System.Runtime.InteropServices.UnmanagedType.I4" /> or an <see cref="F:System.Runtime.InteropServices.UnmanagedType.U4" /> and that causes the parameter to be exported as an HRESULT in the exported type library.</summary>
		Error,
		/// <summary>A Windows Runtime interface pointer. You can use this member on the <see cref="T:System.Object" /> data type.</summary>
		[ComVisible(false)]
		IInspectable,
		/// <summary>A Windows Runtime string. You can use this member on the <see cref="T:System.String" /> data type.</summary>
		[ComVisible(false)]
		HString,
		/// <summary>A pointer to a UTF-8 encoded string.</summary>
		[ComVisible(false)]
		LPUTF8Str
	}
}
