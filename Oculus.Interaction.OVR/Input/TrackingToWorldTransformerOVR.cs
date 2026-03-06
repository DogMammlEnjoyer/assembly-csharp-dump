using System;
using Meta.XR.Util;
using UnityEngine;

namespace Oculus.Interaction.Input
{
	[Feature(Feature.Interaction)]
	public class TrackingToWorldTransformerOVR : MonoBehaviour, ITrackingToWorldTransformer
	{
		public IOVRCameraRigRef CameraRigRef { get; private set; }

		public Transform Transform
		{
			get
			{
				return this.CameraRigRef.CameraRig.trackingSpace;
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

		public Quaternion WorldToTrackingWristJointFixup
		{
			get
			{
				return FromOVRHandDataSource.WristFixupRotation;
			}
		}

		protected virtual void Awake()
		{
			this.CameraRigRef = (this._cameraRigRef as IOVRCameraRigRef);
		}

		protected virtual void Start()
		{
		}

		public void InjectAllTrackingToWorldTransformerOVR(IOVRCameraRigRef cameraRigRef)
		{
			this.InjectCameraRigRef(cameraRigRef);
		}

		public void InjectCameraRigRef(IOVRCameraRigRef cameraRigRef)
		{
			this._cameraRigRef = (cameraRigRef as Object);
			this.CameraRigRef = cameraRigRef;
		}

		Pose ITrackingToWorldTransformer.ToTrackingPose(in Pose worldPose)
		{
			return this.ToTrackingPose(worldPose);
		}

		[SerializeField]
		[Interface(typeof(IOVRCameraRigRef), new Type[]
		{

		})]
		private Object _cameraRigRef;
	}
}
