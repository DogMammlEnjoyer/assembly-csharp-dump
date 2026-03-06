using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Oculus.Interaction.Locomotion
{
	[Obsolete("Use FirstPersonLocomotor instead")]
	public class CapsuleLocomotionHandler : MonoBehaviour, ILocomotionEventHandler, IDeltaTimeConsumer
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

		public float MaxWallPenetrationDistance
		{
			get
			{
				return this._maxWallPenetrationDistance;
			}
			set
			{
				this._maxWallPenetrationDistance = value;
			}
		}

		public float ExitHotspotDistance
		{
			get
			{
				return this._exitHotspotDistance;
			}
			set
			{
				this._exitHotspotDistance = value;
			}
		}

		public bool AutoUpdateHeight
		{
			get
			{
				return this._autoUpdateHeight;
			}
			set
			{
				this._autoUpdateHeight = value;
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

		public float DefaultHeight
		{
			get
			{
				return this._defaultHeight;
			}
			set
			{
				this._defaultHeight = value;
			}
		}

		public float HeightOffset
		{
			get
			{
				return this._heightOffset;
			}
			set
			{
				this._heightOffset = value;
			}
		}

		public float CrouchHeightOffset
		{
			get
			{
				return this._crouchHeightOffset;
			}
			set
			{
				this._crouchHeightOffset = value;
			}
		}

		public float SpeedFactor
		{
			get
			{
				return this._speedFactor;
			}
			set
			{
				this._speedFactor = value;
			}
		}

		public float CrouchSpeedFactor
		{
			get
			{
				return this._crouchSpeedFactor;
			}
			set
			{
				this._crouchSpeedFactor = value;
			}
		}

		public float RunningSpeedFactor
		{
			get
			{
				return this._runningSpeedFactor;
			}
			set
			{
				this._runningSpeedFactor = value;
			}
		}

		public float Acceleration
		{
			get
			{
				return this._acceleration;
			}
			set
			{
				this._acceleration = value;
			}
		}

		public float GroundDamping
		{
			get
			{
				return this._groundDamping;
			}
			set
			{
				this._groundDamping = value;
			}
		}

		public float JumpDamping
		{
			get
			{
				return this._jumpDamping;
			}
			set
			{
				this._jumpDamping = value;
			}
		}

		public float AirDamping
		{
			get
			{
				return this._airDamping;
			}
			set
			{
				this._airDamping = value;
			}
		}

		public float JumpForce
		{
			get
			{
				return this._jumpForce;
			}
			set
			{
				this._jumpForce = value;
			}
		}

		public float GravityFactor
		{
			get
			{
				return this._gravityFactor;
			}
			set
			{
				this._gravityFactor = value;
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

		public void SetDeltaTimeProvider(Func<float> deltaTimeProvider)
		{
			this._deltaTimeProvider = deltaTimeProvider;
		}

		public event Action<LocomotionEvent, Pose> WhenLocomotionEventHandled
		{
			add
			{
				this._whenLocomotionEventHandled = (Action<LocomotionEvent, Pose>)Delegate.Combine(this._whenLocomotionEventHandled, value);
			}
			remove
			{
				this._whenLocomotionEventHandled = (Action<LocomotionEvent, Pose>)Delegate.Remove(this._whenLocomotionEventHandled, value);
			}
		}

		public bool IsGrounded
		{
			get
			{
				return this._isGrounded;
			}
		}

		public bool IsRunning
		{
			get
			{
				return this._isRunning;
			}
		}

		public bool IsCrouching
		{
			get
			{
				return this._isCrouching;
			}
		}

		public bool IgnoringVelocity
		{
			get
			{
				return this._velocityDisabled || this._isHeadInHotspot;
			}
		}

		private bool ControllingPlayer
		{
			get
			{
				return this._playerOrigin != null && this._playerEyes != null;
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			if (!(this._playerOrigin != null))
			{
				this._playerEyes != null;
			}
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._endOfFrameRoutine = base.StartCoroutine(this.EndOfFrameCoroutine());
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._accumulatedDeltaFrame = Pose.identity;
				base.StopCoroutine(this._endOfFrameRoutine);
				this._endOfFrameRoutine = null;
			}
		}

		protected virtual void Update()
		{
			this.TryExitHotspot(false);
			this.UpdateCharacterHeight();
			if (!this.IgnoringVelocity)
			{
				this.CatchUpCharacterToPlayer();
				this.UpdateVelocity();
				Pose pose = this._capsule.transform.GetPose(Space.World);
				Vector3 delta = this._velocity * this._deltaTimeProvider();
				this.MoveCharacter(delta);
				Pose pose2 = this._capsule.transform.GetPose(Space.World);
				this.AccumulateDelta(ref this._accumulatedDeltaFrame, pose, pose2);
			}
			this.UpdateAnchorPoints();
		}

		protected virtual void LateUpdate()
		{
			this.ConsumeDeferredLocomotionEvents();
		}

		protected virtual void LastUpdate()
		{
			this.CatchUpPlayerToCharacter(this._accumulatedDeltaFrame, this.GetCharacterFeet().y);
			this._accumulatedDeltaFrame = Pose.identity;
		}

		public void Jump()
		{
			if (!this._isGrounded)
			{
				return;
			}
			if (this._isCrouching)
			{
				this.Crouch(false);
				return;
			}
			this.TryExitHotspot(true);
			this._velocity += Vector3.up * this._jumpForce;
		}

		public void ToggleCrouch()
		{
			this.Crouch(!this._isCrouching);
		}

		public void Crouch(bool crouch)
		{
			if (this._isCrouching != crouch)
			{
				this._isCrouching = crouch;
			}
		}

		public void ToggleRun()
		{
			this.Run(!this._isRunning);
		}

		public void Run(bool run)
		{
			if (this._isRunning != run)
			{
				this._isRunning = run;
				this.TryExitHotspot(true);
			}
		}

		public void HandleLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Velocity)
			{
				this.AddVelocity(locomotionEvent.Pose.position);
				if (this.IsHeadFarFromPoint(this.GetCharacterHead(), this._maxWallPenetrationDistance))
				{
					this.ResetPlayerToCharacter();
				}
				this._whenLocomotionEventHandled(locomotionEvent, locomotionEvent.Pose);
				return;
			}
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute || locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel || locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
			{
				this._velocity = Vector3.zero;
			}
			this._deferredLocomotionEvent.Enqueue(locomotionEvent);
		}

		private void ConsumeDeferredLocomotionEvents()
		{
			if (this._deferredLocomotionEvent.Count == 0)
			{
				return;
			}
			Pose pose = this._capsule.transform.GetPose(Space.World);
			while (this._deferredLocomotionEvent.Count > 0)
			{
				LocomotionEvent locomotionEvent = this._deferredLocomotionEvent.Dequeue();
				this.HandleDeferredLocomotionEvent(locomotionEvent);
			}
			Pose pose2 = this._capsule.transform.GetPose(Space.World);
			this.AccumulateDelta(ref this._accumulatedDeltaFrame, pose, pose2);
		}

		private void HandleDeferredLocomotionEvent(LocomotionEvent locomotionEvent)
		{
			Pose pose = this._capsule.transform.GetPose(Space.World);
			if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Absolute)
			{
				this.MoveAbsoluteFeet(locomotionEvent.Pose.position);
			}
			else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.AbsoluteEyeLevel)
			{
				this.MoveAbsoluteHead(locomotionEvent.Pose.position);
			}
			else if (locomotionEvent.Translation == LocomotionEvent.TranslationType.Relative)
			{
				this.MoveRelative(locomotionEvent.Pose.position);
			}
			if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Absolute)
			{
				this.RotateAbsolute(locomotionEvent.Pose.rotation);
			}
			else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Relative)
			{
				this.RotateRelative(locomotionEvent.Pose.rotation);
			}
			else if (locomotionEvent.Rotation == LocomotionEvent.RotationType.Velocity)
			{
				this.RotateVelocity(locomotionEvent.Pose.rotation);
			}
			Pose pose2 = this._capsule.transform.GetPose(Space.World);
			Pose identity = Pose.identity;
			this.AccumulateDelta(ref identity, pose, pose2);
			this._whenLocomotionEventHandled(locomotionEvent, identity);
		}

		private void AccumulateDelta(ref Pose accumulator, in Pose from, in Pose to)
		{
			accumulator.position = accumulator.position + to.position - from.position;
			accumulator.rotation = Quaternion.Inverse(from.rotation) * to.rotation * accumulator.rotation;
		}

		private void AddVelocity(Vector3 velocity)
		{
			this.TryExitHotspot(true);
			this._velocity += velocity * this.GetModifiedSpeedFactor();
		}

		private void MoveAbsoluteFeet(Vector3 target)
		{
			this.TryExitHotspot(true);
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = target - characterFeet;
			this._capsule.transform.position += b;
			Vector3 b2;
			if (this.CheckMoveCharacter(Vector3.down * this._maxStep, out b2))
			{
				this._capsule.transform.position += b2;
				this.UpdateGrounded(true);
				return;
			}
			this.UpdateGrounded(false);
		}

		private void MoveAbsoluteHead(Vector3 target)
		{
			Vector3 characterHead = this.GetCharacterHead();
			Vector3 b = target - characterHead;
			this._capsule.transform.position += b;
			this._isHeadInHotspot = true;
			this._headHotspotCenter = new Vector3?(this.GetCharacterHead());
		}

		private void MoveRelative(Vector3 offset)
		{
			if (this._isGrounded)
			{
				this.TryExitHotspot(true);
				this._velocity = Vector3.zero;
				this.MoveCharacter(offset);
			}
		}

		private void RotateAbsolute(Quaternion target)
		{
			this._capsule.transform.rotation = target;
		}

		private void RotateRelative(Quaternion target)
		{
			this._capsule.transform.rotation = target * this._capsule.transform.rotation;
		}

		private void RotateVelocity(Quaternion target)
		{
			float num;
			Vector3 axis;
			target.ToAngleAxis(out num, out axis);
			num *= this._deltaTimeProvider();
			this._capsule.transform.rotation = Quaternion.AngleAxis(num, axis) * this._capsule.transform.rotation;
		}

		public void DisableMovement()
		{
			this._velocityDisabled = true;
		}

		public void EnableMovement()
		{
			if (!this.IgnoringVelocity)
			{
				return;
			}
			this._velocityDisabled = false;
			this._isHeadInHotspot = false;
			this._headHotspotCenter = null;
			this._velocity = Vector3.zero;
			RaycastHit hit;
			if (this.CalculateGround(out hit) && this.IsFlat(hit.normal))
			{
				Vector3 position = this._capsule.transform.position;
				float num;
				this.RaycastHitPlane(hit, position, Vector3.down, out num);
				position.y = position.y - num + this._capsule.height * 0.5f + this._skinWidth;
				this._capsule.transform.position = position;
			}
		}

		private bool TryExitHotspot(bool force = false)
		{
			if (this._isHeadInHotspot && this._headHotspotCenter != null && (force || this.IsHeadFarFromPoint(this._headHotspotCenter.Value, this._exitHotspotDistance)))
			{
				this.EnableMovement();
				return true;
			}
			return false;
		}

		private void UpdateCharacterHeight()
		{
			float height = this._capsule.height;
			float a = this._heightOffset + (this._isCrouching ? this._crouchHeightOffset : 0f) + ((this.ControllingPlayer && this._autoUpdateHeight) ? (this.GetPlayerHeadTop().y - this._playerOrigin.position.y) : this._defaultHeight);
			float skinWidth = this._skinWidth;
			float num = Mathf.Max(a, this._capsule.radius * 2f) - height;
			Vector3 vector;
			if (num > skinWidth && this.CheckMoveCharacter(Vector3.up * num, out vector))
			{
				num = Mathf.Max(0f, vector.y - this._skinWidth);
			}
			if (Mathf.Abs(num) <= skinWidth)
			{
				return;
			}
			this._capsule.height = height + num;
			this._capsule.transform.position += Vector3.up * num * 0.5f;
		}

		private void CatchUpCharacterToPlayer()
		{
			if (!this.ControllingPlayer)
			{
				return;
			}
			Vector3 delta = Vector3.ProjectOnPlane(this.GetPlayerHead() - this._capsule.transform.position, Vector3.up);
			this.MoveCharacter(delta);
			Vector3 forward = Vector3.ProjectOnPlane(this._playerEyes.forward, Vector3.up);
			this._capsule.transform.rotation = Quaternion.LookRotation(forward, Vector3.up);
		}

		private void CatchUpPlayerToCharacter(Pose delta, float feetHeight)
		{
			if (!this.ControllingPlayer)
			{
				return;
			}
			Pose pose = this._capsule.transform.GetPose(Space.World);
			Vector3 playerHead = this.GetPlayerHead();
			this._playerOrigin.rotation = delta.rotation * this._playerOrigin.rotation;
			this._playerOrigin.position = this._playerOrigin.position + playerHead - this.GetPlayerHead();
			Vector3 b = Vector3.ProjectOnPlane(delta.position, Vector3.up);
			Vector3 position = this._playerOrigin.position + b;
			position.y = feetHeight + (this._isCrouching ? this._crouchHeightOffset : 0f) + this._heightOffset;
			this._playerOrigin.position = position;
			this._capsule.transform.SetPose(pose, Space.World);
		}

		public void ResetPlayerToCharacter()
		{
			if (!this.ControllingPlayer)
			{
				return;
			}
			Pose pose = this._capsule.transform.GetPose(Space.World);
			Vector3 characterFeet = this.GetCharacterFeet();
			Vector3 b = this._playerOrigin.position - this.GetPlayerHead();
			b.y = 0f;
			this._playerOrigin.position = characterFeet + b;
			this._accumulatedDeltaFrame = Pose.identity;
			this._capsule.transform.SetPose(pose, Space.World);
		}

		private void UpdateVelocity()
		{
			float num = this._deltaTimeProvider();
			if (this._isGrounded && this._velocity.y <= 0f)
			{
				this._velocity *= 1f / (1f + this._groundDamping * num);
				this._velocity.y = 0f;
				return;
			}
			float num2 = 1f / (1f + this._airDamping * num);
			this._velocity.x = this._velocity.x * num2;
			this._velocity.z = this._velocity.z * num2;
			if (this._velocity.y > 0f)
			{
				this._velocity.y = this._velocity.y * (1f / (1f + this._jumpDamping * num));
			}
			this._velocity += Physics.gravity * this._gravityFactor * num;
		}

		private void MoveCharacter(Vector3 delta)
		{
			if (this._isGrounded)
			{
				delta = Vector3.ProjectOnPlane(Vector3.ProjectOnPlane(delta, Vector3.up), this._groundHit.normal) + Vector3.up * delta.y;
			}
			Vector3 vector = this.Rebound(delta, this._maxReboundSteps);
			this._capsule.transform.position += vector;
			this.UpdateGrounded(delta.y < 0f && delta.y < vector.y && Mathf.Abs(vector.y) < 0.001f);
		}

		private Vector3 Rebound(Vector3 delta, int bounces)
		{
			Vector3 b = Vector3.up * Mathf.Max(0f, this._capsule.height * 0.5f - this._capsule.radius);
			Vector3 capsuleTop = this._capsule.transform.position + b;
			Vector3 capsuleBase = this._capsule.transform.position - b;
			Vector3 originalFlatDelta = Vector3.ProjectOnPlane(delta, Vector3.up);
			return this.<Rebound>g__ReboundRecursive|148_0(capsuleBase, capsuleTop, this._capsule.radius, delta, originalFlatDelta, bounces);
		}

		private bool ClimbStep(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, out Vector3 climbDelta, out RaycastHit? stepHit)
		{
			stepHit = null;
			climbDelta = Vector3.zero;
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
			if (!this.CalculateGround(capsuleTop + delta, radius, this._capsule.height - radius, out raycastHit2) || !CapsuleLocomotionHandler.RaycastSphere(raycastHit2.point, Vector3.up, vector + delta, radius + this._skinWidth, out num2) || raycastHit2.point.y - (capsuleBase.y - radius) > this._maxStep || !this.IsFlat(raycastHit2.normal))
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
			float num = Mathf.Max(0f, Vector3.Dot(normalized, -hit.normal)) * this._skinWidth;
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
			this._isGrounded = (this.CalculateGround(out this._groundHit) && this.IsFlat(this._groundHit.normal));
			if (!this._isGrounded && forceGrounded)
			{
				this._isGrounded = true;
				this._groundHit.normal = Vector3.up;
				this._groundHit.point = this._capsule.transform.position + Vector3.down * (this._capsule.height * 0.5f + this._skinWidth);
			}
		}

		private bool CalculateGround(out RaycastHit groundHit)
		{
			Vector3 origin = this._capsule.transform.position + Vector3.down * (this._capsule.height * 0.5f - this._capsule.radius);
			return this.CalculateGround(origin, this._capsule.radius + this._skinWidth, this._capsule.radius + this._skinWidth, out groundHit) || this.CalculateGround(this._capsule.transform.position, this._capsule.radius + this._skinWidth, this._capsule.height * 0.5f + this._skinWidth, out groundHit);
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
			if (this._logicalHead != null)
			{
				this._logicalHead.transform.SetPositionAndRotation(this.GetCharacterHead(), this._capsule.transform.rotation);
			}
			if (this._logicalFeet != null)
			{
				this._logicalFeet.transform.SetPositionAndRotation(this.GetCharacterFeet(), this._capsule.transform.rotation);
			}
		}

		private float GetModifiedSpeedFactor()
		{
			if (!this._isGrounded || this._velocity.y > 0f)
			{
				return 0f;
			}
			return this._acceleration * (this._isCrouching ? this._crouchSpeedFactor : (this._isRunning ? this._runningSpeedFactor : this._speedFactor)) * this._deltaTimeProvider();
		}

		private Vector3 GetCharacterFeet()
		{
			return this._capsule.transform.position + Vector3.down * (this._capsule.height * 0.5f + this._skinWidth);
		}

		private Vector3 GetCharacterHead()
		{
			return this._capsule.transform.position + Vector3.up * (this._capsule.height * 0.5f - 0.1085f + this._skinWidth);
		}

		private Vector3 GetPlayerHead()
		{
			return this._playerEyes.position - this._playerEyes.forward * 0.0965f;
		}

		private Vector3 GetPlayerHeadTop()
		{
			return this.GetPlayerHead() + Vector3.up * 0.1085f;
		}

		private bool IsHeadFarFromPoint(Vector3 point, float maxDistance)
		{
			return Vector3.ProjectOnPlane((this.ControllingPlayer ? this.GetPlayerHead() : this.GetCharacterHead()) - point, Vector3.up).sqrMagnitude >= maxDistance * maxDistance;
		}

		private IEnumerator EndOfFrameCoroutine()
		{
			for (;;)
			{
				yield return this._endOfFrame;
				this.LastUpdate();
			}
			yield break;
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

		public void InjectAllCapsuleLocomotionHandler(CapsuleCollider capsule)
		{
			this.InjectCapsule(capsule);
		}

		public void InjectCapsule(CapsuleCollider capsule)
		{
			this._capsule = capsule;
		}

		public void InjectOptionalPlayerEyes(Transform playerEyes)
		{
			this._playerEyes = playerEyes;
		}

		public void InjectOptionalPlayerOrigin(Transform playerOrigin)
		{
			this._playerOrigin = playerOrigin;
		}

		[CompilerGenerated]
		private Vector3 <Rebound>g__ReboundRecursive|148_0(Vector3 capsuleBase, Vector3 capsuleTop, float radius, Vector3 delta, Vector3 originalFlatDelta, int bounceStep)
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
			Vector3 b;
			if (this._isGrounded && raycastHit != null && raycastHit.Value.point.y - (capsuleBase.y - radius - this._skinWidth) <= this._maxStep && this.ClimbStep(capsuleBase, capsuleTop, radius, delta2, out b, out raycastHit2))
			{
				if (raycastHit2 != null)
				{
					delta2 = this.DecomposeDelta(delta2, raycastHit2.Value).Item2;
					delta2 = this.SlideDelta(delta2, originalFlatDelta, raycastHit2.Value);
				}
				else
				{
					delta2 = Vector3.zero;
				}
				capsuleBase += b;
				capsuleTop += b;
				a += b;
			}
			if (raycastHit != null && raycastHit2 == null)
			{
				delta2 = this.SlideDelta(delta2, originalFlatDelta, raycastHit.Value);
			}
			return a + this.<Rebound>g__ReboundRecursive|148_0(capsuleBase, capsuleTop, radius, delta2, originalFlatDelta, bounceStep - 1);
		}

		[Header("Character")]
		[SerializeField]
		[Tooltip("Capsule collider that represents the character and will be moved by the locomotor.")]
		private CapsuleCollider _capsule;

		[SerializeField]
		[Min(0f)]
		[Tooltip("Extra offset added to the radius of the capsule for soft collisions.")]
		private float _skinWidth = 0.02f;

		[SerializeField]
		[Tooltip("LayerMask check for collisions when moving.")]
		private LayerMask _layerMask = -1;

		[Header("VR Player (Optional)")]
		[SerializeField]
		[Optional]
		[Tooltip("Optional. Root of the actual VR player so it can be sync with with capsule. If you provided a _playerEyes you must also provide a _playerOrigin.")]
		private Transform _playerOrigin;

		[SerializeField]
		[Optional]
		[Tooltip("Optional. Eyes of the actual VR player so it can be sync with the capsule. If you provided a _playerOrigin you must also provide a _playerEyes.")]
		private Transform _playerEyes;

		[SerializeField]
		[Tooltip("After the player penetrates the head inside a collider (for example a wall), the maximum distance before the player gets reset to the capsule position when trying to move synthetically.")]
		private float _maxWallPenetrationDistance = 0.3f;

		[SerializeField]
		[Tooltip("After using LocomotionEvent.TranslationType.AbsoluteEyeLevel that disables the ground checks. What is the maximum deviation of the player before the physics are re-enabled.")]
		private float _exitHotspotDistance = 0.3f;

		[SerializeField]
		[Tooltip("When _playerOrigin and _playerEyes are present. This will force the capsule height to update using the actual player height, instead of using _defaultHeight")]
		private bool _autoUpdateHeight = true;

		[Header("Parameters")]
		[SerializeField]
		[Range(0f, 90f)]
		[Tooltip("Max climbable slope angle in degrees.")]
		private float _maxSlopeAngle = 45f;

		[SerializeField]
		[Min(0f)]
		[Tooltip("Max climbable height for steps.")]
		private float _maxStep = 0.1f;

		[SerializeField]
		[Tooltip("Height of the character capsule when standing normally. This might be overriden by _autoUpdateHeight")]
		private float _defaultHeight = 1.4f;

		[SerializeField]
		[Tooltip("General height offset applied to the capsule.")]
		private float _heightOffset;

		[SerializeField]
		[Tooltip("Height offset added while crouching.")]
		private float _crouchHeightOffset = -0.5f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while moving normally.")]
		private float _speedFactor = 1f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while crouching.")]
		private float _crouchSpeedFactor = 0.5f;

		[SerializeField]
		[Tooltip("Speed multiplier applied while running.")]
		private float _runningSpeedFactor = 2f;

		[SerializeField]
		[Tooltip("The rate of acceleration during movement.")]
		private float _acceleration = 70f;

		[SerializeField]
		[Tooltip("The rate of damping on movement while grounded.")]
		private float _groundDamping = 30f;

		[SerializeField]
		[Tooltip("The rate of damping on the vertical movement while jumping.")]
		private float _jumpDamping = 30f;

		[SerializeField]
		[Tooltip("The rate of damping on the horizontal movement while in the air.")]
		private float _airDamping = 5f;

		[SerializeField]
		[Tooltip("The force applied to the character when jumping.")]
		private float _jumpForce = 100f;

		[SerializeField]
		[Tooltip("Modifies the strength of gravity.")]
		private float _gravityFactor = 1f;

		[SerializeField]
		[Min(1f)]
		[Tooltip("Max iterations for sliding the delta movement after colliding with an obstacle.")]
		private int _maxReboundSteps = 3;

		[SerializeField]
		[Tooltip("When Velocity is ignored the character will not try to catch up to the player and the character won't slide or fall.It is preferred to re-enable the movement by calling EnableMovement instead of setting this variable to false directly.")]
		private bool _velocityDisabled;

		[Header("Anchors")]
		[SerializeField]
		[Optional]
		[Tooltip("Optional. This transform pose will be updated with the pose of the character head.")]
		private Transform _logicalHead;

		[SerializeField]
		[Optional]
		[Tooltip("Optional. This transform pose will be updated with the pose of the character feet.")]
		private Transform _logicalFeet;

		private Func<float> _deltaTimeProvider = () => Time.deltaTime;

		protected Action<LocomotionEvent, Pose> _whenLocomotionEventHandled = delegate(LocomotionEvent <p0>, Pose <p1>)
		{
		};

		private Pose _accumulatedDeltaFrame;

		private Vector3 _velocity;

		private bool _isHeadInHotspot;

		private Vector3? _headHotspotCenter;

		private RaycastHit _groundHit;

		private bool _isGrounded;

		private bool _isRunning;

		private bool _isCrouching;

		private const float _sellionToTopOfHead = 0.1085f;

		private const float _sellionToBackOfHeadHalf = 0.0965f;

		private const float _cornerHitEpsilon = 0.001f;

		private Queue<LocomotionEvent> _deferredLocomotionEvent = new Queue<LocomotionEvent>();

		private YieldInstruction _endOfFrame = new WaitForEndOfFrame();

		private Coroutine _endOfFrameRoutine;

		protected bool _started;
	}
}
