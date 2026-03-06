using System;
using GorillaExtensions;
using GorillaNetworking;
using UnityEngine;

namespace GorillaTag.Reactions
{
	public class SpawnWorldEffects : MonoBehaviour
	{
		protected void OnEnable()
		{
			if (GorillaComputer.instance == null)
			{
				Debug.LogError("SpawnWorldEffects: Disabling because GorillaComputer not found! Hierarchy path: " + base.transform.GetPath(), this);
				base.enabled = false;
				return;
			}
			if (this._prefabToSpawn != null && !this._isPrefabInPool)
			{
				if (this._prefabToSpawn.CompareTag("Untagged"))
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab has no tag! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._isPrefabInPool = ObjectPools.instance.DoesPoolExist(this._prefabToSpawn);
				if (!this._isPrefabInPool)
				{
					Debug.LogError("SpawnWorldEffects: Disabling because Spawn Prefab not in pool! Hierarchy path: " + base.transform.GetPath(), this);
					base.enabled = false;
					return;
				}
				this._pool = ObjectPools.instance.GetPoolByObjectType(this._prefabToSpawn);
			}
			this._hasPrefabToSpawn = (this._prefabToSpawn != null && this._isPrefabInPool);
		}

		public void RequestSpawn(Vector3 worldPosition)
		{
			this.RequestSpawn(worldPosition, Vector3.up);
		}

		public void RequestSpawn(Vector3 worldPosition, Vector3 normal)
		{
			if (this._maxParticleHitReactionRate < 1E-05f || !FireManager.hasInstance)
			{
				return;
			}
			double num = GTTime.TimeAsDouble();
			if ((float)(num - this._lastCollisionTime) < 1f / this._maxParticleHitReactionRate)
			{
				return;
			}
			if (this._hasPrefabToSpawn && this._isPrefabInPool && this._pool.GetInactiveCount() > 0)
			{
				Vector3 vector = normal;
				if (this._useNormalOrientation)
				{
					vector = this.GetSurfaceNormal(worldPosition, normal);
				}
				Quaternion? rotationOverride = null;
				if (this._forwardOrientationSource != null)
				{
					Vector3 vector2 = Vector3.ProjectOnPlane(SpawnWorldEffects.GetAxisVector(this._forwardOrientationSource, this._forwardSourceAxis), vector);
					if (vector2.sqrMagnitude < 0.0001f)
					{
						vector2 = Vector3.ProjectOnPlane((Mathf.Abs(vector.y) < 0.99f) ? Vector3.up : Vector3.right, vector);
					}
					rotationOverride = new Quaternion?(Quaternion.LookRotation(vector2.normalized, vector));
				}
				FireManager.SpawnFire(this._pool, worldPosition, vector, base.transform.lossyScale.x, rotationOverride);
			}
			this._lastCollisionTime = num;
		}

		private static Vector3 GetAxisVector(Transform source, SpawnWorldEffects.TransformAxis axis)
		{
			Vector3 result;
			switch (axis)
			{
			case SpawnWorldEffects.TransformAxis.Forward:
				result = source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Back:
				result = -source.forward;
				break;
			case SpawnWorldEffects.TransformAxis.Right:
				result = source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Left:
				result = -source.right;
				break;
			case SpawnWorldEffects.TransformAxis.Up:
				result = source.up;
				break;
			case SpawnWorldEffects.TransformAxis.Down:
				result = -source.up;
				break;
			default:
				result = source.forward;
				break;
			}
			return result;
		}

		private Vector3 GetSurfaceNormal(Vector3 worldPosition, Vector3 hitNormal)
		{
			Vector3 vector;
			if (this._raycastDirectionSource != null)
			{
				Vector3 a = this._raycastDirectionSource.forward;
				if (this._raycastDirectionUseNegativeForward)
				{
					a = -a;
				}
				vector = ((a.sqrMagnitude > 1E-06f) ? a.normalized : Vector3.down);
			}
			else
			{
				vector = -((hitNormal.sqrMagnitude > 1E-06f) ? hitNormal.normalized : Vector3.up);
			}
			Vector3 origin = worldPosition + -vector * 0.05f;
			Vector3 direction = vector;
			RaycastHit raycastHit;
			if (Physics.Raycast(origin, direction, out raycastHit, this._normalRaycastDistance + 0.05f, this._normalRaycastLayers, QueryTriggerInteraction.Ignore))
			{
				return raycastHit.normal;
			}
			return hitNormal;
		}

		[Tooltip("The defaults are numbers for the flamethrower hair dryer.")]
		private readonly float _maxParticleHitReactionRate = 2f;

		[Tooltip("Must be in the global object pool and have a tag.")]
		[SerializeField]
		private GameObject _prefabToSpawn;

		[Tooltip("When enabled, a short raycast is fired from the spawn position to find the exact surface normal. The spawned object's Up vector will be aligned to that normal instead of world Up.")]
		[SerializeField]
		private bool _useNormalOrientation;

		[SerializeField]
		private float _normalRaycastDistance = 0.3f;

		[SerializeField]
		private LayerMask _normalRaycastLayers = 134218241;

		[Header("Raycast Direction Override")]
		[Tooltip("Optional. When assigned, the raycast used for normal-orientation will shoot along this transform's forward axis instead of along the incoming hit normal.")]
		[SerializeField]
		private Transform _raycastDirectionSource;

		[Tooltip("If true, uses -forward instead of forward from Raycast Direction Source.")]
		[SerializeField]
		private bool _raycastDirectionUseNegativeForward;

		[Header("Forward Orientation")]
		[Tooltip("Optional. When assigned, the spawned object's forward vector will be aligned to the chosen axis of this transform, projected onto the spawn surface.")]
		[SerializeField]
		private Transform _forwardOrientationSource;

		[Tooltip("Which local axis of the Forward Orientation Source to use as the spawned object's forward.")]
		[SerializeField]
		private SpawnWorldEffects.TransformAxis _forwardSourceAxis;

		private bool _hasPrefabToSpawn;

		private bool _isPrefabInPool;

		private double _lastCollisionTime;

		private SinglePool _pool;

		private enum TransformAxis
		{
			Forward,
			Back,
			Right,
			Left,
			Up,
			Down
		}
	}
}
