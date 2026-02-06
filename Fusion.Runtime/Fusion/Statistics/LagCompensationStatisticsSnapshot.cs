using System;
using System.Diagnostics;

namespace Fusion.Statistics
{
	public class LagCompensationStatisticsSnapshot
	{
		public double TotalElapsedTime
		{
			get
			{
				return this.AdvanceBufferTime + this.AddOnBufferTime + this.AddOnBVHTime + this.UpdateBufferTime + this.UpdateBVHTime + this.RefitBVHTime;
			}
		}

		public int BVHMaxDeep { get; private set; }

		public int BVHNodesCount { get; private set; }

		public int HitboxesCount { get; private set; }

		public double AddOnBufferTime { get; private set; }

		public double AddOnBVHTime { get; private set; }

		public double UpdateBVHTime { get; private set; }

		public double UpdateBufferTime { get; private set; }

		public double AdvanceBufferTime { get; private set; }

		public double RefitBVHTime { get; private set; }

		internal void CopyFromSnapshot(LagCompensationStatisticsSnapshot snapshot)
		{
			this.BVHMaxDeep = snapshot.BVHMaxDeep;
			this.BVHNodesCount = snapshot.BVHNodesCount;
			this.HitboxesCount = snapshot.HitboxesCount;
			this.AddOnBufferTime = snapshot.AddOnBufferTime;
			this.AddOnBVHTime = snapshot.AddOnBVHTime;
			this.UpdateBVHTime = snapshot.UpdateBVHTime;
			this.UpdateBufferTime = snapshot.UpdateBufferTime;
			this.AdvanceBufferTime = snapshot.AdvanceBufferTime;
			this.RefitBVHTime = snapshot.RefitBVHTime;
		}

		internal void ClearSnapshot()
		{
			this.BVHMaxDeep = 0;
			this.BVHNodesCount = 0;
			this.HitboxesCount = 0;
			this.AddOnBufferTime = 0.0;
			this.AddOnBVHTime = 0.0;
			this.UpdateBVHTime = 0.0;
			this.UpdateBufferTime = 0.0;
			this.AdvanceBufferTime = 0.0;
			this.RefitBVHTime = 0.0;
		}

		[Conditional("DEBUG")]
		internal void SetBVHMaxDeep(int value, bool overrideValue = false)
		{
			this.BVHMaxDeep = (overrideValue ? value : (this.BVHMaxDeep + value));
		}

		[Conditional("DEBUG")]
		internal void SetBVHNodeCount(int value, bool overrideValue = false)
		{
			this.BVHNodesCount = (overrideValue ? value : (this.BVHMaxDeep + value));
		}

		[Conditional("DEBUG")]
		internal void SetHitboxesCount(int value, bool overrideValue = false)
		{
			this.HitboxesCount = (overrideValue ? value : (this.HitboxesCount + value));
		}

		[Conditional("DEBUG")]
		internal void SetAddOnBufferTime(double value, bool overrideValue = false)
		{
			this.AddOnBufferTime = (overrideValue ? value : (this.AddOnBufferTime + value));
		}

		[Conditional("DEBUG")]
		internal void SetAddOnBVHTime(double value, bool overrideValue = false)
		{
			this.AddOnBVHTime = (overrideValue ? value : (this.AddOnBVHTime + value));
		}

		[Conditional("DEBUG")]
		internal void SetUpdateBVHTime(double value, bool overrideValue = false)
		{
			this.UpdateBVHTime = (overrideValue ? value : (this.UpdateBVHTime + value));
		}

		[Conditional("DEBUG")]
		internal void SetUpdateBufferTime(double value, bool overrideValue = false)
		{
			this.UpdateBufferTime = (overrideValue ? value : (this.UpdateBufferTime + value));
		}

		[Conditional("DEBUG")]
		internal void SetAdvanceBufferTime(double value, bool overrideValue = false)
		{
			this.AdvanceBufferTime = (overrideValue ? value : (this.AdvanceBufferTime + value));
		}

		[Conditional("DEBUG")]
		internal void SetRefitBVHTime(double value, bool overrideValue = false)
		{
			this.RefitBVHTime = (overrideValue ? value : (this.RefitBVHTime + value));
		}
	}
}
