using System;

namespace System.Data.SqlClient
{
	/// <summary>Describes the different notification types that can be received by an <see cref="T:System.Data.SqlClient.OnChangeEventHandler" /> event handler through the <see cref="T:System.Data.SqlClient.SqlNotificationEventArgs" /> parameter.</summary>
	public enum SqlNotificationType
	{
		/// <summary>Data on the server being monitored changed. Use the <see cref="T:System.Data.SqlClient.SqlNotificationInfo" /> item to determine the details of the change.</summary>
		Change,
		/// <summary>There was a failure to create a notification subscription. Use the <see cref="T:System.Data.SqlClient.SqlNotificationEventArgs" /> object's <see cref="T:System.Data.SqlClient.SqlNotificationInfo" /> item to determine the cause of the failure.</summary>
		Subscribe,
		/// <summary>Used when the type option sent by the server was not recognized by the client.</summary>
		Unknown = -1
	}
}
