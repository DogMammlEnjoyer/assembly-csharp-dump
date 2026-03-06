using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Cinemachine
{
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineNoiseProfiles.html")]
	public sealed class NoiseSettings : SignalSourceAsset
	{
		public static Vector3 GetCombinedFilterResults(NoiseSettings.TransformNoiseParams[] noiseParams, float time, Vector3 timeOffsets)
		{
			Vector3 vector = Vector3.zero;
			if (noiseParams != null)
			{
				for (int i = 0; i < noiseParams.Length; i++)
				{
					vector += noiseParams[i].GetValueAt(time, timeOffsets);
				}
			}
			return vector;
		}

		public override float SignalDuration
		{
			get
			{
				return 0f;
			}
		}

		public override void GetSignal(float timeSinceSignalStart, out Vector3 pos, out Quaternion rot)
		{
			pos = NoiseSettings.GetCombinedFilterResults(this.PositionNoise, timeSinceSignalStart, Vector3.zero);
			rot = Quaternion.Euler(NoiseSettings.GetCombinedFilterResults(this.OrientationNoise, timeSinceSignalStart, Vector3.zero));
		}

		[Tooltip("These are the noise channels for the virtual camera's position. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
		[FormerlySerializedAs("m_Position")]
		public NoiseSettings.TransformNoiseParams[] PositionNoise = new NoiseSettings.TransformNoiseParams[0];

		[Tooltip("These are the noise channels for the virtual camera's orientation. Convincing noise setups typically mix low, medium and high frequencies together, so start with a size of 3")]
		[FormerlySerializedAs("m_Orientation")]
		public NoiseSettings.TransformNoiseParams[] OrientationNoise = new NoiseSettings.TransformNoiseParams[0];

		[Serializable]
		public struct NoiseParams
		{
			public float GetValueAt(float time, float timeOffset)
			{
				float num = this.Frequency * time + timeOffset;
				if (this.Constant)
				{
					return Mathf.Cos(num * 2f * 3.1415927f) * this.Amplitude * 0.5f;
				}
				return (Mathf.PerlinNoise(num, 0f) - 0.5f) * this.Amplitude;
			}

			[Tooltip("The frequency of noise for this channel.  Higher magnitudes vibrate faster.")]
			public float Frequency;

			[Tooltip("The amplitude of the noise for this channel.  Larger numbers vibrate higher.")]
			public float Amplitude;

			[Tooltip("If checked, then the amplitude and frequency will not be randomized.")]
			public bool Constant;
		}

		[Serializable]
		public struct TransformNoiseParams
		{
			public Vector3 GetValueAt(float time, Vector3 timeOffsets)
			{
				return new Vector3(this.X.GetValueAt(time, timeOffsets.x), this.Y.GetValueAt(time, timeOffsets.y), this.Z.GetValueAt(time, timeOffsets.z));
			}

			[Tooltip("Noise definition for X-axis")]
			public NoiseSettings.NoiseParams X;

			[Tooltip("Noise definition for Y-axis")]
			public NoiseSettings.NoiseParams Y;

			[Tooltip("Noise definition for Z-axis")]
			public NoiseSettings.NoiseParams Z;
		}
	}
}
