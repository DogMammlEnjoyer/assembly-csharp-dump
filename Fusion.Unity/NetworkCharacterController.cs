using System;
using UnityEngine;

namespace Fusion
{
	[DisallowMultipleComponent]
	[RequireComponent(typeof(CharacterController))]
	[NetworkBehaviourWeaved(18)]
	public sealed class NetworkCharacterController : NetworkTRSP, INetworkTRSPTeleport, IBeforeAllTicks, IPublicFacingInterface, IAfterAllTicks, IBeforeCopyPreviousState
	{
		private new ref NetworkCCData Data
		{
			get
			{
				return base.ReinterpretState<NetworkCCData>(0);
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return this.Data.Velocity;
			}
			set
			{
				this.Data.Velocity = value;
			}
		}

		public bool Grounded
		{
			get
			{
				return this.Data.Grounded;
			}
			set
			{
				this.Data.Grounded = value;
			}
		}

		public void Teleport(Vector3? position = null, Quaternion? rotation = null)
		{
			this._controller.enabled = false;
			NetworkTRSP.Teleport(this, base.transform, position, rotation);
			this._controller.enabled = true;
		}

		public void Jump(bool ignoreGrounded = false, float? overrideImpulse = null)
		{
			if (this.Data.Grounded || ignoreGrounded)
			{
				Vector3 velocity = this.Data.Velocity;
				velocity.y += (overrideImpulse ?? this.jumpImpulse);
				this.Data.Velocity = velocity;
			}
		}

		public void Move(Vector3 direction)
		{
			float deltaTime = base.Runner.DeltaTime;
			Vector3 position = base.transform.position;
			Vector3 velocity = this.Data.Velocity;
			direction = direction.normalized;
			if (this.Data.Grounded && velocity.y < 0f)
			{
				velocity.y = 0f;
			}
			velocity.y += this.gravity * base.Runner.DeltaTime;
			Vector3 vector = default(Vector3);
			vector.x = velocity.x;
			vector.z = velocity.z;
			if (direction == default(Vector3))
			{
				vector = Vector3.Lerp(vector, default(Vector3), this.braking * deltaTime);
			}
			else
			{
				vector = Vector3.ClampMagnitude(vector + direction * this.acceleration * deltaTime, this.maxSpeed);
				base.transform.rotation = Quaternion.Slerp(base.transform.rotation, Quaternion.LookRotation(direction), this.rotationSpeed * base.Runner.DeltaTime);
			}
			velocity.x = vector.x;
			velocity.z = vector.z;
			this._controller.Move(velocity * deltaTime);
			this.Data.Velocity = (base.transform.position - position) * (float)base.Runner.TickRate;
			this.Data.Grounded = this._controller.isGrounded;
		}

		public override void Spawned()
		{
			this._initial = default(Tick);
			base.TryGetComponent<CharacterController>(out this._controller);
			this._controller.enabled = false;
			this._controller.enabled = true;
			this.CopyToBuffer();
		}

		public override void Render()
		{
			NetworkTRSP.Render(this, base.transform, false, false, false, ref this._initial);
		}

		void IBeforeAllTicks.BeforeAllTicks(bool resimulation, int tickCount)
		{
			this.CopyToEngine();
		}

		void IAfterAllTicks.AfterAllTicks(bool resimulation, int tickCount)
		{
			this.CopyToBuffer();
		}

		void IBeforeCopyPreviousState.BeforeCopyPreviousState()
		{
			this.CopyToBuffer();
		}

		private void Awake()
		{
			base.TryGetComponent<CharacterController>(out this._controller);
		}

		private void CopyToBuffer()
		{
			this.Data.TRSPData.Position = base.transform.position;
			this.Data.TRSPData.Rotation = base.transform.rotation;
		}

		private void CopyToEngine()
		{
			this._controller.enabled = false;
			base.transform.SetPositionAndRotation(this.Data.TRSPData.Position, this.Data.TRSPData.Rotation);
			this._controller.enabled = true;
		}

		[Header("Character Controller Settings")]
		public float gravity = -20f;

		public float jumpImpulse = 8f;

		public float acceleration = 10f;

		public float braking = 10f;

		public float maxSpeed = 2f;

		public float rotationSpeed = 15f;

		private Tick _initial;

		private CharacterController _controller;
	}
}
