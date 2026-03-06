using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using Valve.VR;

namespace Unity.XR.OpenVR
{
	public class OpenVREvents
	{
		public static void Initialize(bool lazyLoadEvents = false)
		{
			OpenVREvents.instance = new OpenVREvents(lazyLoadEvents);
		}

		public bool IsInitialized()
		{
			return OpenVREvents.instance != null;
		}

		public OpenVREvents(bool lazyLoadEvents = false)
		{
			if (OpenVRHelpers.IsUsingSteamVRInput())
			{
				OpenVREvents.enabled = false;
				return;
			}
			OpenVREvents.instance = this;
			this.events = new OpenVREvent[19999];
			this.vrEvent = default(VREvent_t);
			this.vrEventSize = (uint)Marshal.SizeOf(typeof(VREvent_t));
			if (!lazyLoadEvents)
			{
				for (int i = 0; i < this.events.Length; i++)
				{
					this.events[i] = new OpenVREvent();
				}
			}
			else
			{
				this.preloadedEvents = true;
			}
			this.RegisterDefaultEvents();
		}

		public void RegisterDefaultEvents()
		{
			OpenVREvents.AddListener(EVREventType.VREvent_Quit, new UnityAction<VREvent_t>(this.On_VREvent_Quit), false);
		}

		public static void AddListener(EVREventType eventType, UnityAction<VREvent_t> action, bool removeOtherListeners = false)
		{
			OpenVREvents.instance.Add(eventType, action, removeOtherListeners);
		}

		public void Add(EVREventType eventType, UnityAction<VREvent_t> action, bool removeOtherListeners = false)
		{
			if (!OpenVREvents.enabled)
			{
				Debug.LogError("[OpenVR XR Plugin] This events class is currently not enabled, please use SteamVR_Events instead.");
				return;
			}
			if (!this.preloadedEvents && this.events[(int)eventType] == null)
			{
				this.events[(int)eventType] = new OpenVREvent();
			}
			if (removeOtherListeners)
			{
				this.events[(int)eventType].RemoveAllListeners();
			}
			this.events[(int)eventType].AddListener(action);
		}

		public static void RemoveListener(EVREventType eventType, UnityAction<VREvent_t> action)
		{
			OpenVREvents.instance.Remove(eventType, action);
		}

		public void Remove(EVREventType eventType, UnityAction<VREvent_t> action)
		{
			if (this.preloadedEvents || this.events[(int)eventType] != null)
			{
				this.events[(int)eventType].RemoveListener(action);
			}
		}

		public static void Update()
		{
			OpenVREvents.instance.PollEvents();
		}

		public void PollEvents()
		{
			if (OpenVR.System != null && OpenVREvents.enabled)
			{
				int num = 0;
				while (num < 64 && OpenVR.System != null && OpenVR.System.PollNextEvent(ref this.vrEvent, this.vrEventSize))
				{
					int eventType = (int)this.vrEvent.eventType;
					if (OpenVREvents.debugLogAllEvents)
					{
						EVREventType evreventType = (EVREventType)eventType;
						Debug.Log(string.Format("[{0}] {1}", Time.frameCount, evreventType.ToString()));
					}
					if (this.events[eventType] != null)
					{
						this.events[eventType].Invoke(this.vrEvent);
					}
					num++;
				}
			}
		}

		private void On_VREvent_Quit(VREvent_t pEvent)
		{
			if (this.exiting)
			{
				return;
			}
			this.exiting = true;
			if (OpenVR.System != null)
			{
				OpenVR.System.AcknowledgeQuit_Exiting();
			}
			Debug.Log("<b>[OpenVR]</b> Quit requested from OpenVR. Exiting application via Application.Quit");
			Application.Quit();
		}

		private static OpenVREvents instance;

		private OpenVREvent[] events;

		private int[] eventIndicies;

		private VREvent_t vrEvent;

		private uint vrEventSize;

		private bool preloadedEvents;

		private const int maxEventsPerUpdate = 64;

		private static bool debugLogAllEvents = false;

		private static bool enabled = true;

		private bool exiting;
	}
}
