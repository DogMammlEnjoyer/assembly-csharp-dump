using System;
using System.Runtime.InteropServices;
using System.Text;

namespace OVR.OpenVR
{
	public class CVRRenderModels
	{
		internal CVRRenderModels(IntPtr pInterface)
		{
			this.FnTable = (IVRRenderModels)Marshal.PtrToStructure(pInterface, typeof(IVRRenderModels));
		}

		public EVRRenderModelError LoadRenderModel_Async(string pchRenderModelName, ref IntPtr ppRenderModel)
		{
			return this.FnTable.LoadRenderModel_Async(pchRenderModelName, ref ppRenderModel);
		}

		public void FreeRenderModel(IntPtr pRenderModel)
		{
			this.FnTable.FreeRenderModel(pRenderModel);
		}

		public EVRRenderModelError LoadTexture_Async(int textureId, ref IntPtr ppTexture)
		{
			return this.FnTable.LoadTexture_Async(textureId, ref ppTexture);
		}

		public void FreeTexture(IntPtr pTexture)
		{
			this.FnTable.FreeTexture(pTexture);
		}

		public EVRRenderModelError LoadTextureD3D11_Async(int textureId, IntPtr pD3D11Device, ref IntPtr ppD3D11Texture2D)
		{
			return this.FnTable.LoadTextureD3D11_Async(textureId, pD3D11Device, ref ppD3D11Texture2D);
		}

		public EVRRenderModelError LoadIntoTextureD3D11_Async(int textureId, IntPtr pDstTexture)
		{
			return this.FnTable.LoadIntoTextureD3D11_Async(textureId, pDstTexture);
		}

		public void FreeTextureD3D11(IntPtr pD3D11Texture2D)
		{
			this.FnTable.FreeTextureD3D11(pD3D11Texture2D);
		}

		public uint GetRenderModelName(uint unRenderModelIndex, StringBuilder pchRenderModelName, uint unRenderModelNameLen)
		{
			return this.FnTable.GetRenderModelName(unRenderModelIndex, pchRenderModelName, unRenderModelNameLen);
		}

		public uint GetRenderModelCount()
		{
			return this.FnTable.GetRenderModelCount();
		}

		public uint GetComponentCount(string pchRenderModelName)
		{
			return this.FnTable.GetComponentCount(pchRenderModelName);
		}

		public uint GetComponentName(string pchRenderModelName, uint unComponentIndex, StringBuilder pchComponentName, uint unComponentNameLen)
		{
			return this.FnTable.GetComponentName(pchRenderModelName, unComponentIndex, pchComponentName, unComponentNameLen);
		}

		public ulong GetComponentButtonMask(string pchRenderModelName, string pchComponentName)
		{
			return this.FnTable.GetComponentButtonMask(pchRenderModelName, pchComponentName);
		}

		public uint GetComponentRenderModelName(string pchRenderModelName, string pchComponentName, StringBuilder pchComponentRenderModelName, uint unComponentRenderModelNameLen)
		{
			return this.FnTable.GetComponentRenderModelName(pchRenderModelName, pchComponentName, pchComponentRenderModelName, unComponentRenderModelNameLen);
		}

		public bool GetComponentStateForDevicePath(string pchRenderModelName, string pchComponentName, ulong devicePath, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState)
		{
			return this.FnTable.GetComponentStateForDevicePath(pchRenderModelName, pchComponentName, devicePath, ref pState, ref pComponentState);
		}

		public bool GetComponentState(string pchRenderModelName, string pchComponentName, ref VRControllerState_t pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState)
		{
			if (Environment.OSVersion.Platform == PlatformID.MacOSX || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				VRControllerState_t_Packed vrcontrollerState_t_Packed = new VRControllerState_t_Packed(pControllerState);
				CVRRenderModels.GetComponentStateUnion getComponentStateUnion;
				getComponentStateUnion.pGetComponentStatePacked = null;
				getComponentStateUnion.pGetComponentState = this.FnTable.GetComponentState;
				bool result = getComponentStateUnion.pGetComponentStatePacked(pchRenderModelName, pchComponentName, ref vrcontrollerState_t_Packed, ref pState, ref pComponentState);
				vrcontrollerState_t_Packed.Unpack(ref pControllerState);
				return result;
			}
			return this.FnTable.GetComponentState(pchRenderModelName, pchComponentName, ref pControllerState, ref pState, ref pComponentState);
		}

		public bool RenderModelHasComponent(string pchRenderModelName, string pchComponentName)
		{
			return this.FnTable.RenderModelHasComponent(pchRenderModelName, pchComponentName);
		}

		public uint GetRenderModelThumbnailURL(string pchRenderModelName, StringBuilder pchThumbnailURL, uint unThumbnailURLLen, ref EVRRenderModelError peError)
		{
			return this.FnTable.GetRenderModelThumbnailURL(pchRenderModelName, pchThumbnailURL, unThumbnailURLLen, ref peError);
		}

		public uint GetRenderModelOriginalPath(string pchRenderModelName, StringBuilder pchOriginalPath, uint unOriginalPathLen, ref EVRRenderModelError peError)
		{
			return this.FnTable.GetRenderModelOriginalPath(pchRenderModelName, pchOriginalPath, unOriginalPathLen, ref peError);
		}

		public string GetRenderModelErrorNameFromEnum(EVRRenderModelError error)
		{
			return Marshal.PtrToStringAnsi(this.FnTable.GetRenderModelErrorNameFromEnum(error));
		}

		private IVRRenderModels FnTable;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetComponentStatePacked(string pchRenderModelName, string pchComponentName, ref VRControllerState_t_Packed pControllerState, ref RenderModel_ControllerMode_State_t pState, ref RenderModel_ComponentState_t pComponentState);

		[StructLayout(LayoutKind.Explicit)]
		private struct GetComponentStateUnion
		{
			[FieldOffset(0)]
			public IVRRenderModels._GetComponentState pGetComponentState;

			[FieldOffset(0)]
			public CVRRenderModels._GetComponentStatePacked pGetComponentStatePacked;
		}
	}
}
