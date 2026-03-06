using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public class MoveFromTarget : IMovement
	{
		public Pose Pose { get; private set; } = Pose.identity;

		public bool Stopped
		{
			get
			{
				return true;
			}
		}

		public void StopMovement()
		{
		}

		public void MoveTo(Pose target)
		{
			this.Pose = target;
		}

		public void UpdateTarget(Pose target)
		{
			this.Pose = target;
		}

		public void StopAndSetPose(Pose source)
		{
			this.Pose = source;
		}

		public void Tick()
		{
		}
	}
}
