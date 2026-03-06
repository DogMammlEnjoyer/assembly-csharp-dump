using System;
using Pathfinding.Util;
using UnityEngine;
using UnityEngine.Serialization;

namespace Pathfinding
{
	[Serializable]
	public class AutoRepathPolicy
	{
		public virtual bool ShouldRecalculatePath(Vector3 position, float radius, Vector3 destination)
		{
			if (this.mode == AutoRepathPolicy.Mode.Never || float.IsPositiveInfinity(destination.x))
			{
				return false;
			}
			float num = Time.time - this.lastRepathTime;
			if (this.mode == AutoRepathPolicy.Mode.EveryNSeconds)
			{
				return num >= this.period;
			}
			float num2 = (destination - this.lastDestination).sqrMagnitude / Mathf.Max((position - this.lastDestination).sqrMagnitude, radius * radius) * (this.sensitivity * this.sensitivity);
			return num2 > 1f || float.IsNaN(num2) || num >= this.maximumPeriod * (1f - Mathf.Sqrt(num2));
		}

		public virtual void Reset()
		{
			this.lastRepathTime = float.NegativeInfinity;
		}

		public virtual void DidRecalculatePath(Vector3 destination)
		{
			this.lastRepathTime = Time.time;
			this.lastDestination = destination;
		}

		public void DrawGizmos(Vector3 position, float radius)
		{
			if (this.visualizeSensitivity && !float.IsPositiveInfinity(this.lastDestination.x))
			{
				float radius2 = Mathf.Sqrt(Mathf.Max((position - this.lastDestination).sqrMagnitude, radius * radius) / (this.sensitivity * this.sensitivity));
				Draw.Gizmos.CircleXZ(this.lastDestination, radius2, Color.magenta, 0f, 6.2831855f);
			}
		}

		public AutoRepathPolicy.Mode mode = AutoRepathPolicy.Mode.Dynamic;

		[FormerlySerializedAs("interval")]
		public float period = 0.5f;

		public float sensitivity = 10f;

		[FormerlySerializedAs("maximumInterval")]
		public float maximumPeriod = 2f;

		public bool visualizeSensitivity;

		private Vector3 lastDestination = new Vector3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);

		private float lastRepathTime = float.NegativeInfinity;

		public enum Mode
		{
			Never,
			EveryNSeconds,
			Dynamic
		}
	}
}
