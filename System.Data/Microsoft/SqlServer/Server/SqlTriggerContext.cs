using System;
using System.Data.Common;
using System.Data.SqlTypes;
using Unity;

namespace Microsoft.SqlServer.Server
{
	/// <summary>Provides contextual information about the trigger that was fired.</summary>
	public sealed class SqlTriggerContext
	{
		internal SqlTriggerContext(TriggerAction triggerAction, bool[] columnsUpdated, SqlXml eventInstanceData)
		{
			this._triggerAction = triggerAction;
			this._columnsUpdated = columnsUpdated;
			this._eventInstanceData = eventInstanceData;
		}

		/// <summary>Gets the number of columns contained by the data table bound to the trigger. This property is read-only.</summary>
		/// <returns>The number of columns contained by the data table bound to the trigger, as an integer.</returns>
		public int ColumnCount
		{
			get
			{
				int result = 0;
				if (this._columnsUpdated != null)
				{
					result = this._columnsUpdated.Length;
				}
				return result;
			}
		}

		/// <summary>Gets the event data specific to the action that fired the trigger.</summary>
		/// <returns>The event data specific to the action that fired the trigger as a <see cref="T:System.Data.SqlTypes.SqlXml" /> if more information is available; <see langword="null" /> otherwise.</returns>
		public SqlXml EventData
		{
			get
			{
				return this._eventInstanceData;
			}
		}

		/// <summary>Indicates what action fired the trigger.</summary>
		/// <returns>The action that fired the trigger as a <see cref="T:Microsoft.SqlServer.Server.TriggerAction" />.</returns>
		public TriggerAction TriggerAction
		{
			get
			{
				return this._triggerAction;
			}
		}

		/// <summary>Returns <see langword="true" /> if a column was affected by an INSERT or UPDATE statement.</summary>
		/// <param name="columnOrdinal">The zero-based ordinal of the column.</param>
		/// <returns>
		///   <see langword="true" /> if the column was affected by an INSERT or UPDATE operation.</returns>
		/// <exception cref="T:System.InvalidOperationException">Called in the context of a trigger where the value of the <see cref="P:Microsoft.SqlServer.Server.SqlTriggerContext.TriggerAction" /> property is not <see langword="Insert" /> or <see langword="Update" />.</exception>
		public bool IsUpdatedColumn(int columnOrdinal)
		{
			if (this._columnsUpdated != null)
			{
				return this._columnsUpdated[columnOrdinal];
			}
			throw ADP.IndexOutOfRange(columnOrdinal);
		}

		internal SqlTriggerContext()
		{
			ThrowStub.ThrowNotSupportedException();
		}

		private TriggerAction _triggerAction;

		private bool[] _columnsUpdated;

		private SqlXml _eventInstanceData;
	}
}
