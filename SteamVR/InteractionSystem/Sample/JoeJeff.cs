using System;
using UnityEngine;

namespace Valve.VR.InteractionSystem.Sample
{
	public class JoeJeff : MonoBehaviour
	{
		private void Start()
		{
			this.animator = base.GetComponent<Animator>();
			this.rigidbody = base.GetComponent<Rigidbody>();
			this.interactable = base.GetComponent<Interactable>();
			this.animator.speed = this.animationSpeed;
		}

		private void Update()
		{
			this.held = (this.interactable.attachedToHand != null);
			this.jumpTimer -= Time.deltaTime;
			this.CheckGrounded();
			this.rigidbody.freezeRotation = !this.held;
			if (!this.held)
			{
				this.FixRotation();
			}
		}

		private void FixRotation()
		{
			Vector3 eulerAngles = base.transform.eulerAngles;
			eulerAngles.x = 0f;
			eulerAngles.z = 0f;
			Quaternion b = Quaternion.Euler(eulerAngles);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, b, Time.deltaTime * (float)(this.isGrounded ? 20 : 3));
		}

		public void OnAnimatorMove()
		{
			if (Time.deltaTime > 0f)
			{
				Vector3 vector = this.animator.deltaPosition / Time.deltaTime;
				vector = Vector3.ProjectOnPlane(vector, this.footHit.normal);
				if (this.isGrounded && this.jumpTimer < 0f)
				{
					if (this.groundedTime < this.frictionTime)
					{
						float num = Mathf.InverseLerp(0f, this.frictionTime, this.groundedTime);
						Vector3 vector2 = Vector3.Lerp(this.rigidbody.linearVelocity, vector, num * Time.deltaTime * 30f);
						vector.x = vector2.x;
						vector.z = vector2.z;
					}
					vector.y += -0.2f;
					this.rigidbody.linearVelocity = vector;
					return;
				}
				this.rigidbody.linearVelocity += this.input * Time.deltaTime * this.airControl;
			}
		}

		public void Move(Vector3 move, bool jump)
		{
			this.input = move;
			if (move.magnitude > 1f)
			{
				move.Normalize();
			}
			move = base.transform.InverseTransformDirection(move);
			this.turnAmount = Mathf.Atan2(move.x, move.z);
			this.forwardAmount = move.z;
			this.ApplyExtraTurnRotation();
			if (this.isGrounded)
			{
				this.HandleGroundedMovement(jump);
			}
			this.UpdateAnimator(move);
		}

		private void UpdateAnimator(Vector3 move)
		{
			this.animator.speed = (this.fire.isBurning ? (this.animationSpeed * 2f) : this.animationSpeed);
			this.animator.SetFloat("Forward", this.fire.isBurning ? 2f : this.forwardAmount, 0.1f, Time.deltaTime);
			this.animator.SetFloat("Turn", this.turnAmount, 0.1f, Time.deltaTime);
			this.animator.SetBool("OnGround", this.isGrounded);
			this.animator.SetBool("Holding", this.held);
			if (!this.isGrounded)
			{
				this.animator.SetFloat("FallSpeed", Mathf.Abs(this.rigidbody.linearVelocity.y));
				this.animator.SetFloat("Jump", this.rigidbody.linearVelocity.y);
			}
		}

		private void ApplyExtraTurnRotation()
		{
			float num = Mathf.Lerp(this.m_StationaryTurnSpeed, this.m_MovingTurnSpeed, this.forwardAmount);
			base.transform.Rotate(0f, this.turnAmount * num * Time.deltaTime, 0f);
		}

		private void CheckGrounded()
		{
			this.isGrounded = false;
			if (this.jumpTimer < 0f & !this.held)
			{
				this.isGrounded = Physics.SphereCast(new Ray(base.transform.position + Vector3.up * this.footHeight, Vector3.down), this.footRadius, out this.footHit, this.footHeight - this.footRadius);
				if (Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(this.footHit.point.x, this.footHit.point.z)) > this.footRadius / 2f)
				{
					this.isGrounded = false;
				}
			}
		}

		private void FixedUpdate()
		{
			this.groundedTime += Time.fixedDeltaTime;
			if (!this.isGrounded)
			{
				this.groundedTime = 0f;
			}
			if (this.isGrounded & !this.held)
			{
				Debug.DrawLine(base.transform.position, this.footHit.point);
				this.rigidbody.position = new Vector3(this.rigidbody.position.x, this.footHit.point.y, this.rigidbody.position.z);
			}
		}

		private void HandleGroundedMovement(bool jump)
		{
			if (jump && this.isGrounded)
			{
				this.Jump();
			}
		}

		public void Jump()
		{
			this.isGrounded = false;
			this.jumpTimer = 0.1f;
			this.animator.applyRootMotion = false;
			this.rigidbody.position += Vector3.up * 0.03f;
			Vector3 linearVelocity = this.rigidbody.linearVelocity;
			linearVelocity.y = this.jumpVelocity;
			this.rigidbody.linearVelocity = linearVelocity;
		}

		public float animationSpeed;

		public float jumpVelocity;

		[SerializeField]
		private float m_MovingTurnSpeed = 360f;

		[SerializeField]
		private float m_StationaryTurnSpeed = 180f;

		public float airControl;

		[Tooltip("The time it takes after landing a jump to slow down")]
		public float frictionTime = 0.2f;

		[SerializeField]
		private float footHeight = 0.1f;

		[SerializeField]
		private float footRadius = 0.03f;

		private RaycastHit footHit;

		private bool isGrounded;

		private float turnAmount;

		private float forwardAmount;

		private float groundedTime;

		private Animator animator;

		private Vector3 input;

		private bool held;

		private Rigidbody rigidbody;

		private Interactable interactable;

		public FireSource fire;

		private float jumpTimer;
	}
}
