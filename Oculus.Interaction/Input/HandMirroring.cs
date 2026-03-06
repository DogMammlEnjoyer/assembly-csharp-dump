using System;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	public static class HandMirroring
	{
		public static Pose Mirror(Pose pose)
		{
			pose.position = HandMirroring.Mirror(pose.position);
			pose.rotation = HandMirroring.Mirror(pose.rotation);
			return pose;
		}

		public static Vector3 Mirror(in Vector3 position)
		{
			return HandMirroring.TransformPosition(position, HandMirroring.LeftHandSpace, HandMirroring.RightHandSpace);
		}

		public static Quaternion Mirror(in Quaternion rotation)
		{
			return HandMirroring.TransformRotation(rotation, HandMirroring.LeftHandSpace, HandMirroring.RightHandSpace);
		}

		public static Quaternion Reflect(in Quaternion rotation, Vector3 normal)
		{
			Vector3 forward = Vector3.Reflect(rotation * HandMirroring.RightHandSpace.distal, normal);
			Vector3 upwards = Vector3.Reflect(rotation * HandMirroring.RightHandSpace.dorsal, normal);
			return Quaternion.LookRotation(forward, upwards) * Quaternion.Inverse(HandMirroring.LeftHandSpace.rotation);
		}

		public static Pose TransformPose(in Pose pose, in HandMirroring.HandSpace fromHand, in HandMirroring.HandSpace toHand)
		{
			return new Pose(HandMirroring.TransformPosition(pose.position, fromHand, toHand), HandMirroring.TransformRotation(pose.rotation, fromHand, toHand));
		}

		public static Vector3 TransformPosition(in Vector3 position, in HandMirroring.HandSpace fromHand, in HandMirroring.HandSpace toHand)
		{
			Vector3 a = Vector3.Dot(position, fromHand.distal) * toHand.distal;
			Vector3 b = Vector3.Dot(position, fromHand.dorsal) * toHand.dorsal;
			Vector3 b2 = Vector3.Dot(position, fromHand.thumbSide) * toHand.thumbSide;
			return a + b + b2;
		}

		public static Quaternion TransformRotation(in Quaternion rotation, in HandMirroring.HandSpace fromHand, in HandMirroring.HandSpace toHand)
		{
			Vector3 vector = rotation * Vector3.forward;
			Vector3 forward = HandMirroring.TransformPosition(vector, fromHand, toHand);
			vector = rotation * Vector3.up;
			Vector3 upwards = HandMirroring.TransformPosition(vector, fromHand, toHand);
			return Quaternion.LookRotation(forward, upwards) * Quaternion.Inverse(toHand.rotation) * fromHand.rotation;
		}

		public static readonly HandMirroring.HandSpace LeftHandSpace = new HandMirroring.HandSpace(Constants.LeftDistal, Constants.LeftDorsal, Constants.LeftThumbSide);

		public static readonly HandMirroring.HandSpace RightHandSpace = new HandMirroring.HandSpace(Constants.RightDistal, Constants.RightDorsal, Constants.RightThumbSide);

		public struct HandsSpace
		{
			public readonly HandMirroring.HandSpace this[Handedness handedness]
			{
				get
				{
					if (handedness != Handedness.Left)
					{
						return this._rightHand;
					}
					return this._leftHand;
				}
			}

			public HandsSpace(HandMirroring.HandSpace leftHand, HandMirroring.HandSpace rightHand)
			{
				this._leftHand = leftHand;
				this._rightHand = rightHand;
			}

			private readonly HandMirroring.HandSpace _leftHand;

			private readonly HandMirroring.HandSpace _rightHand;
		}

		public struct HandSpace
		{
			public HandSpace(Vector3 distal, Vector3 dorsal, Vector3 thumbSide)
			{
				this.distal = distal;
				this.dorsal = dorsal;
				this.thumbSide = thumbSide;
				this.rotation = Quaternion.LookRotation(distal, dorsal);
			}

			public readonly Vector3 distal;

			public readonly Vector3 dorsal;

			public readonly Vector3 thumbSide;

			public readonly Quaternion rotation;
		}
	}
}
