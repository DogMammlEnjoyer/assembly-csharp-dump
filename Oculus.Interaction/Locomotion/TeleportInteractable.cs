using System;
using System.Runtime.CompilerServices;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class TeleportInteractable : Interactable<TeleportInteractor, TeleportInteractable>
	{
		public bool AllowTeleport
		{
			get
			{
				return this._allowTeleport;
			}
			set
			{
				this._allowTeleport = value;
			}
		}

		public float EqualDistanceToBlockerOverride
		{
			get
			{
				return this._equalDistanceToBlockerOverride;
			}
			set
			{
				this._equalDistanceToBlockerOverride = value;
			}
		}

		public int TieBreakerScore
		{
			get
			{
				return this._tieBreakerScore;
			}
			set
			{
				this._tieBreakerScore = value;
			}
		}

		public ISurface Surface { get; private set; }

		public IBounds SurfaceBounds { get; private set; }

		public bool FaceTargetDirection
		{
			get
			{
				return this._faceTargetDirection;
			}
			set
			{
				this._faceTargetDirection = value;
			}
		}

		public bool EyeLevel
		{
			get
			{
				return this._eyeLevel;
			}
			set
			{
				this._eyeLevel = value;
			}
		}

		protected override void Awake()
		{
			base.Awake();
			this.Surface = (this._surface as ISurface);
			this.SurfaceBounds = (this._surface as IBounds);
		}

		protected override void Start()
		{
			this.BeginStart(ref this._started, delegate
			{
				base.Start();
			});
			this.EndStart(ref this._started);
		}

		public bool IsInRange(in Pose origin, float maxSqrDistance)
		{
			if (this.SurfaceBounds == null)
			{
				return true;
			}
			Bounds bounds = this.SurfaceBounds.Bounds;
			Vector3 center = bounds.center;
			center.y = origin.position.y;
			Vector3 vector = center - origin.position;
			float num = bounds.extents.x * bounds.extents.x + bounds.extents.z * bounds.extents.z;
			if (vector.sqrMagnitude <= num)
			{
				return true;
			}
			if (!TeleportInteractable.<IsInRange>g__CheckSquaredDistances|32_0(vector.sqrMagnitude, num, maxSqrDistance))
			{
				return false;
			}
			Pose pose = origin;
			Vector3 forward = pose.forward;
			float num2 = 1f / Mathf.Sqrt(1f - forward.y * forward.y);
			forward.y = 0f;
			forward.x *= num2;
			forward.z *= num2;
			return TeleportInteractable.<IsInRange>g__SqrDistanceToSegment|32_1(center, origin.position, forward, maxSqrDistance) <= num;
		}

		public bool DetectHit(Vector3 from, Vector3 to, out TeleportHit hit)
		{
			Vector3 direction = to - from;
			Ray ray = new Ray(from, direction);
			SurfaceHit surfaceHit;
			if (this.Surface.Raycast(ray, out surfaceHit, direction.magnitude))
			{
				hit = new TeleportHit(base.transform, surfaceHit.Point, surfaceHit.Normal);
				return true;
			}
			hit = TeleportHit.DEFAULT;
			return false;
		}

		public Pose TargetPose(Pose hitPose)
		{
			Pose result = hitPose;
			if (this._targetPoint != null)
			{
				result.position = this._targetPoint.position;
				result.rotation = this._targetPoint.rotation;
			}
			return result;
		}

		public void InjectAllTeleportInteractable(ISurface surface)
		{
			this.InjectSurface(surface);
		}

		public void InjectSurface(ISurface surface)
		{
			this._surface = (surface as Object);
			this.Surface = surface;
			this.SurfaceBounds = (surface as IBounds);
		}

		public void InjectOptionalTargetPoint(Transform targetPoint)
		{
			this._targetPoint = targetPoint;
		}

		[CompilerGenerated]
		internal static bool <IsInRange>g__CheckSquaredDistances|32_0(float x, float y, float threshold)
		{
			float num = x - y - threshold;
			return x <= y + threshold || num * num <= 4f * y * threshold;
		}

		[CompilerGenerated]
		internal static float <IsInRange>g__SqrDistanceToSegment|32_1(Vector3 point, Vector3 origin, Vector3 dir, float sqrLength)
		{
			float num = Vector3.Dot(point - origin, dir);
			if (num < 0f)
			{
				num = 0f;
			}
			else if (num * num > sqrLength)
			{
				num = Mathf.Sqrt(sqrLength);
			}
			Vector3 b = origin + dir * num;
			return (point - b).sqrMagnitude;
		}

		[SerializeField]
		[Tooltip("Indicates if the interactable is valid for teleport. Setting it to false can be convenient to block the arc.")]
		private bool _allowTeleport = true;

		[SerializeField]
		[Optional]
		[ConditionalHide("_allowTeleport", true)]
		[Tooltip("An override for the Interactor EqualDistanceThreshold used when comparing the interactable against other interactables that does not allow teleport.")]
		private float _equalDistanceToBlockerOverride;

		[SerializeField]
		[Optional]
		[Tooltip("Establishes the priority when several interactables are hit at the same time (EqualDistanceThreshold) by the arc.")]
		private int _tieBreakerScore;

		[SerializeField]
		[Interface(typeof(ISurface), new Type[]
		{

		})]
		[Tooltip("Surface against which the interactor will check collision with the arc.")]
		private Object _surface;

		[Header("Target", order = -1)]
		[SerializeField]
		[Optional]
		[Tooltip("A specific point in space where the player should teleport to.")]
		private Transform _targetPoint;

		[SerializeField]
		[Optional]
		[Tooltip("When true, the player will also face the direction specified by the target point.")]
		private bool _faceTargetDirection;

		[SerializeField]
		[Optional]
		[Tooltip("When true, instead of aligning the players feet to the TargetPoint it will align the head.")]
		private bool _eyeLevel;
	}
}
