using System;

namespace UnityEngine.XR.Interaction.Toolkit.Attachment
{
	public interface IInteractionAttachController
	{
		Transform GetOrCreateAnchorTransform(bool updateTransform = false);

		Transform transformToFollow { get; set; }

		MotionStabilizationMode motionStabilizationMode { get; set; }

		bool hasOffset { get; }

		void MoveTo(Vector3 targetWorldPosition);

		void ApplyLocalPositionOffset(Vector3 offset);

		void ApplyLocalRotationOffset(Quaternion localRotation);

		void ResetOffset();

		void DoUpdate(float deltaTime);
	}
}
