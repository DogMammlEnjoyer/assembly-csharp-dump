using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.XR;

namespace UnityEngine.Rendering
{
	[Serializable]
	public class XRSRPSettings
	{
		public static bool enabled
		{
			get
			{
				return XRSettings.enabled;
			}
		}

		public static bool isDeviceActive
		{
			get
			{
				return XRSRPSettings.enabled && XRSettings.isDeviceActive;
			}
		}

		public static string loadedDeviceName
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSettings.loadedDeviceName;
				}
				return "No XR device loaded";
			}
		}

		public static string[] supportedDevices
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSettings.supportedDevices;
				}
				return new string[1];
			}
		}

		public static RenderTextureDescriptor eyeTextureDesc
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSettings.eyeTextureDesc;
				}
				return new RenderTextureDescriptor(0, 0);
			}
		}

		public static int eyeTextureWidth
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSettings.eyeTextureWidth;
				}
				return 0;
			}
		}

		public static int eyeTextureHeight
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSettings.eyeTextureHeight;
				}
				return 0;
			}
		}

		public static float occlusionMeshScale
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSystem.GetOcclusionMeshScale();
				}
				return 0f;
			}
			set
			{
				if (XRSRPSettings.enabled)
				{
					XRSystem.SetOcclusionMeshScale(value);
				}
			}
		}

		public static bool useVisibilityMesh
		{
			get
			{
				return XRSRPSettings.enabled && XRSystem.GetUseVisibilityMesh();
			}
			set
			{
				if (XRSRPSettings.enabled)
				{
					XRSystem.SetUseVisibilityMesh(value);
				}
			}
		}

		public static int mirrorViewMode
		{
			get
			{
				if (XRSRPSettings.enabled)
				{
					return XRSystem.GetMirrorViewMode();
				}
				return 0;
			}
			set
			{
				if (XRSRPSettings.enabled)
				{
					XRSystem.SetMirrorViewMode(value);
				}
			}
		}
	}
}
