using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Camera Manager Events")]
	[SaveDuringPlay]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineCameraManagerEvents.html")]
	public class CinemachineCameraManagerEvents : CinemachineMixerEventsBase
	{
		protected override ICinemachineMixer GetMixer()
		{
			return this.CameraManager;
		}

		private void OnEnable()
		{
			if (this.CameraManager == null)
			{
				base.TryGetComponent<CinemachineCameraManagerBase>(out this.CameraManager);
			}
			base.InstallHandlers(this.CameraManager);
		}

		private void OnDisable()
		{
			base.UninstallHandlers();
		}

		[Tooltip("This is the CinemachineCameraManager emitting the events.  If null and the current GameObject has a CinemachineCameraManager component, that component will be used.")]
		public CinemachineCameraManagerBase CameraManager;
	}
}
