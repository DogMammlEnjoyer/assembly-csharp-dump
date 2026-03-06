using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace Oculus.Interaction
{
	public class PokeInteractable : PointerInteractable<PokeInteractor, PokeInteractable>
	{
		public ISurfacePatch SurfacePatch { get; private set; }

		public float EnterHoverNormal
		{
			get
			{
				return this._enterHoverNormal;
			}
			set
			{
				this._enterHoverNormal = value;
			}
		}

		public float EnterHoverTangent
		{
			get
			{
				return this._enterHoverTangent;
			}
			set
			{
				this._enterHoverTangent = value;
			}
		}

		public float ExitHoverNormal
		{
			get
			{
				return this._exitHoverNormal;
			}
			set
			{
				this._exitHoverNormal = value;
			}
		}

		public float ExitHoverTangent
		{
			get
			{
				return this._exitHoverTangent;
			}
			set
			{
				this._exitHoverTangent = value;
			}
		}

		public float CancelSelectNormal
		{
			get
			{
				return this._cancelSelectNormal;
			}
			set
			{
				this._cancelSelectNormal = value;
			}
		}

		public float CancelSelectTangent
		{
			get
			{
				return this._cancelSelectTangent;
			}
			set
			{
				this._cancelSelectTangent = value;
			}
		}

		public float CloseDistanceThreshold
		{
			get
			{
				return this._closeDistanceThreshold;
			}
			set
			{
				this._closeDistanceThreshold = value;
			}
		}

		public int TiebreakerScore
		{
			get
			{
				return this._tiebreakerScore;
			}
			set
			{
				this._tiebreakerScore = value;
			}
		}

		public PokeInteractable.MinThresholdsConfig MinThresholds
		{
			get
			{
				return this._minThresholds;
			}
			set
			{
				this._minThresholds = value;
			}
		}

		public PokeInteractable.DragThresholdsConfig DragThresholds
		{
			get
			{
				return this._dragThresholds;
			}
			set
			{
				this._dragThresholds = value;
			}
		}

		public PokeInteractable.PositionPinningConfig PositionPinning
		{
			get
			{
				return this._positionPinning;
			}
			set
			{
				this._positionPinning = value;
			}
		}

		public PokeInteractable.RecoilAssistConfig RecoilAssist
		{
			get
			{
				return this._recoilAssist;
			}
			set
			{
				this._recoilAssist = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.SurfacePatch = (this._surfacePatch as ISurfacePatch);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this._exitHoverNormal = Mathf.Max(this._enterHoverNormal, this._exitHoverNormal);
			this._exitHoverTangent = Mathf.Max(this._enterHoverTangent, this._exitHoverTangent);
			if (this._cancelSelectTangent > 0f)
			{
				this._cancelSelectTangent = Mathf.Max(this._exitHoverTangent, this._cancelSelectTangent);
			}
			if (this._minThresholds.Enabled && this._minThresholds.MinNormal > 0f)
			{
				this._minThresholds.MinNormal = Mathf.Min(this._minThresholds.MinNormal, this._enterHoverNormal);
			}
			this.EndStart(ref this._started);
		}

		public bool ClosestSurfacePatchHit(Vector3 point, out SurfaceHit hit)
		{
			return this.SurfacePatch.ClosestSurfacePoint(point, out hit, 0f);
		}

		public bool ClosestBackingSurfaceHit(Vector3 point, out SurfaceHit hit)
		{
			return this.SurfacePatch.BackingSurface.ClosestSurfacePoint(point, out hit, 0f);
		}

		private void Reset()
		{
			this._minThresholds.Enabled = false;
			this._dragThresholds.Enabled = false;
			this._positionPinning.Enabled = true;
			this._recoilAssist.Enabled = true;
			this._recoilAssist.UseVelocityExpansion = true;
			this._recoilAssist.UseDynamicDecay = true;
		}

		public void InjectAllPokeInteractable(ISurfacePatch surfacePatch)
		{
			this.InjectSurfacePatch(surfacePatch);
		}

		public void InjectSurfacePatch(ISurfacePatch surfacePatch)
		{
			this._surfacePatch = (surfacePatch as Object);
			this.SurfacePatch = surfacePatch;
		}

		[Tooltip("Represents the pokeable surface area of this interactable.")]
		[SerializeField]
		[Interface(typeof(ISurfacePatch), new Type[]
		{

		})]
		private Object _surfacePatch;

		[SerializeField]
		[FormerlySerializedAs("_maxDistance")]
		[Tooltip("The distance required for a poke interactor to enter hovering, measured along the normal to the surface (in meters)")]
		private float _enterHoverNormal = 0.15f;

		[SerializeField]
		[Tooltip("The distance required for a poke interactor to enter hovering, measured along the tangent plane to the surface (in meters)")]
		private float _enterHoverTangent;

		[SerializeField]
		[Tooltip("The distance required for a poke interactor to exit hovering, measured along the normal to the surface (in meters)")]
		private float _exitHoverNormal = 0.2f;

		[SerializeField]
		[Tooltip("The distance required for a poke interactor to exit hovering, measured along the tangent plane to the surface (in meters)")]
		private float _exitHoverTangent;

		[SerializeField]
		[FormerlySerializedAs("_releaseDistance")]
		[Tooltip("If greater than zero, the distance required for a selecting poke interactor to cancel selection, measured along the negative normal to the surface (in meters).")]
		private float _cancelSelectNormal = 0.3f;

		[SerializeField]
		[Tooltip("If greater than zero, the distance required for a selecting poke interactor to cancel selection, measured along the tangent plane to the surface (in meters).")]
		private float _cancelSelectTangent = 0.03f;

		[SerializeField]
		[Tooltip("If enabled, a poke interactor must approach the surface from at least a minimum distance of the surface (in meters).")]
		private PokeInteractable.MinThresholdsConfig _minThresholds = new PokeInteractable.MinThresholdsConfig
		{
			Enabled = false,
			MinNormal = 0.01f
		};

		[SerializeField]
		[FormerlySerializedAs("_dragThresholding")]
		[Tooltip("If enabled, drag thresholds will be applied in 3D space. Useful for disambiguating press vs drag and suppressing move pointer events when a poke interactor follows a pressing motion.")]
		private PokeInteractable.DragThresholdsConfig _dragThresholds = new PokeInteractable.DragThresholdsConfig
		{
			Enabled = true,
			DragNormal = 0.01f,
			DragTangent = 0.01f,
			DragEaseCurve = new ProgressCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.05f)
		};

		[SerializeField]
		[Tooltip("If enabled, position pinning will be applied to surface motion during drag. Useful for adding a sense of friction to initial drag motion.")]
		private PokeInteractable.PositionPinningConfig _positionPinning = new PokeInteractable.PositionPinningConfig
		{
			Enabled = false,
			MaxPinDistance = 0.075f,
			PinningEaseCurve = AnimationCurve.EaseInOut(0.2f, 0f, 1f, 1f),
			ResyncCurve = new ProgressCurve(AnimationCurve.EaseInOut(0f, 0f, 1f, 1f), 0.2f)
		};

		[SerializeField]
		[Tooltip("If enabled, recoil assist will affect unselection and reselection criteria. Useful for triggering unselect in response to a smaller motion in the negative direction from a surface.")]
		private PokeInteractable.RecoilAssistConfig _recoilAssist = new PokeInteractable.RecoilAssistConfig
		{
			Enabled = false,
			UseDynamicDecay = false,
			DynamicDecayCurve = new AnimationCurve(new Keyframe[]
			{
				new Keyframe(0f, 50f),
				new Keyframe(0.9f, 0.5f, -47f, -47f)
			}),
			UseVelocityExpansion = false,
			VelocityExpansionMinSpeed = 0.4f,
			VelocityExpansionMaxSpeed = 1.4f,
			VelocityExpansionDistance = 0.055f,
			VelocityExpansionDecayRate = 0.125f,
			ExitDistance = 0.02f,
			ReEnterDistance = 0.02f
		};

		[SerializeField]
		[Optional]
		[Tooltip("(Meters, World) The threshold below which distances near this surface are treated as equal in depth for the purposes of ranking.")]
		private float _closeDistanceThreshold = 0.001f;

		[SerializeField]
		[Optional]
		private int _tiebreakerScore;

		[Serializable]
		public class MinThresholdsConfig
		{
			[Tooltip("If true, minimum thresholds will be applied.")]
			public bool Enabled;

			[Tooltip("The minimum distance required for a poke interactor to surpass before being able to hover, measured along the normal to the surface (in meters).")]
			public float MinNormal = 0.01f;
		}

		[Serializable]
		public class DragThresholdsConfig
		{
			[Tooltip("If true, drag thresholds will be applied.")]
			public bool Enabled;

			[FormerlySerializedAs("ZThreshold")]
			[Tooltip("The distance a poke interactor must travel to be treated as a press, measured as a distance along the normal to the surface (in meters).")]
			public float DragNormal;

			[FormerlySerializedAs("SurfaceThreshold")]
			[Tooltip("The distance a poke interactor must travel to be treated as a drag, measured as a distance along the tangent plane to the surface (in meters).")]
			public float DragTangent;

			[Tooltip("The curve that a poke interactor will use to ease when transitioning between a press and drag state.")]
			public ProgressCurve DragEaseCurve;
		}

		[Serializable]
		public class PositionPinningConfig
		{
			[Tooltip("If true, position pinning will be applied.")]
			public bool Enabled;

			[Tooltip("The distance over which a poke interactor drag motion will be remapped measured along the tangent plane to the surface (in meters)")]
			public float MaxPinDistance;

			[Tooltip("The poke interactor position will be remapped along this curve from the initial touch point to the current position on surface.")]
			public AnimationCurve PinningEaseCurve;

			[Tooltip("In cases where a resync is necessary between the pinned position and the unpinned position, this time-based curve will be used.")]
			public ProgressCurve ResyncCurve;
		}

		[Serializable]
		public class RecoilAssistConfig
		{
			[Tooltip("If true, recoil assist will be applied.")]
			public bool Enabled;

			[Tooltip("If true, DynamicDecayCurve will be used to decay the max distance based on the normal velocity.")]
			public bool UseDynamicDecay;

			[Tooltip("A function of the normal movement ratio to determine the rate of decay.")]
			public AnimationCurve DynamicDecayCurve;

			[Tooltip("Expand recoil window when fast Z motion is detected.")]
			public bool UseVelocityExpansion;

			[Tooltip("When average velocity in interactable Z is greater than min speed, the recoil window will begin expanding.")]
			public float VelocityExpansionMinSpeed;

			[Tooltip("Full recoil window expansion reached at this speed.")]
			public float VelocityExpansionMaxSpeed;

			[Tooltip("Window will expand by this distance when Z velocity reaches max speed.")]
			public float VelocityExpansionDistance;

			[Tooltip("Window will contract toward ExitDistance at this rate (in meters) per second when velocity lowers.")]
			public float VelocityExpansionDecayRate;

			[Tooltip("The distance over which a poke interactor must surpass to trigger an early unselect, measured along the normal to the surface (in meters)")]
			public float ExitDistance;

			[Tooltip("When in recoil, the distance which a poke interactor must surpass to trigger a subsequent select, measured along the negative normal to the surface (in meters)")]
			public float ReEnterDistance;
		}
	}
}
