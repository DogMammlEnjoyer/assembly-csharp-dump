using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public struct HandSphere
	{
		public readonly Vector3 Position { get; }

		public readonly float Radius { get; }

		public readonly HandJointId Joint { get; }

		public HandSphere(Vector3 position, float radius, HandJointId joint)
		{
			this.Position = position;
			this.Radius = radius;
			this.Joint = joint;
		}
	}
}
