using System;

namespace UnityEngine.Rendering
{
	internal struct OccluderDerivedData
	{
		public static OccluderDerivedData FromParameters(in OccluderSubviewUpdate occluderSubviewUpdate)
		{
			Vector3 viewOffsetWorldSpace = occluderSubviewUpdate.viewOffsetWorldSpace;
			Matrix4x4 invViewMatrix = occluderSubviewUpdate.invViewMatrix;
			Vector3 v = viewOffsetWorldSpace + invViewMatrix.GetColumn(3);
			invViewMatrix = occluderSubviewUpdate.invViewMatrix;
			Vector3 a = invViewMatrix.GetColumn(0);
			invViewMatrix = occluderSubviewUpdate.invViewMatrix;
			Vector3 b = invViewMatrix.GetColumn(1);
			invViewMatrix = occluderSubviewUpdate.invViewMatrix;
			Vector3 vector = invViewMatrix.GetColumn(2);
			Matrix4x4 viewMatrix = occluderSubviewUpdate.viewMatrix;
			viewMatrix.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
			return new OccluderDerivedData
			{
				viewOriginWorldSpace = v,
				facingDirWorldSpace = vector.normalized,
				radialDirWorldSpace = (a + b).normalized,
				viewProjMatrix = occluderSubviewUpdate.gpuProjMatrix * viewMatrix
			};
		}

		public Matrix4x4 viewProjMatrix;

		public Vector4 viewOriginWorldSpace;

		public Vector4 radialDirWorldSpace;

		public Vector4 facingDirWorldSpace;
	}
}
