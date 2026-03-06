using System;
using System.Data.SqlClient;
using System.Security.Principal;

namespace System.Data.ProviderBase
{
	[Serializable]
	internal sealed class DbConnectionPoolIdentity
	{
		internal static DbConnectionPoolIdentity GetCurrent()
		{
			if (!TdsParserStateObjectFactory.UseManagedSNI)
			{
				return DbConnectionPoolIdentity.GetCurrentNative();
			}
			return DbConnectionPoolIdentity.GetCurrentManaged();
		}

		private static DbConnectionPoolIdentity GetCurrentNative()
		{
			DbConnectionPoolIdentity result;
			using (WindowsIdentity current = WindowsIdentity.GetCurrent())
			{
				IntPtr token = current.AccessToken.DangerousGetHandle();
				bool flag = current.User.IsWellKnown(WellKnownSidType.NetworkSid);
				string value = current.User.Value;
				bool flag2 = Win32NativeMethods.IsTokenRestrictedWrapper(token);
				DbConnectionPoolIdentity dbConnectionPoolIdentity = DbConnectionPoolIdentity.s_lastIdentity;
				if (dbConnectionPoolIdentity != null && dbConnectionPoolIdentity._sidString == value && dbConnectionPoolIdentity._isRestricted == flag2 && dbConnectionPoolIdentity._isNetwork == flag)
				{
					result = dbConnectionPoolIdentity;
				}
				else
				{
					result = new DbConnectionPoolIdentity(value, flag2, flag);
				}
			}
			DbConnectionPoolIdentity.s_lastIdentity = result;
			return result;
		}

		private DbConnectionPoolIdentity(string sidString, bool isRestricted, bool isNetwork)
		{
			this._sidString = sidString;
			this._isRestricted = isRestricted;
			this._isNetwork = isNetwork;
			this._hashCode = ((sidString == null) ? 0 : sidString.GetHashCode());
		}

		internal bool IsRestricted
		{
			get
			{
				return this._isRestricted;
			}
		}

		public override bool Equals(object value)
		{
			bool flag = this == DbConnectionPoolIdentity.NoIdentity || this == value;
			if (!flag && value != null)
			{
				DbConnectionPoolIdentity dbConnectionPoolIdentity = (DbConnectionPoolIdentity)value;
				flag = (this._sidString == dbConnectionPoolIdentity._sidString && this._isRestricted == dbConnectionPoolIdentity._isRestricted && this._isNetwork == dbConnectionPoolIdentity._isNetwork);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return this._hashCode;
		}

		internal static DbConnectionPoolIdentity GetCurrentManaged()
		{
			string sidString = ((!string.IsNullOrWhiteSpace(Environment.UserDomainName)) ? (Environment.UserDomainName + "\\") : "") + Environment.UserName;
			bool isNetwork = false;
			bool isRestricted = false;
			return new DbConnectionPoolIdentity(sidString, isRestricted, isNetwork);
		}

		private static DbConnectionPoolIdentity s_lastIdentity = null;

		public static readonly DbConnectionPoolIdentity NoIdentity = new DbConnectionPoolIdentity(string.Empty, false, true);

		private readonly string _sidString;

		private readonly bool _isRestricted;

		private readonly bool _isNetwork;

		private readonly int _hashCode;
	}
}
