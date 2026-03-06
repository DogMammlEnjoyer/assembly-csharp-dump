using System;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class WallPenetrationTunneling : MonoBehaviour
	{
		public AnimationCurve PenetrationFov
		{
			get
			{
				return this._penetrationFov;
			}
			set
			{
				this._penetrationFov = value;
			}
		}

		public float ExtraDistance
		{
			get
			{
				return this._extraDistance;
			}
			set
			{
				this._extraDistance = value;
			}
		}

		public string IgnoreTag
		{
			get
			{
				return this._ignoreTag;
			}
			set
			{
				this._ignoreTag = value;
			}
		}

		public LayerMask LayerMask
		{
			get
			{
				return this._layerMask;
			}
			set
			{
				this._layerMask = value;
			}
		}

		protected virtual void Awake()
		{
			this._hits = new RaycastHit[this._maxCollidersCheck];
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void LateUpdate()
		{
			float penetrationDistance;
			bool headBlocked = this.CalculatePenetration(out penetrationDistance);
			this.UpdateTunneling(headBlocked, penetrationDistance);
		}

		private bool CalculatePenetration(out float distance)
		{
			Vector3 position = this._logicalPosition.position;
			Vector3 vector = this._trackedPosition.position - position;
			float num = vector.magnitude + this._extraDistance;
			int num2 = Physics.RaycastNonAlloc(new Ray(position, vector.normalized), this._hits, num, this._layerMask.value);
			if (num2 > 0)
			{
				if (string.IsNullOrEmpty(this._ignoreTag))
				{
					distance = Mathf.Max(0f, num - this._hits[0].distance);
					return true;
				}
				for (int i = 0; i < num2; i++)
				{
					if (this._ignoreTag != this._hits[i].collider.tag)
					{
						distance = Mathf.Max(0f, num - this._hits[i].distance);
						return true;
					}
				}
			}
			distance = 0f;
			return false;
		}

		private void UpdateTunneling(bool headBlocked, float penetrationDistance)
		{
			float num = this._penetrationFov.Evaluate(penetrationDistance);
			if (!headBlocked || num >= 360f)
			{
				this._tunneling.enabled = false;
				this._tunneling.UserFOV = 360f;
				return;
			}
			Vector3 normalized = (this._logicalPosition.position - this._trackedPosition.position).normalized;
			this._tunneling.enabled = true;
			this._tunneling.UseAimingTarget = true;
			this._tunneling.AimingDirection = normalized;
			this._tunneling.UserFOV = num;
		}

		public void InjectAllWallPenetrationTunneling(Transform trackedPosition, Transform logicalPosition, TunnelingEffect tunneling, int maxCollidersCheck)
		{
			this.InjectTrackedPosition(trackedPosition);
			this.InjectLogicalPosition(logicalPosition);
			this.InjectTunneling(tunneling);
			this.InjectMaxCollidersCheck(maxCollidersCheck);
		}

		public void InjectTrackedPosition(Transform trackedPosition)
		{
			this._trackedPosition = trackedPosition;
		}

		public void InjectLogicalPosition(Transform logicalPosition)
		{
			this._logicalPosition = logicalPosition;
		}

		public void InjectTunneling(TunnelingEffect tunneling)
		{
			this._tunneling = tunneling;
		}

		public void InjectMaxCollidersCheck(int maxCollidersCheck)
		{
			this._maxCollidersCheck = maxCollidersCheck;
		}

		[SerializeField]
		private Transform _trackedPosition;

		[SerializeField]
		private Transform _logicalPosition;

		[SerializeField]
		private TunnelingEffect _tunneling;

		[SerializeField]
		private AnimationCurve _penetrationFov;

		[SerializeField]
		private float _extraDistance = 0.22f;

		[SerializeField]
		[Min(1f)]
		private int _maxCollidersCheck = 5;

		[SerializeField]
		[Optional]
		private string _ignoreTag = "Player";

		[SerializeField]
		private LayerMask _layerMask = -1;

		private RaycastHit[] _hits;

		protected bool _started;
	}
}
