using System;

namespace System.Data.SqlClient
{
	/// <summary>This enumeration provides additional information about the different notifications that can be received by the dependency event handler.</summary>
	public enum SqlNotificationInfo
	{
		/// <summary>One or more tables were truncated.</summary>
		Truncate,
		/// <summary>Data was changed by an INSERT statement.</summary>
		Insert,
		/// <summary>Data was changed by an UPDATE statement.</summary>
		Update,
		/// <summary>Data was changed by a DELETE statement.</summary>
		Delete,
		/// <summary>An underlying object related to the query was dropped.</summary>
		Drop,
		/// <summary>An underlying server object related to the query was modified.</summary>
		Alter,
		/// <summary>The server was restarted (notifications are sent during restart.).</summary>
		Restart,
		/// <summary>An internal server error occurred.</summary>
		Error,
		/// <summary>A SELECT statement that cannot be notified or was provided.</summary>
		Query,
		/// <summary>A statement was provided that cannot be notified (for example, an UPDATE statement).</summary>
		Invalid,
		/// <summary>The SET options were not set appropriately at subscription time.</summary>
		Options,
		/// <summary>The statement was executed under an isolation mode that was not valid (for example, Snapshot).</summary>
		Isolation,
		/// <summary>The <see langword="SqlDependency" /> object has expired.</summary>
		Expired,
		/// <summary>Fires as a result of server resource pressure.</summary>
		Resource,
		/// <summary>A previous statement has caused query notifications to fire under the current transaction.</summary>
		PreviousFire,
		/// <summary>The subscribing query causes the number of templates on one of the target tables to exceed the maximum allowable limit.</summary>
		TemplateLimit,
		/// <summary>Used to distinguish the server-side cause for a query notification firing.</summary>
		Merge,
		/// <summary>Used when the info option sent by the server was not recognized by the client.</summary>
		Unknown = -1,
		/// <summary>The <see langword="SqlDependency" /> object already fired, and new commands cannot be added to it.</summary>
		AlreadyChanged = -2
	}
}
