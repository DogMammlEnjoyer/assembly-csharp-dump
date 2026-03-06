using System;
using System.Collections.Generic;
using UnityEngine.XR;

namespace UnityEngine.SpatialTracking
{
	public static class PoseDataSource
	{
		internal static PoseDataFlags GetNodePoseData(XRNode node, out Pose resultPose)
		{
			PoseDataFlags poseDataFlags = PoseDataFlags.NoData;
			InputTracking.GetNodeStates(PoseDataSource.nodeStates);
			foreach (XRNodeState xrnodeState in PoseDataSource.nodeStates)
			{
				if (xrnodeState.nodeType == node)
				{
					if (xrnodeState.TryGetPosition(out resultPose.position))
					{
						poseDataFlags |= PoseDataFlags.Position;
					}
					if (xrnodeState.TryGetRotation(out resultPose.rotation))
					{
						poseDataFlags |= PoseDataFlags.Rotation;
					}
					return poseDataFlags;
				}
			}
			resultPose = Pose.identity;
			return poseDataFlags;
		}

		public static bool TryGetDataFromSource(TrackedPoseDriver.TrackedPose poseSource, out Pose resultPose)
		{
			return PoseDataSource.GetDataFromSource(poseSource, out resultPose) == (PoseDataFlags.Position | PoseDataFlags.Rotation);
		}

		public static PoseDataFlags GetDataFromSource(TrackedPoseDriver.TrackedPose poseSource, out Pose resultPose)
		{
			switch (poseSource)
			{
			case TrackedPoseDriver.TrackedPose.LeftEye:
				return PoseDataSource.GetNodePoseData(XRNode.LeftEye, out resultPose);
			case TrackedPoseDriver.TrackedPose.RightEye:
				return PoseDataSource.GetNodePoseData(XRNode.RightEye, out resultPose);
			case TrackedPoseDriver.TrackedPose.Center:
				return PoseDataSource.GetNodePoseData(XRNode.CenterEye, out resultPose);
			case TrackedPoseDriver.TrackedPose.Head:
				return PoseDataSource.GetNodePoseData(XRNode.Head, out resultPose);
			case TrackedPoseDriver.TrackedPose.LeftPose:
				return PoseDataSource.GetNodePoseData(XRNode.LeftHand, out resultPose);
			case TrackedPoseDriver.TrackedPose.RightPose:
				return PoseDataSource.GetNodePoseData(XRNode.RightHand, out resultPose);
			case TrackedPoseDriver.TrackedPose.ColorCamera:
				return PoseDataSource.GetNodePoseData(XRNode.CenterEye, out resultPose);
			case TrackedPoseDriver.TrackedPose.RemotePose:
			{
				PoseDataFlags nodePoseData = PoseDataSource.GetNodePoseData(XRNode.RightHand, out resultPose);
				if (nodePoseData == PoseDataFlags.NoData)
				{
					return PoseDataSource.GetNodePoseData(XRNode.LeftHand, out resultPose);
				}
				return nodePoseData;
			}
			}
			Debug.LogWarningFormat("Unable to retrieve pose data for poseSource: {0}", new object[]
			{
				poseSource.ToString()
			});
			resultPose = Pose.identity;
			return PoseDataFlags.NoData;
		}

		internal static List<XRNodeState> nodeStates = new List<XRNodeState>();
	}
}
