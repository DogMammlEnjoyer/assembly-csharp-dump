using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Platform
{
	public static class Callback
	{
		internal static void SetNotificationCallback<T>(Message.MessageType type, Message<T>.Callback callback)
		{
			if (callback == null)
			{
				throw new Exception("Cannot provide a null notification callback.");
			}
			Callback.notificationCallbacks[type] = new Callback.RequestCallback<T>(callback);
			if (type == Message.MessageType.Notification_GroupPresence_JoinIntentReceived)
			{
				Callback.FlushJoinIntentNotificationQueue();
			}
		}

		internal static void SetNotificationCallback(Message.MessageType type, Message.Callback callback)
		{
			if (callback == null)
			{
				throw new Exception("Cannot provide a null notification callback.");
			}
			Callback.notificationCallbacks[type] = new Callback.RequestCallback(callback);
		}

		internal static void AddRequest(Request request)
		{
			if (request.RequestID == 0UL)
			{
				Debug.LogError("An unknown error occurred. Request failed.");
				return;
			}
			Callback.requestIDsToRequests[request.RequestID] = request;
		}

		internal static void RunCallbacks()
		{
			for (;;)
			{
				Message message = Message.PopMessage();
				if (message == null)
				{
					break;
				}
				Callback.HandleMessage(message);
			}
		}

		internal static void RunLimitedCallbacks(uint limit)
		{
			int num = 0;
			while ((long)num < (long)((ulong)limit))
			{
				Message message = Message.PopMessage();
				if (message == null)
				{
					break;
				}
				Callback.HandleMessage(message);
				num++;
			}
		}

		internal static void OnApplicationQuit()
		{
			Callback.requestIDsToRequests.Clear();
			Callback.notificationCallbacks.Clear();
		}

		private static void FlushJoinIntentNotificationQueue()
		{
			Callback.hasRegisteredJoinIntentNotificationHandler = true;
			if (Callback.latestPendingJoinIntentNotifications != null)
			{
				Callback.HandleMessage(Callback.latestPendingJoinIntentNotifications);
			}
			Callback.latestPendingJoinIntentNotifications = null;
		}

		internal static void HandleMessage(Message msg)
		{
			Request request;
			if (msg.RequestID != 0UL && Callback.requestIDsToRequests.TryGetValue(msg.RequestID, out request))
			{
				try
				{
					request.HandleMessage(msg);
				}
				finally
				{
					Callback.requestIDsToRequests.Remove(msg.RequestID);
				}
				return;
			}
			Callback.RequestCallback requestCallback;
			if (Callback.notificationCallbacks.TryGetValue(msg.Type, out requestCallback))
			{
				requestCallback.HandleMessage(msg);
				return;
			}
			if (!Callback.hasRegisteredJoinIntentNotificationHandler && msg.Type == Message.MessageType.Notification_GroupPresence_JoinIntentReceived)
			{
				Callback.latestPendingJoinIntentNotifications = msg;
			}
		}

		private static Dictionary<ulong, Request> requestIDsToRequests = new Dictionary<ulong, Request>();

		private static Dictionary<Message.MessageType, Callback.RequestCallback> notificationCallbacks = new Dictionary<Message.MessageType, Callback.RequestCallback>();

		private static bool hasRegisteredJoinIntentNotificationHandler = false;

		private static Message latestPendingJoinIntentNotifications;

		private class RequestCallback
		{
			public RequestCallback()
			{
			}

			public RequestCallback(Message.Callback callback)
			{
				this.messageCallback = callback;
			}

			public virtual void HandleMessage(Message msg)
			{
				if (this.messageCallback != null)
				{
					this.messageCallback(msg);
				}
			}

			private Message.Callback messageCallback;
		}

		private sealed class RequestCallback<T> : Callback.RequestCallback
		{
			public RequestCallback(Message<T>.Callback callback)
			{
				this.callback = callback;
			}

			public override void HandleMessage(Message msg)
			{
				if (this.callback != null)
				{
					if (msg is Message<T>)
					{
						this.callback((Message<T>)msg);
						return;
					}
					string str = "Unable to handle message: ";
					Type type = msg.GetType();
					Debug.LogError(str + ((type != null) ? type.ToString() : null));
				}
			}

			private Message<T>.Callback callback;
		}
	}
}
