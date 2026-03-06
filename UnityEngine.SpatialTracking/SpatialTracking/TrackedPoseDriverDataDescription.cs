using System;
using System.Collections.Generic;

namespace UnityEngine.SpatialTracking
{
	internal class TrackedPoseDriverDataDescription
	{
		internal static List<TrackedPoseDriverDataDescription.PoseData> DeviceData = new List<TrackedPoseDriverDataDescription.PoseData>
		{
			new TrackedPoseDriverDataDescription.PoseData
			{
				PoseNames = new List<string>
				{
					"Left Eye",
					"Right Eye",
					"Center Eye - HMD Reference",
					"Head",
					"Color Camera"
				},
				Poses = new List<TrackedPoseDriver.TrackedPose>
				{
					TrackedPoseDriver.TrackedPose.LeftEye,
					TrackedPoseDriver.TrackedPose.RightEye,
					TrackedPoseDriver.TrackedPose.Center,
					TrackedPoseDriver.TrackedPose.Head,
					TrackedPoseDriver.TrackedPose.ColorCamera
				}
			},
			new TrackedPoseDriverDataDescription.PoseData
			{
				PoseNames = new List<string>
				{
					"Left Controller",
					"Right Controller"
				},
				Poses = new List<TrackedPoseDriver.TrackedPose>
				{
					TrackedPoseDriver.TrackedPose.LeftPose,
					TrackedPoseDriver.TrackedPose.RightPose
				}
			},
			new TrackedPoseDriverDataDescription.PoseData
			{
				PoseNames = new List<string>
				{
					"Device Pose"
				},
				Poses = new List<TrackedPoseDriver.TrackedPose>
				{
					TrackedPoseDriver.TrackedPose.RemotePose
				}
			}
		};

		internal struct PoseData
		{
			public List<string> PoseNames;

			public List<TrackedPoseDriver.TrackedPose> Poses;
		}
	}
}
