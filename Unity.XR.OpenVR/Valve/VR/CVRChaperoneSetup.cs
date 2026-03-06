using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Valve.VR
{
	public class CVRChaperoneSetup
	{
		internal CVRChaperoneSetup(IntPtr pInterface)
		{
			this.FnTable = (IVRChaperoneSetup)Marshal.PtrToStructure(pInterface, typeof(IVRChaperoneSetup));
		}

		public bool CommitWorkingCopy(EChaperoneConfigFile configFile)
		{
			return this.FnTable.CommitWorkingCopy(configFile);
		}

		public void RevertWorkingCopy()
		{
			this.FnTable.RevertWorkingCopy();
		}

		public bool GetWorkingPlayAreaSize(ref float pSizeX, ref float pSizeZ)
		{
			pSizeX = 0f;
			pSizeZ = 0f;
			return this.FnTable.GetWorkingPlayAreaSize(ref pSizeX, ref pSizeZ);
		}

		public bool GetWorkingPlayAreaRect(ref HmdQuad_t rect)
		{
			return this.FnTable.GetWorkingPlayAreaRect(ref rect);
		}

		public bool GetWorkingCollisionBoundsInfo(out HmdQuad_t[] pQuadsBuffer)
		{
			uint num = 0U;
			this.FnTable.GetWorkingCollisionBoundsInfo(null, ref num);
			pQuadsBuffer = new HmdQuad_t[num];
			return this.FnTable.GetWorkingCollisionBoundsInfo(pQuadsBuffer, ref num);
		}

		public bool GetLiveCollisionBoundsInfo(out HmdQuad_t[] pQuadsBuffer)
		{
			uint num = 0U;
			this.FnTable.GetLiveCollisionBoundsInfo(null, ref num);
			pQuadsBuffer = new HmdQuad_t[num];
			return this.FnTable.GetLiveCollisionBoundsInfo(pQuadsBuffer, ref num);
		}

		public bool GetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetWorkingSeatedZeroPoseToRawTrackingPose(ref pmatSeatedZeroPoseToRawTrackingPose);
		}

		public bool GetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatStandingZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetWorkingStandingZeroPoseToRawTrackingPose(ref pmatStandingZeroPoseToRawTrackingPose);
		}

		public void SetWorkingPlayAreaSize(float sizeX, float sizeZ)
		{
			this.FnTable.SetWorkingPlayAreaSize(sizeX, sizeZ);
		}

		public void SetWorkingCollisionBoundsInfo(HmdQuad_t[] pQuadsBuffer)
		{
			this.FnTable.SetWorkingCollisionBoundsInfo(pQuadsBuffer, (uint)pQuadsBuffer.Length);
		}

		public void SetWorkingPerimeter(HmdVector2_t[] pPointBuffer)
		{
			this.FnTable.SetWorkingPerimeter(pPointBuffer, (uint)pPointBuffer.Length);
		}

		public void SetWorkingSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatSeatedZeroPoseToRawTrackingPose)
		{
			this.FnTable.SetWorkingSeatedZeroPoseToRawTrackingPose(ref pMatSeatedZeroPoseToRawTrackingPose);
		}

		public void SetWorkingStandingZeroPoseToRawTrackingPose(ref HmdMatrix34_t pMatStandingZeroPoseToRawTrackingPose)
		{
			this.FnTable.SetWorkingStandingZeroPoseToRawTrackingPose(ref pMatStandingZeroPoseToRawTrackingPose);
		}

		public void ReloadFromDisk(EChaperoneConfigFile configFile)
		{
			this.FnTable.ReloadFromDisk(configFile);
		}

		public bool GetLiveSeatedZeroPoseToRawTrackingPose(ref HmdMatrix34_t pmatSeatedZeroPoseToRawTrackingPose)
		{
			return this.FnTable.GetLiveSeatedZeroPoseToRawTrackingPose(ref pmatSeatedZeroPoseToRawTrackingPose);
		}

		public bool ExportLiveToBuffer(StringBuilder pBuffer, ref uint pnBufferLength)
		{
			pnBufferLength = 0U;
			return this.FnTable.ExportLiveToBuffer(pBuffer, ref pnBufferLength);
		}

		public bool ImportFromBufferToWorking(string pBuffer, uint nImportFlags)
		{
			IntPtr intPtr = Utils.ToUtf8(pBuffer);
			bool result = this.FnTable.ImportFromBufferToWorking(intPtr, nImportFlags);
			Marshal.FreeHGlobal(intPtr);
			return result;
		}

		public void ShowWorkingSetPreview()
		{
			this.FnTable.ShowWorkingSetPreview();
		}

		public void HideWorkingSetPreview()
		{
			this.FnTable.HideWorkingSetPreview();
		}

		public void RoomSetupStarting()
		{
			this.FnTable.RoomSetupStarting();
		}

		private IVRChaperoneSetup FnTable;
	}
}
