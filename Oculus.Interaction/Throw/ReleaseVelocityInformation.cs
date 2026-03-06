using System;
using UnityEngine;

namespace Oculus.Interaction.Throw
{
	public struct ReleaseVelocityInformation
	{
		public ReleaseVelocityInformation(Vector3 linearVelocity, Vector3 angularVelocity, Vector3 origin, bool isSelectedVelocity = false)
		{
			this.LinearVelocity = linearVelocity;
			this.AngularVelocity = angularVelocity;
			this.Origin = origin;
			this.IsSelectedVelocity = isSelectedVelocity;
		}

		public Vector3 LinearVelocity;

		public Vector3 AngularVelocity;

		public Vector3 Origin;

		public bool IsSelectedVelocity;
	}
}
