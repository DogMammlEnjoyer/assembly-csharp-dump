using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.XR;

[HelpURL("https://developer.oculus.com/reference/unity/v67/class_o_v_r_display")]
public class OVRDisplay
{
	public OVRDisplay()
	{
		this.UpdateTextures();
	}

	public void Update()
	{
		this.UpdateTextures();
		if (this.recenterRequested && Time.frameCount > this.recenterRequestedFrameCount)
		{
			Debug.Log("Recenter event detected");
			if (this.RecenteredPose != null)
			{
				this.RecenteredPose();
			}
			this.recenterRequested = false;
			this.recenterRequestedFrameCount = int.MaxValue;
		}
		if (OVRPlugin.GetSystemHeadsetType() >= OVRPlugin.SystemHeadset.Oculus_Quest && OVRPlugin.GetSystemHeadsetType() < OVRPlugin.SystemHeadset.Rift_DK1)
		{
			int num = OVRPlugin.GetLocalTrackingSpaceRecenterCount();
			if (this.localTrackingSpaceRecenterCount != num)
			{
				Debug.Log("Recenter event detected");
				if (this.RecenteredPose != null)
				{
					this.RecenteredPose();
				}
				this.localTrackingSpaceRecenterCount = num;
			}
		}
	}

	public event Action RecenteredPose;

	public void RecenterPose()
	{
		XRInputSubsystem currentInputSubsystem = OVRManager.GetCurrentInputSubsystem();
		if (currentInputSubsystem != null)
		{
			currentInputSubsystem.TryRecenter();
		}
		this.recenterRequested = true;
		this.recenterRequestedFrameCount = Time.frameCount;
		OVRMixedReality.RecenterPose();
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public Vector3 acceleration
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			Vector3 zero = Vector3.zero;
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.Acceleration, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
	}

	[Obsolete("Deprecated. Acceleration is not supported in OpenXR", false)]
	public Vector3 angularAcceleration
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			Vector3 zero = Vector3.zero;
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.AngularAcceleration, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
	}

	public Vector3 velocity
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			Vector3 zero = Vector3.zero;
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.Velocity, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
	}

	public Vector3 angularVelocity
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return Vector3.zero;
			}
			Vector3 zero = Vector3.zero;
			if (OVRNodeStateProperties.GetNodeStatePropertyVector3(XRNode.Head, NodeStatePropertyType.AngularVelocity, OVRPlugin.Node.Head, OVRPlugin.Step.Render, out zero))
			{
				return zero;
			}
			return Vector3.zero;
		}
	}

	public OVRDisplay.EyeRenderDesc GetEyeRenderDesc(XRNode eye)
	{
		return this.eyeDescs[(int)eye];
	}

	public OVRDisplay.LatencyData latency
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return default(OVRDisplay.LatencyData);
			}
			string latency = OVRPlugin.latency;
			Regex regex = new Regex("Render: ([0-9]+[.][0-9]+)ms, TimeWarp: ([0-9]+[.][0-9]+)ms, PostPresent: ([0-9]+[.][0-9]+)ms", RegexOptions.None);
			OVRDisplay.LatencyData result = default(OVRDisplay.LatencyData);
			Match match = regex.Match(latency);
			if (match.Success)
			{
				result.render = float.Parse(match.Groups[1].Value);
				result.timeWarp = float.Parse(match.Groups[2].Value);
				result.postPresent = float.Parse(match.Groups[3].Value);
			}
			return result;
		}
	}

	public float appFramerate
	{
		get
		{
			if (!OVRManager.isHmdPresent)
			{
				return 0f;
			}
			return OVRPlugin.GetAppFramerate();
		}
	}

	public float[] displayFrequenciesAvailable
	{
		get
		{
			return OVRPlugin.systemDisplayFrequenciesAvailable;
		}
	}

	public float displayFrequency
	{
		get
		{
			return OVRPlugin.systemDisplayFrequency;
		}
		set
		{
			OVRPlugin.systemDisplayFrequency = value;
		}
	}

	protected void UpdateTextures()
	{
		this.ConfigureEyeDesc(XRNode.LeftEye);
		this.ConfigureEyeDesc(XRNode.RightEye);
	}

	protected void ConfigureEyeDesc(XRNode eye)
	{
		if (!OVRManager.isHmdPresent)
		{
			return;
		}
		int eyeTextureWidth = XRSettings.eyeTextureWidth;
		int eyeTextureHeight = XRSettings.eyeTextureHeight;
		this.eyeDescs[(int)eye] = default(OVRDisplay.EyeRenderDesc);
		this.eyeDescs[(int)eye].resolution = new Vector2((float)eyeTextureWidth, (float)eyeTextureHeight);
		OVRPlugin.Frustumf2 frustumf;
		if (OVRPlugin.GetNodeFrustum2((OVRPlugin.Node)eye, out frustumf))
		{
			this.eyeDescs[(int)eye].fullFov.LeftFov = 57.29578f * Mathf.Atan(frustumf.Fov.LeftTan);
			this.eyeDescs[(int)eye].fullFov.RightFov = 57.29578f * Mathf.Atan(frustumf.Fov.RightTan);
			this.eyeDescs[(int)eye].fullFov.UpFov = 57.29578f * Mathf.Atan(frustumf.Fov.UpTan);
			this.eyeDescs[(int)eye].fullFov.DownFov = 57.29578f * Mathf.Atan(frustumf.Fov.DownTan);
		}
		else
		{
			OVRPlugin.Frustumf eyeFrustum = OVRPlugin.GetEyeFrustum((OVRPlugin.Eye)eye);
			this.eyeDescs[(int)eye].fullFov.LeftFov = 57.29578f * eyeFrustum.fovX * 0.5f;
			this.eyeDescs[(int)eye].fullFov.RightFov = 57.29578f * eyeFrustum.fovX * 0.5f;
			this.eyeDescs[(int)eye].fullFov.UpFov = 57.29578f * eyeFrustum.fovY * 0.5f;
			this.eyeDescs[(int)eye].fullFov.DownFov = 57.29578f * eyeFrustum.fovY * 0.5f;
		}
		float num = Mathf.Max(this.eyeDescs[(int)eye].fullFov.LeftFov, this.eyeDescs[(int)eye].fullFov.RightFov);
		float num2 = Mathf.Max(this.eyeDescs[(int)eye].fullFov.UpFov, this.eyeDescs[(int)eye].fullFov.DownFov);
		this.eyeDescs[(int)eye].fov.x = num * 2f;
		this.eyeDescs[(int)eye].fov.y = num2 * 2f;
		if (!OVRPlugin.AsymmetricFovEnabled)
		{
			this.eyeDescs[(int)eye].fullFov.LeftFov = num;
			this.eyeDescs[(int)eye].fullFov.RightFov = num;
			this.eyeDescs[(int)eye].fullFov.UpFov = num2;
			this.eyeDescs[(int)eye].fullFov.DownFov = num2;
		}
	}

	protected bool needsConfigureTexture;

	protected OVRDisplay.EyeRenderDesc[] eyeDescs = new OVRDisplay.EyeRenderDesc[2];

	protected bool recenterRequested;

	protected int recenterRequestedFrameCount = int.MaxValue;

	protected int localTrackingSpaceRecenterCount;

	public struct EyeFov
	{
		public float UpFov;

		public float DownFov;

		public float LeftFov;

		public float RightFov;
	}

	public struct EyeRenderDesc
	{
		public Vector2 resolution;

		public Vector2 fov;

		public OVRDisplay.EyeFov fullFov;
	}

	public struct LatencyData
	{
		public float render;

		public float timeWarp;

		public float postPresent;

		public float renderError;

		public float timeWarpError;
	}
}
