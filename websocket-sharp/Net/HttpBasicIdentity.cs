using System;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public class HttpBasicIdentity : GenericIdentity
	{
		internal HttpBasicIdentity(string username, string password) : base(username, "Basic")
		{
			this._password = password;
		}

		public virtual string Password
		{
			get
			{
				return this._password;
			}
		}

		private string _password;
	}
}
