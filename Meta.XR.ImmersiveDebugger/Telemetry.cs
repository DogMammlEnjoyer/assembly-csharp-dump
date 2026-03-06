using System;
using System.Collections.Generic;
using Meta.XR.ImmersiveDebugger.Manager;
using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using Meta.XR.ImmersiveDebugger.Utils;

namespace Meta.XR.ImmersiveDebugger
{
	internal static class Telemetry
	{
		internal static string GetTypeHash(this Type type)
		{
			int hashCode = type.GetHashCode();
			string fullName = type.FullName;
			int num = (fullName != null) ? fullName.GetHashCode() : 0;
			return (hashCode ^ num).ToString();
		}

		private static bool IsTypeCustom(this Type type)
		{
			string name = type.Assembly.GetName().Name;
			foreach (string value in Telemetry.NonCustomAssemblies)
			{
				if (name.StartsWith(value, StringComparison.InvariantCultureIgnoreCase))
				{
					return false;
				}
			}
			return true;
		}

		public static void OnPanelActiveStateChanged(Panel panel)
		{
			if (!panel.Initialised)
			{
				return;
			}
			OVRTelemetry.Start(panel.isActiveAndEnabled ? 163057243 : 163059919, 0, -1L).AddAnnotation("action", panel.name).AddAnnotation("action_type", panel.GetType().Name).AddAnnotation("platform", OVRTelemetry.GetPlayModeOrigin()).Send();
		}

		public static void OnButtonClicked(Button button)
		{
			Panel panel = Telemetry.FetchPanel(button);
			OVRTelemetryMarker ovrtelemetryMarker = OVRTelemetry.Start(163058794, 0, -1L);
			ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("action", button.name);
			ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("action_type", button.GetType().Name);
			ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("origin", (panel != null) ? panel.name : null);
			ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("origin_type", (panel != null) ? panel.GetType().Name : null);
			ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("platform", OVRTelemetry.GetPlayModeOrigin());
			ovrtelemetryMarker.Send();
		}

		private static Panel FetchPanel(Controller controller)
		{
			if (controller == null)
			{
				return null;
			}
			Panel panel = controller as Panel;
			if (panel != null)
			{
				return panel;
			}
			return Telemetry.FetchPanel(controller.Owner);
		}

		private static readonly List<string> NonCustomAssemblies = new List<string>
		{
			"Oculus.",
			"Meta."
		};

		[OVRTelemetry.MarkersAttribute]
		internal static class MarkerId
		{
			public const int ComponentTracked = 163059554;

			public const int Run = 163061656;

			public const int FrameUpdate = 163056655;

			public const int PanelOpen = 163057243;

			public const int PanelClose = 163059919;

			public const int PanelInteraction = 163058794;
		}

		internal enum State
		{
			OnStart,
			OnFocusLost,
			OnDisable
		}

		internal enum Method
		{
			Attributes,
			DebugInspector,
			Hierarchy
		}

		internal static class AnnotationType
		{
			public const string Type = "Type";

			public const string Method = "Method";

			public const string State = "State";

			public const string Instances = "Instances";

			public const string Gizmos = "Gizmos";

			public const string Watches = "Watches";

			public const string Tweaks = "Tweaks";

			public const string Actions = "Actions";

			public const string IsCustom = "IsCustom";

			public const string Action = "action";

			public const string ActionType = "action_type";

			public const string Origin = "origin";

			public const string OriginType = "origin_type";

			public const string Platform = "platform";
		}

		internal class TelemetryTracker
		{
			public static Telemetry.TelemetryTracker Init(Telemetry.Method method, IEnumerable<IDebugManager> managers, InstanceCache cache, DebugManager debugManager)
			{
				Telemetry.TelemetryTracker telemetryTracker = new Telemetry.TelemetryTracker(method, managers, cache);
				debugManager.OnFocusLostAction += telemetryTracker.OnFocusLost;
				debugManager.OnDisableAction += telemetryTracker.OnDisable;
				return telemetryTracker;
			}

			private TelemetryTracker(Telemetry.Method method, IEnumerable<IDebugManager> managers, InstanceCache cache)
			{
				this._method = method;
				this._cache = cache;
				this._managers = managers;
				this._runTelemetryMarker = OVRTelemetry.Start(163061656, 0, -1L).AddAnnotation("Method", this._method.ToString()).AddAnnotation("State", Telemetry.State.OnStart.ToString()).AddPlayModeOrigin();
			}

			public void OnStart()
			{
				this.SendStart();
				this.SendComponentTracked(Telemetry.State.OnStart);
			}

			private void OnFocusLost()
			{
				this.SendComponentTracked(Telemetry.State.OnFocusLost);
			}

			private void OnDisable()
			{
				this.SendComponentTracked(Telemetry.State.OnDisable);
			}

			private void SendStart()
			{
				this._runTelemetryMarker.Send();
			}

			private void SendComponentTracked(Telemetry.State state)
			{
				foreach (KeyValuePair<Type, List<InstanceHandle>> keyValuePair in this._cache.CacheData)
				{
					Type type;
					List<InstanceHandle> list;
					keyValuePair.Deconstruct(out type, out list);
					Type type2 = type;
					List<InstanceHandle> list2 = list;
					if (list2.Count > 0)
					{
						OVRTelemetryMarker ovrtelemetryMarker = OVRTelemetry.Start(163059554, 0, -1L).AddPlayModeOrigin().AddAnnotation("State", state.ToString()).AddAnnotation("Method", this._method.ToString()).AddAnnotation("Instances", list2.Count.ToString());
						if (type2.IsTypeCustom())
						{
							ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("Type", type2.GetTypeHash()).AddAnnotation("IsCustom", true);
						}
						else
						{
							ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation("Type", type2.FullName).AddAnnotation("IsCustom", false);
						}
						foreach (IDebugManager debugManager in this._managers)
						{
							ovrtelemetryMarker = ovrtelemetryMarker.AddAnnotation(debugManager.TelemetryAnnotation, debugManager.GetCountPerType(type2).ToString());
						}
						ovrtelemetryMarker.Send();
					}
				}
			}

			private readonly Telemetry.Method _method;

			private readonly InstanceCache _cache;

			private readonly IEnumerable<IDebugManager> _managers;

			private OVRTelemetryMarker _runTelemetryMarker;
		}
	}
}
