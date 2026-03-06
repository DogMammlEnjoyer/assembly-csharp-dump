using System;
using Oculus.Platform.Models;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Cowatching
	{
		public static Request<string> GetPresenterData()
		{
			if (Core.IsInitialized())
			{
				return new Request<string>(CAPI.ovr_Cowatching_GetPresenterData());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<CowatchViewerList> GetViewersData()
		{
			if (Core.IsInitialized())
			{
				return new Request<CowatchViewerList>(CAPI.ovr_Cowatching_GetViewersData());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request<CowatchingState> IsInSession()
		{
			if (Core.IsInitialized())
			{
				return new Request<CowatchingState>(CAPI.ovr_Cowatching_IsInSession());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request JoinSession()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_JoinSession());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request LaunchInviteDialog()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_LaunchInviteDialog());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request LeaveSession()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_LeaveSession());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request RequestToPresent()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_RequestToPresent());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request ResignFromPresenting()
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_ResignFromPresenting());
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request SetPresenterData(string video_title, string presenter_data)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_SetPresenterData(video_title, presenter_data));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static Request SetViewerData(string viewer_data)
		{
			if (Core.IsInitialized())
			{
				return new Request(CAPI.ovr_Cowatching_SetViewerData(viewer_data));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}

		public static void SetApiNotReadyNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_ApiNotReady, callback);
		}

		public static void SetApiReadyNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_ApiReady, callback);
		}

		public static void SetInSessionChangedNotificationCallback(Message<CowatchingState>.Callback callback)
		{
			Callback.SetNotificationCallback<CowatchingState>(Message.MessageType.Notification_Cowatching_InSessionChanged, callback);
		}

		public static void SetInitializedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_Initialized, callback);
		}

		public static void SetPresenterDataChangedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_PresenterDataChanged, callback);
		}

		public static void SetSessionStartedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_SessionStarted, callback);
		}

		public static void SetSessionStoppedNotificationCallback(Message<string>.Callback callback)
		{
			Callback.SetNotificationCallback<string>(Message.MessageType.Notification_Cowatching_SessionStopped, callback);
		}

		public static void SetViewersDataChangedNotificationCallback(Message<CowatchViewerUpdate>.Callback callback)
		{
			Callback.SetNotificationCallback<CowatchViewerUpdate>(Message.MessageType.Notification_Cowatching_ViewersDataChanged, callback);
		}

		public static Request<CowatchViewerList> GetNextCowatchViewerListPage(CowatchViewerList list)
		{
			if (!list.HasNextPage)
			{
				Debug.LogWarning("Oculus.Platform.GetNextCowatchViewerListPage: List has no next page");
				return null;
			}
			if (Core.IsInitialized())
			{
				return new Request<CowatchViewerList>(CAPI.ovr_HTTP_GetWithMessageType(list.NextUrl, 490748210));
			}
			Debug.LogError(Core.PlatformUninitializedError);
			return null;
		}
	}
}
