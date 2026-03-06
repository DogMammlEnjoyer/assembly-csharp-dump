using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.VFX.Utility
{
	[VFXBinder("URP/URP Camera")]
	public class URPCameraBinder : VFXBinderBase
	{
		public void SetCameraProperty(string name)
		{
			this.CameraProperty = name;
			this.UpdateSubProperties();
		}

		private void UpdateSubProperties()
		{
			if (this.AdditionalData != null)
			{
				this.m_Camera = this.AdditionalData.GetComponent<Camera>();
			}
			this.m_Position = this.CameraProperty + "_transform_position";
			this.m_Angles = this.CameraProperty + "_transform_angles";
			this.m_Scale = this.CameraProperty + "_transform_scale";
			this.m_Orthographic = this.CameraProperty + "_orthographic";
			this.m_FieldOfView = this.CameraProperty + "_fieldOfView";
			this.m_NearPlane = this.CameraProperty + "_nearPlane";
			this.m_FarPlane = this.CameraProperty + "_farPlane";
			this.m_OrthographicSize = this.CameraProperty + "_orthographicSize";
			this.m_AspectRatio = this.CameraProperty + "_aspectRatio";
			this.m_Dimensions = this.CameraProperty + "_pixelDimensions";
			this.m_LensShift = this.CameraProperty + "_lensShift";
			this.m_DepthBuffer = this.CameraProperty + "_depthBuffer";
			this.m_ColorBuffer = this.CameraProperty + "_colorBuffer";
			this.m_ScaledDimensions = this.CameraProperty + "_scaledPixelDimensions";
		}

		private static void RequestHistoryAccess(IPerFrameHistoryAccessTracker access)
		{
			if (access != null)
			{
				access.RequestAccess<RawColorHistory>();
			}
			if (access != null)
			{
				access.RequestAccess<RawDepthHistory>();
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (this.AdditionalData != null)
			{
				this.AdditionalData.history.OnGatherHistoryRequests += URPCameraBinder.RequestHistoryAccess;
			}
			this.UpdateSubProperties();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			if (this.AdditionalData != null)
			{
				this.AdditionalData.history.OnGatherHistoryRequests -= URPCameraBinder.RequestHistoryAccess;
			}
		}

		private void OnValidate()
		{
			this.UpdateSubProperties();
			if (this.AdditionalData != null)
			{
				this.AdditionalData.history.OnGatherHistoryRequests += URPCameraBinder.RequestHistoryAccess;
			}
		}

		public override bool IsValid(VisualEffect component)
		{
			return this.AdditionalData != null && this.m_Camera != null && component.HasVector3(this.m_Position) && component.HasVector3(this.m_Angles) && component.HasVector3(this.m_Scale) && component.HasBool(this.m_Orthographic) && component.HasFloat(this.m_FieldOfView) && component.HasFloat(this.m_NearPlane) && component.HasFloat(this.m_FarPlane) && component.HasFloat(this.m_OrthographicSize) && component.HasFloat(this.m_AspectRatio) && component.HasVector2(this.m_Dimensions) && component.HasVector2(this.m_LensShift) && component.HasTexture(this.m_DepthBuffer) && component.HasTexture(this.m_ColorBuffer) && component.HasVector2(this.m_ScaledDimensions);
		}

		public override void UpdateBinding(VisualEffect component)
		{
			UniversalRenderPipelineAsset asset = UniversalRenderPipeline.asset;
			if (this.AdditionalData == null || asset == null)
			{
				return;
			}
			Matrix4x4 matrix4x;
			if (component.visualEffectAsset.GetExposedSpace(this.m_Position) == VFXSpace.Local)
			{
				matrix4x = component.transform.worldToLocalMatrix * this.AdditionalData.transform.localToWorldMatrix;
			}
			else
			{
				matrix4x = this.AdditionalData.transform.localToWorldMatrix;
			}
			component.SetVector3(this.m_Position, matrix4x.GetPosition());
			component.SetVector3(this.m_Angles, matrix4x.rotation.eulerAngles);
			component.SetVector3(this.m_Scale, matrix4x.lossyScale);
			component.SetBool(this.m_Orthographic, this.m_Camera.orthographic);
			component.SetFloat(this.m_OrthographicSize, this.m_Camera.orthographicSize);
			component.SetFloat(this.m_FieldOfView, 0.017453292f * this.m_Camera.fieldOfView);
			component.SetFloat(this.m_NearPlane, this.m_Camera.nearClipPlane);
			component.SetFloat(this.m_FarPlane, this.m_Camera.farClipPlane);
			component.SetVector2(this.m_LensShift, this.m_Camera.lensShift);
			component.SetFloat(this.m_AspectRatio, this.m_Camera.aspect);
			Vector2 v = new Vector2((float)this.m_Camera.pixelWidth, (float)this.m_Camera.pixelHeight);
			component.SetVector2(this.m_Dimensions, v);
			Vector2 v2 = new Vector2((float)this.m_Camera.scaledPixelWidth, (float)this.m_Camera.scaledPixelHeight) * asset.renderScale;
			component.SetVector2(this.m_ScaledDimensions, v2);
			RawDepthHistory historyForRead = this.AdditionalData.history.GetHistoryForRead<RawDepthHistory>();
			RTHandle rthandle = (historyForRead != null) ? historyForRead.GetCurrentTexture(0) : null;
			RawColorHistory historyForRead2 = this.AdditionalData.history.GetHistoryForRead<RawColorHistory>();
			RTHandle rthandle2 = (historyForRead2 != null) ? historyForRead2.GetCurrentTexture(0) : null;
			if (rthandle != null)
			{
				component.SetTexture(this.m_DepthBuffer, rthandle);
			}
			if (rthandle2 != null)
			{
				component.SetTexture(this.m_ColorBuffer, rthandle2);
			}
		}

		public override string ToString()
		{
			return string.Format(string.Format("URP Camera : '{0}' -> {1}", (this.AdditionalData == null) ? "null" : this.AdditionalData.gameObject.name, this.CameraProperty), Array.Empty<object>());
		}

		public UniversalAdditionalCameraData AdditionalData;

		private Camera m_Camera;

		[VFXPropertyBinding(new string[]
		{
			"UnityEditor.VFX.CameraType"
		})]
		[SerializeField]
		private ExposedProperty CameraProperty = "Camera";

		private ExposedProperty m_Position;

		private ExposedProperty m_Angles;

		private ExposedProperty m_Scale;

		private ExposedProperty m_FieldOfView;

		private ExposedProperty m_NearPlane;

		private ExposedProperty m_FarPlane;

		private ExposedProperty m_AspectRatio;

		private ExposedProperty m_Dimensions;

		private ExposedProperty m_ScaledDimensions;

		private ExposedProperty m_DepthBuffer;

		private ExposedProperty m_ColorBuffer;

		private ExposedProperty m_Orthographic;

		private ExposedProperty m_OrthographicSize;

		private ExposedProperty m_LensShift;
	}
}
