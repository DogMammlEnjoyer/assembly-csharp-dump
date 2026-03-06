using System;
using System.Runtime.InteropServices;

namespace Valve.VR
{
	public class CVRHeadsetView
	{
		internal CVRHeadsetView(IntPtr pInterface)
		{
			this.FnTable = (IVRHeadsetView)Marshal.PtrToStructure(pInterface, typeof(IVRHeadsetView));
		}

		public void SetHeadsetViewSize(uint nWidth, uint nHeight)
		{
			this.FnTable.SetHeadsetViewSize(nWidth, nHeight);
		}

		public void GetHeadsetViewSize(ref uint pnWidth, ref uint pnHeight)
		{
			pnWidth = 0U;
			pnHeight = 0U;
			this.FnTable.GetHeadsetViewSize(ref pnWidth, ref pnHeight);
		}

		public void SetHeadsetViewMode(uint eHeadsetViewMode)
		{
			this.FnTable.SetHeadsetViewMode(eHeadsetViewMode);
		}

		public uint GetHeadsetViewMode()
		{
			return this.FnTable.GetHeadsetViewMode();
		}

		public void SetHeadsetViewCropped(bool bCropped)
		{
			this.FnTable.SetHeadsetViewCropped(bCropped);
		}

		public bool GetHeadsetViewCropped()
		{
			return this.FnTable.GetHeadsetViewCropped();
		}

		public float GetHeadsetViewAspectRatio()
		{
			return this.FnTable.GetHeadsetViewAspectRatio();
		}

		public void SetHeadsetViewBlendRange(float flStartPct, float flEndPct)
		{
			this.FnTable.SetHeadsetViewBlendRange(flStartPct, flEndPct);
		}

		public void GetHeadsetViewBlendRange(ref float pStartPct, ref float pEndPct)
		{
			pStartPct = 0f;
			pEndPct = 0f;
			this.FnTable.GetHeadsetViewBlendRange(ref pStartPct, ref pEndPct);
		}

		private IVRHeadsetView FnTable;
	}
}
