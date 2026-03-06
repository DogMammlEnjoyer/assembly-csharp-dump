using System;
using UnityEngine;

namespace Meta.XR.EnvironmentDepth
{
	internal static class EnvironmentDepthUtils
	{
		internal static Vector4 ComputeNdcToLinearDepthParameters(float near, float far)
		{
			float x;
			float y;
			if (far < near || float.IsInfinity(far))
			{
				x = -2f * near;
				y = -1f;
			}
			else
			{
				x = -2f * far * near / (far - near);
				y = -(far + near) / (far - near);
			}
			return new Vector4(x, y, 0f, 0f);
		}

		internal static Matrix4x4 CalculateReprojection(DepthFrameDesc frameDesc)
		{
			Matrix4x4 lhs;
			Matrix4x4 rhs;
			EnvironmentDepthUtils.CalculateDepthCameraMatrices(frameDesc, out lhs, out rhs);
			return lhs * rhs;
		}

		internal static void CalculateDepthCameraMatrices(DepthFrameDesc frameDesc, out Matrix4x4 projMatrix, out Matrix4x4 viewMatrix)
		{
			float fovLeftAngleTangent = frameDesc.fovLeftAngleTangent;
			float fovRightAngleTangent = frameDesc.fovRightAngleTangent;
			float fovDownAngleTangent = frameDesc.fovDownAngleTangent;
			float fovTopAngleTangent = frameDesc.fovTopAngleTangent;
			float nearZ = frameDesc.nearZ;
			float farZ = frameDesc.farZ;
			float m = 2f / (fovRightAngleTangent + fovLeftAngleTangent);
			float m2 = 2f / (fovTopAngleTangent + fovDownAngleTangent);
			float m3 = (fovRightAngleTangent - fovLeftAngleTangent) / (fovRightAngleTangent + fovLeftAngleTangent);
			float m4 = (fovTopAngleTangent - fovDownAngleTangent) / (fovTopAngleTangent + fovDownAngleTangent);
			float m5;
			float m6;
			if (float.IsInfinity(farZ))
			{
				m5 = -1f;
				m6 = -2f * nearZ;
			}
			else
			{
				m5 = -(farZ + nearZ) / (farZ - nearZ);
				m6 = -(2f * farZ * nearZ) / (farZ - nearZ);
			}
			float m7 = -1f;
			projMatrix = new Matrix4x4
			{
				m00 = m,
				m01 = 0f,
				m02 = m3,
				m03 = 0f,
				m10 = 0f,
				m11 = m2,
				m12 = m4,
				m13 = 0f,
				m20 = 0f,
				m21 = 0f,
				m22 = m5,
				m23 = m6,
				m30 = 0f,
				m31 = 0f,
				m32 = m7,
				m33 = 0f
			};
			viewMatrix = Matrix4x4.TRS(frameDesc.createPoseLocation, frameDesc.createPoseRotation, EnvironmentDepthUtils._scalingVector3).inverse;
		}

		private static readonly Vector3 _scalingVector3 = new Vector3(1f, 1f, -1f);
	}
}
