using System;

namespace Microsoft.SqlServer.Server
{
	/// <summary>The <see cref="T:Microsoft.SqlServer.Server.TriggerAction" /> enumeration is used by the <see cref="T:Microsoft.SqlServer.Server.SqlTriggerContext" /> class to indicate what action fired the trigger.</summary>
	public enum TriggerAction
	{
		/// <summary>An invalid trigger action, one that is not exposed to the user, occurred.</summary>
		Invalid,
		/// <summary>An INSERT Transact-SQL statement was executed.</summary>
		Insert,
		/// <summary>An UPDATE Transact-SQL statement was executed.</summary>
		Update,
		/// <summary>A DELETE Transact-SQL statement was executed.</summary>
		Delete,
		/// <summary>A CREATE TABLE Transact-SQL statement was executed.</summary>
		CreateTable = 21,
		/// <summary>An ALTER TABLE Transact-SQL statement was executed.</summary>
		AlterTable,
		/// <summary>A DROP TABLE Transact-SQL statement was executed.</summary>
		DropTable,
		/// <summary>A CREATE INDEX Transact-SQL statement was executed.</summary>
		CreateIndex,
		/// <summary>An ALTER INDEX Transact-SQL statement was executed.</summary>
		AlterIndex,
		/// <summary>A DROP INDEX Transact-SQL statement was executed.</summary>
		DropIndex,
		/// <summary>A CREATE SYNONYM Transact-SQL statement was executed.</summary>
		CreateSynonym = 34,
		/// <summary>A DROP SYNONYM Transact-SQL statement was executed.</summary>
		DropSynonym = 36,
		/// <summary>Not available.</summary>
		CreateSecurityExpression = 31,
		/// <summary>Not available.</summary>
		DropSecurityExpression = 33,
		/// <summary>A CREATE VIEW Transact-SQL statement was executed.</summary>
		CreateView = 41,
		/// <summary>An ALTER VIEW Transact-SQL statement was executed.</summary>
		AlterView,
		/// <summary>A DROP VIEW Transact-SQL statement was executed.</summary>
		DropView,
		/// <summary>A CREATE PROCEDURE Transact-SQL statement was executed.</summary>
		CreateProcedure = 51,
		/// <summary>An ALTER PROCEDURE Transact-SQL statement was executed.</summary>
		AlterProcedure,
		/// <summary>A DROP PROCEDURE Transact-SQL statement was executed.</summary>
		DropProcedure,
		/// <summary>A CREATE FUNCTION Transact-SQL statement was executed.</summary>
		CreateFunction = 61,
		/// <summary>An ALTER FUNCTION Transact-SQL statement was executed.</summary>
		AlterFunction,
		/// <summary>A DROP FUNCTION Transact-SQL statement was executed.</summary>
		DropFunction,
		/// <summary>A CREATE TRIGGER Transact-SQL statement was executed.</summary>
		CreateTrigger = 71,
		/// <summary>An ALTER TRIGGER Transact-SQL statement was executed.</summary>
		AlterTrigger,
		/// <summary>A DROP TRIGGER Transact-SQL statement was executed.</summary>
		DropTrigger,
		/// <summary>A CREATE EVENT NOTIFICATION Transact-SQL statement was executed.</summary>
		CreateEventNotification,
		/// <summary>A DROP EVENT NOTIFICATION Transact-SQL statement was executed.</summary>
		DropEventNotification = 76,
		/// <summary>A CREATE TYPE Transact-SQL statement was executed.</summary>
		CreateType = 91,
		/// <summary>A DROP TYPE Transact-SQL statement was executed.</summary>
		DropType = 93,
		/// <summary>A CREATE ASSEMBLY Transact-SQL statement was executed.</summary>
		CreateAssembly = 101,
		/// <summary>An ALTER ASSEMBLY Transact-SQL statement was executed.</summary>
		AlterAssembly,
		/// <summary>A DROP ASSEMBLY Transact-SQL statement was executed.</summary>
		DropAssembly,
		/// <summary>A CREATE USER Transact-SQL statement was executed.</summary>
		CreateUser = 131,
		/// <summary>An ALTER USER Transact-SQL statement was executed.</summary>
		AlterUser,
		/// <summary>A DROP USER Transact-SQL statement was executed.</summary>
		DropUser,
		/// <summary>A CREATE ROLE Transact-SQL statement was executed.</summary>
		CreateRole,
		/// <summary>An ALTER ROLE Transact-SQL statement was executed.</summary>
		AlterRole,
		/// <summary>A DROP ROLE Transact-SQL statement was executed.</summary>
		DropRole,
		/// <summary>A CREATE APPLICATION ROLE Transact-SQL statement was executed.</summary>
		CreateAppRole,
		/// <summary>An ALTER APPLICATION ROLE Transact-SQL statement was executed.</summary>
		AlterAppRole,
		/// <summary>A DROP APPLICATION ROLE Transact-SQL statement was executed.</summary>
		DropAppRole,
		/// <summary>A CREATE SCHEMA Transact-SQL statement was executed.</summary>
		CreateSchema = 141,
		/// <summary>An ALTER SCHEMA Transact-SQL statement was executed.</summary>
		AlterSchema,
		/// <summary>A DROP SCHEMA Transact-SQL statement was executed.</summary>
		DropSchema,
		/// <summary>A CREATE LOGIN Transact-SQL statement was executed.</summary>
		CreateLogin,
		/// <summary>An ALTER LOGIN Transact-SQL statement was executed.</summary>
		AlterLogin,
		/// <summary>A DROP LOGIN Transact-SQL statement was executed.</summary>
		DropLogin,
		/// <summary>A CREATE MESSAGE TYPE Transact-SQL statement was executed.</summary>
		CreateMsgType = 151,
		/// <summary>A DROP MESSAGE TYPE Transact-SQL statement was executed.</summary>
		DropMsgType = 153,
		/// <summary>A CREATE CONTRACT Transact-SQL statement was executed.</summary>
		CreateContract,
		/// <summary>A DROP CONTRACT Transact-SQL statement was executed.</summary>
		DropContract = 156,
		/// <summary>A CREATE QUEUE Transact-SQL statement was executed.</summary>
		CreateQueue,
		/// <summary>An ALTER QUEUE Transact-SQL statement was executed.</summary>
		AlterQueue,
		/// <summary>A DROP QUEUE Transact-SQL statement was executed.</summary>
		DropQueue,
		/// <summary>A CREATE SERVICE Transact-SQL statement was executed.</summary>
		CreateService = 161,
		/// <summary>An ALTER SERVICE Transact-SQL statement was executed.</summary>
		AlterService,
		/// <summary>A DROP SERVICE Transact-SQL statement was executed.</summary>
		DropService,
		/// <summary>A CREATE ROUTE Transact-SQL statement was executed.</summary>
		CreateRoute,
		/// <summary>An ALTER ROUTE Transact-SQL statement was executed.</summary>
		AlterRoute,
		/// <summary>A DROP ROUTE Transact-SQL statement was executed.</summary>
		DropRoute,
		/// <summary>A GRANT Transact-SQL statement was executed.</summary>
		GrantStatement,
		/// <summary>A DENY Transact-SQL statement was executed.</summary>
		DenyStatement,
		/// <summary>A REVOKE Transact-SQL statement was executed.</summary>
		RevokeStatement,
		/// <summary>A GRANT OBJECT Transact-SQL statement was executed.</summary>
		GrantObject,
		/// <summary>A DENY Object Permissions Transact-SQL statement was executed.</summary>
		DenyObject,
		/// <summary>A REVOKE OBJECT Transact-SQL statement was executed.</summary>
		RevokeObject,
		/// <summary>A CREATE_REMOTE_SERVICE_BINDING event type was specified when an event notification was created on the database or server instance.</summary>
		CreateBinding = 174,
		/// <summary>An ALTER_REMOTE_SERVICE_BINDING event type was specified when an event notification was created on the database or server instance.</summary>
		AlterBinding,
		/// <summary>A DROP_REMOTE_SERVICE_BINDING event type was specified when an event notification was created on the database or server instance.</summary>
		DropBinding,
		/// <summary>A CREATE PARTITION FUNCTION Transact-SQL statement was executed.</summary>
		CreatePartitionFunction = 191,
		/// <summary>An ALTER PARTITION FUNCTION Transact-SQL statement was executed.</summary>
		AlterPartitionFunction,
		/// <summary>A DROP PARTITION FUNCTION Transact-SQL statement was executed.</summary>
		DropPartitionFunction,
		/// <summary>A CREATE PARTITION SCHEME Transact-SQL statement was executed.</summary>
		CreatePartitionScheme,
		/// <summary>An ALTER PARTITION SCHEME Transact-SQL statement was executed.</summary>
		AlterPartitionScheme,
		/// <summary>A DROP PARTITION SCHEME Transact-SQL statement was executed.</summary>
		DropPartitionScheme
	}
}
