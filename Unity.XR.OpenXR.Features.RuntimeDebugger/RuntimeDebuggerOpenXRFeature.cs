using System;
using System.Runtime.InteropServices;
using UnityEngine.Events;
using UnityEngine.Networking.PlayerConnection;

namespace UnityEngine.XR.OpenXR.Features.RuntimeDebugger
{
	public class RuntimeDebuggerOpenXRFeature : OpenXRFeature
	{
		protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
		{
			PlayerConnection.instance.Register(RuntimeDebuggerOpenXRFeature.kEditorToPlayerRequestDebuggerOutput, new UnityAction<MessageEventArgs>(this.RecvMsg));
			RuntimeDebuggerOpenXRFeature.Native_StartDataAccess();
			RuntimeDebuggerOpenXRFeature.Native_EndDataAccess();
			this.lutOffset = 0U;
			return RuntimeDebuggerOpenXRFeature.Native_HookGetInstanceProcAddr(func, this.cacheSize, this.perThreadCacheSize);
		}

		internal void RecvMsg(MessageEventArgs args)
		{
			RuntimeDebuggerOpenXRFeature.Native_StartDataAccess();
			IntPtr source;
			uint num;
			RuntimeDebuggerOpenXRFeature.Native_GetLUTData(out source, out num, this.lutOffset);
			byte[] array = new byte[num];
			if (num > 0U)
			{
				this.lutOffset = num;
				Marshal.Copy(source, array, 0, (int)num);
			}
			IntPtr source2;
			uint num2;
			RuntimeDebuggerOpenXRFeature.Native_GetDataForRead(out source2, out num2);
			IntPtr source3;
			uint num3;
			RuntimeDebuggerOpenXRFeature.Native_GetDataForRead(out source3, out num3);
			byte[] array2 = new byte[num2 + num3];
			if (num2 > 0U)
			{
				Marshal.Copy(source2, array2, 0, (int)num2);
			}
			if (num3 > 0U)
			{
				Marshal.Copy(source3, array2, (int)num2, (int)num3);
			}
			RuntimeDebuggerOpenXRFeature.Native_EndDataAccess();
			PlayerConnection.instance.Send(RuntimeDebuggerOpenXRFeature.kPlayerToEditorSendDebuggerOutput, array);
			PlayerConnection.instance.Send(RuntimeDebuggerOpenXRFeature.kPlayerToEditorSendDebuggerOutput, array2);
		}

		[DllImport("openxr_runtime_debugger", EntryPoint = "HookXrInstanceProcAddr")]
		private static extern IntPtr Native_HookGetInstanceProcAddr(IntPtr func, uint cacheSize, uint perThreadCacheSize);

		[DllImport("openxr_runtime_debugger", EntryPoint = "GetDataForRead")]
		[return: MarshalAs(UnmanagedType.U1)]
		private static extern bool Native_GetDataForRead(out IntPtr ptr, out uint size);

		[DllImport("openxr_runtime_debugger", EntryPoint = "GetLUTData")]
		private static extern void Native_GetLUTData(out IntPtr ptr, out uint size, uint offset);

		[DllImport("openxr_runtime_debugger", EntryPoint = "StartDataAccess")]
		private static extern void Native_StartDataAccess();

		[DllImport("openxr_runtime_debugger", EntryPoint = "EndDataAccess")]
		private static extern void Native_EndDataAccess();

		internal static readonly Guid kEditorToPlayerRequestDebuggerOutput = new Guid("B3E6DED1-C6C7-411C-BE58-86031A0877E7");

		internal static readonly Guid kPlayerToEditorSendDebuggerOutput = new Guid("B3E6DED1-C6C7-411C-BE58-86031A0877E8");

		public uint cacheSize = 1048576U;

		public uint perThreadCacheSize = 51200U;

		private uint lutOffset;

		private const string Library = "openxr_runtime_debugger";
	}
}
