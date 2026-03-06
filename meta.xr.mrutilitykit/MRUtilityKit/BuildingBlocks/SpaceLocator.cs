using System;
using UnityEngine;
using UnityEngine.Events;

namespace Meta.XR.MRUtilityKit.BuildingBlocks
{
	public abstract class SpaceLocator : MonoBehaviour
	{
		protected virtual Transform RaycastOrigin { get; set; }

		protected virtual float MaxRaycastDistance { get; set; } = 100f;

		public UnityEvent<Pose, bool> OnSpaceLocateCompleted
		{
			get
			{
				return this._onSpaceLocateCompleted;
			}
			set
			{
				this._onSpaceLocateCompleted = value;
			}
		}

		public EnvironmentRaycastHit RaycastHitResult
		{
			get
			{
				return this._raycastHit;
			}
		}

		private void Start()
		{
			this._raycastManager = Object.FindFirstObjectByType<EnvironmentRaycastManager>();
		}

		protected internal abstract Ray GetRaycastRay();

		protected virtual bool TryLocateSpace(out Pose surfacePose)
		{
			surfacePose = default(Pose);
			Ray raycastRay = this.GetRaycastRay();
			EnvironmentRaycastHit environmentRaycastHit;
			if (!(this._raycastManager.Raycast(raycastRay, out environmentRaycastHit, this.MaxRaycastDistance) & environmentRaycastHit.normalConfidence > 0.4f))
			{
				UnityEvent<Pose, bool> onSpaceLocateCompleted = this.OnSpaceLocateCompleted;
				if (onSpaceLocateCompleted != null)
				{
					onSpaceLocateCompleted.Invoke(default(Pose), false);
				}
				return false;
			}
			if (this.PreferredSurfaceOrientation != SpaceLocator.SurfaceOrientation.Any && (SpaceLocator.GetSurfaceOrientation(environmentRaycastHit.normal) & this.PreferredSurfaceOrientation) == SpaceLocator.SurfaceOrientation.None)
			{
				UnityEvent<Pose, bool> onSpaceLocateCompleted2 = this.OnSpaceLocateCompleted;
				if (onSpaceLocateCompleted2 != null)
				{
					onSpaceLocateCompleted2.Invoke(default(Pose), false);
				}
				return false;
			}
			bool flag = this.TryCalculateSurfacePose(environmentRaycastHit, raycastRay, out surfacePose);
			UnityEvent<Pose, bool> onSpaceLocateCompleted3 = this.OnSpaceLocateCompleted;
			if (onSpaceLocateCompleted3 != null)
			{
				onSpaceLocateCompleted3.Invoke(surfacePose, flag);
			}
			return flag;
		}

		private bool TryCalculateSurfacePose(EnvironmentRaycastHit hit, Ray ray, out Pose surfacePose)
		{
			surfacePose = default(Pose);
			Vector3 upwards = this.CalculateUpwardFromPlacementSide(hit, base.transform, ray);
			Bounds? prefabBounds = Utilities.GetPrefabBounds(base.transform.gameObject);
			Vector3 vector = (prefabBounds != null) ? prefabBounds.GetValueOrDefault().size : (Vector3.one * 0.05f);
			Vector3 boxSize = this.UseCustomSize ? this.CustomSize : vector;
			if (!this._raycastManager.PlaceBox(ray, boxSize, upwards, out this._raycastHit))
			{
				return false;
			}
			Quaternion rotation = Quaternion.LookRotation(Vector3.Cross(this._raycastHit.normal, Vector3.Cross(base.transform.forward, this._raycastHit.normal).normalized), this._raycastHit.normal);
			surfacePose = new Pose(this._raycastHit.point, rotation);
			return true;
		}

		private Vector3 CalculateUpwardFromPlacementSide(EnvironmentRaycastHit hit, Transform rayOrigin, Ray ray)
		{
			if (!SpaceLocator.IsVertical(hit.normal))
			{
				return Vector3.ProjectOnPlane(rayOrigin.up, Vector3.Cross(ray.direction, Vector3.up));
			}
			return Vector3.up;
		}

		private static bool IsVertical(Vector3 normal)
		{
			return Mathf.Abs(Vector3.Dot(normal, Vector3.up)) < 0.3f;
		}

		private static bool IsHorizontalDown(Vector3 normal)
		{
			return Vector3.Dot(normal, Vector3.down) > 0.7f;
		}

		private static bool IsHorizontalUp(Vector3 normal)
		{
			return Vector3.Dot(normal, Vector3.up) > 0.7f;
		}

		private static SpaceLocator.SurfaceOrientation GetSurfaceOrientation(Vector3 normal)
		{
			SpaceLocator.SurfaceOrientation result = SpaceLocator.SurfaceOrientation.None;
			if (SpaceLocator.IsHorizontalDown(normal))
			{
				result = SpaceLocator.SurfaceOrientation.HorizontalFaceDown;
			}
			else if (SpaceLocator.IsHorizontalUp(normal))
			{
				result = SpaceLocator.SurfaceOrientation.HorizontalFaceUp;
			}
			else if (SpaceLocator.IsVertical(normal))
			{
				result = SpaceLocator.SurfaceOrientation.Vertical;
			}
			return result;
		}

		public SpaceLocator.SurfaceOrientation PreferredSurfaceOrientation = SpaceLocator.SurfaceOrientation.Vertical | SpaceLocator.SurfaceOrientation.HorizontalFaceUp | SpaceLocator.SurfaceOrientation.HorizontalFaceDown;

		[Tooltip("Use CustomSize instead of local scale of Target")]
		[SerializeField]
		public bool UseCustomSize;

		[Tooltip("Size the of the space to locate")]
		[SerializeField]
		public Vector3 CustomSize = Vector3.one * 0.25f;

		[Space]
		[Tooltip("This event will trigger when a suitable space is located within user's physical environment")]
		[Space]
		[SerializeField]
		private UnityEvent<Pose, bool> _onSpaceLocateCompleted = new UnityEvent<Pose, bool>();

		private EnvironmentRaycastManager _raycastManager;

		private EnvironmentRaycastHit _raycastHit;

		private const float VerticalSurfaceAngleThreshold = 0.3f;

		private const float HorizontalSurfaceAngleThreshold = 0.7f;

		private const float NormalConfidenceThreshold = 0.4f;

		private Vector3 _sizeToLocate;

		[Flags]
		public enum SurfaceOrientation
		{
			None = 0,
			Any = 1,
			Vertical = 2,
			HorizontalFaceUp = 4,
			HorizontalFaceDown = 8
		}
	}
}
