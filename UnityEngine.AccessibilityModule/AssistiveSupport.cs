using System;
using System.Diagnostics;
using UnityEngine.Bindings;

namespace UnityEngine.Accessibility
{
	public static class AssistiveSupport
	{
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<AccessibilityNode> nodeFocusChanged;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		public static event Action<bool> screenReaderStatusChanged;

		public static bool isScreenReaderEnabled { get; private set; }

		public static IAccessibilityNotificationDispatcher notificationDispatcher { get; } = new AssistiveSupport.NotificationDispatcher();

		internal static void Initialize()
		{
			AssistiveSupport.isScreenReaderEnabled = AccessibilityManager.IsScreenReaderEnabled();
			AccessibilityManager.screenReaderStatusChanged += AssistiveSupport.ScreenReaderStatusChanged;
			AccessibilityManager.nodeFocusChanged += AssistiveSupport.NodeFocusChanged;
			AssistiveSupport.s_ServiceManager = new ServiceManager();
		}

		internal static T GetService<T>() where T : IService
		{
			bool flag = AssistiveSupport.s_ServiceManager == null;
			T result;
			if (flag)
			{
				result = default(T);
			}
			else
			{
				result = AssistiveSupport.s_ServiceManager.GetService<T>();
			}
			return result;
		}

		internal static bool IsServiceRunning<T>() where T : IService
		{
			IService service = AssistiveSupport.GetService<T>();
			return service != null;
		}

		internal static void SetApplicationAccessibilityLanguage(SystemLanguage language)
		{
			AccessibilityManager.SetApplicationAccessibilityLanguage(language);
		}

		private static void ScreenReaderStatusChanged(bool screenReaderEnabled)
		{
			bool flag = AssistiveSupport.isScreenReaderEnabled == screenReaderEnabled;
			if (!flag)
			{
				AssistiveSupport.isScreenReaderEnabled = screenReaderEnabled;
				Action<bool> action = AssistiveSupport.screenReaderStatusChanged;
				if (action != null)
				{
					action(AssistiveSupport.isScreenReaderEnabled);
				}
			}
		}

		private static void NodeFocusChanged(AccessibilityNode currentNode)
		{
			Action<AccessibilityNode> action = AssistiveSupport.nodeFocusChanged;
			if (action != null)
			{
				action(currentNode);
			}
		}

		public static AccessibilityHierarchy activeHierarchy
		{
			get
			{
				AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
				return (service != null) ? service.hierarchy : null;
			}
			set
			{
				AssistiveSupport.CheckPlatformSupported();
				using (AccessibilityManager.GetExclusiveLock())
				{
					AccessibilityHierarchyService service = AssistiveSupport.GetService<AccessibilityHierarchyService>();
					bool flag = service != null;
					if (flag)
					{
						service.hierarchy = value;
						Action<AccessibilityHierarchy> action = AssistiveSupport.s_ActiveHierarchyChanged;
						if (action != null)
						{
							action(value);
						}
					}
				}
			}
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static event Action<AccessibilityHierarchy> s_ActiveHierarchyChanged;

		internal static event Action<AccessibilityHierarchy> activeHierarchyChanged
		{
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.AccessibilityModule"
			})]
			add
			{
				AssistiveSupport.s_ActiveHierarchyChanged += value;
			}
			[VisibleToOtherModules(new string[]
			{
				"UnityEditor.AccessibilityModule"
			})]
			remove
			{
				AssistiveSupport.s_ActiveHierarchyChanged -= value;
			}
		}

		internal static void OnHierarchyNodeFramesRefreshed(AccessibilityHierarchy hierarchy)
		{
			bool flag = AssistiveSupport.activeHierarchy == hierarchy;
			if (flag)
			{
				AssistiveSupport.notificationDispatcher.SendLayoutChanged(null);
			}
		}

		private static void CheckPlatformSupported()
		{
			bool flag = !Application.isEditor && !AccessibilityManager.isSupportedPlatform;
			if (flag)
			{
				throw new PlatformNotSupportedException(string.Format("This API is not supported for platform {0}", Application.platform));
			}
		}

		private static ServiceManager s_ServiceManager;

		internal class NotificationDispatcher : IAccessibilityNotificationDispatcher
		{
			private static void Send(in AccessibilityNotificationContext context)
			{
				AccessibilityManager.SendAccessibilityNotification(context);
			}

			public void SendAnnouncement(string announcement)
			{
				AccessibilityNotificationContext accessibilityNotificationContext = new AccessibilityNotificationContext
				{
					notification = AccessibilityNotification.Announcement,
					announcement = announcement
				};
				AssistiveSupport.NotificationDispatcher.Send(accessibilityNotificationContext);
			}

			public void SendPageScrolledAnnouncement(string announcement)
			{
				AccessibilityNotificationContext accessibilityNotificationContext = new AccessibilityNotificationContext
				{
					notification = AccessibilityNotification.PageScrolled,
					announcement = announcement
				};
				AssistiveSupport.NotificationDispatcher.Send(accessibilityNotificationContext);
			}

			public void SendScreenChanged(AccessibilityNode nodeToFocus = null)
			{
				AccessibilityNotificationContext accessibilityNotificationContext = new AccessibilityNotificationContext
				{
					notification = AccessibilityNotification.ScreenChanged,
					nextNodeId = ((nodeToFocus == null) ? -1 : nodeToFocus.id)
				};
				AssistiveSupport.NotificationDispatcher.Send(accessibilityNotificationContext);
			}

			public void SendLayoutChanged(AccessibilityNode nodeToFocus = null)
			{
				AccessibilityNotificationContext accessibilityNotificationContext = new AccessibilityNotificationContext
				{
					notification = AccessibilityNotification.LayoutChanged,
					nextNodeId = ((nodeToFocus == null) ? -1 : nodeToFocus.id)
				};
				AssistiveSupport.NotificationDispatcher.Send(accessibilityNotificationContext);
			}
		}
	}
}
