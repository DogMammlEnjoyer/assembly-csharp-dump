using System;

namespace UnityEngine.Rendering.Universal.Internal
{
	public static class NormalReconstruction
	{
		public static void SetupProperties(CommandBuffer cmd, in CameraData cameraData)
		{
			NormalReconstruction.SetupProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
		}

		public static void SetupProperties(RasterCommandBuffer cmd, in CameraData cameraData)
		{
			CameraData cameraData2 = cameraData;
			UniversalCameraData universalCameraData = cameraData2.universalCameraData;
			NormalReconstruction.SetupProperties(cmd, universalCameraData);
		}

		public static void SetupProperties(CommandBuffer cmd, UniversalCameraData cameraData)
		{
			NormalReconstruction.SetupProperties(CommandBufferHelpers.GetRasterCommandBuffer(cmd), cameraData);
		}

		public static void SetupProperties(RasterCommandBuffer cmd, in UniversalCameraData cameraData)
		{
			int num = (cameraData.xr.enabled && cameraData.xr.singlePassEnabled) ? 2 : 1;
			for (int i = 0; i < num; i++)
			{
				Matrix4x4 viewMatrix = cameraData.GetViewMatrix(i);
				Matrix4x4 projectionMatrix = cameraData.GetProjectionMatrix(i);
				NormalReconstruction.s_NormalReconstructionMatrix[i] = projectionMatrix * viewMatrix;
				Matrix4x4 rhs = viewMatrix;
				rhs.SetColumn(3, new Vector4(0f, 0f, 0f, 1f));
				Matrix4x4 inverse = (projectionMatrix * rhs).inverse;
				NormalReconstruction.s_NormalReconstructionMatrix[i] = inverse;
			}
			cmd.SetGlobalMatrixArray(NormalReconstruction.s_NormalReconstructionMatrixID, NormalReconstruction.s_NormalReconstructionMatrix);
		}

		private static readonly int s_NormalReconstructionMatrixID = Shader.PropertyToID("_NormalReconstructionMatrix");

		private static Matrix4x4[] s_NormalReconstructionMatrix = new Matrix4x4[2];
	}
}
