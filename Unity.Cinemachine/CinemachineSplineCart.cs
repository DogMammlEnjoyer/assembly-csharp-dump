using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace Unity.Cinemachine
{
	[ExecuteAlways]
	[DisallowMultipleComponent]
	[AddComponentMenu("Cinemachine/Helpers/Cinemachine Spline Cart")]
	[HelpURL("https://docs.unity3d.com/Packages/com.unity.cinemachine@3.1/manual/CinemachineSplineCart.html")]
	public class CinemachineSplineCart : MonoBehaviour, ISplineReferencer
	{
		public ref SplineSettings SplineSettings
		{
			get
			{
				return ref this.m_SplineSettings;
			}
		}

		public SplineContainer Spline
		{
			get
			{
				return this.m_SplineSettings.Spline;
			}
			set
			{
				this.m_SplineSettings.Spline = value;
			}
		}

		public float SplinePosition
		{
			get
			{
				return this.m_SplineSettings.Position;
			}
			set
			{
				this.m_SplineSettings.Position = value;
			}
		}

		public PathIndexUnit PositionUnits
		{
			get
			{
				return this.m_SplineSettings.Units;
			}
			set
			{
				this.m_SplineSettings.ChangeUnitPreservePosition(value);
			}
		}

		private void PerformLegacyUpgrade()
		{
			if (this.m_LegacyPosition != -1f)
			{
				this.m_SplineSettings.Position = this.m_LegacyPosition;
				this.m_SplineSettings.Units = this.m_LegacyUnits;
				this.m_LegacyPosition = -1f;
				this.m_LegacyUnits = PathIndexUnit.Distance;
			}
			if (this.m_LegacySpline != null)
			{
				this.m_SplineSettings.Spline = this.m_LegacySpline;
				this.m_LegacySpline = null;
			}
		}

		private void OnValidate()
		{
			this.PerformLegacyUpgrade();
			SplineAutoDolly.ISplineAutoDolly method = this.AutomaticDolly.Method;
			if (method == null)
			{
				return;
			}
			method.Validate();
		}

		private void Reset()
		{
			this.m_SplineSettings = new SplineSettings
			{
				Units = PathIndexUnit.Normalized
			};
			this.UpdateMethod = CinemachineSplineCart.UpdateMethods.Update;
			this.AutomaticDolly.Method = null;
			this.TrackingTarget = null;
		}

		private void OnEnable()
		{
			this.m_RollCache.Refresh(this);
			SplineAutoDolly.ISplineAutoDolly method = this.AutomaticDolly.Method;
			if (method == null)
			{
				return;
			}
			method.Reset();
		}

		private void OnDisable()
		{
			this.SplineSettings.InvalidateCache();
		}

		private void FixedUpdate()
		{
			if (this.UpdateMethod == CinemachineSplineCart.UpdateMethods.FixedUpdate)
			{
				this.UpdateCartPosition();
			}
		}

		private void Update()
		{
			if (!Application.isPlaying)
			{
				this.SetCartPosition(this.SplinePosition);
				return;
			}
			if (this.UpdateMethod == CinemachineSplineCart.UpdateMethods.Update)
			{
				this.UpdateCartPosition();
			}
		}

		private void LateUpdate()
		{
			if (!Application.isPlaying)
			{
				this.SetCartPosition(this.SplinePosition);
				return;
			}
			if (this.UpdateMethod == CinemachineSplineCart.UpdateMethods.LateUpdate)
			{
				this.UpdateCartPosition();
			}
		}

		private void UpdateCartPosition()
		{
			if (this.AutomaticDolly.Enabled && this.AutomaticDolly.Method != null)
			{
				this.SplinePosition = this.AutomaticDolly.Method.GetSplinePosition(this, this.TrackingTarget, this.Spline, this.SplinePosition, this.PositionUnits, Time.deltaTime);
			}
			this.SetCartPosition(this.SplinePosition);
		}

		private void SetCartPosition(float distanceAlongPath)
		{
			CachedScaledSpline cachedSpline = this.m_SplineSettings.GetCachedSpline();
			if (cachedSpline != null)
			{
				Spline spline = this.Spline.Splines[0];
				float num;
				this.SplinePosition = cachedSpline.StandardizePosition(distanceAlongPath, this.PositionUnits, out num);
				float tNormalized = spline.ConvertIndexUnit(this.SplinePosition, this.PositionUnits, PathIndexUnit.Normalized);
				Vector3 pos;
				Quaternion rot;
				cachedSpline.EvaluateSplineWithRoll(this.Spline.transform, tNormalized, this.m_RollCache.GetSplineRoll(this), out pos, out rot);
				base.transform.ConservativeSetPositionAndRotation(pos, rot);
			}
		}

		[SerializeField]
		[FormerlySerializedAs("SplineSettings")]
		private SplineSettings m_SplineSettings = new SplineSettings
		{
			Units = PathIndexUnit.Normalized
		};

		[Tooltip("When to move the cart, if Speed is non-zero")]
		public CinemachineSplineCart.UpdateMethods UpdateMethod;

		[FoldoutWithEnabledButton("Enabled")]
		[Tooltip("Controls how automatic dollying occurs.  A tracking target may be necessary to use this feature.")]
		public SplineAutoDolly AutomaticDolly;

		[Tooltip("Used only by Automatic Dolly settings that require it")]
		public Transform TrackingTarget;

		private CinemachineSplineRoll.RollCache m_RollCache;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("SplinePosition")]
		private float m_LegacyPosition = -1f;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("PositionUnits")]
		private PathIndexUnit m_LegacyUnits;

		[SerializeField]
		[HideInInspector]
		[NoSaveDuringPlay]
		[FormerlySerializedAs("Spline")]
		private SplineContainer m_LegacySpline;

		public enum UpdateMethods
		{
			Update,
			FixedUpdate,
			LateUpdate
		}
	}
}
