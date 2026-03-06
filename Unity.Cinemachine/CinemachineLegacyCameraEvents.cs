using System;
using UnityEngine;
using UnityEngine.Events;

namespace Unity.Cinemachine
{
	[Obsolete("Please use CinemachineCameraEvents instead.")]
	[AddComponentMenu("")]
	public class CinemachineLegacyCameraEvents : MonoBehaviour
	{
		private void OnEnable()
		{
			base.TryGetComponent<CinemachineVirtualCameraBase>(out this.m_Vcam);
			if (this.m_Vcam != null)
			{
				CinemachineCore.CameraActivatedEvent.AddListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(this.OnCameraActivated));
			}
		}

		private void OnDisable()
		{
			CinemachineCore.CameraActivatedEvent.RemoveListener(new UnityAction<ICinemachineCamera.ActivationEventParams>(this.OnCameraActivated));
		}

		private void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
		{
			if (evt.IncomingCamera == this.m_Vcam)
			{
				this.OnCameraLive.Invoke(evt.IncomingCamera, evt.OutgoingCamera);
			}
		}

		[Tooltip("This event fires when the CinemachineCamera goes Live")]
		public CinemachineLegacyCameraEvents.OnCameraLiveEvent OnCameraLive = new CinemachineLegacyCameraEvents.OnCameraLiveEvent();

		private CinemachineVirtualCameraBase m_Vcam;

		[Serializable]
		public class OnCameraLiveEvent : UnityEvent<ICinemachineCamera, ICinemachineCamera>
		{
		}
	}
}
