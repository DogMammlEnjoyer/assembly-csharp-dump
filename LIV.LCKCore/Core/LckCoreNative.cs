using System;
using System.Runtime.InteropServices;

namespace Liv.Lck.Core
{
	internal static class LckCoreNative
	{
		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern void set_max_log_level(LevelFilter level);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode initialize_android(IntPtr context);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode initialize([MarshalAs(UnmanagedType.LPUTF8Str)] string tracking_id, GameInfo game_info, LckInfo lck_info);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode check_login_attempt_completed(IntPtr complete);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode get_remaining_backoff_time_seconds(IntPtr remaining);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode is_user_subscribed(IntPtr subscribed);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode has_user_configured_streaming(IntPtr configured);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode start_login_attempt(LckCoreNative.start_login_attempt_callback_delegate callback);

		[DllImport("lck_core", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
		internal static extern ReturnCode dispose();

		private const string __DllName = "lck_core";

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		internal delegate void start_login_attempt_callback_delegate(ReturnCode return_code, [MarshalAs(UnmanagedType.LPUTF8Str)] string login_code);
	}
}
