using System;

namespace System.EnterpriseServices.CompensatingResourceManager
{
	/// <summary>Represents the base class for all Compensating Resource Manager (CRM) Compensators.</summary>
	public class Compensator : ServicedComponent
	{
		/// <summary>Initializes a new instance of the <see cref="T:System.EnterpriseServices.CompensatingResourceManager.Compensator" /> class.</summary>
		[MonoTODO]
		public Compensator()
		{
			throw new NotImplementedException();
		}

		/// <summary>Gets a value representing the Compensating Resource Manager (CRM) <see cref="T:System.EnterpriseServices.CompensatingResourceManager.Clerk" /> object.</summary>
		/// <returns>The <see cref="T:System.EnterpriseServices.CompensatingResourceManager.Clerk" /> object.</returns>
		public Clerk Clerk
		{
			[MonoTODO]
			get
			{
				throw new NotImplementedException();
			}
		}

		/// <summary>Delivers a log record to the Compensating Resource Manager (CRM) Compensator during the abort phase.</summary>
		/// <param name="rec">The log record to be delivered.</param>
		/// <returns>
		///   <see langword="true" /> if the delivered record should be forgotten; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public virtual bool AbortRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator of the abort phase of the transaction completion, and the upcoming delivery of records.</summary>
		/// <param name="fRecovery">
		///   <see langword="true" /> to begin abort phase; otherwise, <see langword="false" />.</param>
		[MonoTODO]
		public virtual void BeginAbort(bool fRecovery)
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator of the commit phase of the transaction completion and the upcoming delivery of records.</summary>
		/// <param name="fRecovery">
		///   <see langword="true" /> to begin commit phase; otherwise, <see langword="false" />.</param>
		[MonoTODO]
		public virtual void BeginCommit(bool fRecovery)
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator of the prepare phase of the transaction completion and the upcoming delivery of records.</summary>
		[MonoTODO]
		public virtual void BeginPrepare()
		{
			throw new NotImplementedException();
		}

		/// <summary>Delivers a log record in forward order during the commit phase.</summary>
		/// <param name="rec">The log record to forward.</param>
		/// <returns>
		///   <see langword="true" /> if the delivered record should be forgotten; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public virtual bool CommitRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator that it has received all the log records available during the abort phase.</summary>
		[MonoTODO]
		public virtual void EndAbort()
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator that it has delivered all the log records available during the commit phase.</summary>
		[MonoTODO]
		public virtual void EndCommit()
		{
			throw new NotImplementedException();
		}

		/// <summary>Notifies the Compensating Resource Manager (CRM) Compensator that it has had all the log records available during the prepare phase.</summary>
		/// <returns>
		///   <see langword="true" /> if successful; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public virtual bool EndPrepare()
		{
			throw new NotImplementedException();
		}

		/// <summary>Delivers a log record in forward order during the prepare phase.</summary>
		/// <param name="rec">The log record to forward.</param>
		/// <returns>
		///   <see langword="true" /> if the delivered record should be forgotten; otherwise, <see langword="false" />.</returns>
		[MonoTODO]
		public virtual bool PrepareRecord(LogRecord rec)
		{
			throw new NotImplementedException();
		}
	}
}
