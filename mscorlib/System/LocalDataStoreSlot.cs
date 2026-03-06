using System;
using System.Runtime.InteropServices;
using Unity;

namespace System
{
	/// <summary>Encapsulates a memory slot to store local data. This class cannot be inherited.</summary>
	[ComVisible(true)]
	public sealed class LocalDataStoreSlot
	{
		internal LocalDataStoreSlot(LocalDataStoreMgr mgr, int slot, long cookie)
		{
			this.m_mgr = mgr;
			this.m_slot = slot;
			this.m_cookie = cookie;
		}

		internal LocalDataStoreMgr Manager
		{
			get
			{
				return this.m_mgr;
			}
		}

		internal int Slot
		{
			get
			{
				return this.m_slot;
			}
		}

		internal long Cookie
		{
			get
			{
				return this.m_cookie;
			}
		}

		/// <summary>Ensures that resources are freed and other cleanup operations are performed when the garbage collector reclaims the <see cref="T:System.LocalDataStoreSlot" /> object.</summary>
		protected override void Finalize()
		{
			try
			{
				LocalDataStoreMgr mgr = this.m_mgr;
				if (mgr != null)
				{
					int slot = this.m_slot;
					this.m_slot = -1;
					mgr.FreeDataSlot(slot, this.m_cookie);
				}
			}
			finally
			{
				base.Finalize();
			}
		}

		internal LocalDataStoreSlot()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private LocalDataStoreMgr m_mgr;

		private int m_slot;

		private long m_cookie;
	}
}
