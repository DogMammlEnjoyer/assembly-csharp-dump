using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;

namespace Unity.Cinemachine
{
	internal class BlendManager : CameraBlendStack
	{
		public override void OnEnable()
		{
			base.OnEnable();
			this.m_PreviousLiveCameras.Clear();
			this.m_PreviousActiveCamera = null;
			this.m_WasBlending = false;
		}

		public ICinemachineCamera ActiveVirtualCamera
		{
			get
			{
				return BlendManager.DeepCamBFromBlend(this.m_CurrentLiveCameras);
			}
		}

		private static ICinemachineCamera DeepCamBFromBlend(CinemachineBlend blend)
		{
			ICinemachineCamera cinemachineCamera = (blend != null) ? blend.CamB : null;
			for (;;)
			{
				NestedBlendSource nestedBlendSource = cinemachineCamera as NestedBlendSource;
				if (nestedBlendSource == null)
				{
					break;
				}
				cinemachineCamera = nestedBlendSource.Blend.CamB;
			}
			if (cinemachineCamera != null && cinemachineCamera.IsValid)
			{
				return cinemachineCamera;
			}
			return null;
		}

		public CinemachineBlend ActiveBlend
		{
			get
			{
				if (this.m_CurrentLiveCameras.CamA == null || this.m_CurrentLiveCameras.IsComplete)
				{
					return null;
				}
				return this.m_CurrentLiveCameras;
			}
			set
			{
				base.SetRootBlend(value);
			}
		}

		public bool IsBlending
		{
			get
			{
				return this.ActiveBlend != null;
			}
		}

		public string Description
		{
			get
			{
				if (this.ActiveVirtualCamera == null)
				{
					return "[(none)]";
				}
				StringBuilder stringBuilder = CinemachineDebug.SBFromPool();
				stringBuilder.Append("[");
				stringBuilder.Append(this.IsBlending ? this.ActiveBlend.Description : this.ActiveVirtualCamera.Name);
				stringBuilder.Append("]");
				string result = stringBuilder.ToString();
				CinemachineDebug.ReturnToPool(stringBuilder);
				return result;
			}
		}

		public bool IsLiveInBlend(ICinemachineCamera cam)
		{
			if (cam != null)
			{
				if (cam == this.m_CurrentLiveCameras.CamA)
				{
					return true;
				}
				NestedBlendSource nestedBlendSource = this.m_CurrentLiveCameras.CamA as NestedBlendSource;
				if (nestedBlendSource != null && nestedBlendSource.Blend.Uses(cam))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsLive(ICinemachineCamera cam)
		{
			return this.m_CurrentLiveCameras.Uses(cam);
		}

		public CameraState CameraState
		{
			get
			{
				return this.m_CurrentLiveCameras.State;
			}
		}

		public void ComputeCurrentBlend()
		{
			base.ProcessOverrideFrames(ref this.m_CurrentLiveCameras, 0);
		}

		public void RefreshCurrentCameraState(Vector3 up, float deltaTime)
		{
			this.m_CurrentLiveCameras.UpdateCameraState(up, deltaTime);
		}

		public ICinemachineCamera ProcessActiveCamera(ICinemachineMixer mixer, Vector3 up, float deltaTime)
		{
			foreach (ICinemachineCamera cinemachineCamera in this.m_PreviousLiveCameras)
			{
				if (!this.IsLive(cinemachineCamera))
				{
					CinemachineCore.CameraDeactivatedEvent.Invoke(mixer, cinemachineCamera);
				}
			}
			ICinemachineCamera activeVirtualCamera = this.ActiveVirtualCamera;
			if (activeVirtualCamera != null && activeVirtualCamera.IsValid)
			{
				ICinemachineCamera cinemachineCamera2 = this.m_PreviousActiveCamera;
				if (cinemachineCamera2 != null && !cinemachineCamera2.IsValid)
				{
					cinemachineCamera2 = null;
				}
				if (activeVirtualCamera == cinemachineCamera2)
				{
					if (this.m_WasBlending && this.m_CurrentLiveCameras.CamA == null)
					{
						CinemachineCore.BlendFinishedEvent.Invoke(mixer, activeVirtualCamera);
					}
				}
				else
				{
					ICinemachineCamera cinemachineCamera3 = cinemachineCamera2;
					if (this.IsBlending)
					{
						cinemachineCamera3 = new NestedBlendSource(this.ActiveBlend);
						cinemachineCamera3.UpdateCameraState(up, deltaTime);
					}
					ICinemachineCamera.ActivationEventParams activationEventParams = new ICinemachineCamera.ActivationEventParams
					{
						Origin = mixer,
						OutgoingCamera = cinemachineCamera3,
						IncomingCamera = activeVirtualCamera,
						IsCut = !this.IsBlending,
						WorldUp = up,
						DeltaTime = deltaTime
					};
					activeVirtualCamera.OnCameraActivated(activationEventParams);
					mixer.OnCameraActivated(activationEventParams);
					CinemachineCore.CameraActivatedEvent.Invoke(activationEventParams);
					activeVirtualCamera.UpdateCameraState(up, deltaTime);
				}
			}
			this.m_PreviousLiveCameras.Clear();
			BlendManager.<ProcessActiveCamera>g__CollectLiveCameras|21_0(this.m_CurrentLiveCameras, ref this.m_PreviousLiveCameras);
			this.m_PreviousActiveCamera = BlendManager.DeepCamBFromBlend(this.m_CurrentLiveCameras);
			this.m_WasBlending = (this.m_CurrentLiveCameras.CamA != null);
			return activeVirtualCamera;
		}

		[CompilerGenerated]
		internal static void <ProcessActiveCamera>g__CollectLiveCameras|21_0(CinemachineBlend blend, ref HashSet<ICinemachineCamera> cams)
		{
			NestedBlendSource nestedBlendSource = blend.CamA as NestedBlendSource;
			if (nestedBlendSource != null && nestedBlendSource.Blend != null)
			{
				BlendManager.<ProcessActiveCamera>g__CollectLiveCameras|21_0(nestedBlendSource.Blend, ref cams);
			}
			else if (blend.CamA != null)
			{
				cams.Add(blend.CamA);
			}
			NestedBlendSource nestedBlendSource2 = blend.CamB as NestedBlendSource;
			if (nestedBlendSource2 != null && nestedBlendSource2.Blend != null)
			{
				BlendManager.<ProcessActiveCamera>g__CollectLiveCameras|21_0(nestedBlendSource2.Blend, ref cams);
				return;
			}
			if (blend.CamB != null)
			{
				cams.Add(blend.CamB);
			}
		}

		private CinemachineBlend m_CurrentLiveCameras = new CinemachineBlend();

		private HashSet<ICinemachineCamera> m_PreviousLiveCameras = new HashSet<ICinemachineCamera>();

		private ICinemachineCamera m_PreviousActiveCamera;

		private bool m_WasBlending;
	}
}
