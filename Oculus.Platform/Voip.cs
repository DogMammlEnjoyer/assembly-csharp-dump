using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Voip
	{
		public static void Start(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Start(userID);
			}
		}

		public static void Accept(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Accept(userID);
			}
		}

		public static void Stop(ulong userID)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_Stop(userID);
			}
		}

		public static void SetMicrophoneFilterCallback(CAPI.FilterCallback callback)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_SetMicrophoneFilterCallbackWithFixedSizeBuffer(callback, (UIntPtr)480UL);
			}
		}

		public static void SetMicrophoneMuted(VoipMuteState state)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_SetMicrophoneMuted(state);
			}
		}

		public static VoipMuteState GetSystemVoipMicrophoneMuted()
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetSystemVoipMicrophoneMuted();
			}
			return VoipMuteState.Unknown;
		}

		public static SystemVoipStatus GetSystemVoipStatus()
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetSystemVoipStatus();
			}
			return SystemVoipStatus.Unknown;
		}

		public static VoipDtxState GetIsConnectionUsingDtx(ulong peerID)
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetIsConnectionUsingDtx(peerID);
			}
			return VoipDtxState.Unknown;
		}

		public static VoipBitrate GetLocalBitrate(ulong peerID)
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetLocalBitrate(peerID);
			}
			return VoipBitrate.Unknown;
		}

		public static VoipBitrate GetRemoteBitrate(ulong peerID)
		{
			if (Core.IsInitialized())
			{
				return CAPI.ovr_Voip_GetRemoteBitrate(peerID);
			}
			return VoipBitrate.Unknown;
		}

		public static void SetNewConnectionOptions(VoipOptions voipOptions)
		{
			if (Core.IsInitialized())
			{
				CAPI.ovr_Voip_SetNewConnectionOptions((IntPtr)voipOptions);
			}
		}

		public static Request<MicrophoneAvailabilityState> GetMicrophoneAvailability()
		{
			if (Core.IsInitialized())
			{
				return new Request<MicrophoneAvailabilityState>(CAPI.ovr_Voip_GetMicrophoneAvailability());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<SystemVoipState> SetSystemVoipSuppressed(bool suppressed)
		{
			if (Core.IsInitialized())
			{
				return new Request<SystemVoipState>(CAPI.ovr_Voip_SetSystemVoipSuppressed(suppressed));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static void SetMicrophoneAvailabilityStateUpdateNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Voip_MicrophoneAvailabilityStateUpdate, callback);
		}

		public static void SetSystemVoipStateNotificationCallback(Message<SystemVoipState>.Callback callback)
		{
			Callback.SetNotificationCallback<SystemVoipState>(Message.MessageType.Notification_Voip_SystemVoipState, callback);
		}
	}
}
