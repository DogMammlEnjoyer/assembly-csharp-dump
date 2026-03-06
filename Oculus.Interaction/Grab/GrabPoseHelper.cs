using System;
using UnityEngine;

namespace Oculus.Interaction.Grab
{
	public static class GrabPoseHelper
	{
		public static GrabPoseScore CalculateBestPoseAtSurface(in Pose desiredPose, in Pose offset, out Pose bestPose, in PoseMeasureParameters scoringModifier, Transform relativeTo, GrabPoseHelper.PoseCalculator minimalTranslationPoseCalculator, GrabPoseHelper.PoseCalculator minimalRotationPoseCalculator)
		{
			PoseMeasureParameters poseMeasureParameters = scoringModifier;
			if (poseMeasureParameters.PositionRotationWeight == 1f)
			{
				bestPose = minimalRotationPoseCalculator(desiredPose, relativeTo);
				return new GrabPoseScore(ref desiredPose, ref bestPose, ref offset, scoringModifier);
			}
			poseMeasureParameters = scoringModifier;
			if (poseMeasureParameters.PositionRotationWeight == 0f)
			{
				bestPose = minimalTranslationPoseCalculator(desiredPose, relativeTo);
				return new GrabPoseScore(ref desiredPose, ref bestPose, ref offset, scoringModifier);
			}
			Pose pose = minimalTranslationPoseCalculator(desiredPose, relativeTo);
			Pose pose2 = minimalRotationPoseCalculator(desiredPose, relativeTo);
			GrabPoseScore result;
			bestPose = GrabPoseHelper.SelectBestPose(pose2, pose, desiredPose, offset, scoringModifier, out result);
			return result;
		}

		public static Pose SelectBestPose(in Pose poseA, in Pose poseB, in Pose reference, in Pose offset, PoseMeasureParameters scoringModifier, out GrabPoseScore bestScore)
		{
			GrabPoseScore grabPoseScore = new GrabPoseScore(ref reference, ref poseA, ref offset, scoringModifier);
			GrabPoseScore grabPoseScore2 = new GrabPoseScore(ref reference, ref poseB, ref offset, scoringModifier);
			if (grabPoseScore.IsBetterThan(grabPoseScore2))
			{
				bestScore = grabPoseScore;
				return poseA;
			}
			bestScore = grabPoseScore2;
			return poseB;
		}

		public static GrabPoseScore CollidersScore(Vector3 position, Collider[] colliders, out Vector3 hitPoint)
		{
			GrabPoseScore grabPoseScore = GrabPoseScore.Max;
			hitPoint = position;
			foreach (Collider collider in colliders)
			{
				bool flag = Collisions.IsPointWithinCollider(position, collider);
				Vector3 vector = flag ? collider.bounds.center : collider.ClosestPoint(position);
				GrabPoseScore grabPoseScore2 = new GrabPoseScore(position, vector, flag);
				if (grabPoseScore2.IsBetterThan(grabPoseScore))
				{
					hitPoint = (flag ? position : vector);
					grabPoseScore = grabPoseScore2;
				}
			}
			return grabPoseScore;
		}

		public delegate Pose PoseCalculator(in Pose desiredPose, Transform relativeTo);
	}
}
