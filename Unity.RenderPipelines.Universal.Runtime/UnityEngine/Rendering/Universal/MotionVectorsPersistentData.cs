using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.Universal
{
	internal sealed class MotionVectorsPersistentData
	{
		internal MotionVectorsPersistentData()
		{
			this.Reset();
		}

		internal int lastFrameIndex
		{
			get
			{
				return this.m_LastFrameIndex[0];
			}
		}

		internal Matrix4x4 viewProjection
		{
			get
			{
				return this.m_ViewProjection[0];
			}
		}

		internal Matrix4x4 previousViewProjection
		{
			get
			{
				return this.m_PreviousViewProjection[0];
			}
		}

		internal Matrix4x4[] viewProjectionStereo
		{
			get
			{
				return this.m_ViewProjection;
			}
		}

		internal Matrix4x4[] previousViewProjectionStereo
		{
			get
			{
				return this.m_PreviousViewProjection;
			}
		}

		internal Matrix4x4[] projectionStereo
		{
			get
			{
				return this.m_Projection;
			}
		}

		internal Matrix4x4[] previousProjectionStereo
		{
			get
			{
				return this.m_PreviousProjection;
			}
		}

		internal Matrix4x4[] previousPreviousProjectionStereo
		{
			get
			{
				return this.m_PreviousPreviousProjection;
			}
		}

		internal Matrix4x4[] viewStereo
		{
			get
			{
				return this.m_View;
			}
		}

		internal Matrix4x4[] previousViewStereo
		{
			get
			{
				return this.m_PreviousView;
			}
		}

		internal Matrix4x4[] previousPreviousViewStereo
		{
			get
			{
				return this.m_PreviousPreviousView;
			}
		}

		internal float deltaTime
		{
			get
			{
				return this.m_deltaTime;
			}
		}

		internal float lastDeltaTime
		{
			get
			{
				return this.m_lastDeltaTime;
			}
		}

		internal Vector3 worldSpaceCameraPos
		{
			get
			{
				return this.m_worldSpaceCameraPos;
			}
		}

		internal Vector3 previousWorldSpaceCameraPos
		{
			get
			{
				return this.m_previousWorldSpaceCameraPos;
			}
		}

		internal Vector3 previousPreviousWorldSpaceCameraPos
		{
			get
			{
				return this.m_previousPreviousWorldSpaceCameraPos;
			}
		}

		public void Reset()
		{
			for (int i = 0; i < 2; i++)
			{
				this.m_Projection[i] = Matrix4x4.identity;
				this.m_View[i] = Matrix4x4.identity;
				this.m_ViewProjection[i] = Matrix4x4.identity;
				this.m_PreviousProjection[i] = Matrix4x4.identity;
				this.m_PreviousView[i] = Matrix4x4.identity;
				this.m_PreviousViewProjection[i] = Matrix4x4.identity;
				this.m_PreviousProjection[i] = Matrix4x4.identity;
				this.m_PreviousView[i] = Matrix4x4.identity;
				this.m_PreviousViewProjection[i] = Matrix4x4.identity;
				this.m_LastFrameIndex[i] = -1;
				this.m_PrevAspectRatio[i] = -1f;
			}
			this.m_deltaTime = 0f;
			this.m_lastDeltaTime = 0f;
			this.m_worldSpaceCameraPos = Vector3.zero;
			this.m_previousWorldSpaceCameraPos = Vector3.zero;
			this.m_previousPreviousWorldSpaceCameraPos = Vector3.zero;
		}

		private static int GetXRMultiPassId(XRPass xr)
		{
			if (!xr.enabled)
			{
				return 0;
			}
			return xr.multipassId;
		}

		public void Update(UniversalCameraData cameraData)
		{
			int xrmultiPassId = MotionVectorsPersistentData.GetXRMultiPassId(cameraData.xr);
			bool flag = !cameraData.xr.enabled || cameraData.xr.singlePassEnabled || xrmultiPassId == 0;
			int frameCount = Time.frameCount;
			if (flag)
			{
				bool flag2 = this.m_LastFrameIndex[0] == -1;
				float deltaTime = Time.deltaTime;
				Vector3 position = cameraData.camera.transform.position;
				if (flag2)
				{
					this.m_lastDeltaTime = deltaTime;
					this.m_deltaTime = deltaTime;
					this.m_previousPreviousWorldSpaceCameraPos = position;
					this.m_previousWorldSpaceCameraPos = position;
					this.m_worldSpaceCameraPos = position;
				}
				this.m_lastDeltaTime = this.m_deltaTime;
				this.m_deltaTime = deltaTime;
				this.m_previousPreviousWorldSpaceCameraPos = this.m_previousWorldSpaceCameraPos;
				this.m_previousWorldSpaceCameraPos = this.m_worldSpaceCameraPos;
				this.m_worldSpaceCameraPos = position;
			}
			bool flag3 = this.m_PrevAspectRatio[xrmultiPassId] != cameraData.aspectRatio;
			if (this.m_LastFrameIndex[xrmultiPassId] != frameCount || flag3)
			{
				bool flag4 = this.m_LastFrameIndex[xrmultiPassId] == -1 || flag3;
				int num = cameraData.xr.enabled ? cameraData.xr.viewCount : 1;
				for (int i = 0; i < num; i++)
				{
					int num2 = i + xrmultiPassId;
					Matrix4x4 gpuprojectionMatrix = GL.GetGPUProjectionMatrix(cameraData.GetProjectionMatrixNoJitter(i), true);
					Matrix4x4 viewMatrix = cameraData.GetViewMatrix(i);
					Matrix4x4 matrix4x = gpuprojectionMatrix * viewMatrix;
					if (flag4)
					{
						this.m_PreviousPreviousProjection[num2] = gpuprojectionMatrix;
						this.m_PreviousProjection[num2] = gpuprojectionMatrix;
						this.m_Projection[num2] = gpuprojectionMatrix;
						this.m_PreviousPreviousView[num2] = viewMatrix;
						this.m_PreviousView[num2] = viewMatrix;
						this.m_View[num2] = viewMatrix;
						this.m_ViewProjection[num2] = matrix4x;
						this.m_PreviousViewProjection[num2] = matrix4x;
					}
					this.m_PreviousPreviousProjection[num2] = this.m_PreviousProjection[num2];
					this.m_PreviousProjection[num2] = this.m_Projection[num2];
					this.m_Projection[num2] = gpuprojectionMatrix;
					this.m_PreviousPreviousView[num2] = this.m_PreviousView[num2];
					this.m_PreviousView[num2] = this.m_View[num2];
					this.m_View[num2] = viewMatrix;
					this.m_PreviousViewProjection[num2] = this.m_ViewProjection[num2];
					this.m_ViewProjection[num2] = matrix4x;
				}
				this.m_LastFrameIndex[xrmultiPassId] = frameCount;
				this.m_PrevAspectRatio[xrmultiPassId] = cameraData.aspectRatio;
			}
		}

		public void SetGlobalMotionMatrices(RasterCommandBuffer cmd, XRPass xr)
		{
			int xrmultiPassId = MotionVectorsPersistentData.GetXRMultiPassId(xr);
			if (xr.enabled && xr.singlePassEnabled)
			{
				cmd.SetGlobalMatrixArray(ShaderPropertyId.previousViewProjectionNoJitterStereo, this.previousViewProjectionStereo);
				cmd.SetGlobalMatrixArray(ShaderPropertyId.viewProjectionNoJitterStereo, this.viewProjectionStereo);
				return;
			}
			cmd.SetGlobalMatrix(ShaderPropertyId.previousViewProjectionNoJitter, this.previousViewProjectionStereo[xrmultiPassId]);
			cmd.SetGlobalMatrix(ShaderPropertyId.viewProjectionNoJitter, this.viewProjectionStereo[xrmultiPassId]);
		}

		private const int k_EyeCount = 2;

		private readonly Matrix4x4[] m_Projection = new Matrix4x4[2];

		private readonly Matrix4x4[] m_View = new Matrix4x4[2];

		private readonly Matrix4x4[] m_ViewProjection = new Matrix4x4[2];

		private readonly Matrix4x4[] m_PreviousProjection = new Matrix4x4[2];

		private readonly Matrix4x4[] m_PreviousView = new Matrix4x4[2];

		private readonly Matrix4x4[] m_PreviousViewProjection = new Matrix4x4[2];

		private readonly Matrix4x4[] m_PreviousPreviousProjection = new Matrix4x4[2];

		private readonly Matrix4x4[] m_PreviousPreviousView = new Matrix4x4[2];

		private readonly int[] m_LastFrameIndex = new int[2];

		private readonly float[] m_PrevAspectRatio = new float[2];

		private float m_deltaTime;

		private float m_lastDeltaTime;

		private Vector3 m_worldSpaceCameraPos;

		private Vector3 m_previousWorldSpaceCameraPos;

		private Vector3 m_previousPreviousWorldSpaceCameraPos;
	}
}
