using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public class NestedBlendSource : ICinemachineCamera
	{
		public NestedBlendSource(CinemachineBlend blend)
		{
			this.Blend = blend;
		}

		public CinemachineBlend Blend { get; internal set; }

		public string Name
		{
			get
			{
				if (this.m_Name == null)
				{
					this.m_Name = ((this.Blend == null || this.Blend.CamB == null) ? "(null)" : ("mid-blend to " + this.Blend.CamB.Name));
				}
				return this.m_Name;
			}
		}

		public string Description
		{
			get
			{
				if (this.Blend != null)
				{
					return this.Blend.Description;
				}
				return "(null)";
			}
		}

		public CameraState State { get; private set; }

		public bool IsValid
		{
			get
			{
				return this.Blend != null && this.Blend.IsValid;
			}
		}

		public ICinemachineMixer ParentCamera
		{
			get
			{
				return null;
			}
		}

		public void UpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			if (this.Blend != null)
			{
				this.Blend.UpdateCameraState(worldUp, deltaTime);
				this.State = this.Blend.State;
			}
		}

		public void OnCameraActivated(ICinemachineCamera.ActivationEventParams evt)
		{
		}

		private string m_Name;
	}
}
