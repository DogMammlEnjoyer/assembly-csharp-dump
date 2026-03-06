using System;
using UnityEngine;

namespace Meta.XR.Samples
{
	[ExecuteAlways]
	internal class SampleMetadata : MonoBehaviour
	{
		private void Awake()
		{
			this._timestampOpen = Time.realtimeSinceStartup;
		}

		private void OnDestroy()
		{
		}

		private void Start()
		{
			if (Application.isPlaying)
			{
				this.SendEvent(163061602);
				return;
			}
			this.SendEvent(163055403);
		}

		public void OnEditorShutdown()
		{
			this.SendEvent(163056880);
		}

		private void SendEvent(int eventType)
		{
			float num = Time.realtimeSinceStartup - this._timestampOpen;
			OVRTelemetry.Start(eventType, 0, -1L).AddAnnotation("Sample", base.gameObject.scene.name).AddAnnotation("RuntimePlatform", Application.platform.ToString()).AddAnnotation("InEditor", Application.isEditor.ToString()).AddAnnotation("TimeSinceEditorStart", Time.realtimeSinceStartup.ToString("F0")).AddAnnotation("TimeSpent", num.ToString("F0")).Send();
		}

		private float _timestampOpen;
	}
}
