using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine.Bindings;
using UnityEngine.Pool;
using UnityEngine.Scripting;

namespace UnityEngine.Accessibility
{
	[VisibleToOtherModules(new string[]
	{
		"UnityEditor.AccessibilityModule"
	})]
	[NativeHeader("Modules/Accessibility/Native/AccessibilityManager.h")]
	internal static class AccessibilityManager
	{
		public static bool isSupportedPlatform
		{
			get
			{
				RuntimePlatform platform = Application.platform;
				return platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer;
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> screenReaderStatusChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<AccessibilityNode> nodeFocusChanged;

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern bool IsScreenReaderEnabled();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SendAccessibilityNotification(in AccessibilityNotificationContext context);

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern SystemLanguage GetApplicationAccessibilityLanguage();

		[MethodImpl(MethodImplOptions.InternalCall)]
		internal static extern void SetApplicationAccessibilityLanguage(SystemLanguage languageCode);

		[RequiredByNativeCode]
		[VisibleToOtherModules(new string[]
		{
			"UnityEditor.AccessibilityModule"
		})]
		internal static void Internal_Initialize()
		{
			AssistiveSupport.Initialize();
		}

		[RequiredByNativeCode]
		private static void Internal_Update()
		{
			bool flag = AccessibilityManager.asyncNotificationContexts.Count == 0;
			if (!flag)
			{
				Queue<AccessibilityManager.NotificationContext> obj = AccessibilityManager.asyncNotificationContexts;
				AccessibilityManager.NotificationContext[] array;
				lock (obj)
				{
					bool flag3 = AccessibilityManager.asyncNotificationContexts.Count == 0;
					if (flag3)
					{
						return;
					}
					array = AccessibilityManager.asyncNotificationContexts.ToArray();
					AccessibilityManager.asyncNotificationContexts.Clear();
				}
				using (AccessibilityManager.GetExclusiveLock())
				{
					foreach (AccessibilityManager.NotificationContext notificationContext in array)
					{
						switch (notificationContext.notification)
						{
						case AccessibilityNotification.ScreenReaderStatusChanged:
						{
							Action<bool> action = AccessibilityManager.screenReaderStatusChanged;
							if (action != null)
							{
								action(notificationContext.isScreenReaderEnabled);
							}
							break;
						}
						case AccessibilityNotification.ElementFocused:
						{
							notificationContext.currentNode.InvokeFocusChanged(true);
							Action<AccessibilityNode> action2 = AccessibilityManager.nodeFocusChanged;
							if (action2 != null)
							{
								action2(notificationContext.currentNode);
							}
							break;
						}
						case AccessibilityNotification.ElementUnfocused:
							notificationContext.currentNode.InvokeFocusChanged(false);
							break;
						case AccessibilityNotification.FontScaleChanged:
							AccessibilitySettings.InvokeFontScaleChanged(notificationContext.fontScale);
							break;
						case AccessibilityNotification.BoldTextStatusChanged:
							AccessibilitySettings.InvokeBoldTextStatusChanged(notificationContext.isBoldTextEnabled);
							break;
						case AccessibilityNotification.ClosedCaptioningStatusChanged:
							AccessibilitySettings.InvokeClosedCaptionStatusChanged(notificationContext.isClosedCaptioningEnabled);
							break;
						}
					}
				}
			}
		}

		[RequiredByNativeCode]
		internal static int[] Internal_GetRootNodeIds()
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			List<AccessibilityNode> list = (service != null) ? service.GetRootNodes() : null;
			bool flag = list == null || list.Count == 0;
			int[] result;
			if (flag)
			{
				result = null;
			}
			else
			{
				List<int> list2;
				using (CollectionPool<List<int>, int>.Get(out list2))
				{
					for (int i = 0; i < list.Count; i++)
					{
						list2.Add(list[i].id);
					}
					result = ((list2.Count == 0) ? null : list2.ToArray());
				}
			}
			return result;
		}

		[RequiredByNativeCode]
		internal static bool Internal_GetNode(int id, ref AccessibilityNodeData nodeData)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			bool flag = service == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				AccessibilityNode accessibilityNode;
				bool flag2 = service.TryGetNode(id, out accessibilityNode);
				if (flag2)
				{
					accessibilityNode.GetNodeData(ref nodeData);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		[RequiredByNativeCode]
		internal static int Internal_GetNodeIdAt(float x, float y)
		{
			AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
			AccessibilityNode accessibilityNode;
			return (service != null && service.TryGetNodeAt(x, y, out accessibilityNode)) ? accessibilityNode.id : -1;
		}

		[RequiredByNativeCode]
		internal static void Internal_OnAccessibilityNotificationReceived(ref AccessibilityNotificationContext context)
		{
			bool flag = context.notification == AccessibilityNotification.ElementFocused;
			if (!flag)
			{
				AccessibilityManager.QueueNotification(new AccessibilityManager.NotificationContext(ref context));
			}
		}

		internal static void QueueNotification(AccessibilityManager.NotificationContext notification)
		{
			Queue<AccessibilityManager.NotificationContext> obj = AccessibilityManager.asyncNotificationContexts;
			lock (obj)
			{
				AccessibilityManager.asyncNotificationContexts.Enqueue(notification);
			}
		}

		internal static IDisposable GetExclusiveLock()
		{
			return new AccessibilityManager.ExclusiveLock();
		}

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Lock();

		[ThreadSafe]
		[MethodImpl(MethodImplOptions.InternalCall)]
		private static extern void Unlock();

		internal static Queue<AccessibilityManager.NotificationContext> asyncNotificationContexts = new Queue<AccessibilityManager.NotificationContext>();

		public struct NotificationContext
		{
			public AccessibilityNotification notification { readonly get; set; }

			public bool isScreenReaderEnabled { readonly get; set; }

			public string announcement { readonly get; set; }

			public bool wasAnnouncementSuccessful { readonly get; set; }

			public AccessibilityNode currentNode { readonly get; set; }

			public AccessibilityNode nextNode { readonly get; set; }

			public float fontScale { readonly get; set; }

			public bool isBoldTextEnabled { readonly get; set; }

			public bool isClosedCaptioningEnabled { readonly get; set; }

			public AccessibilityNotificationContext nativeContext { readonly get; set; }

			public NotificationContext(ref AccessibilityNotificationContext nativeNotification)
			{
				this.nativeContext = nativeNotification;
				this.notification = nativeNotification.notification;
				this.isScreenReaderEnabled = nativeNotification.isScreenReaderEnabled;
				this.announcement = nativeNotification.announcement;
				this.wasAnnouncementSuccessful = nativeNotification.wasAnnouncementSuccessful;
				AccessibilityNode accessibilityNode = null;
				AccessibilityHierarchy activeHierarchy = AssistiveSupport.activeHierarchy;
				if (activeHierarchy != null)
				{
					activeHierarchy.TryGetNode(nativeNotification.currentNodeId, out accessibilityNode);
				}
				this.currentNode = accessibilityNode;
				AccessibilityHierarchy activeHierarchy2 = AssistiveSupport.activeHierarchy;
				if (activeHierarchy2 != null)
				{
					activeHierarchy2.TryGetNode(nativeNotification.nextNodeId, out accessibilityNode);
				}
				this.nextNode = accessibilityNode;
				this.fontScale = 1f;
				this.isBoldTextEnabled = false;
				this.isClosedCaptioningEnabled = false;
			}
		}

		private sealed class ExclusiveLock : IDisposable
		{
			public ExclusiveLock()
			{
				AccessibilityManager.Lock();
			}

			~ExclusiveLock()
			{
				this.InternalDispose();
			}

			private void InternalDispose()
			{
				bool flag = !this.m_Disposed;
				if (flag)
				{
					AccessibilityManager.Unlock();
					this.m_Disposed = true;
				}
			}

			public void Dispose()
			{
				this.InternalDispose();
				GC.SuppressFinalize(this);
			}

			private bool m_Disposed;
		}
	}
}
