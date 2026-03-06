using System;
using Oculus.Interaction.Grab;
using Oculus.Interaction.GrabAPI;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public static class HandGrabInteraction
	{
		[Obsolete("Use CalculateBestGrab instead")]
		public static bool TryCalculateBestGrab(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable interactable, GrabTypeFlags grabTypes, out HandGrabTarget.GrabAnchor anchorMode, ref HandGrabResult handGrabResult)
		{
			GrabTypeFlags grabTypeFlags;
			handGrabInteractor.CalculateBestGrab(interactable, grabTypes, out grabTypeFlags, ref handGrabResult);
			if (grabTypeFlags.HasFlag(GrabTypeFlags.Pinch))
			{
				anchorMode = HandGrabTarget.GrabAnchor.Pinch;
			}
			else if (grabTypeFlags.HasFlag(GrabTypeFlags.Palm))
			{
				anchorMode = HandGrabTarget.GrabAnchor.Palm;
			}
			else
			{
				anchorMode = HandGrabTarget.GrabAnchor.Wrist;
			}
			return true;
		}

		[Obsolete]
		public static GrabTypeFlags CurrentGrabType(this IHandGrabInteractor handGrabInteractor)
		{
			return handGrabInteractor.HandGrabTarget.Anchor;
		}

		public static void CalculateBestGrab(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable interactable, GrabTypeFlags grabFlags, out GrabTypeFlags activeGrabFlags, ref HandGrabResult result)
		{
			activeGrabFlags = (grabFlags & interactable.SupportedGrabTypes);
			Pose pose;
			Pose pose2;
			handGrabInteractor.GetPoseOffset(activeGrabFlags, out pose, out pose2);
			interactable.CalculateBestPose(pose, pose2, interactable.RelativeTo, handGrabInteractor.Hand.Scale, handGrabInteractor.Hand.Handedness, ref result);
		}

		public static IMovement GenerateMovement(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable interactable)
		{
			Pose targetGrabPose = handGrabInteractor.GetTargetGrabPose();
			Pose handGrabPose = handGrabInteractor.GetHandGrabPose();
			return interactable.GenerateMovement(targetGrabPose, handGrabPose);
		}

		public static Pose GetHandGrabPose(this IHandGrabInteractor handGrabInteractor)
		{
			Pose pose;
			Pose wristToGrabPoseOffset;
			handGrabInteractor.GetPoseOffset(GrabTypeFlags.None, out pose, out wristToGrabPoseOffset);
			wristToGrabPoseOffset = handGrabInteractor.WristToGrabPoseOffset;
			return PoseUtils.Multiply(pose, wristToGrabPoseOffset);
		}

		public static GrabPoseScore GetPoseScore(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable interactable, GrabTypeFlags grabTypes, ref HandGrabResult result)
		{
			GrabTypeFlags anchorMode = grabTypes & interactable.SupportedGrabTypes;
			Pose pose;
			Pose pose2;
			handGrabInteractor.GetPoseOffset(anchorMode, out pose, out pose2);
			interactable.CalculateBestPose(pose, pose2, interactable.RelativeTo, handGrabInteractor.Hand.Scale, handGrabInteractor.Hand.Handedness, ref result);
			return result.Score;
		}

		public static bool CanInteractWith(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			return handGrabInteractable.SupportsHandedness(handGrabInteractor.Hand.Handedness) && (handGrabInteractor.SupportedGrabTypes & handGrabInteractable.SupportedGrabTypes) > GrabTypeFlags.None;
		}

		public static Pose GetGrabOffset(this IHandGrabInteractor handGrabInteractor)
		{
			Pose pose;
			Pose result;
			handGrabInteractor.GetPoseOffset(handGrabInteractor.HandGrabTarget.Anchor, out pose, out result);
			return result;
		}

		public static float ComputeHandGrabScore(IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable, out GrabTypeFlags handGrabTypes, bool includeSelecting = false)
		{
			HandGrabAPI handGrabApi = handGrabInteractor.HandGrabApi;
			handGrabTypes = GrabTypeFlags.None;
			float num = 0f;
			if (HandGrabInteraction.SupportsPinch(handGrabInteractor, handGrabInteractable))
			{
				HandGrabAPI handGrabAPI = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PinchGrabRules;
				float handPinchScore = handGrabAPI.GetHandPinchScore(grabbingRule, includeSelecting);
				if (handPinchScore > num)
				{
					num = handPinchScore;
					handGrabTypes = GrabTypeFlags.Pinch;
				}
			}
			if (HandGrabInteraction.SupportsPalm(handGrabInteractor, handGrabInteractable))
			{
				HandGrabAPI handGrabAPI2 = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PalmGrabRules;
				float handPalmScore = handGrabAPI2.GetHandPalmScore(grabbingRule, includeSelecting);
				if (handPalmScore > num)
				{
					num = handPalmScore;
					handGrabTypes = GrabTypeFlags.Palm;
				}
			}
			return num;
		}

		public static GrabTypeFlags ComputeShouldSelect(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			if (handGrabInteractable == null)
			{
				return GrabTypeFlags.None;
			}
			HandGrabAPI handGrabApi = handGrabInteractor.HandGrabApi;
			GrabTypeFlags grabTypeFlags = GrabTypeFlags.None;
			if (HandGrabInteraction.SupportsPinch(handGrabInteractor, handGrabInteractable))
			{
				HandGrabAPI handGrabAPI = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PinchGrabRules;
				if (handGrabAPI.IsHandSelectPinchFingersChanged(grabbingRule))
				{
					grabTypeFlags |= GrabTypeFlags.Pinch;
				}
			}
			if (HandGrabInteraction.SupportsPalm(handGrabInteractor, handGrabInteractable))
			{
				HandGrabAPI handGrabAPI2 = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PalmGrabRules;
				if (handGrabAPI2.IsHandSelectPalmFingersChanged(grabbingRule))
				{
					grabTypeFlags |= GrabTypeFlags.Palm;
				}
			}
			return grabTypeFlags;
		}

		public static GrabTypeFlags ComputeShouldUnselect(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			HandGrabAPI handGrabApi = handGrabInteractor.HandGrabApi;
			HandFingerFlags grabbingFingers = handGrabApi.HandPinchGrabbingFingers();
			HandFingerFlags grabbingFingers2 = handGrabApi.HandPalmGrabbingFingers();
			if (handGrabInteractable.SupportedGrabTypes == GrabTypeFlags.None)
			{
				HandGrabAPI handGrabAPI = handGrabApi;
				GrabbingRule grabbingRule = GrabbingRule.FullGrab;
				if (!handGrabAPI.IsSustainingGrab(grabbingRule, grabbingFingers))
				{
					HandGrabAPI handGrabAPI2 = handGrabApi;
					GrabbingRule grabbingRule2 = GrabbingRule.FullGrab;
					if (!handGrabAPI2.IsSustainingGrab(grabbingRule2, grabbingFingers2))
					{
						return GrabTypeFlags.All;
					}
				}
				return GrabTypeFlags.None;
			}
			GrabTypeFlags grabTypeFlags = GrabTypeFlags.None;
			if (HandGrabInteraction.SupportsPinch(handGrabInteractor, handGrabInteractable.SupportedGrabTypes))
			{
				HandGrabAPI handGrabAPI3 = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PinchGrabRules;
				if (!handGrabAPI3.IsSustainingGrab(grabbingRule, grabbingFingers))
				{
					HandGrabAPI handGrabAPI4 = handGrabApi;
					GrabbingRule grabbingRule2 = handGrabInteractable.PinchGrabRules;
					if (handGrabAPI4.IsHandUnselectPinchFingersChanged(grabbingRule2))
					{
						grabTypeFlags |= GrabTypeFlags.Pinch;
					}
				}
			}
			if (HandGrabInteraction.SupportsPalm(handGrabInteractor, handGrabInteractable.SupportedGrabTypes))
			{
				HandGrabAPI handGrabAPI5 = handGrabApi;
				GrabbingRule grabbingRule = handGrabInteractable.PalmGrabRules;
				if (!handGrabAPI5.IsSustainingGrab(grabbingRule, grabbingFingers2))
				{
					HandGrabAPI handGrabAPI6 = handGrabApi;
					GrabbingRule grabbingRule2 = handGrabInteractable.PalmGrabRules;
					if (handGrabAPI6.IsHandUnselectPalmFingersChanged(grabbingRule2))
					{
						grabTypeFlags |= GrabTypeFlags.Palm;
					}
				}
			}
			return grabTypeFlags;
		}

		public static HandFingerFlags GrabbingFingers(this IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			HandGrabAPI handGrabApi = handGrabInteractor.HandGrabApi;
			if (handGrabInteractable == null)
			{
				return HandFingerFlags.None;
			}
			HandFingerFlags handFingerFlags = HandFingerFlags.None;
			if (HandGrabInteraction.SupportsPinch(handGrabInteractor, handGrabInteractable))
			{
				HandFingerFlags handFingerFlags2 = handGrabApi.HandPinchGrabbingFingers();
				handGrabInteractable.PinchGrabRules.StripIrrelevant(ref handFingerFlags2);
				handFingerFlags |= handFingerFlags2;
			}
			if (HandGrabInteraction.SupportsPalm(handGrabInteractor, handGrabInteractable))
			{
				HandFingerFlags handFingerFlags3 = handGrabApi.HandPalmGrabbingFingers();
				handGrabInteractable.PalmGrabRules.StripIrrelevant(ref handFingerFlags3);
				handFingerFlags |= handFingerFlags3;
			}
			return handFingerFlags;
		}

		private static bool SupportsPinch(IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			return HandGrabInteraction.SupportsPinch(handGrabInteractor, handGrabInteractable.SupportedGrabTypes);
		}

		private static bool SupportsPalm(IHandGrabInteractor handGrabInteractor, IHandGrabInteractable handGrabInteractable)
		{
			return HandGrabInteraction.SupportsPalm(handGrabInteractor, handGrabInteractable.SupportedGrabTypes);
		}

		private static bool SupportsPinch(IHandGrabInteractor handGrabInteractor, GrabTypeFlags grabTypes)
		{
			return (handGrabInteractor.SupportedGrabTypes & grabTypes & GrabTypeFlags.Pinch) > GrabTypeFlags.None;
		}

		private static bool SupportsPalm(IHandGrabInteractor handGrabInteractor, GrabTypeFlags grabTypes)
		{
			return (handGrabInteractor.SupportedGrabTypes & grabTypes & GrabTypeFlags.Palm) > GrabTypeFlags.None;
		}

		public static void GetPoseOffset(this IHandGrabInteractor handGrabInteractor, GrabTypeFlags anchorMode, out Pose pose, out Pose offset)
		{
			handGrabInteractor.Hand.GetRootPose(out pose);
			offset = Pose.identity;
			if (anchorMode == GrabTypeFlags.None)
			{
				return;
			}
			if ((anchorMode & GrabTypeFlags.Pinch) != GrabTypeFlags.None && handGrabInteractor.PinchPoint != null)
			{
				Pose pose2 = handGrabInteractor.PinchPoint.GetPose(Space.World);
				offset = PoseUtils.Delta(pose, pose2);
				return;
			}
			if ((anchorMode & GrabTypeFlags.Palm) != GrabTypeFlags.None && handGrabInteractor.PalmPoint != null)
			{
				Pose pose2 = handGrabInteractor.PalmPoint.GetPose(Space.World);
				offset = PoseUtils.Delta(pose, pose2);
			}
		}
	}
}
