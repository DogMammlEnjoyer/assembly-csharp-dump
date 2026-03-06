using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRSettings
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetSettingsErrorNameFromEnum GetSettingsErrorNameFromEnum;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetBool SetBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetInt32 SetInt32;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetFloat SetFloat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._SetString SetString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetBool GetBool;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetInt32 GetInt32;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetFloat GetFloat;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._GetString GetString;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._RemoveSection RemoveSection;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRSettings._RemoveKeyInSection RemoveKeyInSection;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate IntPtr _GetSettingsErrorNameFromEnum(EVRSettingsError eError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetBool(IntPtr pchSection, IntPtr pchSettingsKey, bool bValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetInt32(IntPtr pchSection, IntPtr pchSettingsKey, int nValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetFloat(IntPtr pchSection, IntPtr pchSettingsKey, float flValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetString(IntPtr pchSection, IntPtr pchSettingsKey, IntPtr pchValue, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetBool(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate int _GetInt32(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate float _GetFloat(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _GetString(IntPtr pchSection, IntPtr pchSettingsKey, StringBuilder pchValue, uint unValueLen, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RemoveSection(IntPtr pchSection, ref EVRSettingsError peError);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RemoveKeyInSection(IntPtr pchSection, IntPtr pchSettingsKey, ref EVRSettingsError peError);
	}
}
