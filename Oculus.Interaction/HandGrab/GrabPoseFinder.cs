using System;
using System.Collections.Generic;
using Oculus.Interaction.Grab;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.HandGrab
{
	public class GrabPoseFinder
	{
		public bool UsesHandPose
		{
			get
			{
				return this._handGrabPoses.Count > 0 && this._handGrabPoses[0].HandPose != null;
			}
		}

		public GrabPoseFinder(List<HandGrabPose> handGrabPoses, Transform relativeTo)
		{
			this._handGrabPoses = handGrabPoses;
			this._relativeTo = relativeTo;
		}

		public bool SupportsHandedness(Handedness handedness)
		{
			return !this.UsesHandPose || this._handGrabPoses[0].HandPose.Handedness == handedness;
		}

		public bool FindBestPose(in Pose userPose, in Pose offset, float handScale, Handedness handedness, PoseMeasureParameters scoringModifier, ref HandGrabResult result)
		{
			if (this._handGrabPoses.Count == 1)
			{
				this._handGrabPoses[0].CalculateBestPose(userPose, offset, this._relativeTo, handedness, scoringModifier, ref result);
				return true;
			}
			if (this._handGrabPoses.Count > 1)
			{
				this.CalculateBestScaleInterpolatedPose(userPose, offset, handedness, handScale, scoringModifier, ref result);
				return true;
			}
			return false;
		}

		private void CalculateBestScaleInterpolatedPose(in Pose userPose, in Pose offset, Handedness handedness, float handScale, PoseMeasureParameters scoringModifier, ref HandGrabResult result)
		{
			result.HasHandPose = false;
			HandGrabPose handGrabPose;
			HandGrabPose handGrabPose2;
			float num;
			GrabPoseFinder.FindInterpolationRange(handScale / this._relativeTo.lossyScale.x, this._handGrabPoses, out handGrabPose, out handGrabPose2, out num);
			if (num < 0f)
			{
				handGrabPose.CalculateBestPose(userPose, offset, this._relativeTo, handedness, scoringModifier, ref this._interpolationCache.underResult);
				Pose pose = this._relativeTo.GlobalPose(this._interpolationCache.underResult.RelativePose);
				handGrabPose2.CalculateBestPose(pose, offset, this._relativeTo, handedness, PoseMeasureParameters.DEFAULT, ref this._interpolationCache.overResult);
			}
			else if (num > 1f)
			{
				handGrabPose2.CalculateBestPose(userPose, offset, this._relativeTo, handedness, scoringModifier, ref this._interpolationCache.overResult);
				Pose pose2 = this._relativeTo.GlobalPose(this._interpolationCache.overResult.RelativePose);
				handGrabPose.CalculateBestPose(pose2, offset, this._relativeTo, handedness, PoseMeasureParameters.DEFAULT, ref this._interpolationCache.underResult);
			}
			else
			{
				handGrabPose.CalculateBestPose(userPose, offset, this._relativeTo, handedness, scoringModifier, ref this._interpolationCache.underResult);
				handGrabPose2.CalculateBestPose(userPose, offset, this._relativeTo, handedness, scoringModifier, ref this._interpolationCache.overResult);
			}
			if (this._interpolationCache.underResult.HasHandPose && this._interpolationCache.overResult.HasHandPose)
			{
				result.HasHandPose = true;
				result.HandPose.CopyFrom(this._interpolationCache.underResult.HandPose, false);
				HandPose.Lerp(this._interpolationCache.underResult.HandPose, this._interpolationCache.overResult.HandPose, num, ref result.HandPose);
				PoseUtils.Lerp(this._interpolationCache.underResult.RelativePose, this._interpolationCache.overResult.RelativePose, num, ref result.RelativePose);
			}
			else if (this._interpolationCache.underResult.HasHandPose)
			{
				result.HasHandPose = true;
				result.HandPose.CopyFrom(this._interpolationCache.underResult.HandPose, false);
				ref result.RelativePose.CopyFrom(this._interpolationCache.underResult.RelativePose);
			}
			else if (this._interpolationCache.overResult.HasHandPose)
			{
				result.HasHandPose = true;
				result.HandPose.CopyFrom(this._interpolationCache.overResult.HandPose, false);
				ref result.RelativePose.CopyFrom(this._interpolationCache.overResult.RelativePose);
			}
			result.Score = GrabPoseScore.Lerp(this._interpolationCache.underResult.Score, this._interpolationCache.overResult.Score, num);
		}

		public static bool FindInterpolationRange(float relativeHandScale, List<HandGrabPose> grabPoses, out HandGrabPose from, out HandGrabPose to, out float t)
		{
			if (grabPoses.Count == 0)
			{
				HandGrabPose handGrabPose;
				to = (handGrabPose = null);
				from = handGrabPose;
				t = 0f;
				return false;
			}
			if (grabPoses.Count == 1)
			{
				t = 0f;
				HandGrabPose handGrabPose;
				to = (handGrabPose = grabPoses[0]);
				from = handGrabPose;
				return true;
			}
			from = GrabPoseFinder.FindPreviousScaledGrabPose(grabPoses, relativeHandScale, false);
			to = GrabPoseFinder.FindNextScaledGrabPose(grabPoses, relativeHandScale, false);
			if (from == null && to == null)
			{
				t = 0f;
				return false;
			}
			if (to == null)
			{
				to = from;
				from = GrabPoseFinder.FindPreviousScaledGrabPose(grabPoses, from.RelativeScale, true);
			}
			if (from == null)
			{
				from = to;
				to = GrabPoseFinder.FindNextScaledGrabPose(grabPoses, to.RelativeScale, true);
			}
			float num = to.RelativeScale - from.RelativeScale;
			if (num == 0f)
			{
				t = 0f;
			}
			else
			{
				t = (relativeHandScale - from.RelativeScale) / num;
			}
			return true;
		}

		private static HandGrabPose FindPreviousScaledGrabPose(List<HandGrabPose> grabPoses, float upLimit, bool notEqual = false)
		{
			float num = float.NegativeInfinity;
			HandGrabPose result = null;
			foreach (HandGrabPose handGrabPose in grabPoses)
			{
				float relativeScale = handGrabPose.RelativeScale;
				if (((!notEqual && relativeScale <= upLimit) || (notEqual && relativeScale < upLimit)) && relativeScale > num)
				{
					num = relativeScale;
					result = handGrabPose;
				}
			}
			return result;
		}

		private static HandGrabPose FindNextScaledGrabPose(List<HandGrabPose> grabPoses, float lowLimit, bool notEqual = false)
		{
			float num = float.PositiveInfinity;
			HandGrabPose result = null;
			foreach (HandGrabPose handGrabPose in grabPoses)
			{
				float relativeScale = handGrabPose.RelativeScale;
				if (((!notEqual && relativeScale >= lowLimit) || (notEqual && relativeScale > lowLimit)) && relativeScale < num)
				{
					num = relativeScale;
					result = handGrabPose;
				}
			}
			return result;
		}

		private List<HandGrabPose> _handGrabPoses;

		private Transform _relativeTo;

		private GrabPoseFinder.InterpolationCache _interpolationCache = new GrabPoseFinder.InterpolationCache();

		[Obsolete]
		public enum FindResult
		{
			NotFound,
			NotCompatible,
			Found
		}

		private class InterpolationCache
		{
			public HandGrabResult underResult = new HandGrabResult();

			public HandGrabResult overResult = new HandGrabResult();
		}
	}
}
