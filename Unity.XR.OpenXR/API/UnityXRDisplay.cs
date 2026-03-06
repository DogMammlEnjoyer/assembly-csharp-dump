using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR.API
{
	public static class UnityXRDisplay
	{
		[DllImport("UnityOpenXR", EntryPoint = "Display_CreateTexture")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool CreateTexture(UnityXRRenderTextureDesc desc, out uint id);

		[DllImport("UnityOpenXR", EntryPoint = "Display_DestroyTexture")]
		[return: MarshalAs(UnmanagedType.U1)]
		public static extern bool DestroyTexture(uint textureId);

		public const uint kUnityXRRenderTextureIdDontCare = 0U;

		private const string k_UnityOpenXRLib = "UnityOpenXR";
	}
}
