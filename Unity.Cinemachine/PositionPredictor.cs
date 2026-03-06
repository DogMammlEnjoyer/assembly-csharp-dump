using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public struct PositionPredictor
	{
		public bool IsEmpty
		{
			get
			{
				return !this.m_HavePos;
			}
		}

		public Vector3 CurrentPosition
		{
			get
			{
				return this.m_Pos;
			}
		}

		public void ApplyTransformDelta(Vector3 positionDelta)
		{
			this.m_Pos += positionDelta;
		}

		public void ApplyRotationDelta(Quaternion rotationDelta)
		{
			this.m_Velocity = rotationDelta * this.m_Velocity;
			this.m_SmoothDampVelocity = rotationDelta * this.m_SmoothDampVelocity;
		}

		public void Reset()
		{
			this.m_HavePos = false;
			this.m_SmoothDampVelocity = Vector3.zero;
			this.m_Velocity = Vector3.zero;
		}

		public void AddPosition(Vector3 pos, float deltaTime)
		{
			if (deltaTime < 0f)
			{
				this.Reset();
			}
			if (this.m_HavePos && deltaTime > 0.0001f)
			{
				Vector3 target = (pos - this.m_Pos) / deltaTime;
				bool flag = target.sqrMagnitude < this.m_Velocity.sqrMagnitude;
				this.m_Velocity = Vector3.SmoothDamp(this.m_Velocity, target, ref this.m_SmoothDampVelocity, this.Smoothing / (float)(flag ? 30 : 10), float.PositiveInfinity, deltaTime);
			}
			this.m_Pos = pos;
			this.m_HavePos = true;
		}

		public Vector3 PredictPositionDelta(float lookaheadTime)
		{
			return this.m_Velocity * lookaheadTime;
		}

		private Vector3 m_Velocity;

		private Vector3 m_SmoothDampVelocity;

		private Vector3 m_Pos;

		private bool m_HavePos;

		public float Smoothing;
	}
}
