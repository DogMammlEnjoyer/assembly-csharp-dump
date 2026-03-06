using System;

namespace UnityEngine.Localization.Tables
{
	public class SequentialIDGenerator : IKeyGenerator
	{
		public long NextAvailableId
		{
			get
			{
				return this.m_NextAvailableId;
			}
		}

		public SequentialIDGenerator()
		{
		}

		public SequentialIDGenerator(long startingId)
		{
			this.m_NextAvailableId = startingId;
		}

		public long GetNextKey()
		{
			long nextAvailableId = this.m_NextAvailableId;
			this.m_NextAvailableId = nextAvailableId + 1L;
			return nextAvailableId;
		}

		[SerializeField]
		private long m_NextAvailableId = 1L;
	}
}
