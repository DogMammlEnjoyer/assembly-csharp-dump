using System;
using System.Collections.Generic;
using Oculus.Interaction.Grab;
using Oculus.Interaction.Grab.GrabSurfaces;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public static class HandGrabUtils
	{
		public static HandGrabInteractable CreateHandGrabInteractable(Transform parent, string name = null)
		{
			GameObject gameObject = new GameObject(name ?? "HandGrabInteractable");
			gameObject.transform.SetParent(parent, false);
			gameObject.SetActive(false);
			HandGrabInteractable handGrabInteractable = gameObject.AddComponent<HandGrabInteractable>();
			handGrabInteractable.InjectRigidbody(parent.GetComponentInParent<Rigidbody>());
			handGrabInteractable.InjectOptionalPointableElement(parent.GetComponentInParent<Grabbable>());
			gameObject.SetActive(true);
			return handGrabInteractable;
		}

		public static HandGrabPose CreateHandGrabPose(Transform parent, Transform relativeTo)
		{
			GameObject gameObject = new GameObject("HandGrabPose");
			gameObject.transform.SetParent(parent, false);
			HandGrabPose handGrabPose = gameObject.AddComponent<HandGrabPose>();
			handGrabPose.InjectAllHandGrabPose(relativeTo);
			return handGrabPose;
		}

		public static void MirrorHandGrabPose(HandGrabPose originalPoint, HandGrabPose mirrorPoint, Transform relativeTo)
		{
			Handedness handedness = (originalPoint.HandPose.Handedness == Handedness.Left) ? Handedness.Right : Handedness.Left;
			HandGrabUtils.HandGrabPoseData handGrabPoseData = HandGrabUtils.SaveHandGrabPoseData(originalPoint);
			HandPose handPose = handGrabPoseData.handPose;
			handPose.Handedness = handedness;
			for (int i = 0; i < handPose.JointRotations.Length; i++)
			{
				handPose.JointRotations[i] = HandMirroring.Mirror(handPose.JointRotations[i]);
			}
			if (originalPoint.SnapSurface != null)
			{
				handGrabPoseData.gripPose = originalPoint.SnapSurface.MirrorPose(handGrabPoseData.gripPose, relativeTo);
			}
			else
			{
				Quaternion quaternion = Quaternion.Euler(180f, 0f, 180f);
				handGrabPoseData.gripPose = HandMirroring.Mirror(handGrabPoseData.gripPose);
				handGrabPoseData.gripPose.position = quaternion * handGrabPoseData.gripPose.position;
				handGrabPoseData.gripPose.rotation = quaternion * handGrabPoseData.gripPose.rotation;
			}
			HandGrabUtils.LoadHandGrabPoseData(mirrorPoint, handGrabPoseData, relativeTo);
			if (originalPoint.SnapSurface != null)
			{
				IGrabSurface surface = originalPoint.SnapSurface.CreateMirroredSurface(mirrorPoint.gameObject);
				mirrorPoint.InjectOptionalSurface(surface);
			}
		}

		private static HandGrabUtils.HandGrabPoseData SaveHandGrabPoseData(HandGrabPose handGrabPose)
		{
			return new HandGrabUtils.HandGrabPoseData
			{
				handPose = new HandPose(handGrabPose.HandPose),
				scale = handGrabPose.RelativeScale,
				gripPose = handGrabPose.RelativePose
			};
		}

		private static void LoadHandGrabPoseData(HandGrabPose handGrabPose, HandGrabUtils.HandGrabPoseData data, Transform relativeTo)
		{
			handGrabPose.transform.localScale = Vector3.one * data.scale;
			Transform transform = handGrabPose.transform;
			Pose pose = PoseUtils.GlobalPoseScaled(relativeTo, data.gripPose);
			transform.SetPose(pose, Space.World);
			if (data.handPose != null)
			{
				handGrabPose.InjectOptionalHandPose(new HandPose(data.handPose));
			}
		}

		public static HandGrabUtils.HandGrabInteractableData SaveData(HandGrabInteractable interactable)
		{
			List<HandGrabUtils.HandGrabPoseData> list = new List<HandGrabUtils.HandGrabPoseData>();
			foreach (HandGrabPose handGrabPose in interactable.HandGrabPoses)
			{
				list.Add(HandGrabUtils.SaveHandGrabPoseData(handGrabPose));
			}
			return new HandGrabUtils.HandGrabInteractableData
			{
				poses = list,
				scoringModifier = interactable.ScoreModifier,
				grabType = interactable.SupportedGrabTypes,
				handAlignment = interactable.HandAlignment,
				pinchGrabRules = interactable.PinchGrabRules,
				palmGrabRules = interactable.PalmGrabRules
			};
		}

		public static void LoadData(HandGrabInteractable interactable, HandGrabUtils.HandGrabInteractableData data)
		{
			interactable.InjectSupportedGrabTypes(data.grabType);
			interactable.InjectPinchGrabRules(data.pinchGrabRules);
			interactable.InjectPalmGrabRules(data.palmGrabRules);
			interactable.InjectOptionalScoreModifier(data.scoringModifier);
			interactable.HandAlignment = data.handAlignment;
			if (data.poses == null)
			{
				return;
			}
			List<HandGrabPose> list = new List<HandGrabPose>();
			foreach (HandGrabUtils.HandGrabPoseData poseData in data.poses)
			{
				list.Add(HandGrabUtils.LoadHandGrabPose(interactable, poseData));
			}
			interactable.InjectOptionalHandGrabPoses(list);
		}

		public static HandGrabPose LoadHandGrabPose(HandGrabInteractable interactable, HandGrabUtils.HandGrabPoseData poseData)
		{
			HandGrabPose handGrabPose = HandGrabUtils.CreateHandGrabPose(interactable.transform, interactable.RelativeTo);
			HandGrabUtils.LoadHandGrabPoseData(handGrabPose, poseData, interactable.RelativeTo);
			interactable.HandGrabPoses.Add(handGrabPose);
			return handGrabPose;
		}

		[Serializable]
		public struct HandGrabInteractableData
		{
			public List<HandGrabUtils.HandGrabPoseData> poses;

			public GrabTypeFlags grabType;

			public HandAlignType handAlignment;

			public PoseMeasureParameters scoringModifier;

			public GrabbingRule pinchGrabRules;

			public GrabbingRule palmGrabRules;
		}

		[Serializable]
		public struct HandGrabPoseData
		{
			public Pose gripPose;

			public HandPose handPose;

			public float scale;
		}
	}
}
