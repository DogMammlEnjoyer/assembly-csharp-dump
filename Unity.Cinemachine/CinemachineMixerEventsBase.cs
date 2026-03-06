using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	public abstract class CinemachineMixerEventsBase : MonoBehaviour
	{
		protected abstract ICinemachineMixer GetMixer();

		protected void InstallHandlers(ICinemachineMixer mixer)
		{
			if (mixer != null)
			{
				CinemachineCore.CameraActivatedEvent.AddListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(this.OnCameraActivated));
				CinemachineCore.CameraDeactivatedEvent.AddListener(new UnityAction<ICinemachineMixer, ICinemachineCamera>(this.OnCameraDeactivated));
				CinemachineCore.BlendCreatedEvent.AddListener(new UnityAction<CinemachineCore.BlendEventParams>(this.OnBlendCreated));
				CinemachineCore.BlendFinishedEvent.AddListener(new UnityAction<ICinemachineMixer, ICinemachineCamera>(this.OnBlendFinished));
			}
		}

		protected void UninstallHandlers()
		{
			CinemachineCore.CameraActivatedEvent.RemoveListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(this.OnCameraActivated));
			CinemachineCore.CameraDeactivatedEvent.RemoveListener(new UnityAction<ICinemachineMixer, ICinemachineCamera>(this.OnCameraDeactivated));
			CinemachineCore.BlendCreatedEvent.RemoveListener(new UnityAction<CinemachineCore.BlendEventParams>(this.OnBlendCreated));
			CinemachineCore.BlendFinishedEvent.RemoveListener(new UnityAction<ICinemachineMixer, ICinemachineCamera>(this.OnBlendFinished));
		}

		private void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
		{
			ICinemachineMixer mixer = this.GetMixer();
			if (evt.Origin == mixer)
			{
				this.CameraActivatedEvent.Invoke(mixer, evt.IncomingCamera);
				if (evt.IsCut)
				{
					this.CameraCutEvent.Invoke(mixer, evt.IncomingCamera);
				}
			}
		}

		private void OnCameraDeactivated(ICinemachineMixer mixer, ICinemachineCamera cam)
		{
			if (mixer == this.GetMixer())
			{
				this.CameraDeactivatedEvent.Invoke(mixer, cam);
			}
		}

		private void OnBlendCreated(CinemachineCore.BlendEventParams evt)
		{
			if (evt.Origin == this.GetMixer())
			{
				this.BlendCreatedEvent.Invoke(evt);
			}
		}

		private void OnBlendFinished(ICinemachineMixer mixer, ICinemachineCamera cam)
		{
			if (mixer == this.GetMixer())
			{
				this.BlendFinishedEvent.Invoke(mixer, cam);
			}
		}

		[Space]
		[Tooltip("This event will fire whenever a virtual camera goes live.  If a blend is involved, then the event will fire on the first frame of the blend.")]
		public CinemachineCore.CameraEvent CameraActivatedEvent = new CinemachineCore.CameraEvent();

		[Tooltip("This event will fire whenever a virtual stops being live.  If a blend is involved, then the event will fire after the last frame of the blend.")]
		public CinemachineCore.CameraEvent CameraDeactivatedEvent = new CinemachineCore.CameraEvent();

		[Tooltip("This event will fire whenever a blend is created in the root frame of this Brain.  The handler can modify any settings in the blend, except the cameras themselves.  Note: timeline tracks will not generate these events.")]
		public CinemachineCore.BlendEvent BlendCreatedEvent = new CinemachineCore.BlendEvent();

		[Tooltip("This event will fire whenever a virtual camera finishes blending in.  It will not fire if the blend length is zero.")]
		public CinemachineCore.CameraEvent BlendFinishedEvent = new CinemachineCore.CameraEvent();

		[Tooltip("This event is fired when there is a camera cut.  A camera cut is a camera activation with a zero-length blend.")]
		public CinemachineCore.CameraEvent CameraCutEvent = new CinemachineCore.CameraEvent();
	}
}
