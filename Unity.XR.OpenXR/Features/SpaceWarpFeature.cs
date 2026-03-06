using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features
{
	public class SpaceWarpFeature : OpenXRFeature
	{
		public bool useRightHandedNDC
		{
			get
			{
				return this.m_UseRightHandedNDC;
			}
			set
			{
				if (OpenXRLoaderBase.Instance != null && SpaceWarpFeature.Internal_SetSpaceWarpRightHandedNDC(value))
				{
					this.m_UseRightHandedNDC = value;
				}
			}
		}

		protected internal override bool OnInstanceCreate(ulong xrInstance)
		{
			return OpenXRRuntime.IsExtensionEnabled("XR_FB_space_warp") && SpaceWarpFeature.Internal_SetSpaceWarpRightHandedNDC(this.m_UseRightHandedNDC) && base.OnInstanceCreate(xrInstance);
		}

		public static bool SetSpaceWarp(bool enabled)
		{
			return SpaceWarpFeature.MetaSetSpaceWarp(enabled) == XrResult.Success;
		}

		public static bool SetAppSpacePosition(Vector3 position)
		{
			return SpaceWarpFeature.MetaSetAppSpacePosition(position.x, position.y, position.z) == XrResult.Success;
		}

		public static bool SetAppSpaceRotation(Quaternion rotation)
		{
			return SpaceWarpFeature.MetaSetAppSpaceRotation(rotation.x, rotation.y, rotation.z, rotation.w) == XrResult.Success;
		}

		[DllImport("UnityOpenXR")]
		private static extern XrResult MetaSetSpaceWarp([MarshalAs(UnmanagedType.I1)] bool enabled);

		[DllImport("UnityOpenXR")]
		private static extern XrResult MetaSetAppSpacePosition(float x, float y, float z);

		[DllImport("UnityOpenXR")]
		private static extern XrResult MetaSetAppSpaceRotation(float x, float y, float z, float w);

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_SetSpaceWarpRightHandedNDC")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_SetSpaceWarpRightHandedNDC([MarshalAs(UnmanagedType.I1)] bool useRightHandedNDC);

		public const string k_UiName = "Application SpaceWarp";

		public const string k_OpenXRRequestedExtensions = "XR_FB_space_warp";

		public const string k_SpaceWarpFeatureId = "com.unity.openxr.feature.spacewarp";

		[SerializeField]
		[Tooltip("Check this box if motion vector uses right-handed normalized device coordinates")]
		internal bool m_UseRightHandedNDC;

		private const string k_Library = "UnityOpenXR";
	}
}
