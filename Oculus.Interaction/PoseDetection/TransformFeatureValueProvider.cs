using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction.PoseDetection
{
	public class TransformFeatureValueProvider
	{
		public static float GetValue(TransformFeature transformFeature, TransformJointData transformJointData, TransformConfig transformConfig)
		{
			TransformFeatureValueProvider.TransformProperties transformProperties = new TransformFeatureValueProvider.TransformProperties(transformJointData.CenterEyePose, transformJointData.WristPose, transformJointData.Handedness, transformJointData.TrackingSystemUp, transformJointData.TrackingSystemForward);
			switch (transformFeature)
			{
			case TransformFeature.WristUp:
				return TransformFeatureValueProvider.GetWristUpValue(transformProperties, transformConfig);
			case TransformFeature.WristDown:
				return TransformFeatureValueProvider.GetWristDownValue(transformProperties, transformConfig);
			case TransformFeature.PalmDown:
				return TransformFeatureValueProvider.GetPalmDownValue(transformProperties, transformConfig);
			case TransformFeature.PalmUp:
				return TransformFeatureValueProvider.GetPalmUpValue(transformProperties, transformConfig);
			case TransformFeature.PalmTowardsFace:
				return TransformFeatureValueProvider.GetPalmTowardsFaceValue(transformProperties, transformConfig);
			case TransformFeature.PalmAwayFromFace:
				return TransformFeatureValueProvider.GetPalmAwayFromFaceValue(transformProperties, transformConfig);
			case TransformFeature.FingersUp:
				return TransformFeatureValueProvider.GetFingersUpValue(transformProperties, transformConfig);
			case TransformFeature.FingersDown:
				return TransformFeatureValueProvider.GetFingersDownValue(transformProperties, transformConfig);
			}
			return TransformFeatureValueProvider.GetPinchClearValue(transformProperties, transformConfig);
		}

		[Obsolete("The TransformConfig parameter is obsolete")]
		public static Vector3 GetHandVectorForFeature(TransformFeature transformFeature, in TransformJointData transformJointData, in TransformConfig transformConfig)
		{
			return TransformFeatureValueProvider.GetHandVectorForFeature(transformFeature, transformJointData);
		}

		public static Vector3 GetHandVectorForFeature(TransformFeature transformFeature, in TransformJointData transformJointData)
		{
			TransformFeatureValueProvider.TransformProperties transformProperties = new TransformFeatureValueProvider.TransformProperties(transformJointData.CenterEyePose, transformJointData.WristPose, transformJointData.Handedness, transformJointData.TrackingSystemUp, transformJointData.TrackingSystemForward);
			return TransformFeatureValueProvider.GetHandVectorForFeature(transformFeature, transformProperties);
		}

		private static Vector3 GetHandVectorForFeature(TransformFeature transformFeature, in TransformFeatureValueProvider.TransformProperties transformProps)
		{
			Quaternion rotation = transformProps.WristPose.rotation;
			bool flag = transformProps.Handedness == Handedness.Left;
			Vector3 result;
			switch (transformFeature)
			{
			case TransformFeature.WristUp:
				result = rotation * (flag ? Constants.LeftThumbSide : Constants.RightThumbSide);
				break;
			case TransformFeature.WristDown:
				result = rotation * (flag ? Constants.LeftPinkySide : Constants.RightPinkySide);
				break;
			case TransformFeature.PalmDown:
				result = rotation * (flag ? Constants.LeftDorsal : Constants.RightDorsal);
				break;
			case TransformFeature.PalmUp:
				result = rotation * (flag ? Constants.LeftPalmar : Constants.RightPalmar);
				break;
			case TransformFeature.PalmTowardsFace:
				result = rotation * (flag ? Constants.LeftDorsal : Constants.RightDorsal);
				break;
			case TransformFeature.PalmAwayFromFace:
				result = rotation * (flag ? Constants.LeftPalmar : Constants.RightPalmar);
				break;
			case TransformFeature.FingersUp:
				result = rotation * (flag ? Constants.LeftDistal : Constants.RightDistal);
				break;
			case TransformFeature.FingersDown:
				result = rotation * (flag ? Constants.LeftProximal : Constants.RightProximal);
				break;
			case TransformFeature.PinchClear:
				result = rotation * (flag ? Constants.LeftPinkySide : Constants.RightPinkySide);
				break;
			default:
				result = rotation * (flag ? Constants.LeftPinkySide : Constants.RightPinkySide);
				break;
			}
			return result;
		}

		public static Vector3 GetTargetVectorForFeature(TransformFeature transformFeature, in TransformJointData transformJointData, in TransformConfig transformConfig)
		{
			TransformFeatureValueProvider.TransformProperties transformProperties = new TransformFeatureValueProvider.TransformProperties(transformJointData.CenterEyePose, transformJointData.WristPose, transformJointData.Handedness, transformJointData.TrackingSystemUp, transformJointData.TrackingSystemForward);
			return TransformFeatureValueProvider.GetTargetVectorForFeature(transformFeature, transformProperties, transformConfig);
		}

		private static Vector3 GetTargetVectorForFeature(TransformFeature transformFeature, in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 result = Vector3.zero;
			switch (transformFeature)
			{
			case TransformFeature.WristUp:
			case TransformFeature.WristDown:
			case TransformFeature.PalmDown:
			case TransformFeature.PalmUp:
			case TransformFeature.FingersUp:
			case TransformFeature.FingersDown:
			{
				Vector3 vector = TransformFeatureValueProvider.GetVerticalVector(transformProps.CenterEyePose, transformProps.TrackingSystemUp, transformConfig);
				result = TransformFeatureValueProvider.OffsetVectorWithRotation(transformProps, vector, transformConfig);
				break;
			}
			case TransformFeature.PalmTowardsFace:
			case TransformFeature.PalmAwayFromFace:
			case TransformFeature.PinchClear:
			{
				Vector3 vector = transformProps.CenterEyePose.forward;
				result = TransformFeatureValueProvider.OffsetVectorWithRotation(transformProps, vector, transformConfig);
				break;
			}
			}
			return result;
		}

		private static float GetWristDownValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.WristDown, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.WristDown, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetWristUpValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.WristUp, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.WristUp, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetPalmDownValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.PalmDown, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.PalmDown, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetPalmUpValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.PalmUp, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.PalmUp, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetPalmTowardsFaceValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.PalmTowardsFace, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.PalmTowardsFace, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetPalmAwayFromFaceValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.PalmAwayFromFace, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.PalmAwayFromFace, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetFingersUpValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.FingersUp, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.FingersUp, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetFingersDownValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.FingersDown, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.FingersDown, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static float GetPinchClearValue(in TransformFeatureValueProvider.TransformProperties transformProps, in TransformConfig transformConfig)
		{
			Vector3 handVectorForFeature = TransformFeatureValueProvider.GetHandVectorForFeature(TransformFeature.PinchClear, transformProps);
			Vector3 targetVectorForFeature = TransformFeatureValueProvider.GetTargetVectorForFeature(TransformFeature.PinchClear, transformProps, transformConfig);
			return Vector3.Angle(handVectorForFeature, targetVectorForFeature);
		}

		private static Vector3 GetVerticalVector(in Pose centerEyePose, in Vector3 trackingSystemUp, in TransformConfig transformConfig)
		{
			switch (transformConfig.UpVectorType)
			{
			case UpVectorType.Head:
			{
				Pose pose = centerEyePose;
				return pose.up;
			}
			case UpVectorType.Tracking:
				return trackingSystemUp;
			case UpVectorType.World:
				return Vector3.up;
			default:
				return Vector3.up;
			}
		}

		private static Vector3 OffsetVectorWithRotation(in TransformFeatureValueProvider.TransformProperties transformProps, in Vector3 originalVector, in TransformConfig transformConfig)
		{
			Quaternion quaternion;
			switch (transformConfig.UpVectorType)
			{
			case UpVectorType.Head:
				quaternion = transformProps.CenterEyePose.rotation;
				goto IL_44;
			case UpVectorType.Tracking:
				quaternion = Quaternion.LookRotation(transformProps.TrackingSystemForward, transformProps.TrackingSystemUp);
				goto IL_44;
			}
			quaternion = Quaternion.identity;
			IL_44:
			Quaternion rhs = Quaternion.Euler(transformConfig.RotationOffset);
			return quaternion * rhs * Quaternion.Inverse(quaternion) * originalVector;
		}

		public struct TransformProperties
		{
			public TransformProperties(Pose centerEyePos, Pose wristPose, Handedness handedness, Vector3 trackingSystemUp, Vector3 trackingSystemForward)
			{
				this.CenterEyePose = centerEyePos;
				this.WristPose = wristPose;
				this.Handedness = handedness;
				this.TrackingSystemUp = trackingSystemUp;
				this.TrackingSystemForward = trackingSystemForward;
			}

			public readonly Pose CenterEyePose;

			public readonly Pose WristPose;

			public readonly Handedness Handedness;

			public readonly Vector3 TrackingSystemUp;

			public readonly Vector3 TrackingSystemForward;
		}
	}
}
