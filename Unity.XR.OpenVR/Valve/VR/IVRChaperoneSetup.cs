using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public struct IVRChaperoneSetup
	{
		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._CommitWorkingCopy CommitWorkingCopy;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._RevertWorkingCopy RevertWorkingCopy;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetWorkingPlayAreaSize GetWorkingPlayAreaSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetWorkingPlayAreaRect GetWorkingPlayAreaRect;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetWorkingCollisionBoundsInfo GetWorkingCollisionBoundsInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetLiveCollisionBoundsInfo GetLiveCollisionBoundsInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetWorkingSeatedZeroPoseToRawTrackingPose GetWorkingSeatedZeroPoseToRawTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetWorkingStandingZeroPoseToRawTrackingPose GetWorkingStandingZeroPoseToRawTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._SetWorkingPlayAreaSize SetWorkingPlayAreaSize;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._SetWorkingCollisionBoundsInfo SetWorkingCollisionBoundsInfo;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._SetWorkingPerimeter SetWorkingPerimeter;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._SetWorkingSeatedZeroPoseToRawTrackingPose SetWorkingSeatedZeroPoseToRawTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._SetWorkingStandingZeroPoseToRawTrackingPose SetWorkingStandingZeroPoseToRawTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._ReloadFromDisk ReloadFromDisk;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._GetLiveSeatedZeroPoseToRawTrackingPose GetLiveSeatedZeroPoseToRawTrackingPose;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._ExportLiveToBuffer ExportLiveToBuffer;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._ImportFromBufferToWorking ImportFromBufferToWorking;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._ShowWorkingSetPreview ShowWorkingSetPreview;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._HideWorkingSetPreview HideWorkingSetPreview;

		[MarshalAs(UnmanagedType.FunctionPtr)]
		internal IVRChaperoneSetup._RoomSetupStarting RoomSetupStarting;

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _CommitWorkingCopy(EChaperoneConfigFile configFile);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RevertWorkingCopy();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetWorkingPlayAreaSize(ref float pSizeX, ref float pSizeZ);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetWorkingPlayAreaRect(ref HmdQuad_t rect);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetWorkingCollisionBoundsInfo([In] [Out] HmdQuad_t[] pQuadsBuffer, ref uint punQuadsCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetLiveCollisionBoundsInfo([In] [Out] HmdQuad_t[] pQuadsBuffer, ref uint punQuadsCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatStandingZeroPoseToRawTrackingPose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetWorkingPlayAreaSize(float sizeX, float sizeZ);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetWorkingCollisionBoundsInfo([In] [Out] HmdQuad_t[] pQuadsBuffer, uint unQuadsCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetWorkingPerimeter([In] [Out] HmdVector2_t[] pPointBuffer, uint unPointCount);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatSeatedZeroPoseToRawTrackingPose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _SetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatStandingZeroPoseToRawTrackingPose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ReloadFromDisk(EChaperoneConfigFile configFile);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _GetLiveSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ExportLiveToBuffer(StringBuilder pBuffer, ref uint pnBufferLength);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate bool _ImportFromBufferToWorking(IntPtr pBuffer, uint nImportFlags);

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _ShowWorkingSetPreview();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _HideWorkingSetPreview();

		[UnmanagedFunctionPointer(CallingConvention.StdCall)]
		internal delegate void _RoomSetupStarting();
	}
}
