using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab.Visuals
{
	[Serializable]
	public class HandJointMap
	{
		public Quaternion RotationOffset
		{
			get
			{
				return Quaternion.Euler(this.rotationOffset);
			}
		}

		public Quaternion TrackedRotation
		{
			get
			{
				return Quaternion.Inverse(this.RotationOffset) * this.transform.localRotation;
			}
		}

		public HandJointId id;

		public Transform transform;

		public Vector3 rotationOffset;
	}
}
