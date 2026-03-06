using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	public class CharacterController : MonoBehaviour
	{
		public float SkinWidth
		{
			get
			{
				return this._skinWidth;
			}
			set
			{
				this._skinWidth = value;
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

		public float MaxSlopeAngle
		{
			get
			{
				return this._maxSlopeAngle;
			}
			set
			{
				this._maxSlopeAngle = value;
			}
		}

		public float MaxStep
		{
			get
			{
				return this._maxStep;
			}
			set
			{
				this._maxStep = value;
			}
		}

		public int MaxReboundSteps
		{
			get
			{
				return this._maxReboundSteps;
			}
			set
			{
				this._maxReboundSteps = value;
			}
		}

		public bool IsGrounded
		{
			get
			{
				return this._isGrounded;
			}
		}

		public float Height
		{
			get
			{
				return this._capsule.height;
			}
		}

		public float Radius
		{
			get
			{
				return this._capsule.radius;
			}
		}

		public Pose Pose
		{
			get
			{
				return this._capsule.transform.GetPose(Space.World);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.UpdateAnchorPoints();
			}
		}

		public bool TrySetHeight(float desiredHeight)
		{
			float height = this._capsule.height;
			float skinWidth = this._skinWidth;
			float num = desiredHeight - height;
			Vector3 vector;
			if (num > skinWidth && this.CheckMoveCharacter(Vector3.up * num, out vector))
			{
				num = Mathf.Max(0f, vector.y - this._skinWidth);
			}
			if (Mathf.Abs(num) <= skinWidth)
			{
				return false;
			}
			this._capsule.height = height + num;
			this._capsule.transform.position += Vector3.up * num * 0.5f;
			this.UpdateAnchorPoints();
			return true;
		}

		public bool TryGround(float extraDistance = 0f)
		{
			RaycastHit raycastHit;
			if (this.CalculateGround(out raycastHit, extraDistance) && this.IsFlat(raycastHit.normal))
			{
				Vector3 position = this._capsule.transform.position;
				float num;
				this.RaycastHitPlane(raycastHit, position, Vector3.down, out num);
				position.y = position.y - num + this._capsule.height * 0.5f + this._skinWidth;
				this._capsule.transform.position = position;
				this._groundHit = raycastHit;
				this._isGrounded = true;
				this.UpdateAnchorPoints();
				return true;
			}
			return false;
		}

		public void SetRotation(Quaternion rotation)
		{
			this._capsule.transform.rotation = rotation;
			this.UpdateAnchorPoints();
		}

		public void SetPosition(Vector3 position)
		{
			this._capsule.transform.position = position;
			this.UpdateAnchorPoints();
		}

		public void Move(Vector3 delta)
		{
			if (this._isGrounded)
			{
				delta = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(delta, Vector3.up), this._groundHit.normal) + Vector3.up * delta.y;
			}
			Vector3 vector = this.Rebound(delta, this._maxReboundSteps);
			this._capsule.transform.position += vector;
			this.UpdateGrounded(delta.y < 0f && delta.y < vector.y && Mathf.Abs(vector.y) < 0.001f);
			this.UpdateAnchorPoints();
		}

		private Vector3 Rebound(Vector3 delta, int bounces)
		{
			Vector3 b = Vector3.up * Mathf.Max(0f, this._capsule.height * 0.5f - this._capsule.radius);
			Vector3 capsuleTop = this._capsule.transform.position + b;
			Vector3 capsuleBase = this._capsule.transform.position - b;
			Vector3 originalFlatDelta = Vector3.ProjectOnPlane(delta, Vector3.up);
			return this.<Rebound>g__ReboundRecursive|42_0(capsuleBase, capsuleTop, this._capsule.radius, delta, originalFlatDelta, bounces);
		}

		private bool ClimbStep(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, out Vector3 climbDelta, out RaycastHit? stepHit)
		{
			stepHit = null;
			climbDelta = Vector3.zero;
			delta = Vector3.ProjectOnPlane(delta, Vector3.up);
			float num = Mathf.Min(this._maxStep, capsuleTop.y - capsuleBase.y);
			float d = Mathf.Max(0f, this._maxStep - num);
			Vector3 vector = capsuleBase + Vector3.up * num;
			Vector3 capsuleTop2 = capsuleTop + Vector3.up * d;
			RaycastHit? raycastHit;
			if (this.MoveCapsuleCollides(vector, capsuleTop2, radius, delta, out raycastHit))
			{
				stepHit = raycastHit;
				Vector3 vector2 = capsuleTop - capsuleBase;
				if (Mathf.Approximately(vector2.sqrMagnitude, 0f) || Mathf.Abs(Vector3.Dot(raycastHit.Value.normal, vector2.normalized)) > 0.001f)
				{
					Vector3 vector3 = -raycastHit.Value.normal;
					Ray ray = new Ray(raycastHit.Value.point - vector3 * raycastHit.Value.distance, vector3);
					RaycastHit value;
					if (raycastHit.Value.collider.Raycast(ray, out value, raycastHit.Value.distance + 0.001f))
					{
						raycastHit = new RaycastHit?(value);
					}
				}
				delta = this.DecomposeDelta(delta, raycastHit.Value).Item1;
			}
			RaycastHit raycastHit2;
			float num2;
			if (!this.CalculateGround(capsuleTop + delta, radius, this._capsule.height - radius, out raycastHit2) || !CharacterController.RaycastSphere(raycastHit2.point, Vector3.up, vector + delta, radius + this._skinWidth, out num2) || raycastHit2.point.y - (capsuleBase.y - radius) > this._maxStep || !this.IsFlat(raycastHit2.normal))
			{
				return false;
			}
			delta.y = Mathf.Max(delta.y, num - num2);
			Vector3 delta2 = Vector3.up * delta.y;
			RaycastHit? raycastHit3;
			if (this.MoveCapsuleCollides(capsuleBase, capsuleTop, radius, delta2, out raycastHit3))
			{
				return false;
			}
			climbDelta = delta;
			return true;
		}

		private bool CheckMoveCharacter(Vector3 delta, out Vector3 movement)
		{
			Vector3 b = Vector3.up * Mathf.Max(0f, this._capsule.height * 0.5f - this._capsule.radius);
			Vector3 capsuleTop = this._capsule.transform.position + b;
			Vector3 capsuleBase = this._capsule.transform.position - b;
			float radius = this._capsule.radius;
			RaycastHit? raycastHit;
			if (this.MoveCapsuleCollides(capsuleBase, capsuleTop, radius, delta, out raycastHit))
			{
				delta = this.DecomposeDelta(delta, raycastHit.Value).Item1;
				movement = delta;
				return true;
			}
			movement = Vector3.zero;
			return false;
		}

		private bool MoveCapsuleCollides(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, out RaycastHit? moveHit)
		{
			float sqrMagnitude = delta.sqrMagnitude;
			if (Mathf.Approximately(sqrMagnitude, 0f))
			{
				moveHit = null;
				return false;
			}
			float maxDistance = (sqrMagnitude < this._skinWidth * this._skinWidth) ? this._skinWidth : Mathf.Sqrt(sqrMagnitude);
			RaycastHit value;
			bool flag = Physics.CapsuleCast(capsuleBase, capsuleTop, radius, delta.normalized, out value, maxDistance, this._layerMask.value, QueryTriggerInteraction.Ignore);
			moveHit = (flag ? new RaycastHit?(value) : null);
			return flag;
		}

		private ValueTuple<Vector3, Vector3> DecomposeDelta(Vector3 delta, RaycastHit hit)
		{
			Vector3 normalized = delta.normalized;
			float num = Mathf.Max(0.1f, Vector3.Dot(normalized, -hit.normal)) * this._skinWidth;
			Vector3 vector = normalized * Mathf.Max(0f, hit.distance - num);
			Vector3 item = delta - vector;
			return new ValueTuple<Vector3, Vector3>(vector, item);
		}

		private Vector3 SlideDelta(Vector3 delta, Vector3 originalFlatDelta, RaycastHit hit)
		{
			Vector3 vector = hit.normal;
			if (!this.IsFlat(vector))
			{
				vector = Vector3.ProjectOnPlane(hit.normal, Vector3.up).normalized;
			}
			Vector3 vector2 = Vector3.ProjectOnPlane(delta, Vector3.up);
			vector2 = Vector3.ProjectOnPlane(vector2, vector);
			if (Vector3.Dot(vector2, originalFlatDelta) <= 0f)
			{
				vector2 = Vector3.zero;
			}
			Vector3 vector3 = Vector3.up * delta.y;
			vector3 = Vector3.ProjectOnPlane(vector3, hit.normal);
			return vector2 + vector3;
		}

		private bool IsFlat(Vector3 groundNormal)
		{
			return Vector3.Angle(Vector3.up, groundNormal) <= this._maxSlopeAngle;
		}

		private void UpdateGrounded(bool forceGrounded = false)
		{
			this._isGrounded = (this.CalculateGround(out this._groundHit, 0f) && this.IsFlat(this._groundHit.normal));
			if (!this._isGrounded && forceGrounded)
			{
				this._isGrounded = true;
				this._groundHit.normal = Vector3.up;
				this._groundHit.point = this._capsule.transform.position + Vector3.down * (this._capsule.height * 0.5f + this._skinWidth);
			}
		}

		private bool CalculateGround(out RaycastHit groundHit, float extraDistance = 0f)
		{
			Vector3 origin = this._capsule.transform.position + Vector3.down * (this._capsule.height * 0.5f - this._capsule.radius);
			return this.CalculateGround(origin, this._capsule.radius + this._skinWidth, this._capsule.radius + this._skinWidth + extraDistance, out groundHit) || this.CalculateGround(this._capsule.transform.position, this._capsule.radius + this._skinWidth, this._capsule.height * 0.5f + this._skinWidth + extraDistance, out groundHit);
		}

		private bool CalculateGround(Vector3 origin, float radius, float distance, out RaycastHit groundHit)
		{
			Vector3 down = Vector3.down;
			RaycastHit raycastHit;
			bool flag = Physics.Raycast(origin, down, out raycastHit, distance, this._layerMask.value, QueryTriggerInteraction.Ignore);
			RaycastHit raycastHit2;
			bool flag2 = Physics.SphereCast(origin, radius, down, out raycastHit2, distance - radius, this._layerMask.value, QueryTriggerInteraction.Ignore);
			RaycastHit raycastHit3;
			if (flag2 && Physics.Raycast(raycastHit2.point - down * 0.01f, down, out raycastHit3, 0.011f, this._layerMask.value, QueryTriggerInteraction.Ignore))
			{
				raycastHit2.normal = raycastHit3.normal;
			}
			if (flag2 && flag)
			{
				groundHit = ((raycastHit2.normal.y > raycastHit.normal.y) ? raycastHit2 : raycastHit);
				groundHit.distance = Vector3.Project(groundHit.point - origin, down).magnitude;
				return true;
			}
			if (flag2 || flag)
			{
				groundHit = (flag2 ? raycastHit2 : raycastHit);
				groundHit.normal = (flag ? raycastHit.normal : raycastHit2.normal);
				groundHit.distance = Vector3.Project(groundHit.point - origin, down).magnitude;
				return true;
			}
			groundHit = default(RaycastHit);
			return false;
		}

		private void UpdateAnchorPoints()
		{
			Vector3 b = Vector3.up * Mathf.Max(0f, this._capsule.height * 0.5f + this._skinWidth);
			Vector3 position = this._capsule.transform.position + b;
			Vector3 position2 = this._capsule.transform.position - b;
			Quaternion rotation = this._capsule.transform.rotation;
			if (this._headAnchor != null)
			{
				this._headAnchor.transform.SetPositionAndRotation(position, rotation);
			}
			if (this._feetAnchor != null)
			{
				this._feetAnchor.transform.SetPositionAndRotation(position2, rotation);
			}
		}

		private static bool RaycastSphere(Vector3 origin, Vector3 direction, Vector3 sphereCenter, float radius, out float distance)
		{
			distance = float.MaxValue;
			Vector3 vector = origin - sphereCenter;
			float num = Vector3.Dot(direction, direction);
			float num2 = 2f * Vector3.Dot(vector, direction);
			float num3 = Vector3.Dot(vector, vector) - radius * radius;
			float num4 = num2 * num2 - 4f * num * num3;
			if (num4 < 0f)
			{
				return false;
			}
			distance = (-num2 - (float)Math.Sqrt((double)num4)) / (2f * num);
			return true;
		}

		private bool RaycastHitPlane(RaycastHit hit, Vector3 origin, Vector3 direction, out float enter)
		{
			enter = 0f;
			float num = Vector3.Dot(hit.normal, hit.point) - Vector3.Dot(origin, hit.normal);
			float num2 = Vector3.Dot(direction, hit.normal);
			if (!Mathf.Approximately(num2, 0f))
			{
				enter = num / num2;
				return true;
			}
			return false;
		}

		public void InjectAllCharacterController(CapsuleCollider capsule)
		{
			this.InjectCapsule(capsule);
		}

		public void InjectCapsule(CapsuleCollider capsule)
		{
			this._capsule = capsule;
		}

		public void InjectOptionalFeetAnchor(Transform feetAnchor)
		{
			this._feetAnchor = feetAnchor;
		}

		public void InjectOptionalHeadAnchor(Transform headAnchor)
		{
			this._headAnchor = headAnchor;
		}

		[CompilerGenerated]
		private Vector3 <Rebound>g__ReboundRecursive|42_0(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, Vector3 originalFlatDelta, int bounceStep)
		{
			if (bounceStep <= 0 || Mathf.Approximately(delta.sqrMagnitude, 0f))
			{
				return Vector3.zero;
			}
			Vector3 a = Vector3.zero;
			Vector3 delta2 = Vector3.zero;
			RaycastHit? raycastHit = null;
			RaycastHit? raycastHit2 = null;
			if (this.MoveCapsuleCollides(capsuleBase, capsuleTop, radius, delta, out raycastHit))
			{
				ValueTuple<Vector3, Vector3> valueTuple = this.DecomposeDelta(delta, raycastHit.Value);
				delta = valueTuple.Item1;
				delta2 = valueTuple.Item2;
			}
			capsuleBase += delta;
			capsuleTop += delta;
			a += delta;
			Vector3 vector;
			if (this._isGrounded && this._maxStep > 0f && raycastHit != null && raycastHit.Value.point.y - (capsuleBase.y - radius - this._skinWidth) <= this._maxStep && this.ClimbStep(capsuleBase, capsuleTop, radius, delta2, out vector, out raycastHit2))
			{
				capsuleBase += vector;
				capsuleTop += vector;
				a += vector;
				if (raycastHit2 != null)
				{
					delta2 = this.DecomposeDelta(vector, raycastHit2.Value).Item2;
					delta2 = this.SlideDelta(delta2, originalFlatDelta, raycastHit2.Value);
				}
				else
				{
					delta2 = Vector3.zero;
				}
			}
			if (raycastHit != null && raycastHit2 == null)
			{
				delta2 = this.SlideDelta(delta2, originalFlatDelta, raycastHit.Value);
			}
			return a + this.<Rebound>g__ReboundRecursive|42_0(capsuleBase, capsuleTop, radius, delta2, originalFlatDelta, bounceStep - 1);
		}

		[Header("Character")]
		[SerializeField]
		[Tooltip("Capsule collider that represents the character and will be moved by the locomotor.")]
		private CapsuleCollider _capsule;

		[SerializeField]
		[Min(0f)]
		[Tooltip("Extra offset added to the radius of the capsule for soft collisions.")]
		private float _skinWidth = 0.005f;

		[SerializeField]
		[Tooltip("LayerMask check for collisions when moving.")]
		private LayerMask _layerMask = -1;

		[SerializeField]
		[Range(0f, 90f)]
		[Tooltip("Max climbable slope angle in degrees.")]
		private float _maxSlopeAngle = 50f;

		[SerializeField]
		[Min(0f)]
		[Tooltip("Max climbable height for steps.")]
		private float _maxStep = 0.3f;

		[SerializeField]
		[Min(1f)]
		[Tooltip("Max iterations for sliding the delta movement after colliding with an obstacle.")]
		private int _maxReboundSteps = 3;

		[Header("Anchors")]
		[SerializeField]
		[Optional]
		[Tooltip("Optional. This transform pose will be updated with the pose of the character top.")]
		private Transform _headAnchor;

		[SerializeField]
		[Optional]
		[Tooltip("Optional. This transform pose will be updated with the pose of the character base.")]
		private Transform _feetAnchor;

		protected RaycastHit _groundHit;

		protected bool _isGrounded;

		protected bool _started;

		private const float _cornerHitEpsilon = 0.001f;
	}
}
