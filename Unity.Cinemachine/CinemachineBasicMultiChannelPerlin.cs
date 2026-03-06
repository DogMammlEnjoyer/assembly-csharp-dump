using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[AddComponentMenu("Cinemachine/Procedural/Noise/Cinemachine Basic Multi Channel Perlin")]
	[SaveDuringPlay]
	[DisallowMultipleComponent]
	[CameraPipeline(CinemachineCore.Stage.Noise)]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineBasicMultiChannelPerlin.html")]
	public class CinemachineBasicMultiChannelPerlin : CinemachineComponentBase, CinemachineFreeLookModifier.IModifiableNoise
	{
		ValueTuple<float, float> CinemachineFreeLookModifier.IModifiableNoise.NoiseAmplitudeFrequency
		{
			get
			{
				return new ValueTuple<float, float>(this.AmplitudeGain, this.FrequencyGain);
			}
			set
			{
				this.AmplitudeGain = value.Item1;
				this.FrequencyGain = value.Item2;
			}
		}

		public override bool IsValid
		{
			get
			{
				return base.enabled && this.NoiseProfile != null;
			}
		}

		public override CinemachineCore.Stage Stage
		{
			get
			{
				return CinemachineCore.Stage.Noise;
			}
		}

		public override void MutateCameraState(ref CameraState curState, float deltaTime)
		{
			if (!this.IsValid || deltaTime < 0f)
			{
				this.m_Initialized = false;
				return;
			}
			if (!this.m_Initialized)
			{
				this.Initialize();
			}
			if (TargetPositionCache.CacheMode == TargetPositionCache.Mode.Playback && TargetPositionCache.HasCurrentTime)
			{
				this.m_NoiseTime = TargetPositionCache.CurrentTime * this.FrequencyGain;
			}
			else
			{
				this.m_NoiseTime += deltaTime * this.FrequencyGain;
			}
			curState.PositionCorrection += curState.GetCorrectedOrientation() * NoiseSettings.GetCombinedFilterResults(this.NoiseProfile.PositionNoise, this.m_NoiseTime, this.m_NoiseOffsets) * this.AmplitudeGain;
			Quaternion quaternion = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(this.NoiseProfile.OrientationNoise, this.m_NoiseTime, this.m_NoiseOffsets) * this.AmplitudeGain);
			if (this.PivotOffset != Vector3.zero)
			{
				Matrix4x4 rhs = Matrix4x4.Translate(-this.PivotOffset);
				rhs = Matrix4x4.Rotate(quaternion) * rhs;
				rhs = Matrix4x4.Translate(this.PivotOffset) * rhs;
				curState.PositionCorrection += curState.GetCorrectedOrientation() * rhs.MultiplyPoint(Vector3.zero);
			}
			curState.OrientationCorrection *= quaternion;
		}

		public void ReSeed()
		{
			this.m_NoiseOffsets = new Vector3(Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f), Random.Range(-1000f, 1000f));
		}

		private void Initialize()
		{
			this.m_Initialized = true;
			this.m_NoiseTime = CinemachineCore.CurrentTime * this.FrequencyGain;
			if (this.m_NoiseOffsets == Vector3.zero)
			{
				this.ReSeed();
			}
		}

		[Tooltip("The asset containing the Noise Profile.  Define the frequencies and amplitudes there to make a characteristic noise profile.  Make your own or just use one of the many presets.")]
		[FormerlySerializedAs("m_Definition")]
		[FormerlySerializedAs("m_NoiseProfile")]
		public NoiseSettings NoiseProfile;

		[Tooltip("When rotating the camera, offset the camera's pivot position by this much (camera space)")]
		[FormerlySerializedAs("m_PivotOffset")]
		public Vector3 PivotOffset = Vector3.zero;

		[Tooltip("Gain to apply to the amplitudes defined in the NoiseSettings asset.  1 is normal.  Setting this to 0 completely mutes the noise.")]
		[FormerlySerializedAs("m_AmplitudeGain")]
		public float AmplitudeGain = 1f;

		[Tooltip("Scale factor to apply to the frequencies defined in the NoiseSettings asset.  1 is normal.  Larger magnitudes will make the noise shake more rapidly.")]
		[FormerlySerializedAs("m_FrequencyGain")]
		public float FrequencyGain = 1f;

		private bool m_Initialized;

		private float m_NoiseTime;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("mNoiseOffsets")]
		private Vector3 m_NoiseOffsets = Vector3.zero;
	}
}
