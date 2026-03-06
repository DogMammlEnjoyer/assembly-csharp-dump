using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Brain Events")]
	[SaveDuringPlay]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBrainEvents.html")]
	public class CinemachineBrainEvents : CinemachineMixerEventsBase
	{
		protected override ICinemachineMixer GetMixer()
		{
			return this.Brain;
		}

		private void OnEnable()
		{
			if (this.Brain == null)
			{
				base.TryGetComponent<CinemachineBrain>(out this.Brain);
			}
			if (this.Brain != null)
			{
				base.InstallHandlers(this.Brain);
				CinemachineCore.CameraUpdatedEvent.AddListener(new UnityAction<CinemachineBrain>(this.OnCameraUpdated));
			}
		}

		private void OnDisable()
		{
			base.UninstallHandlers();
			CinemachineCore.CameraUpdatedEvent.RemoveListener(new UnityAction<CinemachineBrain>(this.OnCameraUpdated));
		}

		private void OnCameraUpdated(CinemachineBrain brain)
		{
			if (brain == this.Brain)
			{
				this.BrainUpdatedEvent.Invoke(brain);
			}
		}

		[Tooltip("This is the CinemachineBrain emitting the events.  If null and the current GameObject has a CinemachineBrain component, that component will be used.")]
		public CinemachineBrain Brain;

		[Tooltip("This event will fire after the brain updates its Camera.")]
		public CinemachineCore.BrainEvent BrainUpdatedEvent = new CinemachineCore.BrainEvent();
	}
}
