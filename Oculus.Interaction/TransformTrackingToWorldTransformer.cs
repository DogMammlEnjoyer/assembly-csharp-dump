using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class TransformTrackingToWorldTransformer : MonoBehaviour, ITrackingToWorldTransformer
	{
		public Transform Transform
		{
			get
			{
				return this.TrackingSpace;
			}
		}

		public Pose ToWorldPose(Pose pose)
		{
			Transform transform = this.Transform;
			pose.position = transform.TransformPoint(pose.position);
			pose.rotation = transform.rotation * pose.rotation;
			return pose;
		}

		public Pose ToTrackingPose(in Pose worldPose)
		{
			Transform transform = this.Transform;
			Vector3 position = transform.InverseTransformPoint(worldPose.position);
			Quaternion rotation = Quaternion.Inverse(transform.rotation) * worldPose.rotation;
			return new Pose(position, rotation);
		}

		public Quaternion WorldToTrackingWristJointFixup { get; } = new Quaternion(0f, 1f, 0f, 0f);

		Pose ITrackingToWorldTransformer.ToTrackingPose(in Pose worldPose)
		{
			return this.ToTrackingPose(worldPose);
		}

		[SerializeField]
		private Transform TrackingSpace;
	}
}
