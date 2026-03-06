using System;
using System.Threading;

namespace UnityEngine.Localization.Tables
{
	[Serializable]
	public class DistributedUIDGenerator : IKeyGenerator
	{
		public long CustomEpoch
		{
			get
			{
				return this.m_CustomEpoch;
			}
		}

		public int MachineId
		{
			get
			{
				if (this.m_MachineId == 0)
				{
					this.m_MachineId = DistributedUIDGenerator.GetMachineId();
				}
				return this.m_MachineId;
			}
			set
			{
				this.m_MachineId = Mathf.Clamp(value, 1, DistributedUIDGenerator.kMaxNodeId);
			}
		}

		public DistributedUIDGenerator()
		{
		}

		public DistributedUIDGenerator(long customEpoch)
		{
			this.m_CustomEpoch = customEpoch;
		}

		public long GetNextKey()
		{
			long num = this.TimeStamp();
			if (num == this.m_LastTimestamp)
			{
				this.m_Sequence = (this.m_Sequence + 1L & (long)DistributedUIDGenerator.kMaxSequence);
				if (this.m_Sequence == 0L)
				{
					num = this.WaitNextMillis(num);
				}
			}
			else
			{
				this.m_Sequence = 0L;
			}
			this.m_LastTimestamp = num;
			return num << 22 | (long)((ulong)((ulong)this.MachineId << 12)) | this.m_Sequence;
		}

		private long TimeStamp()
		{
			return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - this.m_CustomEpoch;
		}

		private static int GetMachineId()
		{
			return Random.Range(0, DistributedUIDGenerator.kMaxNodeId);
		}

		private long WaitNextMillis(long currentTimestamp)
		{
			while (currentTimestamp == this.m_LastTimestamp)
			{
				Thread.Sleep(1);
				currentTimestamp = this.TimeStamp();
			}
			return currentTimestamp;
		}

		private const int kMachineIdBits = 10;

		private const int kSequenceBits = 12;

		private static readonly int kMaxNodeId = (int)(Mathf.Pow(2f, 10f) - 1f);

		private static readonly int kMaxSequence = (int)(Mathf.Pow(2f, 12f) - 1f);

		public const string MachineIdPrefKey = "KeyGenerator-MachineId";

		[SerializeField]
		[HideInInspector]
		private long m_CustomEpoch = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

		private long m_LastTimestamp = -1L;

		private long m_Sequence;

		private int m_MachineId;
	}
}
