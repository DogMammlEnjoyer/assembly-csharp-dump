using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine.Analytics
{
	[Preserve]
	[NativeHeader("Modules/UnityAnalytics/Public/UnityAnalytics.h")]
	[NativeHeader("UnityAnalyticsScriptingClasses.h")]
	[RequiredByNativeCode]
	public static class AnalyticsSessionInfo
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AnalyticsSessionInfo.SessionStateChanged sessionStateChanged;

		[RequiredByNativeCode]
		[Preserve]
		internal static void CallSessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged)
		{
			AnalyticsSessionInfo.SessionStateChanged sessionStateChanged = AnalyticsSessionInfo.sessionStateChanged;
			bool flag = sessionStateChanged != null;
			if (flag)
			{
				sessionStateChanged(sessionState, sessionId, sessionElapsedTime, sessionChanged);
			}
		}

		public static extern AnalyticsSessionState sessionState { [NativeMethod("GetPlayerSessionState")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern long sessionId { [NativeMethod("GetPlayerSessionId")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern long sessionCount { [NativeMethod("GetPlayerSessionCount")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern long sessionElapsedTime { [NativeMethod("GetPlayerSessionElapsedTime")] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static extern bool sessionFirstRun { [NativeMethod("GetPlayerSessionFirstRun", false, true)] [MethodImpl(MethodImplOptions.InternalCall)] get; }

		public static string userId
		{
			[NativeMethod("GetUserId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpan;
					AnalyticsSessionInfo.get_userId_Injected(out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
		}

		public static string customUserId
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = AnalyticsSessionInfo.customUserIdInternal;
				}
				return result;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					AnalyticsSessionInfo.customUserIdInternal = value;
				}
			}
		}

		public static string customDeviceId
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = AnalyticsSessionInfo.customDeviceIdInternal;
				}
				return result;
			}
			set
			{
				bool flag = Analytics.IsInitialized();
				if (flag)
				{
					AnalyticsSessionInfo.customDeviceIdInternal = value;
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event AnalyticsSessionInfo.IdentityTokenChanged identityTokenChanged;

		[RequiredByNativeCode]
		[Preserve]
		internal static void CallIdentityTokenChanged(string token)
		{
			AnalyticsSessionInfo.IdentityTokenChanged identityTokenChanged = AnalyticsSessionInfo.identityTokenChanged;
			bool flag = identityTokenChanged != null;
			if (flag)
			{
				identityTokenChanged(token);
			}
		}

		public static string identityToken
		{
			get
			{
				bool flag = !Analytics.IsInitialized();
				string result;
				if (flag)
				{
					result = null;
				}
				else
				{
					result = AnalyticsSessionInfo.identityTokenInternal;
				}
				return result;
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private static string identityTokenInternal
		{
			[NativeMethod("GetIdentityToken")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpan;
					AnalyticsSessionInfo.get_identityTokenInternal_Injected(out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static string customUserIdInternal
		{
			[NativeMethod("GetCustomUserId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpan;
					AnalyticsSessionInfo.get_customUserIdInternal_Injected(out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
			[NativeMethod("SetCustomUserId")]
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					AnalyticsSessionInfo.set_customUserIdInternal_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[StaticAccessor("GetUnityAnalytics()", StaticAccessorType.Dot)]
		private unsafe static string customDeviceIdInternal
		{
			[NativeMethod("GetCustomDeviceId")]
			get
			{
				string stringAndDispose;
				try
				{
					ManagedSpanWrapper managedSpan;
					AnalyticsSessionInfo.get_customDeviceIdInternal_Injected(out managedSpan);
				}
				finally
				{
					ManagedSpanWrapper managedSpan;
					stringAndDispose = OutStringMarshaller.GetStringAndDispose(managedSpan);
				}
				return stringAndDispose;
			}
			[NativeMethod("SetCustomDeviceId")]
			set
			{
				try
				{
					ManagedSpanWrapper managedSpanWrapper;
					if (!StringMarshaller.TryMarshalEmptyOrNullString(value, ref managedSpanWrapper))
					{
						ReadOnlySpan<char> readOnlySpan = value.AsSpan();
						fixed (char* ptr = readOnlySpan.GetPinnableReference())
						{
							managedSpanWrapper = new ManagedSpanWrapper((void*)ptr, readOnlySpan.Length);
						}
					}
					AnalyticsSessionInfo.set_customDeviceIdInternal_Injected(ref managedSpanWrapper);
				}
				finally
				{
					char* ptr = null;
				}
			}
		}

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_userId_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_identityTokenInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_customUserIdInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customUserIdInternal_Injected(ref ManagedSpanWrapper value);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void get_customDeviceIdInternal_Injected(out ManagedSpanWrapper ret);

		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void set_customDeviceIdInternal_Injected(ref ManagedSpanWrapper value);

		public delegate void SessionStateChanged(AnalyticsSessionState sessionState, long sessionId, long sessionElapsedTime, bool sessionChanged);

		public delegate void IdentityTokenChanged(string token);
	}
}
