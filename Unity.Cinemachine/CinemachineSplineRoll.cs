using System;
using UnityEngine;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[ExecuteInEditMode]
	[DisallowMultipleComponent]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Spline Roll")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSplineRoll.html")]
	[SaveDuringPlay]
	public class CinemachineSplineRoll : MonoBehaviour, ISerializationCallbackReceiver
	{
		public IInterpolator<CinemachineSplineRoll.RollData> GetInterpolator()
		{
			if (!this.Easing)
			{
				return default(CinemachineSplineRoll.LerpRollData);
			}
			return default(CinemachineSplineRoll.LerpRollDataWithEasing);
		}

		private void PerformLegacyUpgrade(int streamedVersion)
		{
			if (streamedVersion < 20240101)
			{
				for (int i = 0; i < this.Roll.Count; i++)
				{
					DataPoint<CinemachineSplineRoll.RollData> value = this.Roll[i];
					value.Value = -value.Value;
					this.Roll[i] = value;
				}
			}
		}

		private void Reset()
		{
			SplineData<CinemachineSplineRoll.RollData> roll = this.Roll;
			if (roll != null)
			{
				roll.Clear();
			}
			this.Easing = true;
		}

		private void OnEnable()
		{
		}

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			if (this.m_StreamingVersion < 20241001)
			{
				this.PerformLegacyUpgrade(this.m_StreamingVersion);
			}
			this.m_StreamingVersion = 20241001;
		}

		[Tooltip("When enabled, roll eases into and out of the data point values.  Otherwise, interpolation is linear.")]
		public bool Easing = true;

		[HideFoldout]
		public SplineData<CinemachineSplineRoll.RollData> Roll;

		[HideInInspector]
		[SerializeField]
		[NoSaveDuringPlay]
		private int m_StreamingVersion;

		[Serializable]
		public struct RollData
		{
			public static implicit operator float(CinemachineSplineRoll.RollData roll)
			{
				return roll.Value;
			}

			public static implicit operator CinemachineSplineRoll.RollData(float roll)
			{
				return new CinemachineSplineRoll.RollData
				{
					Value = roll
				};
			}

			[Tooltip("Roll (in degrees) around the forward direction for specific location on the track.\n- When placed on a SplineContainer, this is going to be a global override that affects all vcams using the Spline.\n- When placed on a CinemachineCamera, this is going to be a local override that only affects that CinemachineCamera.")]
			public float Value;
		}

		public struct LerpRollData : IInterpolator<CinemachineSplineRoll.RollData>
		{
			public CinemachineSplineRoll.RollData Interpolate(CinemachineSplineRoll.RollData a, CinemachineSplineRoll.RollData b, float t)
			{
				return new CinemachineSplineRoll.RollData
				{
					Value = Mathf.Lerp(a.Value, b.Value, t)
				};
			}
		}

		public struct LerpRollDataWithEasing : IInterpolator<CinemachineSplineRoll.RollData>
		{
			public CinemachineSplineRoll.RollData Interpolate(CinemachineSplineRoll.RollData a, CinemachineSplineRoll.RollData b, float t)
			{
				float num = t * t;
				float num2 = 1f - t;
				t = 3f * num2 * num + t * num;
				return new CinemachineSplineRoll.RollData
				{
					Value = Mathf.Lerp(a.Value, b.Value, t)
				};
			}
		}

		internal struct RollCache
		{
			public void Refresh(MonoBehaviour owner)
			{
				this.m_RollCache = null;
				if (!owner.TryGetComponent<CinemachineSplineRoll>(out this.m_RollCache))
				{
					ISplineReferencer splineReferencer = owner as ISplineReferencer;
					if (splineReferencer != null)
					{
						SplineContainer spline = splineReferencer.SplineSettings.Spline;
						if (spline != null)
						{
							Component component = spline;
							if (component != null)
							{
								component.TryGetComponent<CinemachineSplineRoll>(out this.m_RollCache);
							}
						}
					}
				}
			}

			public CinemachineSplineRoll GetSplineRoll(MonoBehaviour owner)
			{
				return this.m_RollCache;
			}

			private CinemachineSplineRoll m_RollCache;
		}
	}
}
