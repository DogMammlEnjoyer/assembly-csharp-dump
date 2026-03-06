using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[SaveDuringPlay]
	[AddComponentMenu("Cinemachine/Procedural/Extensions/Cinemachine Impulse Listener")]
	[ExecuteAlways]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineImpulseListener.html")]
	public class CinemachineImpulseListener : CinemachineExtension
	{
		private void Reset()
		{
			this.ApplyAfter = CinemachineCore.Stage.Noise;
			this.ChannelMask = 1;
			this.Gain = 1f;
			this.Use2DDistance = false;
			this.UseCameraSpace = true;
			this.SignalCombinationMode = CinemachineImpulseListener.SignalCombinationModes.Additive;
			this.ReactionSettings = new CinemachineImpulseListener.ImpulseReaction
			{
				AmplitudeGain = 1f,
				FrequencyGain = 1f,
				Duration = 1f
			};
		}

		protected override void PostPipelineStageCallback(CinemachineVirtualCameraBase vcam, CinemachineCore.Stage stage, ref CameraState state, float deltaTime)
		{
			if (stage == this.ApplyAfter && deltaTime >= 0f)
			{
				Vector3 vector = Vector3.zero;
				Quaternion quaternion = Quaternion.identity;
				bool flag;
				if (this.SignalCombinationMode == CinemachineImpulseListener.SignalCombinationModes.Additive)
				{
					flag = CinemachineImpulseManager.Instance.GetImpulseAt(state.GetFinalPosition(), this.Use2DDistance, this.ChannelMask, out vector, out quaternion);
				}
				else
				{
					flag = CinemachineImpulseManager.Instance.GetStrongestImpulseAt(state.GetFinalPosition(), this.Use2DDistance, this.ChannelMask, out vector, out quaternion);
				}
				Vector3 b;
				Quaternion rhs;
				bool reaction = this.ReactionSettings.GetReaction(deltaTime, vector, out b, out rhs);
				if (flag)
				{
					quaternion = Quaternion.SlerpUnclamped(Quaternion.identity, quaternion, this.Gain);
					vector *= this.Gain;
				}
				if (reaction)
				{
					vector += b;
					quaternion *= rhs;
				}
				if (flag || reaction)
				{
					if (this.UseCameraSpace)
					{
						vector = state.RawOrientation * vector;
					}
					state.PositionCorrection += vector;
					state.OrientationCorrection *= quaternion;
				}
			}
		}

		[Tooltip("When to apply the impulse reaction.  Default is after the Noise stage.  Modify this if necessary to influence the ordering of extension effects")]
		[FormerlySerializedAs("m_ApplyAfter")]
		public CinemachineCore.Stage ApplyAfter = CinemachineCore.Stage.Aim;

		[Tooltip("Impulse events on channels not included in the mask will be ignored.")]
		[CinemachineImpulseChannelProperty]
		[FormerlySerializedAs("m_ChannelMask")]
		public int ChannelMask;

		[Tooltip("Gain to apply to the Impulse signal.  1 is normal strength.  Setting this to 0 completely mutes the signal.")]
		[FormerlySerializedAs("m_Gain")]
		public float Gain;

		[Tooltip("Enable this to perform distance calculation in 2D (ignore Z)")]
		[FormerlySerializedAs("m_Use2DDistance")]
		public bool Use2DDistance;

		[Tooltip("Enable this to process all impulse signals in camera space")]
		[FormerlySerializedAs("m_UseCameraSpace")]
		public bool UseCameraSpace;

		[Tooltip("Controls how the Impulse Listener combines multiple impulses active at the current point in space.\n\n<b>Additive</b>: Combines all the active signals together, like sound waves.  This is the default.\n\n<b>Use Largest</b>: Considers only the signal with the largest amplitude; ignores any others.")]
		public CinemachineImpulseListener.SignalCombinationModes SignalCombinationMode;

		[Tooltip("This controls the secondary reaction of the listener to the incoming impulse.  The impulse might be for example a sharp shock, and the secondary reaction could be a vibration whose amplitude and duration is controlled by the size of the original impulse.  This allows different listeners to respond in different ways to the same impulse signal.")]
		[FormerlySerializedAs("m_ReactionSettings")]
		public CinemachineImpulseListener.ImpulseReaction ReactionSettings;

		public enum SignalCombinationModes
		{
			Additive,
			UseLargest
		}

		[Serializable]
		public struct ImpulseReaction
		{
			public void ReSeed()
			{
				this.m_NoiseOffsets = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
			}

			public bool GetReaction(float deltaTime, Vector3 impulsePos, out Vector3 pos, out Quaternion rot)
			{
				if (!this.m_Initialized)
				{
					this.m_Initialized = true;
					this.m_CurrentAmount = 0f;
					this.m_CurrentDamping = 0f;
					this.m_CurrentTime = CinemachineCore.CurrentTime * this.FrequencyGain;
					if (this.m_NoiseOffsets == Vector3.zero)
					{
						this.ReSeed();
					}
				}
				pos = Vector3.zero;
				rot = Quaternion.identity;
				float sqrMagnitude = impulsePos.sqrMagnitude;
				if (this.m_SecondaryNoise == null || (sqrMagnitude < 0.001f && this.m_CurrentAmount < 0.0001f))
				{
					return false;
				}
				if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback && TargetPositionCache.HasCurrentTime)
				{
					this.m_CurrentTime = TargetPositionCache.CurrentTime * this.FrequencyGain;
				}
				else
				{
					this.m_CurrentTime += deltaTime * this.FrequencyGain;
				}
				this.m_CurrentAmount = Mathf.Max(this.m_CurrentAmount, Mathf.Sqrt(sqrMagnitude));
				this.m_CurrentDamping = Mathf.Max(this.m_CurrentDamping, Mathf.Max(1f, Mathf.Sqrt(this.m_CurrentAmount)) * this.Duration);
				float d = this.m_CurrentAmount * this.AmplitudeGain;
				pos = NoiseSettings.GetCombinedFilterResults(this.m_SecondaryNoise.PositionNoise, this.m_CurrentTime, this.m_NoiseOffsets) * d;
				rot = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(this.m_SecondaryNoise.OrientationNoise, this.m_CurrentTime, this.m_NoiseOffsets) * d);
				this.m_CurrentAmount -= Damper.Damp(this.m_CurrentAmount, this.m_CurrentDamping, deltaTime);
				this.m_CurrentDamping -= Damper.Damp(this.m_CurrentDamping, this.m_CurrentDamping, deltaTime);
				return true;
			}

			[Tooltip("Secondary shake that will be triggered by the primary impulse.")]
			public NoiseSettings m_SecondaryNoise;

			[Tooltip("Gain to apply to the amplitudes defined in the signal source.  1 is normal.  Setting this to 0 completely mutes the signal.")]
			[FormerlySerializedAs("m_AmplitudeGain")]
			public float AmplitudeGain;

			[Tooltip("Scale factor to apply to the time axis.  1 is normal.  Larger magnitudes will make the signal progress more rapidly.")]
			[FormerlySerializedAs("m_FrequencyGain")]
			public float FrequencyGain;

			[Tooltip("How long the secondary reaction lasts.")]
			[FormerlySerializedAs("m_Duration")]
			public float Duration;

			private float m_CurrentAmount;

			private float m_CurrentTime;

			private float m_CurrentDamping;

			private bool m_Initialized;

			[SerializeField]
			[HideInInspector]
			[NoSaveDuringPlay]
			private Vector3 m_NoiseOffsets;
		}
	}
}
