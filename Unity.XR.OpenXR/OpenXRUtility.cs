using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR
{
	public static class OpenXRUtility
	{
		private static Pose Inverse(Pose p)
		{
			Pose pose;
			pose.rotation = Quaternion.Inverse(p.rotation);
			pose.position = pose.rotation * -p.position;
			return pose;
		}

		public static Pose ComputePoseToWorldSpace(Transform t, Camera camera)
		{
			if (camera == null)
			{
				return default(Pose);
			}
			Transform transform = camera.transform;
			Pose lhs = new Pose(transform.localPosition, transform.localRotation);
			Pose p = new Pose(transform.position, transform.rotation);
			Pose pose = new Pose(t.position, t.rotation);
			return pose.GetTransformedBy(OpenXRUtility.Inverse(p)).GetTransformedBy(lhs);
		}

		public static bool IsSessionFocused
		{
			get
			{
				return OpenXRUtility.Internal_IsSessionFocused();
			}
		}

		public static bool IsUserPresent
		{
			get
			{
				return OpenXRUtility.Internal_GetUserPresence();
			}
		}

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_IsSessionFocused")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_IsSessionFocused();

		[DllImport("UnityOpenXR", EntryPoint = "NativeConfig_GetUserPresence")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Internal_GetUserPresence();

		private const string LibraryName = "UnityOpenXR";
	}
}
