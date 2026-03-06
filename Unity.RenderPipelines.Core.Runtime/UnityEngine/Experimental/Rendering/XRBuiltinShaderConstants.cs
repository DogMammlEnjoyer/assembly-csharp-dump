using System;
using UnityEngine.Rendering;

namespace UnityEngine.Experimental.Rendering
{
	public static class XRBuiltinShaderConstants
	{
		public static void UpdateBuiltinShaderConstants(Matrix4x4 viewMatrix, Matrix4x4 projMatrix, bool renderIntoTexture, int viewIndex)
		{
			Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(projMatrix, renderIntoTexture);
			Matrix4x4 matrix4x = gpuprojectionMatrix * viewMatrix;
			XRBuiltinShaderConstants.s_cameraProjMatrix[viewIndex] = projMatrix;
			XRBuiltinShaderConstants.s_projMatrix[viewIndex] = gpuprojectionMatrix;
			XRBuiltinShaderConstants.s_viewMatrix[viewIndex] = viewMatrix;
			Matrix4x4.Inverse3DAffine(viewMatrix, ref XRBuiltinShaderConstants.s_invViewMatrix[viewIndex]);
			XRBuiltinShaderConstants.s_viewProjMatrix[viewIndex] = matrix4x;
			XRBuiltinShaderConstants.s_invCameraProjMatrix[viewIndex] = Matrix4x4.Inverse(projMatrix);
			XRBuiltinShaderConstants.s_invProjMatrix[viewIndex] = Matrix4x4.Inverse(gpuprojectionMatrix);
			XRBuiltinShaderConstants.s_invViewProjMatrix[viewIndex] = Matrix4x4.Inverse(matrix4x);
			XRBuiltinShaderConstants.s_worldSpaceCameraPos[viewIndex] = XRBuiltinShaderConstants.s_invViewMatrix[viewIndex].GetColumn(3);
		}

		public static void SetBuiltinShaderConstants(CommandBuffer cmd)
		{
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoCameraProjection, XRBuiltinShaderConstants.s_cameraProjMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoCameraInvProjection, XRBuiltinShaderConstants.s_invCameraProjMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixV, XRBuiltinShaderConstants.s_viewMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvV, XRBuiltinShaderConstants.s_invViewMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixP, XRBuiltinShaderConstants.s_projMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvP, XRBuiltinShaderConstants.s_invProjMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixVP, XRBuiltinShaderConstants.s_viewProjMatrix);
			cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvVP, XRBuiltinShaderConstants.s_invViewProjMatrix);
			cmd.SetGlobalVectorArray(XRBuiltinShaderConstants.unity_StereoWorldSpaceCameraPos, XRBuiltinShaderConstants.s_worldSpaceCameraPos);
		}

		public static void SetBuiltinShaderConstants(RasterCommandBuffer cmd)
		{
			XRBuiltinShaderConstants.SetBuiltinShaderConstants(cmd.m_WrappedCommandBuffer);
		}

		public static void Update(XRPass xrPass, CommandBuffer cmd, bool renderIntoTexture)
		{
			if (xrPass.enabled)
			{
				cmd.SetViewProjectionMatrices(xrPass.GetViewMatrix(0), xrPass.GetProjMatrix(0));
				if (xrPass.singlePassEnabled)
				{
					for (int i = 0; i < 2; i++)
					{
						XRBuiltinShaderConstants.s_cameraProjMatrix[i] = xrPass.GetProjMatrix(i);
						XRBuiltinShaderConstants.s_viewMatrix[i] = xrPass.GetViewMatrix(i);
						XRBuiltinShaderConstants.s_projMatrix[i] = GL.GetGPUProjectionMatrix(XRBuiltinShaderConstants.s_cameraProjMatrix[i], renderIntoTexture);
						XRBuiltinShaderConstants.s_viewProjMatrix[i] = XRBuiltinShaderConstants.s_projMatrix[i] * XRBuiltinShaderConstants.s_viewMatrix[i];
						XRBuiltinShaderConstants.s_invCameraProjMatrix[i] = Matrix4x4.Inverse(XRBuiltinShaderConstants.s_cameraProjMatrix[i]);
						Matrix4x4.Inverse3DAffine(XRBuiltinShaderConstants.s_viewMatrix[i], ref XRBuiltinShaderConstants.s_invViewMatrix[i]);
						XRBuiltinShaderConstants.s_invProjMatrix[i] = Matrix4x4.Inverse(XRBuiltinShaderConstants.s_projMatrix[i]);
						XRBuiltinShaderConstants.s_invViewProjMatrix[i] = Matrix4x4.Inverse(XRBuiltinShaderConstants.s_viewProjMatrix[i]);
						XRBuiltinShaderConstants.s_worldSpaceCameraPos[i] = XRBuiltinShaderConstants.s_invViewMatrix[i].GetColumn(3);
					}
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoCameraProjection, XRBuiltinShaderConstants.s_cameraProjMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoCameraInvProjection, XRBuiltinShaderConstants.s_invCameraProjMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixV, XRBuiltinShaderConstants.s_viewMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvV, XRBuiltinShaderConstants.s_invViewMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixP, XRBuiltinShaderConstants.s_projMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvP, XRBuiltinShaderConstants.s_invProjMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixVP, XRBuiltinShaderConstants.s_viewProjMatrix);
					cmd.SetGlobalMatrixArray(XRBuiltinShaderConstants.unity_StereoMatrixInvVP, XRBuiltinShaderConstants.s_invViewProjMatrix);
					cmd.SetGlobalVectorArray(XRBuiltinShaderConstants.unity_StereoWorldSpaceCameraPos, XRBuiltinShaderConstants.s_worldSpaceCameraPos);
				}
			}
		}

		public static readonly int unity_StereoCameraProjection = Shader.PropertyToID("unity_StereoCameraProjection");

		public static readonly int unity_StereoCameraInvProjection = Shader.PropertyToID("unity_StereoCameraInvProjection");

		public static readonly int unity_StereoMatrixV = Shader.PropertyToID("unity_StereoMatrixV");

		public static readonly int unity_StereoMatrixInvV = Shader.PropertyToID("unity_StereoMatrixInvV");

		public static readonly int unity_StereoMatrixP = Shader.PropertyToID("unity_StereoMatrixP");

		public static readonly int unity_StereoMatrixInvP = Shader.PropertyToID("unity_StereoMatrixInvP");

		public static readonly int unity_StereoMatrixVP = Shader.PropertyToID("unity_StereoMatrixVP");

		public static readonly int unity_StereoMatrixInvVP = Shader.PropertyToID("unity_StereoMatrixInvVP");

		public static readonly int unity_StereoWorldSpaceCameraPos = Shader.PropertyToID("unity_StereoWorldSpaceCameraPos");

		private static Matrix4x4[] s_cameraProjMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_invCameraProjMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_viewMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_invViewMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_projMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_invProjMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_viewProjMatrix = new Matrix4x4[2];

		private static Matrix4x4[] s_invViewProjMatrix = new Matrix4x4[2];

		private static Vector4[] s_worldSpaceCameraPos = new Vector4[2];
	}
}
