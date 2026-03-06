using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public interface ICameraOverrideStack
	{
		int SetCameraOverride(int overrideId, int priority, ICinemachineCamera camA, ICinemachineCamera camB, float weightB, float deltaTime);

		void ReleaseCameraOverride(int overrideId);

		Vector3 DefaultWorldUp { get; }
	}
}
