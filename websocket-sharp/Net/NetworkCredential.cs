using System;

namespace WebSocketSharp.Net
{
	public class NetworkCredential
	{
		public NetworkCredential(string username, string password) : this(username, password, null, null)
		{
		}

		public NetworkCredential(string username, string password, string domain, params string[] roles)
		{
			bool flag = username == null;
			if (flag)
			{
				throw new ArgumentNullException("username");
			}
			bool flag2 = username.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "username");
			}
			this._username = username;
			this._password = password;
			this._domain = domain;
			this._roles = roles;
		}

		public string Domain
		{
			get
			{
				return this._domain ?? string.Empty;
			}
			internal set
			{
				this._domain = value;
			}
		}

		public string Password
		{
			get
			{
				return this._password ?? string.Empty;
			}
			internal set
			{
				this._password = value;
			}
		}

		public string[] Roles
		{
			get
			{
				return this._roles ?? NetworkCredential._noRoles;
			}
			internal set
			{
				this._roles = value;
			}
		}

		public string Username
		{
			get
			{
				return this._username;
			}
			internal set
			{
				this._username = value;
			}
		}

		private string _domain;

		private static readonly string[] _noRoles = new string[0];

		private string _password;

		private string[] _roles;

		private string _username;
	}
}
