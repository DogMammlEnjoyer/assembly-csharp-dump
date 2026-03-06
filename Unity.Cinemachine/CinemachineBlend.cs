using System;
using System.Text;
using UnityEngine;

namespace Unity.Cinemachine
{
	public class CinemachineBlend
	{
		public float BlendWeight
		{
			get
			{
				if (this.BlendCurve == null || this.BlendCurve.length < 2 || this.IsComplete)
				{
					return 1f;
				}
				return Mathf.Clamp01(this.BlendCurve.Evaluate(this.TimeInBlend / this.Duration));
			}
		}

		public CinemachineBlend.IBlender CustomBlender { get; set; }

		public bool IsValid
		{
			get
			{
				return (this.CamA != null && this.CamA.IsValid) || (this.CamB != null && this.CamB.IsValid);
			}
		}

		public bool IsComplete
		{
			get
			{
				return this.TimeInBlend >= this.Duration || !this.IsValid;
			}
		}

		public string Description
		{
			get
			{
				if (this.CamB == null || !this.CamB.IsValid)
				{
					return "(none)";
				}
				StringBuilder stringBuilder = CinemachineDebug.SBFromPool();
				stringBuilder.Append(this.CamB.Name);
				stringBuilder.Append(" ");
				stringBuilder.Append((int)(this.BlendWeight * 100f));
				stringBuilder.Append("% from ");
				if (this.CamA == null || !this.CamA.IsValid)
				{
					stringBuilder.Append("(none)");
				}
				else
				{
					stringBuilder.Append(this.CamA.Name);
				}
				string result = stringBuilder.ToString();
				CinemachineDebug.ReturnToPool(stringBuilder);
				return result;
			}
		}

		public bool Uses(ICinemachineCamera cam)
		{
			if (cam == null)
			{
				return false;
			}
			if (cam == this.CamA || cam == this.CamB)
			{
				return true;
			}
			NestedBlendSource nestedBlendSource = this.CamA as NestedBlendSource;
			if (nestedBlendSource != null && nestedBlendSource.Blend.Uses(cam))
			{
				return true;
			}
			nestedBlendSource = (this.CamB as NestedBlendSource);
			return nestedBlendSource != null && nestedBlendSource.Blend.Uses(cam);
		}

		public void CopyFrom(CinemachineBlend src)
		{
			this.CamA = src.CamA;
			this.CamB = src.CamB;
			this.BlendCurve = src.BlendCurve;
			this.TimeInBlend = src.TimeInBlend;
			this.Duration = src.Duration;
			this.CustomBlender = src.CustomBlender;
		}

		public void ClearBlend()
		{
			this.CamA = null;
			this.BlendCurve = null;
			this.TimeInBlend = (this.Duration = 0f);
			this.CustomBlender = null;
		}

		public void UpdateCameraState(Vector3 worldUp, float deltaTime)
		{
			if (this.CamA != null && this.CamA.IsValid)
			{
				this.CamA.UpdateCameraState(worldUp, deltaTime);
			}
			if (this.CamB != null && this.CamB.IsValid)
			{
				this.CamB.UpdateCameraState(worldUp, deltaTime);
			}
		}

		public CameraState State
		{
			get
			{
				if (this.CamA == null || !this.CamA.IsValid)
				{
					if (this.CamB == null || !this.CamB.IsValid)
					{
						return CameraState.Default;
					}
					return this.CamB.State;
				}
				else
				{
					if (this.CamB == null || !this.CamB.IsValid)
					{
						return this.CamA.State;
					}
					if (this.CustomBlender != null)
					{
						return this.CustomBlender.GetIntermediateState(this.CamA, this.CamB, this.BlendWeight);
					}
					CameraState state = this.CamA.State;
					CameraState state2 = this.CamB.State;
					return CameraState.Lerp(state, state2, this.BlendWeight);
				}
			}
		}

		public ICinemachineCamera CamA;

		public ICinemachineCamera CamB;

		public AnimationCurve BlendCurve;

		public float TimeInBlend;

		public float Duration;

		public interface IBlender
		{
			CameraState GetIntermediateState(ICinemachineCamera CamA, ICinemachineCamera CamB, float t);
		}
	}
}
