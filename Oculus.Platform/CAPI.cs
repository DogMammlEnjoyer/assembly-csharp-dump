using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Oculus.Platform
{
	public class CAPI
	{
		public static IntPtr ArrayOfStructsToIntPtr(Array ar)
		{
			int num = 0;
			for (int i = 0; i < ar.Length; i++)
			{
				num += Marshal.SizeOf(ar.GetValue(i));
			}
			IntPtr intPtr = Marshal.AllocHGlobal(num);
			IntPtr intPtr2 = intPtr;
			for (int j = 0; j < ar.Length; j++)
			{
				Marshal.StructureToPtr(ar.GetValue(j), intPtr2, false);
				intPtr2 = (IntPtr)((long)intPtr2 + (long)Marshal.SizeOf(ar.GetValue(j)));
			}
			return intPtr;
		}

		public static CAPI.ovrKeyValuePair[] DictionaryToOVRKeyValuePairs(Dictionary<InitConfigOptions, bool> dict)
		{
			if (dict == null || dict.Count == 0)
			{
				return null;
			}
			CAPI.ovrKeyValuePair[] array = new CAPI.ovrKeyValuePair[dict.Count];
			int num = 0;
			foreach (KeyValuePair<InitConfigOptions, bool> keyValuePair in dict)
			{
				array[num] = new CAPI.ovrKeyValuePair(keyValuePair.Key.ToString(), keyValuePair.Value ? 1 : 0);
				num++;
			}
			return array;
		}

		public static CAPI.ovrKeyValuePair[] DictionaryToOVRKeyValuePairs(Dictionary<string, object> dict)
		{
			if (dict == null || dict.Count == 0)
			{
				return null;
			}
			CAPI.ovrKeyValuePair[] array = new CAPI.ovrKeyValuePair[dict.Count];
			int num = 0;
			foreach (KeyValuePair<string, object> keyValuePair in dict)
			{
				if (keyValuePair.Value.GetType() == typeof(int))
				{
					array[num] = new CAPI.ovrKeyValuePair(keyValuePair.Key, (int)keyValuePair.Value);
				}
				else if (keyValuePair.Value.GetType() == typeof(string))
				{
					array[num] = new CAPI.ovrKeyValuePair(keyValuePair.Key, (string)keyValuePair.Value);
				}
				else
				{
					if (!(keyValuePair.Value.GetType() == typeof(double)))
					{
						throw new Exception("Only int, double or string are allowed types in CustomQuery.data");
					}
					array[num] = new CAPI.ovrKeyValuePair(keyValuePair.Key, (double)keyValuePair.Value);
				}
				num++;
			}
			return array;
		}

		public static byte[] IntPtrToByteArray(IntPtr data, ulong size)
		{
			byte[] array = new byte[size];
			Marshal.Copy(data, array, 0, (int)size);
			return array;
		}

		public static Dictionary<string, string> DataStoreFromNative(IntPtr pointer)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			int num = (int)((uint)CAPI.ovr_DataStore_GetNumKeys(pointer));
			for (int i = 0; i < num; i++)
			{
				string key = CAPI.ovr_DataStore_GetKey(pointer, i);
				dictionary[key] = CAPI.ovr_DataStore_GetValue(pointer, key);
			}
			return dictionary;
		}

		public static string StringFromNative(IntPtr pointer)
		{
			if (pointer == IntPtr.Zero)
			{
				return null;
			}
			int nativeStringLengthNotIncludingNullTerminator = CAPI.GetNativeStringLengthNotIncludingNullTerminator(pointer);
			byte[] array = new byte[nativeStringLengthNotIncludingNullTerminator];
			Marshal.Copy(pointer, array, 0, nativeStringLengthNotIncludingNullTerminator);
			return CAPI.nativeStringEncoding.GetString(array);
		}

		public static int GetNativeStringLengthNotIncludingNullTerminator(IntPtr pointer)
		{
			int num = 0;
			while (Marshal.ReadByte(pointer, num) != 0)
			{
				num++;
			}
			return num;
		}

		public static DateTime DateTimeFromNative(ulong seconds_since_the_one_true_epoch)
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			return dateTime.AddSeconds(seconds_since_the_one_true_epoch).ToLocalTime();
		}

		public static ulong DateTimeToNative(DateTime dt)
		{
			DateTime d = (dt.Kind != DateTimeKind.Utc) ? dt.ToUniversalTime() : dt;
			DateTime d2 = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return (ulong)(d - d2).TotalSeconds;
		}

		public static byte[] BlobFromNative(uint size, IntPtr pointer)
		{
			byte[] array = new byte[size];
			for (int i = 0; i < (int)size; i++)
			{
				array[i] = Marshal.ReadByte(pointer, i);
			}
			return array;
		}

		public static byte[] FiledataFromNative(uint size, IntPtr pointer)
		{
			byte[] array = new byte[size];
			Marshal.Copy(pointer, array, 0, (int)size);
			return array;
		}

		public static IntPtr StringToNative(string s)
		{
			if (s == null)
			{
				throw new Exception("StringFromNative: null argument");
			}
			int byteCount = CAPI.nativeStringEncoding.GetByteCount(s);
			byte[] array = new byte[byteCount + 1];
			CAPI.nativeStringEncoding.GetBytes(s, 0, s.Length, array, 0);
			IntPtr intPtr = Marshal.AllocCoTaskMem(byteCount + 1);
			Marshal.Copy(array, 0, intPtr, byteCount + 1);
			return intPtr;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UnityInitWrapper(string appId);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UnityInitGlobals(IntPtr loggingCB);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_UnityInitWrapperAsynchronous(string appId);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UnityInitWrapperStandalone(string accessToken, IntPtr loggingCB);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Platform_InitializeStandaloneOculus(ref CAPI.OculusInitParams init);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PlatformInitializeWithAccessToken(ulong appId, string accessToken);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PlatformInitializeWithAccessTokenAndOptions(ulong appId, string accessToken, CAPI.ovrKeyValuePair[] configOptions, UIntPtr numOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UnityInitWrapperWindows(string appId, IntPtr loggingCB);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_UnityInitWrapperWindowsAsynchronous(string appId, IntPtr loggingCB);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_SetDeveloperAccessToken(string accessToken);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GetLoggedInUserID();

		public static string ovr_GetLoggedInUserLocale()
		{
			return CAPI.StringFromNative(CAPI.ovr_GetLoggedInUserLocale_Native());
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GetLoggedInUserLocale")]
		private static extern IntPtr ovr_GetLoggedInUserLocale_Native();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_PopMessage();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_FreeMessage(IntPtr message);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Voip_CreateEncoder();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_DestroyEncoder(IntPtr encoder);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Voip_CreateDecoder();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_DestroyDecoder(IntPtr decoder);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, ulong compressedSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Microphone_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Microphone_Destroy(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetSystemVoipPassthrough(bool passthrough);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetSystemVoipMicrophoneMuted(VoipMuteState muted);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UnityResetTestPlatform();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_HTTP_GetWithMessageType(string url, int messageType);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_CrashApplication();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetMicrophoneFilterCallback(CAPI.FilterCallback cb);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(CAPI.FilterCallback cb, UIntPtr bufferSizeElements);

		public static void LogNewUnifiedEvent(LogEventName eventName, Dictionary<string, string> values)
		{
			CAPI.LogNewEvent(eventName.ToString(), values);
		}

		public static void LogNewEvent(string eventName, Dictionary<string, string> values)
		{
			IntPtr intPtr = CAPI.StringToNative(eventName);
			int num = (values == null) ? 0 : values.Count;
			IntPtr[] array = new IntPtr[num * 2];
			if (num > 0)
			{
				int num2 = 0;
				foreach (KeyValuePair<string, string> keyValuePair in values)
				{
					array[num2 * 2] = CAPI.StringToNative(keyValuePair.Key);
					array[num2 * 2 + 1] = CAPI.StringToNative(keyValuePair.Value);
					num2++;
				}
			}
			CAPI.ovr_Log_NewEvent(intPtr, array, (UIntPtr)((ulong)((long)num)));
			Marshal.FreeCoTaskMem(intPtr);
			IntPtr[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				Marshal.FreeCoTaskMem(array2[i]);
			}
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Log_NewEvent(IntPtr eventName, IntPtr[] values, UIntPtr length);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ApplicationLifecycle_GetLaunchDetails();

		public static void ovr_ApplicationLifecycle_LogDeeplinkResult(string trackingID, LaunchResult result)
		{
			IntPtr intPtr = CAPI.StringToNative(trackingID);
			CAPI.ovr_ApplicationLifecycle_LogDeeplinkResult_Native(intPtr, result);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationLifecycle_LogDeeplinkResult")]
		private static extern void ovr_ApplicationLifecycle_LogDeeplinkResult_Native(IntPtr trackingID, LaunchResult result);

		public static ulong ovr_HTTP_StartTransfer(string url, CAPI.ovrKeyValuePair[] headers)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			UIntPtr numItems = (UIntPtr)((ulong)((long)headers.Length));
			ulong result = CAPI.ovr_HTTP_StartTransfer_Native(intPtr, headers, numItems);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_HTTP_StartTransfer")]
		private static extern ulong ovr_HTTP_StartTransfer_Native(IntPtr url, CAPI.ovrKeyValuePair[] headers, UIntPtr numItems);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_HTTP_Write(ulong transferId, byte[] bytes, UIntPtr length);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_HTTP_WriteEOM(ulong transferId);

		public static string ovr_Message_GetStringForJavascript(IntPtr message)
		{
			return CAPI.StringFromNative(CAPI.ovr_Message_GetStringForJavascript_Native(message));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Message_GetStringForJavascript")]
		private static extern IntPtr ovr_Message_GetStringForJavascript_Native(IntPtr message);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetAmbisonicFloatPCM(long connection_id, float[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetAmbisonicInt16PCM(long connection_id, short[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetAmbisonicInterleavedFloatPCM(long connection_id, float[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetAmbisonicInterleavedInt16PCM(long connection_id, short[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_NetSync_GetListenerPosition(long connection_id, ulong sessionId, ref CAPI.ovrNetSyncVec3 position);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetMonostreamFloatPCM(long connection_id, ulong sessionId, float[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetMonostreamInt16PCM(long connection_id, ulong session_id, short[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSync_GetPcmBufferMaxSamples();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_NetSync_GetVoipAmplitude(long connection_id, ulong sessionId, ref float amplitude);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_NetSync_SetListenerPosition(long connection_id, ref CAPI.ovrNetSyncVec3 position);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_Party_PluginGetSharedMemHandle();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipMuteState ovr_Party_PluginGetVoipMicrophoneMuted();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_Party_PluginGetVoipPassthrough();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern SystemVoipStatus ovr_Party_PluginGetVoipStatus();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_Accept(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipDtxState ovr_Voip_GetIsConnectionUsingDtx(ulong peerID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipBitrate ovr_Voip_GetLocalBitrate(ulong peerID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetOutputBufferMaxSize();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetPCM(ulong senderID, short[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetPCMFloat(ulong senderID, float[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetPCMSize(ulong senderID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetPCMWithTimestamp(ulong senderID, short[] outputBuffer, UIntPtr outputBufferNumElements, uint[] timestamp);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Voip_GetPCMWithTimestampFloat(ulong senderID, float[] outputBuffer, UIntPtr outputBufferNumElements, uint[] timestamp);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipBitrate ovr_Voip_GetRemoteBitrate(ulong peerID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_Voip_GetSyncTimestamp(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_Voip_GetSyncTimestampDifference(uint lhs, uint rhs);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipMuteState ovr_Voip_GetSystemVoipMicrophoneMuted();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern SystemVoipStatus ovr_Voip_GetSystemVoipStatus();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetMicrophoneMuted(VoipMuteState state);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetNewConnectionOptions(IntPtr voipOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_SetOutputSampleRate(VoipSampleRate rate);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_Start(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Voip_Stop(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AbuseReport_LaunchAdvancedReportFlow(ulong content_id, IntPtr abuse_report_options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AbuseReport_ReportRequestHandled(ReportRequestResponse response);

		public static ulong ovr_Achievements_AddCount(string name, ulong count)
		{
			IntPtr intPtr = CAPI.StringToNative(name);
			ulong result = CAPI.ovr_Achievements_AddCount_Native(intPtr, count);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Achievements_AddCount")]
		private static extern ulong ovr_Achievements_AddCount_Native(IntPtr name, ulong count);

		public static ulong ovr_Achievements_AddFields(string name, string fields)
		{
			IntPtr intPtr = CAPI.StringToNative(name);
			IntPtr intPtr2 = CAPI.StringToNative(fields);
			ulong result = CAPI.ovr_Achievements_AddFields_Native(intPtr, intPtr2);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Achievements_AddFields")]
		private static extern ulong ovr_Achievements_AddFields_Native(IntPtr name, IntPtr fields);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Achievements_GetAllDefinitions();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Achievements_GetAllProgress();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Achievements_GetDefinitionsByName(string[] names, int count);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Achievements_GetProgressByName(string[] names, int count);

		public static ulong ovr_Achievements_Unlock(string name)
		{
			IntPtr intPtr = CAPI.StringToNative(name);
			ulong result = CAPI.ovr_Achievements_Unlock_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Achievements_Unlock")]
		private static extern ulong ovr_Achievements_Unlock_Native(IntPtr name);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_CancelAppDownload();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_CheckAppDownloadProgress();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_GetInstalledApplications();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_GetVersion();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_InstallAppUpdateAndRelaunch(IntPtr deeplink_options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_LaunchOtherApp(ulong appID, IntPtr deeplink_options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_StartAppDownload();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ApplicationLifecycle_GetRegisteredPIDs();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ApplicationLifecycle_GetSessionKey();

		public static ulong ovr_ApplicationLifecycle_RegisterSessionKey(string sessionKey)
		{
			IntPtr intPtr = CAPI.StringToNative(sessionKey);
			ulong result = CAPI.ovr_ApplicationLifecycle_RegisterSessionKey_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationLifecycle_RegisterSessionKey")]
		private static extern ulong ovr_ApplicationLifecycle_RegisterSessionKey_Native(IntPtr sessionKey);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_Delete(ulong assetFileID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_DeleteById(ulong assetFileID);

		public static ulong ovr_AssetFile_DeleteByName(string assetFileName)
		{
			IntPtr intPtr = CAPI.StringToNative(assetFileName);
			ulong result = CAPI.ovr_AssetFile_DeleteByName_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFile_DeleteByName")]
		private static extern ulong ovr_AssetFile_DeleteByName_Native(IntPtr assetFileName);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_Download(ulong assetFileID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_DownloadById(ulong assetFileID);

		public static ulong ovr_AssetFile_DownloadByName(string assetFileName)
		{
			IntPtr intPtr = CAPI.StringToNative(assetFileName);
			ulong result = CAPI.ovr_AssetFile_DownloadByName_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFile_DownloadByName")]
		private static extern ulong ovr_AssetFile_DownloadByName_Native(IntPtr assetFileName);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_DownloadCancel(ulong assetFileID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_DownloadCancelById(ulong assetFileID);

		public static ulong ovr_AssetFile_DownloadCancelByName(string assetFileName)
		{
			IntPtr intPtr = CAPI.StringToNative(assetFileName);
			ulong result = CAPI.ovr_AssetFile_DownloadCancelByName_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFile_DownloadCancelByName")]
		private static extern ulong ovr_AssetFile_DownloadCancelByName_Native(IntPtr assetFileName);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_GetList();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_Status(ulong assetFileID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFile_StatusById(ulong assetFileID);

		public static ulong ovr_AssetFile_StatusByName(string assetFileName)
		{
			IntPtr intPtr = CAPI.StringToNative(assetFileName);
			ulong result = CAPI.ovr_AssetFile_StatusByName_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFile_StatusByName")]
		private static extern ulong ovr_AssetFile_StatusByName_Native(IntPtr assetFileName);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Avatar_LaunchAvatarEditor(IntPtr options);

		public static ulong ovr_Avatar_UpdateMetaData(string avatarMetaData, string imageFilePath)
		{
			IntPtr intPtr = CAPI.StringToNative(avatarMetaData);
			IntPtr intPtr2 = CAPI.StringToNative(imageFilePath);
			ulong result = CAPI.ovr_Avatar_UpdateMetaData_Native(intPtr, intPtr2);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Avatar_UpdateMetaData")]
		private static extern ulong ovr_Avatar_UpdateMetaData_Native(IntPtr avatarMetaData, IntPtr imageFilePath);

		public static ulong ovr_Challenges_Create(string leaderboardName, IntPtr challengeOptions)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Challenges_Create_Native(intPtr, challengeOptions);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Challenges_Create")]
		private static extern ulong ovr_Challenges_Create_Native(IntPtr leaderboardName, IntPtr challengeOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_DeclineInvite(ulong challengeID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_Delete(ulong challengeID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_Get(ulong challengeID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetEntries(ulong challengeID, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetEntriesAfterRank(ulong challengeID, int limit, ulong afterRank);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetEntriesByIds(ulong challengeID, int limit, LeaderboardStartAt startAt, ulong[] userIDs, uint userIDLength);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetList(IntPtr challengeOptions, int limit);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetNextChallenges(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetNextEntries(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetPreviousChallenges(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_GetPreviousEntries(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_Join(ulong challengeID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_Leave(ulong challengeID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenges_UpdateInfo(ulong challengeID, IntPtr challengeOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Colocation_GetCurrentMapUuid();

		public static ulong ovr_Colocation_RequestMap(string uuid)
		{
			IntPtr intPtr = CAPI.StringToNative(uuid);
			ulong result = CAPI.ovr_Colocation_RequestMap_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Colocation_RequestMap")]
		private static extern ulong ovr_Colocation_RequestMap_Native(IntPtr uuid);

		public static ulong ovr_Colocation_ShareMap(string uuid)
		{
			IntPtr intPtr = CAPI.StringToNative(uuid);
			ulong result = CAPI.ovr_Colocation_ShareMap_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Colocation_ShareMap")]
		private static extern ulong ovr_Colocation_ShareMap_Native(IntPtr uuid);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_GetPresenterData();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_GetViewersData();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_IsInSession();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_JoinSession();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_LaunchInviteDialog();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_LeaveSession();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_RequestToPresent();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Cowatching_ResignFromPresenting();

		public static ulong ovr_Cowatching_SetPresenterData(string video_title, string presenter_data)
		{
			IntPtr intPtr = CAPI.StringToNative(video_title);
			IntPtr intPtr2 = CAPI.StringToNative(presenter_data);
			ulong result = CAPI.ovr_Cowatching_SetPresenterData_Native(intPtr, intPtr2);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Cowatching_SetPresenterData")]
		private static extern ulong ovr_Cowatching_SetPresenterData_Native(IntPtr video_title, IntPtr presenter_data);

		public static ulong ovr_Cowatching_SetViewerData(string viewer_data)
		{
			IntPtr intPtr = CAPI.StringToNative(viewer_data);
			ulong result = CAPI.ovr_Cowatching_SetViewerData_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Cowatching_SetViewerData")]
		private static extern ulong ovr_Cowatching_SetViewerData_Native(IntPtr viewer_data);

		public static ulong ovr_DeviceApplicationIntegrity_GetAttestationToken(string challenge_nonce)
		{
			IntPtr intPtr = CAPI.StringToNative(challenge_nonce);
			ulong result = CAPI.ovr_DeviceApplicationIntegrity_GetAttestationToken_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DeviceApplicationIntegrity_GetAttestationToken")]
		private static extern ulong ovr_DeviceApplicationIntegrity_GetAttestationToken_Native(IntPtr challenge_nonce);

		public static ulong ovr_DeviceApplicationIntegrity_GetIntegrityToken(string challenge_nonce)
		{
			IntPtr intPtr = CAPI.StringToNative(challenge_nonce);
			ulong result = CAPI.ovr_DeviceApplicationIntegrity_GetIntegrityToken_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DeviceApplicationIntegrity_GetIntegrityToken")]
		private static extern ulong ovr_DeviceApplicationIntegrity_GetIntegrityToken_Native(IntPtr challenge_nonce);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Entitlement_GetIsViewerEntitled();

		public static ulong ovr_GraphAPI_Get(string url)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			ulong result = CAPI.ovr_GraphAPI_Get_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GraphAPI_Get")]
		private static extern ulong ovr_GraphAPI_Get_Native(IntPtr url);

		public static ulong ovr_GraphAPI_Post(string url)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			ulong result = CAPI.ovr_GraphAPI_Post_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GraphAPI_Post")]
		private static extern ulong ovr_GraphAPI_Post_Native(IntPtr url);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_Clear();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_GetInvitableUsers(IntPtr options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_GetSentInvites();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_LaunchInvitePanel(IntPtr options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_LaunchMultiplayerErrorDialog(IntPtr options);

		public static ulong ovr_GroupPresence_LaunchRejoinDialog(string lobby_session_id, string match_session_id, string destination_api_name)
		{
			IntPtr intPtr = CAPI.StringToNative(lobby_session_id);
			IntPtr intPtr2 = CAPI.StringToNative(match_session_id);
			IntPtr intPtr3 = CAPI.StringToNative(destination_api_name);
			ulong result = CAPI.ovr_GroupPresence_LaunchRejoinDialog_Native(intPtr, intPtr2, intPtr3);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			Marshal.FreeCoTaskMem(intPtr3);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresence_LaunchRejoinDialog")]
		private static extern ulong ovr_GroupPresence_LaunchRejoinDialog_Native(IntPtr lobby_session_id, IntPtr match_session_id, IntPtr destination_api_name);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_LaunchRosterPanel(IntPtr options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_SendInvites(ulong[] userIDs, uint userIDLength);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_Set(IntPtr groupPresenceOptions);

		public static ulong ovr_GroupPresence_SetDeeplinkMessageOverride(string deeplink_message)
		{
			IntPtr intPtr = CAPI.StringToNative(deeplink_message);
			ulong result = CAPI.ovr_GroupPresence_SetDeeplinkMessageOverride_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresence_SetDeeplinkMessageOverride")]
		private static extern ulong ovr_GroupPresence_SetDeeplinkMessageOverride_Native(IntPtr deeplink_message);

		public static ulong ovr_GroupPresence_SetDestination(string api_name)
		{
			IntPtr intPtr = CAPI.StringToNative(api_name);
			ulong result = CAPI.ovr_GroupPresence_SetDestination_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresence_SetDestination")]
		private static extern ulong ovr_GroupPresence_SetDestination_Native(IntPtr api_name);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_GroupPresence_SetIsJoinable(bool is_joinable);

		public static ulong ovr_GroupPresence_SetLobbySession(string id)
		{
			IntPtr intPtr = CAPI.StringToNative(id);
			ulong result = CAPI.ovr_GroupPresence_SetLobbySession_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresence_SetLobbySession")]
		private static extern ulong ovr_GroupPresence_SetLobbySession_Native(IntPtr id);

		public static ulong ovr_GroupPresence_SetMatchSession(string id)
		{
			IntPtr intPtr = CAPI.StringToNative(id);
			ulong result = CAPI.ovr_GroupPresence_SetMatchSession_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresence_SetMatchSession")]
		private static extern ulong ovr_GroupPresence_SetMatchSession_Native(IntPtr id);

		public static ulong ovr_HTTP_Get(string url)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			ulong result = CAPI.ovr_HTTP_Get_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_HTTP_Get")]
		private static extern ulong ovr_HTTP_Get_Native(IntPtr url);

		public static ulong ovr_HTTP_GetToFile(string url, string diskFile)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			IntPtr intPtr2 = CAPI.StringToNative(diskFile);
			ulong result = CAPI.ovr_HTTP_GetToFile_Native(intPtr, intPtr2);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_HTTP_GetToFile")]
		private static extern ulong ovr_HTTP_GetToFile_Native(IntPtr url, IntPtr diskFile);

		public static ulong ovr_HTTP_MultiPartPost(string url, string filepath_param_name, string filepath, string access_token, CAPI.ovrKeyValuePair[] post_params)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			IntPtr intPtr2 = CAPI.StringToNative(filepath_param_name);
			IntPtr intPtr3 = CAPI.StringToNative(filepath);
			IntPtr intPtr4 = CAPI.StringToNative(access_token);
			UIntPtr numItems = (UIntPtr)((ulong)((long)post_params.Length));
			ulong result = CAPI.ovr_HTTP_MultiPartPost_Native(intPtr, intPtr2, intPtr3, intPtr4, post_params, numItems);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			Marshal.FreeCoTaskMem(intPtr3);
			Marshal.FreeCoTaskMem(intPtr4);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_HTTP_MultiPartPost")]
		private static extern ulong ovr_HTTP_MultiPartPost_Native(IntPtr url, IntPtr filepath_param_name, IntPtr filepath, IntPtr access_token, CAPI.ovrKeyValuePair[] post_params, UIntPtr numItems);

		public static ulong ovr_HTTP_Post(string url)
		{
			IntPtr intPtr = CAPI.StringToNative(url);
			ulong result = CAPI.ovr_HTTP_Post_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_HTTP_Post")]
		private static extern ulong ovr_HTTP_Post_Native(IntPtr url);

		public static ulong ovr_IAP_ConsumePurchase(string sku)
		{
			IntPtr intPtr = CAPI.StringToNative(sku);
			ulong result = CAPI.ovr_IAP_ConsumePurchase_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_IAP_ConsumePurchase")]
		private static extern ulong ovr_IAP_ConsumePurchase_Native(IntPtr sku);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_IAP_GetProductsBySKU(string[] skus, int count);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_IAP_GetViewerPurchases();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_IAP_GetViewerPurchasesDurableCache();

		public static ulong ovr_IAP_LaunchCheckoutFlow(string sku)
		{
			IntPtr intPtr = CAPI.StringToNative(sku);
			ulong result = CAPI.ovr_IAP_LaunchCheckoutFlow_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_IAP_LaunchCheckoutFlow")]
		private static extern ulong ovr_IAP_LaunchCheckoutFlow_Native(IntPtr sku);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_LanguagePack_GetCurrent();

		public static ulong ovr_LanguagePack_SetCurrent(string tag)
		{
			IntPtr intPtr = CAPI.StringToNative(tag);
			ulong result = CAPI.ovr_LanguagePack_SetCurrent_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LanguagePack_SetCurrent")]
		private static extern ulong ovr_LanguagePack_SetCurrent_Native(IntPtr tag);

		public static ulong ovr_Leaderboard_Get(string leaderboardName)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_Get_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_Get")]
		private static extern ulong ovr_Leaderboard_Get_Native(IntPtr leaderboardName);

		public static ulong ovr_Leaderboard_GetEntries(string leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_GetEntries_Native(intPtr, limit, filter, startAt);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_GetEntries")]
		private static extern ulong ovr_Leaderboard_GetEntries_Native(IntPtr leaderboardName, int limit, LeaderboardFilterType filter, LeaderboardStartAt startAt);

		public static ulong ovr_Leaderboard_GetEntriesAfterRank(string leaderboardName, int limit, ulong afterRank)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_GetEntriesAfterRank_Native(intPtr, limit, afterRank);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_GetEntriesAfterRank")]
		private static extern ulong ovr_Leaderboard_GetEntriesAfterRank_Native(IntPtr leaderboardName, int limit, ulong afterRank);

		public static ulong ovr_Leaderboard_GetEntriesByIds(string leaderboardName, int limit, LeaderboardStartAt startAt, ulong[] userIDs, uint userIDLength)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_GetEntriesByIds_Native(intPtr, limit, startAt, userIDs, userIDLength);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_GetEntriesByIds")]
		private static extern ulong ovr_Leaderboard_GetEntriesByIds_Native(IntPtr leaderboardName, int limit, LeaderboardStartAt startAt, ulong[] userIDs, uint userIDLength);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Leaderboard_GetNextEntries(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Leaderboard_GetPreviousEntries(IntPtr handle);

		public static ulong ovr_Leaderboard_WriteEntry(string leaderboardName, long score, byte[] extraData, uint extraDataLength, bool forceUpdate)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_WriteEntry_Native(intPtr, score, extraData, extraDataLength, forceUpdate);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_WriteEntry")]
		private static extern ulong ovr_Leaderboard_WriteEntry_Native(IntPtr leaderboardName, long score, byte[] extraData, uint extraDataLength, bool forceUpdate);

		public static ulong ovr_Leaderboard_WriteEntryWithSupplementaryMetric(string leaderboardName, long score, long supplementaryMetric, byte[] extraData, uint extraDataLength, bool forceUpdate)
		{
			IntPtr intPtr = CAPI.StringToNative(leaderboardName);
			ulong result = CAPI.ovr_Leaderboard_WriteEntryWithSupplementaryMetric_Native(intPtr, score, supplementaryMetric, extraData, extraDataLength, forceUpdate);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_WriteEntryWithSupplementaryMetric")]
		private static extern ulong ovr_Leaderboard_WriteEntryWithSupplementaryMetric_Native(IntPtr leaderboardName, long score, long supplementaryMetric, byte[] extraData, uint extraDataLength, bool forceUpdate);

		public static ulong ovr_Livestreaming_IsAllowedForApplication(string packageName)
		{
			IntPtr intPtr = CAPI.StringToNative(packageName);
			ulong result = CAPI.ovr_Livestreaming_IsAllowedForApplication_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Livestreaming_IsAllowedForApplication")]
		private static extern ulong ovr_Livestreaming_IsAllowedForApplication_Native(IntPtr packageName);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Livestreaming_StartPartyStream();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Livestreaming_StartStream(LivestreamingAudience audience, LivestreamingMicrophoneStatus micStatus);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Livestreaming_StopPartyStream();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Livestreaming_StopStream();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Livestreaming_UpdateMicStatus(LivestreamingMicrophoneStatus micStatus);

		public static ulong ovr_Media_ShareToFacebook(string postTextSuggestion, string filePath, MediaContentType contentType)
		{
			IntPtr intPtr = CAPI.StringToNative(postTextSuggestion);
			IntPtr intPtr2 = CAPI.StringToNative(filePath);
			ulong result = CAPI.ovr_Media_ShareToFacebook_Native(intPtr, intPtr2, contentType);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Media_ShareToFacebook")]
		private static extern ulong ovr_Media_ShareToFacebook_Native(IntPtr postTextSuggestion, IntPtr filePath, MediaContentType contentType);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_Connect(IntPtr connect_options);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_Disconnect(long connection_id);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_GetSessions(long connection_id);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_GetVoipAttenuation(long connection_id);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_GetVoipAttenuationDefault();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipAttenuation(long connection_id, float[] distances, float[] decibels, UIntPtr count);

		public static ulong ovr_NetSync_SetVoipAttenuationModel(long connection_id, string name, float[] distances, float[] decibels, UIntPtr count)
		{
			IntPtr intPtr = CAPI.StringToNative(name);
			ulong result = CAPI.ovr_NetSync_SetVoipAttenuationModel_Native(connection_id, intPtr, distances, decibels, count);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSync_SetVoipAttenuationModel")]
		private static extern ulong ovr_NetSync_SetVoipAttenuationModel_Native(long connection_id, IntPtr name, float[] distances, float[] decibels, UIntPtr count);

		public static ulong ovr_NetSync_SetVoipChannelCfg(long connection_id, string channel_name, string attnmodel, bool disable_spatialization)
		{
			IntPtr intPtr = CAPI.StringToNative(channel_name);
			IntPtr intPtr2 = CAPI.StringToNative(attnmodel);
			ulong result = CAPI.ovr_NetSync_SetVoipChannelCfg_Native(connection_id, intPtr, intPtr2, disable_spatialization);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSync_SetVoipChannelCfg")]
		private static extern ulong ovr_NetSync_SetVoipChannelCfg_Native(long connection_id, IntPtr channel_name, IntPtr attnmodel, bool disable_spatialization);

		public static ulong ovr_NetSync_SetVoipGroup(long connection_id, string group_id)
		{
			IntPtr intPtr = CAPI.StringToNative(group_id);
			ulong result = CAPI.ovr_NetSync_SetVoipGroup_Native(connection_id, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSync_SetVoipGroup")]
		private static extern ulong ovr_NetSync_SetVoipGroup_Native(long connection_id, IntPtr group_id);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipListentoChannels(long connection_id, string[] listento_channels, UIntPtr count);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipMicSource(long connection_id, NetSyncVoipMicSource mic_source);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipSessionMuted(long connection_id, ulong session_id, bool muted);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipSpeaktoChannels(long connection_id, string[] speakto_channels, UIntPtr count);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSync_SetVoipStreamMode(long connection_id, ulong sessionId, NetSyncVoipStreamMode streamMode);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Notification_MarkAsRead(ulong notificationID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_GatherInApplication(ulong partyID, ulong appID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_Get(ulong partyID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_GetCurrent();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_GetCurrentForUser(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_Invite(ulong partyID, ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_Join(ulong partyID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_Leave(ulong partyID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PushNotification_Register();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_RichPresence_Clear();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_RichPresence_GetDestinations();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_RichPresence_Set(IntPtr richPresenceOptions);

		public static ulong ovr_RichPresence_SetDestination(string api_name)
		{
			IntPtr intPtr = CAPI.StringToNative(api_name);
			ulong result = CAPI.ovr_RichPresence_SetDestination_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_RichPresence_SetDestination")]
		private static extern ulong ovr_RichPresence_SetDestination_Native(IntPtr api_name);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_RichPresence_SetIsJoinable(bool is_joinable);

		public static ulong ovr_RichPresence_SetLobbySession(string id)
		{
			IntPtr intPtr = CAPI.StringToNative(id);
			ulong result = CAPI.ovr_RichPresence_SetLobbySession_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_RichPresence_SetLobbySession")]
		private static extern ulong ovr_RichPresence_SetLobbySession_Native(IntPtr id);

		public static ulong ovr_RichPresence_SetMatchSession(string id)
		{
			IntPtr intPtr = CAPI.StringToNative(id);
			ulong result = CAPI.ovr_RichPresence_SetMatchSession_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_RichPresence_SetMatchSession")]
		private static extern ulong ovr_RichPresence_SetMatchSession_Native(IntPtr id);

		public static ulong ovr_User_CancelRecordingForReportFlow(string recordingUUID)
		{
			IntPtr intPtr = CAPI.StringToNative(recordingUUID);
			ulong result = CAPI.ovr_User_CancelRecordingForReportFlow_Native(intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_CancelRecordingForReportFlow")]
		private static extern ulong ovr_User_CancelRecordingForReportFlow_Native(IntPtr recordingUUID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_Get(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetAccessToken();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetBlockedUsers();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetLinkedAccounts(IntPtr userOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetLoggedInUser();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetLoggedInUserFriends();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetLoggedInUserManagedInfo();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetOrgScopedID(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetSdkAccounts();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetUserCapabilities();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetUserProof();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_LaunchBlockFlow(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_LaunchFriendRequestFlow(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_LaunchReportFlow(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_LaunchReportFlow2(ulong optionalUserID, IntPtr abuseReportOptions);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_LaunchUnblockFlow(ulong userID);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_NewEntitledTestUser();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_NewTestUser();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_NewTestUserFriends();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_StartRecordingForReportFlow();

		public static ulong ovr_User_StopRecordingAndLaunchReportFlow(ulong optionalUserID, string optionalRecordingUUID)
		{
			IntPtr intPtr = CAPI.StringToNative(optionalRecordingUUID);
			ulong result = CAPI.ovr_User_StopRecordingAndLaunchReportFlow_Native(optionalUserID, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_StopRecordingAndLaunchReportFlow")]
		private static extern ulong ovr_User_StopRecordingAndLaunchReportFlow_Native(ulong optionalUserID, IntPtr optionalRecordingUUID);

		public static ulong ovr_User_StopRecordingAndLaunchReportFlow2(ulong optionalUserID, string optionalRecordingUUID, IntPtr abuseReportOptions)
		{
			IntPtr intPtr = CAPI.StringToNative(optionalRecordingUUID);
			ulong result = CAPI.ovr_User_StopRecordingAndLaunchReportFlow2_Native(optionalUserID, intPtr, abuseReportOptions);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_StopRecordingAndLaunchReportFlow2")]
		private static extern ulong ovr_User_StopRecordingAndLaunchReportFlow2_Native(ulong optionalUserID, IntPtr optionalRecordingUUID, IntPtr abuseReportOptions);

		public static ulong ovr_User_TestUserCreateDeviceManifest(string deviceID, ulong[] appIDs, int numAppIDs)
		{
			IntPtr intPtr = CAPI.StringToNative(deviceID);
			ulong result = CAPI.ovr_User_TestUserCreateDeviceManifest_Native(intPtr, appIDs, numAppIDs);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_TestUserCreateDeviceManifest")]
		private static extern ulong ovr_User_TestUserCreateDeviceManifest_Native(IntPtr deviceID, ulong[] appIDs, int numAppIDs);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_UserAgeCategory_Get();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_UserAgeCategory_Report(AppAgeCategory age_category);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Voip_GetMicrophoneAvailability();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Voip_ReportAppVoipSessions(ulong[] sessionIDs);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Voip_SetSystemVoipSuppressed(bool suppressed);

		public static string ovr_AbuseReportRecording_GetRecordingUuid(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AbuseReportRecording_GetRecordingUuid_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AbuseReportRecording_GetRecordingUuid")]
		private static extern IntPtr ovr_AbuseReportRecording_GetRecordingUuid_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_AchievementDefinition_GetBitfieldLength(IntPtr obj);

		public static string ovr_AchievementDefinition_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementDefinition_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementDefinition_GetName")]
		private static extern IntPtr ovr_AchievementDefinition_GetName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AchievementDefinition_GetTarget(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern AchievementType ovr_AchievementDefinition_GetType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AchievementDefinitionArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_AchievementDefinitionArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementDefinitionArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementDefinitionArray_GetNextUrl")]
		private static extern IntPtr ovr_AchievementDefinitionArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_AchievementDefinitionArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AchievementDefinitionArray_HasNextPage(IntPtr obj);

		public static string ovr_AchievementProgress_GetBitfield(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementProgress_GetBitfield_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementProgress_GetBitfield")]
		private static extern IntPtr ovr_AchievementProgress_GetBitfield_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AchievementProgress_GetCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AchievementProgress_GetIsUnlocked(IntPtr obj);

		public static string ovr_AchievementProgress_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementProgress_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementProgress_GetName")]
		private static extern IntPtr ovr_AchievementProgress_GetName_Native(IntPtr obj);

		public static DateTime ovr_AchievementProgress_GetUnlockTime(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_AchievementProgress_GetUnlockTime_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementProgress_GetUnlockTime")]
		private static extern ulong ovr_AchievementProgress_GetUnlockTime_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AchievementProgressArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_AchievementProgressArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementProgressArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementProgressArray_GetNextUrl")]
		private static extern IntPtr ovr_AchievementProgressArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_AchievementProgressArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AchievementProgressArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AchievementUpdate_GetJustUnlocked(IntPtr obj);

		public static string ovr_AchievementUpdate_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AchievementUpdate_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AchievementUpdate_GetName")]
		private static extern IntPtr ovr_AchievementUpdate_GetName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_AppDownloadProgressResult_GetDownloadBytes(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_AppDownloadProgressResult_GetDownloadedBytes(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern AppStatus ovr_AppDownloadProgressResult_GetStatusCode(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern AppInstallResult ovr_AppDownloadResult_GetAppInstallResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_AppDownloadResult_GetTimestamp(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Application_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ApplicationInvite_GetDestination(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ApplicationInvite_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ApplicationInvite_GetIsActive(IntPtr obj);

		public static string ovr_ApplicationInvite_GetLobbySessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationInvite_GetLobbySessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationInvite_GetLobbySessionId")]
		private static extern IntPtr ovr_ApplicationInvite_GetLobbySessionId_Native(IntPtr obj);

		public static string ovr_ApplicationInvite_GetMatchSessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationInvite_GetMatchSessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationInvite_GetMatchSessionId")]
		private static extern IntPtr ovr_ApplicationInvite_GetMatchSessionId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ApplicationInvite_GetRecipient(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ApplicationInviteArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_ApplicationInviteArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationInviteArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationInviteArray_GetNextUrl")]
		private static extern IntPtr ovr_ApplicationInviteArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_ApplicationInviteArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ApplicationInviteArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_ApplicationVersion_GetCurrentCode(IntPtr obj);

		public static string ovr_ApplicationVersion_GetCurrentName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationVersion_GetCurrentName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationVersion_GetCurrentName")]
		private static extern IntPtr ovr_ApplicationVersion_GetCurrentName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_ApplicationVersion_GetLatestCode(IntPtr obj);

		public static string ovr_ApplicationVersion_GetLatestName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationVersion_GetLatestName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationVersion_GetLatestName")]
		private static extern IntPtr ovr_ApplicationVersion_GetLatestName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_ApplicationVersion_GetReleaseDate(IntPtr obj);

		public static string ovr_ApplicationVersion_GetSize(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ApplicationVersion_GetSize_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationVersion_GetSize")]
		private static extern IntPtr ovr_ApplicationVersion_GetSize_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetDetails_GetAssetId(IntPtr obj);

		public static string ovr_AssetDetails_GetAssetType(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetDetails_GetAssetType_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetDetails_GetAssetType")]
		private static extern IntPtr ovr_AssetDetails_GetAssetType_Native(IntPtr obj);

		public static string ovr_AssetDetails_GetDownloadStatus(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetDetails_GetDownloadStatus_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetDetails_GetDownloadStatus")]
		private static extern IntPtr ovr_AssetDetails_GetDownloadStatus_Native(IntPtr obj);

		public static string ovr_AssetDetails_GetFilepath(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetDetails_GetFilepath_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetDetails_GetFilepath")]
		private static extern IntPtr ovr_AssetDetails_GetFilepath_Native(IntPtr obj);

		public static string ovr_AssetDetails_GetIapStatus(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetDetails_GetIapStatus_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetDetails_GetIapStatus")]
		private static extern IntPtr ovr_AssetDetails_GetIapStatus_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AssetDetails_GetLanguage(IntPtr obj);

		public static string ovr_AssetDetails_GetMetadata(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetDetails_GetMetadata_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetDetails_GetMetadata")]
		private static extern IntPtr ovr_AssetDetails_GetMetadata_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AssetDetailsArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_AssetDetailsArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDeleteResult_GetAssetFileId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDeleteResult_GetAssetId(IntPtr obj);

		public static string ovr_AssetFileDeleteResult_GetFilepath(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetFileDeleteResult_GetFilepath_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFileDeleteResult_GetFilepath")]
		private static extern IntPtr ovr_AssetFileDeleteResult_GetFilepath_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AssetFileDeleteResult_GetSuccess(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadCancelResult_GetAssetFileId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadCancelResult_GetAssetId(IntPtr obj);

		public static string ovr_AssetFileDownloadCancelResult_GetFilepath(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetFileDownloadCancelResult_GetFilepath_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFileDownloadCancelResult_GetFilepath")]
		private static extern IntPtr ovr_AssetFileDownloadCancelResult_GetFilepath_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AssetFileDownloadCancelResult_GetSuccess(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadResult_GetAssetId(IntPtr obj);

		public static string ovr_AssetFileDownloadResult_GetFilepath(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_AssetFileDownloadResult_GetFilepath_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AssetFileDownloadResult_GetFilepath")]
		private static extern IntPtr ovr_AssetFileDownloadResult_GetFilepath_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadUpdate_GetAssetFileId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadUpdate_GetAssetId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_AssetFileDownloadUpdate_GetBytesTotal(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_AssetFileDownloadUpdate_GetBytesTotalLong(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_AssetFileDownloadUpdate_GetBytesTransferred(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_AssetFileDownloadUpdate_GetBytesTransferredLong(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AssetFileDownloadUpdate_GetCompleted(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_AvatarEditorResult_GetRequestSent(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_BillingPlan_GetPaidOffer(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_BillingPlan_GetTrialOffers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_BillingPlanArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_BillingPlanArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_BlockedUser_GetId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_BlockedUserArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_BlockedUserArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_BlockedUserArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_BlockedUserArray_GetNextUrl")]
		private static extern IntPtr ovr_BlockedUserArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_BlockedUserArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_BlockedUserArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ChallengeCreationType ovr_Challenge_GetCreationType(IntPtr obj);

		public static string ovr_Challenge_GetDescription(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Challenge_GetDescription_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Challenge_GetDescription")]
		private static extern IntPtr ovr_Challenge_GetDescription_Native(IntPtr obj);

		public static DateTime ovr_Challenge_GetEndDate(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_Challenge_GetEndDate_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Challenge_GetEndDate")]
		private static extern ulong ovr_Challenge_GetEndDate_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Challenge_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Challenge_GetInvitedUsers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Challenge_GetLeaderboard(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Challenge_GetParticipants(IntPtr obj);

		public static DateTime ovr_Challenge_GetStartDate(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_Challenge_GetStartDate_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Challenge_GetStartDate")]
		private static extern ulong ovr_Challenge_GetStartDate_Native(IntPtr obj);

		public static string ovr_Challenge_GetTitle(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Challenge_GetTitle_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Challenge_GetTitle")]
		private static extern IntPtr ovr_Challenge_GetTitle_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ChallengeVisibility ovr_Challenge_GetVisibility(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ChallengeArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_ChallengeArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ChallengeArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeArray_GetNextUrl")]
		private static extern IntPtr ovr_ChallengeArray_GetNextUrl_Native(IntPtr obj);

		public static string ovr_ChallengeArray_GetPreviousUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ChallengeArray_GetPreviousUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeArray_GetPreviousUrl")]
		private static extern IntPtr ovr_ChallengeArray_GetPreviousUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_ChallengeArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ChallengeArray_GetTotalCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ChallengeArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ChallengeArray_HasPreviousPage(IntPtr obj);

		public static string ovr_ChallengeEntry_GetDisplayScore(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ChallengeEntry_GetDisplayScore_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeEntry_GetDisplayScore")]
		private static extern IntPtr ovr_ChallengeEntry_GetDisplayScore_Native(IntPtr obj);

		public static byte[] ovr_ChallengeEntry_GetExtraData(IntPtr obj)
		{
			return CAPI.BlobFromNative(CAPI.ovr_LeaderboardEntry_GetExtraDataLength(obj), CAPI.ovr_ChallengeEntry_GetExtraData_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeEntry_GetExtraData")]
		private static extern IntPtr ovr_ChallengeEntry_GetExtraData_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_ChallengeEntry_GetExtraDataLength(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ChallengeEntry_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_ChallengeEntry_GetRank(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_ChallengeEntry_GetScore(IntPtr obj);

		public static DateTime ovr_ChallengeEntry_GetTimestamp(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_ChallengeEntry_GetTimestamp_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeEntry_GetTimestamp")]
		private static extern ulong ovr_ChallengeEntry_GetTimestamp_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ChallengeEntry_GetUser(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ChallengeEntryArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_ChallengeEntryArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ChallengeEntryArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeEntryArray_GetNextUrl")]
		private static extern IntPtr ovr_ChallengeEntryArray_GetNextUrl_Native(IntPtr obj);

		public static string ovr_ChallengeEntryArray_GetPreviousUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ChallengeEntryArray_GetPreviousUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeEntryArray_GetPreviousUrl")]
		private static extern IntPtr ovr_ChallengeEntryArray_GetPreviousUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_ChallengeEntryArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_ChallengeEntryArray_GetTotalCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ChallengeEntryArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ChallengeEntryArray_HasPreviousPage(IntPtr obj);

		public static string ovr_ContentRating_GetAgeRatingImageUri(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ContentRating_GetAgeRatingImageUri_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ContentRating_GetAgeRatingImageUri")]
		private static extern IntPtr ovr_ContentRating_GetAgeRatingImageUri_Native(IntPtr obj);

		public static string ovr_ContentRating_GetAgeRatingText(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ContentRating_GetAgeRatingText_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ContentRating_GetAgeRatingText")]
		private static extern IntPtr ovr_ContentRating_GetAgeRatingText_Native(IntPtr obj);

		public static string ovr_ContentRating_GetDescriptor(IntPtr obj, uint index)
		{
			return CAPI.StringFromNative(CAPI.ovr_ContentRating_GetDescriptor_Native(obj, index));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ContentRating_GetDescriptor")]
		private static extern IntPtr ovr_ContentRating_GetDescriptor_Native(IntPtr obj, uint index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_ContentRating_GetDescriptorsSize(IntPtr obj);

		public static string ovr_ContentRating_GetInteractiveElement(IntPtr obj, uint index)
		{
			return CAPI.StringFromNative(CAPI.ovr_ContentRating_GetInteractiveElement_Native(obj, index));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ContentRating_GetInteractiveElement")]
		private static extern IntPtr ovr_ContentRating_GetInteractiveElement_Native(IntPtr obj, uint index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_ContentRating_GetInteractiveElementsSize(IntPtr obj);

		public static string ovr_ContentRating_GetRatingDefinitionUri(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ContentRating_GetRatingDefinitionUri_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ContentRating_GetRatingDefinitionUri")]
		private static extern IntPtr ovr_ContentRating_GetRatingDefinitionUri_Native(IntPtr obj);

		public static string ovr_CowatchViewer_GetData(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_CowatchViewer_GetData_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_CowatchViewer_GetData")]
		private static extern IntPtr ovr_CowatchViewer_GetData_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_CowatchViewer_GetId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_CowatchViewerArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_CowatchViewerArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_CowatchViewerArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_CowatchViewerArray_GetNextUrl")]
		private static extern IntPtr ovr_CowatchViewerArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_CowatchViewerArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_CowatchViewerArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_CowatchViewerUpdate_GetDataList(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_CowatchViewerUpdate_GetId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_CowatchingState_GetInSession(IntPtr obj);

		public static uint ovr_DataStore_Contains(IntPtr obj, string key)
		{
			IntPtr intPtr = CAPI.StringToNative(key);
			uint result = CAPI.ovr_DataStore_Contains_Native(obj, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DataStore_Contains")]
		private static extern uint ovr_DataStore_Contains_Native(IntPtr obj, IntPtr key);

		public static string ovr_DataStore_GetKey(IntPtr obj, int index)
		{
			return CAPI.StringFromNative(CAPI.ovr_DataStore_GetKey_Native(obj, index));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DataStore_GetKey")]
		private static extern IntPtr ovr_DataStore_GetKey_Native(IntPtr obj, int index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_DataStore_GetNumKeys(IntPtr obj);

		public static string ovr_DataStore_GetValue(IntPtr obj, string key)
		{
			IntPtr intPtr = CAPI.StringToNative(key);
			string result = CAPI.StringFromNative(CAPI.ovr_DataStore_GetValue_Native(obj, intPtr));
			Marshal.FreeCoTaskMem(intPtr);
			return result;
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DataStore_GetValue")]
		private static extern IntPtr ovr_DataStore_GetValue_Native(IntPtr obj, IntPtr key);

		public static string ovr_Destination_GetApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Destination_GetApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Destination_GetApiName")]
		private static extern IntPtr ovr_Destination_GetApiName_Native(IntPtr obj);

		public static string ovr_Destination_GetDeeplinkMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Destination_GetDeeplinkMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Destination_GetDeeplinkMessage")]
		private static extern IntPtr ovr_Destination_GetDeeplinkMessage_Native(IntPtr obj);

		public static string ovr_Destination_GetDisplayName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Destination_GetDisplayName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Destination_GetDisplayName")]
		private static extern IntPtr ovr_Destination_GetDisplayName_Native(IntPtr obj);

		public static string ovr_Destination_GetShareableUri(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Destination_GetShareableUri_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Destination_GetShareableUri")]
		private static extern IntPtr ovr_Destination_GetShareableUri_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_DestinationArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_DestinationArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_DestinationArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_DestinationArray_GetNextUrl")]
		private static extern IntPtr ovr_DestinationArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_DestinationArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_DestinationArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_Error_GetCode(IntPtr obj);

		public static string ovr_Error_GetDisplayableMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Error_GetDisplayableMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Error_GetDisplayableMessage")]
		private static extern IntPtr ovr_Error_GetDisplayableMessage_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_Error_GetHttpCode(IntPtr obj);

		public static string ovr_Error_GetMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Error_GetMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Error_GetMessage")]
		private static extern IntPtr ovr_Error_GetMessage_Native(IntPtr obj);

		public static string ovr_GroupPresenceJoinIntent_GetDeeplinkMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceJoinIntent_GetDeeplinkMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceJoinIntent_GetDeeplinkMessage")]
		private static extern IntPtr ovr_GroupPresenceJoinIntent_GetDeeplinkMessage_Native(IntPtr obj);

		public static string ovr_GroupPresenceJoinIntent_GetDestinationApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceJoinIntent_GetDestinationApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceJoinIntent_GetDestinationApiName")]
		private static extern IntPtr ovr_GroupPresenceJoinIntent_GetDestinationApiName_Native(IntPtr obj);

		public static string ovr_GroupPresenceJoinIntent_GetLobbySessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceJoinIntent_GetLobbySessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceJoinIntent_GetLobbySessionId")]
		private static extern IntPtr ovr_GroupPresenceJoinIntent_GetLobbySessionId_Native(IntPtr obj);

		public static string ovr_GroupPresenceJoinIntent_GetMatchSessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceJoinIntent_GetMatchSessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceJoinIntent_GetMatchSessionId")]
		private static extern IntPtr ovr_GroupPresenceJoinIntent_GetMatchSessionId_Native(IntPtr obj);

		public static string ovr_GroupPresenceLeaveIntent_GetDestinationApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceLeaveIntent_GetDestinationApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceLeaveIntent_GetDestinationApiName")]
		private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetDestinationApiName_Native(IntPtr obj);

		public static string ovr_GroupPresenceLeaveIntent_GetLobbySessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceLeaveIntent_GetLobbySessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceLeaveIntent_GetLobbySessionId")]
		private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetLobbySessionId_Native(IntPtr obj);

		public static string ovr_GroupPresenceLeaveIntent_GetMatchSessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_GroupPresenceLeaveIntent_GetMatchSessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceLeaveIntent_GetMatchSessionId")]
		private static extern IntPtr ovr_GroupPresenceLeaveIntent_GetMatchSessionId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_HttpTransferUpdate_GetBytes(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_HttpTransferUpdate_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_HttpTransferUpdate_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_HttpTransferUpdate_IsCompleted(IntPtr obj);

		public static string ovr_InstalledApplication_GetApplicationId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_InstalledApplication_GetApplicationId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_InstalledApplication_GetApplicationId")]
		private static extern IntPtr ovr_InstalledApplication_GetApplicationId_Native(IntPtr obj);

		public static string ovr_InstalledApplication_GetPackageName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_InstalledApplication_GetPackageName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_InstalledApplication_GetPackageName")]
		private static extern IntPtr ovr_InstalledApplication_GetPackageName_Native(IntPtr obj);

		public static string ovr_InstalledApplication_GetStatus(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_InstalledApplication_GetStatus_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_InstalledApplication_GetStatus")]
		private static extern IntPtr ovr_InstalledApplication_GetStatus_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_InstalledApplication_GetVersionCode(IntPtr obj);

		public static string ovr_InstalledApplication_GetVersionName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_InstalledApplication_GetVersionName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_InstalledApplication_GetVersionName")]
		private static extern IntPtr ovr_InstalledApplication_GetVersionName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_InstalledApplicationArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_InstalledApplicationArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_InvitePanelResultInfo_GetInvitesSent(IntPtr obj);

		public static string ovr_LanguagePackInfo_GetEnglishName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LanguagePackInfo_GetEnglishName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LanguagePackInfo_GetEnglishName")]
		private static extern IntPtr ovr_LanguagePackInfo_GetEnglishName_Native(IntPtr obj);

		public static string ovr_LanguagePackInfo_GetNativeName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LanguagePackInfo_GetNativeName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LanguagePackInfo_GetNativeName")]
		private static extern IntPtr ovr_LanguagePackInfo_GetNativeName_Native(IntPtr obj);

		public static string ovr_LanguagePackInfo_GetTag(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LanguagePackInfo_GetTag_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LanguagePackInfo_GetTag")]
		private static extern IntPtr ovr_LanguagePackInfo_GetTag_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchBlockFlowResult_GetDidBlock(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchBlockFlowResult_GetDidCancel(IntPtr obj);

		public static string ovr_LaunchDetails_GetDeeplinkMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetDeeplinkMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetDeeplinkMessage")]
		private static extern IntPtr ovr_LaunchDetails_GetDeeplinkMessage_Native(IntPtr obj);

		public static string ovr_LaunchDetails_GetDestinationApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetDestinationApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetDestinationApiName")]
		private static extern IntPtr ovr_LaunchDetails_GetDestinationApiName_Native(IntPtr obj);

		public static string ovr_LaunchDetails_GetLaunchSource(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetLaunchSource_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetLaunchSource")]
		private static extern IntPtr ovr_LaunchDetails_GetLaunchSource_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern LaunchType ovr_LaunchDetails_GetLaunchType(IntPtr obj);

		public static string ovr_LaunchDetails_GetLobbySessionID(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetLobbySessionID_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetLobbySessionID")]
		private static extern IntPtr ovr_LaunchDetails_GetLobbySessionID_Native(IntPtr obj);

		public static string ovr_LaunchDetails_GetMatchSessionID(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetMatchSessionID_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetMatchSessionID")]
		private static extern IntPtr ovr_LaunchDetails_GetMatchSessionID_Native(IntPtr obj);

		public static string ovr_LaunchDetails_GetTrackingID(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LaunchDetails_GetTrackingID_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LaunchDetails_GetTrackingID")]
		private static extern IntPtr ovr_LaunchDetails_GetTrackingID_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LaunchDetails_GetUsers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchFriendRequestFlowResult_GetDidCancel(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchFriendRequestFlowResult_GetDidSendRequest(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LaunchInvitePanelFlowResult_GetInvitedUsers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchReportFlowResult_GetDidCancel(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_LaunchReportFlowResult_GetUserReportId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchUnblockFlowResult_GetDidCancel(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LaunchUnblockFlowResult_GetDidUnblock(IntPtr obj);

		public static string ovr_Leaderboard_GetApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Leaderboard_GetApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Leaderboard_GetApiName")]
		private static extern IntPtr ovr_Leaderboard_GetApiName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Leaderboard_GetDestination(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Leaderboard_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LeaderboardArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_LeaderboardArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LeaderboardArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardArray_GetNextUrl")]
		private static extern IntPtr ovr_LeaderboardArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_LeaderboardArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LeaderboardArray_HasNextPage(IntPtr obj);

		public static string ovr_LeaderboardEntry_GetDisplayScore(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LeaderboardEntry_GetDisplayScore_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardEntry_GetDisplayScore")]
		private static extern IntPtr ovr_LeaderboardEntry_GetDisplayScore_Native(IntPtr obj);

		public static byte[] ovr_LeaderboardEntry_GetExtraData(IntPtr obj)
		{
			return CAPI.BlobFromNative(CAPI.ovr_LeaderboardEntry_GetExtraDataLength(obj), CAPI.ovr_LeaderboardEntry_GetExtraData_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardEntry_GetExtraData")]
		private static extern IntPtr ovr_LeaderboardEntry_GetExtraData_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_LeaderboardEntry_GetExtraDataLength(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_LeaderboardEntry_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_LeaderboardEntry_GetRank(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_LeaderboardEntry_GetScore(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LeaderboardEntry_GetSupplementaryMetric(IntPtr obj);

		public static DateTime ovr_LeaderboardEntry_GetTimestamp(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_LeaderboardEntry_GetTimestamp_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardEntry_GetTimestamp")]
		private static extern ulong ovr_LeaderboardEntry_GetTimestamp_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LeaderboardEntry_GetUser(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LeaderboardEntryArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_LeaderboardEntryArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LeaderboardEntryArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardEntryArray_GetNextUrl")]
		private static extern IntPtr ovr_LeaderboardEntryArray_GetNextUrl_Native(IntPtr obj);

		public static string ovr_LeaderboardEntryArray_GetPreviousUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LeaderboardEntryArray_GetPreviousUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LeaderboardEntryArray_GetPreviousUrl")]
		private static extern IntPtr ovr_LeaderboardEntryArray_GetPreviousUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_LeaderboardEntryArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_LeaderboardEntryArray_GetTotalCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LeaderboardEntryArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LeaderboardEntryArray_HasPreviousPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LeaderboardUpdateStatus_GetDidUpdate(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_LeaderboardUpdateStatus_GetUpdatedChallengeId(IntPtr obj, uint index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_LeaderboardUpdateStatus_GetUpdatedChallengeIdsSize(IntPtr obj);

		public static string ovr_LinkedAccount_GetAccessToken(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LinkedAccount_GetAccessToken_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LinkedAccount_GetAccessToken")]
		private static extern IntPtr ovr_LinkedAccount_GetAccessToken_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ServiceProvider ovr_LinkedAccount_GetServiceProvider(IntPtr obj);

		public static string ovr_LinkedAccount_GetUserId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LinkedAccount_GetUserId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LinkedAccount_GetUserId")]
		private static extern IntPtr ovr_LinkedAccount_GetUserId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_LinkedAccountArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_LinkedAccountArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LivestreamingApplicationStatus_GetStreamingEnabled(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern LivestreamingStartStatus ovr_LivestreamingStartResult_GetStreamingResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LivestreamingStatus_GetCommentsVisible(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LivestreamingStatus_GetIsPaused(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LivestreamingStatus_GetLivestreamingEnabled(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_LivestreamingStatus_GetLivestreamingType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_LivestreamingStatus_GetMicEnabled(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_LivestreamingVideoStats_GetCommentCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_LivestreamingVideoStats_GetReactionCount(IntPtr obj);

		public static string ovr_LivestreamingVideoStats_GetTotalViews(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_LivestreamingVideoStats_GetTotalViews_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_LivestreamingVideoStats_GetTotalViews")]
		private static extern IntPtr ovr_LivestreamingVideoStats_GetTotalViews_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetDepartment(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetDepartment_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetDepartment")]
		private static extern IntPtr ovr_ManagedInfo_GetDepartment_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetEmail(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetEmail_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetEmail")]
		private static extern IntPtr ovr_ManagedInfo_GetEmail_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetEmployeeNumber(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetEmployeeNumber_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetEmployeeNumber")]
		private static extern IntPtr ovr_ManagedInfo_GetEmployeeNumber_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetExternalId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetExternalId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetExternalId")]
		private static extern IntPtr ovr_ManagedInfo_GetExternalId_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetLocation(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetLocation_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetLocation")]
		private static extern IntPtr ovr_ManagedInfo_GetLocation_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetManager(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetManager_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetManager")]
		private static extern IntPtr ovr_ManagedInfo_GetManager_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetName")]
		private static extern IntPtr ovr_ManagedInfo_GetName_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetOrganizationId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetOrganizationId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetOrganizationId")]
		private static extern IntPtr ovr_ManagedInfo_GetOrganizationId_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetOrganizationName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetOrganizationName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetOrganizationName")]
		private static extern IntPtr ovr_ManagedInfo_GetOrganizationName_Native(IntPtr obj);

		public static string ovr_ManagedInfo_GetPosition(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ManagedInfo_GetPosition_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ManagedInfo_GetPosition")]
		private static extern IntPtr ovr_ManagedInfo_GetPosition_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAbuseReportRecording(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAchievementDefinitionArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAchievementProgressArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAchievementUpdate(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAppDownloadProgressResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAppDownloadResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetApplicationInviteArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetApplicationVersion(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetDetails(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetDetailsArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetFileDeleteResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetFileDownloadCancelResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetFileDownloadResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAssetFileDownloadUpdate(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetAvatarEditorResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetBlockedUserArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetChallenge(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetChallengeArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetChallengeEntryArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetCowatchViewerArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetCowatchViewerUpdate(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetCowatchingState(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetDataStore(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetDestinationArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetError(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetGroupPresenceJoinIntent(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetGroupPresenceLeaveIntent(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetHttpTransferUpdate(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetInstalledApplicationArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetInvitePanelResultInfo(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLaunchBlockFlowResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLaunchFriendRequestFlowResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLaunchInvitePanelFlowResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLaunchReportFlowResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLaunchUnblockFlowResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLeaderboardArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLeaderboardEntryArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLeaderboardUpdateStatus(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLinkedAccountArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLivestreamingApplicationStatus(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLivestreamingStartResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLivestreamingStatus(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetLivestreamingVideoStats(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetMicrophoneAvailabilityState(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNativeMessage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNetSyncConnection(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNetSyncSessionArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNetSyncSessionsChangedNotification(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNetSyncSetSessionPropertyResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetNetSyncVoipAttenuationValueArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetOrgScopedID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetParty(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPartyID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPartyUpdateNotification(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPidArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPlatformInitialize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetProductArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPurchase(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPurchaseArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetPushNotificationResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetRejoinDialogResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Message_GetRequestID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetSdkAccountArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetSendInvitesResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetShareMediaResult(IntPtr obj);

		public static string ovr_Message_GetString(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Message_GetString_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Message_GetString")]
		private static extern IntPtr ovr_Message_GetString_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetSystemVoipState(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern Message.MessageType ovr_Message_GetType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUser(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserAccountAgeCategory(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserCapabilityArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserDataStoreUpdateResponse(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserProof(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Message_GetUserReportID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_Message_IsError(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Microphone_GetNumSamplesAvailable(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Microphone_GetOutputBufferMaxSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Microphone_GetPCM(IntPtr obj, short[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Microphone_GetPCMFloat(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferNumElements);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Microphone_ReadData(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Microphone_SetAcceptableRecordingDelayHint(IntPtr obj, UIntPtr delayMs);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Microphone_Start(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Microphone_Stop(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_MicrophoneAvailabilityState_GetMicrophoneAvailable(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_NetSyncConnection_GetConnectionId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern NetSyncDisconnectReason ovr_NetSyncConnection_GetDisconnectReason(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSyncConnection_GetSessionId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern NetSyncConnectionStatus ovr_NetSyncConnection_GetStatus(IntPtr obj);

		public static string ovr_NetSyncConnection_GetZoneId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_NetSyncConnection_GetZoneId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSyncConnection_GetZoneId")]
		private static extern IntPtr ovr_NetSyncConnection_GetZoneId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_NetSyncSession_GetConnectionId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_NetSyncSession_GetMuted(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSyncSession_GetSessionId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_NetSyncSession_GetUserId(IntPtr obj);

		public static string ovr_NetSyncSession_GetVoipGroup(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_NetSyncSession_GetVoipGroup_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSyncSession_GetVoipGroup")]
		private static extern IntPtr ovr_NetSyncSession_GetVoipGroup_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_NetSyncSessionArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSyncSessionArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_NetSyncSessionsChangedNotification_GetConnectionId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_NetSyncSessionsChangedNotification_GetSessions(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_NetSyncSetSessionPropertyResult_GetSession(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovr_NetSyncVoipAttenuationValue_GetDecibels(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern float ovr_NetSyncVoipAttenuationValue_GetDistance(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_NetSyncVoipAttenuationValueArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_NetSyncVoipAttenuationValueArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_OrgScopedID_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_Packet_Free(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Packet_GetBytes(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Packet_GetSenderID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_Packet_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_PaidOffer_GetPrice(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern OfferTerm ovr_PaidOffer_GetSubscriptionTerm(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Party_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Party_GetInvitedUsers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Party_GetLeader(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Party_GetUsers(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PartyID_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern PartyUpdateAction ovr_PartyUpdateNotification_GetAction(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PartyUpdateNotification_GetPartyId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PartyUpdateNotification_GetSenderId(IntPtr obj);

		public static string ovr_PartyUpdateNotification_GetUpdateTimestamp(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_PartyUpdateNotification_GetUpdateTimestamp_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_PartyUpdateNotification_GetUpdateTimestamp")]
		private static extern IntPtr ovr_PartyUpdateNotification_GetUpdateTimestamp_Native(IntPtr obj);

		public static string ovr_PartyUpdateNotification_GetUserAlias(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_PartyUpdateNotification_GetUserAlias_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_PartyUpdateNotification_GetUserAlias")]
		private static extern IntPtr ovr_PartyUpdateNotification_GetUserAlias_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_PartyUpdateNotification_GetUserId(IntPtr obj);

		public static string ovr_PartyUpdateNotification_GetUserName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_PartyUpdateNotification_GetUserName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_PartyUpdateNotification_GetUserName")]
		private static extern IntPtr ovr_PartyUpdateNotification_GetUserName_Native(IntPtr obj);

		public static string ovr_Pid_GetId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Pid_GetId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Pid_GetId")]
		private static extern IntPtr ovr_Pid_GetId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_PidArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_PidArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern PlatformInitializeResult ovr_PlatformInitialize_GetResult(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern uint ovr_Price_GetAmountInHundredths(IntPtr obj);

		public static string ovr_Price_GetCurrency(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Price_GetCurrency_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Price_GetCurrency")]
		private static extern IntPtr ovr_Price_GetCurrency_Native(IntPtr obj);

		public static string ovr_Price_GetFormatted(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Price_GetFormatted_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Price_GetFormatted")]
		private static extern IntPtr ovr_Price_GetFormatted_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Product_GetBillingPlans(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Product_GetContentRating(IntPtr obj);

		public static string ovr_Product_GetCoverUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetCoverUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetCoverUrl")]
		private static extern IntPtr ovr_Product_GetCoverUrl_Native(IntPtr obj);

		public static string ovr_Product_GetDescription(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetDescription_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetDescription")]
		private static extern IntPtr ovr_Product_GetDescription_Native(IntPtr obj);

		public static string ovr_Product_GetFormattedPrice(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetFormattedPrice_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetFormattedPrice")]
		private static extern IntPtr ovr_Product_GetFormattedPrice_Native(IntPtr obj);

		public static string ovr_Product_GetIconUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetIconUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetIconUrl")]
		private static extern IntPtr ovr_Product_GetIconUrl_Native(IntPtr obj);

		public static string ovr_Product_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetName")]
		private static extern IntPtr ovr_Product_GetName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_Product_GetPrice(IntPtr obj);

		public static string ovr_Product_GetShortDescription(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetShortDescription_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetShortDescription")]
		private static extern IntPtr ovr_Product_GetShortDescription_Native(IntPtr obj);

		public static string ovr_Product_GetSKU(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Product_GetSKU_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Product_GetSKU")]
		private static extern IntPtr ovr_Product_GetSKU_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ProductType ovr_Product_GetType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ProductArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_ProductArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_ProductArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ProductArray_GetNextUrl")]
		private static extern IntPtr ovr_ProductArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_ProductArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_ProductArray_HasNextPage(IntPtr obj);

		public static string ovr_Purchase_GetDeveloperPayload(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Purchase_GetDeveloperPayload_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetDeveloperPayload")]
		private static extern IntPtr ovr_Purchase_GetDeveloperPayload_Native(IntPtr obj);

		public static DateTime ovr_Purchase_GetExpirationTime(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_Purchase_GetExpirationTime_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetExpirationTime")]
		private static extern ulong ovr_Purchase_GetExpirationTime_Native(IntPtr obj);

		public static DateTime ovr_Purchase_GetGrantTime(IntPtr obj)
		{
			return CAPI.DateTimeFromNative(CAPI.ovr_Purchase_GetGrantTime_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetGrantTime")]
		private static extern ulong ovr_Purchase_GetGrantTime_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_Purchase_GetPurchaseID(IntPtr obj);

		public static string ovr_Purchase_GetPurchaseStrID(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Purchase_GetPurchaseStrID_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetPurchaseStrID")]
		private static extern IntPtr ovr_Purchase_GetPurchaseStrID_Native(IntPtr obj);

		public static string ovr_Purchase_GetReportingId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Purchase_GetReportingId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetReportingId")]
		private static extern IntPtr ovr_Purchase_GetReportingId_Native(IntPtr obj);

		public static string ovr_Purchase_GetSKU(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_Purchase_GetSKU_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_Purchase_GetSKU")]
		private static extern IntPtr ovr_Purchase_GetSKU_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ProductType ovr_Purchase_GetType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_PurchaseArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_PurchaseArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_PurchaseArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_PurchaseArray_GetNextUrl")]
		private static extern IntPtr ovr_PurchaseArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_PurchaseArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_PurchaseArray_HasNextPage(IntPtr obj);

		public static string ovr_PushNotificationResult_GetId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_PushNotificationResult_GetId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_PushNotificationResult_GetId")]
		private static extern IntPtr ovr_PushNotificationResult_GetId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_RejoinDialogResult_GetRejoinSelected(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern SdkAccountType ovr_SdkAccount_GetAccountType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_SdkAccount_GetUserId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_SdkAccountArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_SdkAccountArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_SendInvitesResult_GetInvites(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ShareMediaStatus ovr_ShareMediaResult_GetStatus(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_SupplementaryMetric_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern long ovr_SupplementaryMetric_GetMetric(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern VoipMuteState ovr_SystemVoipState_GetMicrophoneMuted(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern SystemVoipStatus ovr_SystemVoipState_GetStatus(IntPtr obj);

		public static string ovr_TestUser_GetAccessToken(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_TestUser_GetAccessToken_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_TestUser_GetAccessToken")]
		private static extern IntPtr ovr_TestUser_GetAccessToken_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TestUser_GetAppAccessArray(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TestUser_GetFbAppAccessArray(IntPtr obj);

		public static string ovr_TestUser_GetFriendAccessToken(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_TestUser_GetFriendAccessToken_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_TestUser_GetFriendAccessToken")]
		private static extern IntPtr ovr_TestUser_GetFriendAccessToken_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TestUser_GetFriendAppAccessArray(IntPtr obj);

		public static string ovr_TestUser_GetUserAlias(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_TestUser_GetUserAlias_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_TestUser_GetUserAlias")]
		private static extern IntPtr ovr_TestUser_GetUserAlias_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_TestUser_GetUserFbid(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_TestUser_GetUserId(IntPtr obj);

		public static string ovr_TestUserAppAccess_GetAccessToken(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_TestUserAppAccess_GetAccessToken_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_TestUserAppAccess_GetAccessToken")]
		private static extern IntPtr ovr_TestUserAppAccess_GetAccessToken_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_TestUserAppAccess_GetAppId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_TestUserAppAccess_GetUserId(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TestUserAppAccessArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_TestUserAppAccessArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ovr_TrialOffer_GetMaxTermCount(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TrialOffer_GetPrice(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern OfferTerm ovr_TrialOffer_GetTrialTerm(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern OfferType ovr_TrialOffer_GetTrialType(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_TrialOfferArray_GetElement(IntPtr obj, UIntPtr index);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_TrialOfferArray_GetSize(IntPtr obj);

		public static string ovr_User_GetDisplayName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetDisplayName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetDisplayName")]
		private static extern IntPtr ovr_User_GetDisplayName_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_User_GetID(IntPtr obj);

		public static string ovr_User_GetImageUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetImageUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetImageUrl")]
		private static extern IntPtr ovr_User_GetImageUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_User_GetManagedInfo(IntPtr obj);

		public static string ovr_User_GetOculusID(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetOculusID_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetOculusID")]
		private static extern IntPtr ovr_User_GetOculusID_Native(IntPtr obj);

		public static string ovr_User_GetPresence(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetPresence_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetPresence")]
		private static extern IntPtr ovr_User_GetPresence_Native(IntPtr obj);

		public static string ovr_User_GetPresenceDeeplinkMessage(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetPresenceDeeplinkMessage_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetPresenceDeeplinkMessage")]
		private static extern IntPtr ovr_User_GetPresenceDeeplinkMessage_Native(IntPtr obj);

		public static string ovr_User_GetPresenceDestinationApiName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetPresenceDestinationApiName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetPresenceDestinationApiName")]
		private static extern IntPtr ovr_User_GetPresenceDestinationApiName_Native(IntPtr obj);

		public static string ovr_User_GetPresenceLobbySessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetPresenceLobbySessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetPresenceLobbySessionId")]
		private static extern IntPtr ovr_User_GetPresenceLobbySessionId_Native(IntPtr obj);

		public static string ovr_User_GetPresenceMatchSessionId(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetPresenceMatchSessionId_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetPresenceMatchSessionId")]
		private static extern IntPtr ovr_User_GetPresenceMatchSessionId_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UserPresenceStatus ovr_User_GetPresenceStatus(IntPtr obj);

		public static string ovr_User_GetSmallImageUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_User_GetSmallImageUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_User_GetSmallImageUrl")]
		private static extern IntPtr ovr_User_GetSmallImageUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern AccountAgeCategory ovr_UserAccountAgeCategory_GetAgeCategory(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_UserArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_UserArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserArray_GetNextUrl")]
		private static extern IntPtr ovr_UserArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_UserArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UserArray_HasNextPage(IntPtr obj);

		public static string ovr_UserCapability_GetDescription(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserCapability_GetDescription_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserCapability_GetDescription")]
		private static extern IntPtr ovr_UserCapability_GetDescription_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UserCapability_GetIsEnabled(IntPtr obj);

		public static string ovr_UserCapability_GetName(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserCapability_GetName_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserCapability_GetName")]
		private static extern IntPtr ovr_UserCapability_GetName_Native(IntPtr obj);

		public static string ovr_UserCapability_GetReasonCode(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserCapability_GetReasonCode_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserCapability_GetReasonCode")]
		private static extern IntPtr ovr_UserCapability_GetReasonCode_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_UserCapabilityArray_GetElement(IntPtr obj, UIntPtr index);

		public static string ovr_UserCapabilityArray_GetNextUrl(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserCapabilityArray_GetNextUrl_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserCapabilityArray_GetNextUrl")]
		private static extern IntPtr ovr_UserCapabilityArray_GetNextUrl_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_UserCapabilityArray_GetSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UserCapabilityArray_HasNextPage(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UserDataStoreUpdateResponse_GetSuccess(IntPtr obj);

		public static string ovr_UserProof_GetNonce(IntPtr obj)
		{
			return CAPI.StringFromNative(CAPI.ovr_UserProof_GetNonce_Native(obj));
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_UserProof_GetNonce")]
		private static extern IntPtr ovr_UserProof_GetNonce_Native(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool ovr_UserReportID_GetDidCancel(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern ulong ovr_UserReportID_GetID(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipDecoder_Decode(IntPtr obj, byte[] compressedData, UIntPtr compressedSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_VoipDecoder_GetDecodedPCM(IntPtr obj, float[] outputBuffer, UIntPtr outputBufferSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipEncoder_AddPCM(IntPtr obj, float[] inputData, uint inputSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_VoipEncoder_GetCompressedData(IntPtr obj, byte[] outputBuffer, UIntPtr intputSize);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern UIntPtr ovr_VoipEncoder_GetCompressedDataSize(IntPtr obj);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AbuseReportOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AbuseReportOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AbuseReportOptions_SetPreventPeopleChooser(IntPtr handle, bool value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AbuseReportOptions_SetReportType(IntPtr handle, AbuseReportType value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AdvancedAbuseReportOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_Destroy(IntPtr handle);

		public static void ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString(IntPtr handle, string key, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(key);
			IntPtr intPtr2 = CAPI.StringToNative(value);
			CAPI.ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString_Native(handle, intPtr, intPtr2);
			Marshal.FreeCoTaskMem(intPtr);
			Marshal.FreeCoTaskMem(intPtr2);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString")]
		private static extern void ovr_AdvancedAbuseReportOptions_SetDeveloperDefinedContextString_Native(IntPtr handle, IntPtr key, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_ClearDeveloperDefinedContext(IntPtr handle);

		public static void ovr_AdvancedAbuseReportOptions_SetObjectType(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_AdvancedAbuseReportOptions_SetObjectType_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AdvancedAbuseReportOptions_SetObjectType")]
		private static extern void ovr_AdvancedAbuseReportOptions_SetObjectType_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_SetReportType(IntPtr handle, AbuseReportType value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_AddSuggestedUser(IntPtr handle, ulong value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_ClearSuggestedUsers(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AdvancedAbuseReportOptions_SetVideoMode(IntPtr handle, AbuseReportVideoMode value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ApplicationOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ApplicationOptions_Destroy(IntPtr handle);

		public static void ovr_ApplicationOptions_SetDeeplinkMessage(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ApplicationOptions_SetDeeplinkMessage_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationOptions_SetDeeplinkMessage")]
		private static extern void ovr_ApplicationOptions_SetDeeplinkMessage_Native(IntPtr handle, IntPtr value);

		public static void ovr_ApplicationOptions_SetDestinationApiName(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ApplicationOptions_SetDestinationApiName_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationOptions_SetDestinationApiName")]
		private static extern void ovr_ApplicationOptions_SetDestinationApiName_Native(IntPtr handle, IntPtr value);

		public static void ovr_ApplicationOptions_SetLobbySessionId(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ApplicationOptions_SetLobbySessionId_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationOptions_SetLobbySessionId")]
		private static extern void ovr_ApplicationOptions_SetLobbySessionId_Native(IntPtr handle, IntPtr value);

		public static void ovr_ApplicationOptions_SetMatchSessionId(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ApplicationOptions_SetMatchSessionId_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ApplicationOptions_SetMatchSessionId")]
		private static extern void ovr_ApplicationOptions_SetMatchSessionId_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ApplicationOptions_SetRoomId(IntPtr handle, ulong value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_AvatarEditorOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_AvatarEditorOptions_Destroy(IntPtr handle);

		public static void ovr_AvatarEditorOptions_SetSourceOverride(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_AvatarEditorOptions_SetSourceOverride_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_AvatarEditorOptions_SetSourceOverride")]
		private static extern void ovr_AvatarEditorOptions_SetSourceOverride_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_ChallengeOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_Destroy(IntPtr handle);

		public static void ovr_ChallengeOptions_SetDescription(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ChallengeOptions_SetDescription_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeOptions_SetDescription")]
		private static extern void ovr_ChallengeOptions_SetDescription_Native(IntPtr handle, IntPtr value);

		public static void ovr_ChallengeOptions_SetEndDate(IntPtr handle, DateTime value)
		{
			ulong value2 = CAPI.DateTimeToNative(value);
			CAPI.ovr_ChallengeOptions_SetEndDate_Native(handle, value2);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeOptions_SetEndDate")]
		private static extern void ovr_ChallengeOptions_SetEndDate_Native(IntPtr handle, ulong value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_SetIncludeActiveChallenges(IntPtr handle, bool value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_SetIncludeFutureChallenges(IntPtr handle, bool value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_SetIncludePastChallenges(IntPtr handle, bool value);

		public static void ovr_ChallengeOptions_SetLeaderboardName(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ChallengeOptions_SetLeaderboardName_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeOptions_SetLeaderboardName")]
		private static extern void ovr_ChallengeOptions_SetLeaderboardName_Native(IntPtr handle, IntPtr value);

		public static void ovr_ChallengeOptions_SetStartDate(IntPtr handle, DateTime value)
		{
			ulong value2 = CAPI.DateTimeToNative(value);
			CAPI.ovr_ChallengeOptions_SetStartDate_Native(handle, value2);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeOptions_SetStartDate")]
		private static extern void ovr_ChallengeOptions_SetStartDate_Native(IntPtr handle, ulong value);

		public static void ovr_ChallengeOptions_SetTitle(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_ChallengeOptions_SetTitle_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_ChallengeOptions_SetTitle")]
		private static extern void ovr_ChallengeOptions_SetTitle_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_SetViewerFilter(IntPtr handle, ChallengeViewerFilter value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_ChallengeOptions_SetVisibility(IntPtr handle, ChallengeVisibility value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_GroupPresenceOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_GroupPresenceOptions_Destroy(IntPtr handle);

		public static void ovr_GroupPresenceOptions_SetDeeplinkMessageOverride(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_GroupPresenceOptions_SetDeeplinkMessageOverride_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceOptions_SetDeeplinkMessageOverride")]
		private static extern void ovr_GroupPresenceOptions_SetDeeplinkMessageOverride_Native(IntPtr handle, IntPtr value);

		public static void ovr_GroupPresenceOptions_SetDestinationApiName(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_GroupPresenceOptions_SetDestinationApiName_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceOptions_SetDestinationApiName")]
		private static extern void ovr_GroupPresenceOptions_SetDestinationApiName_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_GroupPresenceOptions_SetIsJoinable(IntPtr handle, bool value);

		public static void ovr_GroupPresenceOptions_SetLobbySessionId(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_GroupPresenceOptions_SetLobbySessionId_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceOptions_SetLobbySessionId")]
		private static extern void ovr_GroupPresenceOptions_SetLobbySessionId_Native(IntPtr handle, IntPtr value);

		public static void ovr_GroupPresenceOptions_SetMatchSessionId(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_GroupPresenceOptions_SetMatchSessionId_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_GroupPresenceOptions_SetMatchSessionId")]
		private static extern void ovr_GroupPresenceOptions_SetMatchSessionId_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_InviteOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_InviteOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_InviteOptions_AddSuggestedUser(IntPtr handle, ulong value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_InviteOptions_ClearSuggestedUsers(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_MultiplayerErrorOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_MultiplayerErrorOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_MultiplayerErrorOptions_SetErrorKey(IntPtr handle, MultiplayerErrorErrorKey value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_NetSyncOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_NetSyncOptions_Destroy(IntPtr handle);

		public static void ovr_NetSyncOptions_SetVoipGroup(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_NetSyncOptions_SetVoipGroup_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSyncOptions_SetVoipGroup")]
		private static extern void ovr_NetSyncOptions_SetVoipGroup_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_NetSyncOptions_SetVoipStreamDefault(IntPtr handle, NetSyncVoipStreamMode value);

		public static void ovr_NetSyncOptions_SetZoneId(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_NetSyncOptions_SetZoneId_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_NetSyncOptions_SetZoneId")]
		private static extern void ovr_NetSyncOptions_SetZoneId_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_RichPresenceOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_RichPresenceOptions_Destroy(IntPtr handle);

		public static void ovr_RichPresenceOptions_SetApiName(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_RichPresenceOptions_SetApiName_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_RichPresenceOptions_SetApiName")]
		private static extern void ovr_RichPresenceOptions_SetApiName_Native(IntPtr handle, IntPtr value);

		public static void ovr_RichPresenceOptions_SetDeeplinkMessageOverride(IntPtr handle, string value)
		{
			IntPtr intPtr = CAPI.StringToNative(value);
			CAPI.ovr_RichPresenceOptions_SetDeeplinkMessageOverride_Native(handle, intPtr);
			Marshal.FreeCoTaskMem(intPtr);
		}

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovr_RichPresenceOptions_SetDeeplinkMessageOverride")]
		private static extern void ovr_RichPresenceOptions_SetDeeplinkMessageOverride_Native(IntPtr handle, IntPtr value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_RichPresenceOptions_SetIsJoinable(IntPtr handle, bool value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_RosterOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_RosterOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_RosterOptions_AddSuggestedUser(IntPtr handle, ulong value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_RosterOptions_ClearSuggestedUsers(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_UserOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UserOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UserOptions_SetMaxUsers(IntPtr handle, uint value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UserOptions_AddServiceProvider(IntPtr handle, ServiceProvider value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UserOptions_ClearServiceProviders(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_UserOptions_SetTimeWindow(IntPtr handle, TimeWindow value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr ovr_VoipOptions_Create();

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipOptions_Destroy(IntPtr handle);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipOptions_SetBitrateForNewConnections(IntPtr handle, VoipBitrate value);

		[DllImport("LibOVRPlatformImpl64_1", CallingConvention = CallingConvention.Cdecl)]
		public static extern void ovr_VoipOptions_SetCreateNewConnectionUseDtx(IntPtr handle, VoipDtxState value);

		public const string DLL_NAME = "LibOVRPlatformImpl64_1";

		private static UTF8Encoding nativeStringEncoding = new UTF8Encoding(false);

		public const int VoipFilterBufferSize = 480;

		public struct ovrKeyValuePair
		{
			public ovrKeyValuePair(string key, string value)
			{
				this.key_ = key;
				this.valueType_ = KeyValuePairType.String;
				this.stringValue_ = value;
				this.intValue_ = 0;
				this.doubleValue_ = 0.0;
			}

			public ovrKeyValuePair(string key, int value)
			{
				this.key_ = key;
				this.valueType_ = KeyValuePairType.Int;
				this.intValue_ = value;
				this.stringValue_ = null;
				this.doubleValue_ = 0.0;
			}

			public ovrKeyValuePair(string key, double value)
			{
				this.key_ = key;
				this.valueType_ = KeyValuePairType.Double;
				this.doubleValue_ = value;
				this.stringValue_ = null;
				this.intValue_ = 0;
			}

			public string key_;

			private KeyValuePairType valueType_;

			public string stringValue_;

			public int intValue_;

			public double doubleValue_;
		}

		public struct ovrNetSyncVec3
		{
			public float x;

			public float y;

			public float z;
		}

		public struct OculusInitParams
		{
			public int sType;

			public string email;

			public string password;

			public ulong appId;

			public string uriPrefixOverride;
		}

		[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
		public delegate void FilterCallback([MarshalAs(UnmanagedType.LPArray, SizeConst = 480)] [In] [Out] short[] pcmData, UIntPtr pcmDataLength, int frequency, int numChannels);
	}
}
