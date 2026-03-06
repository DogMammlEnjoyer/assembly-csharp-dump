using System;
using System.Collections.Specialized;
using System.Security.Principal;

namespace WebSocketSharp.Net
{
	public class HttpDigestIdentity : GenericIdentity
	{
		internal HttpDigestIdentity(NameValueCollection parameters) : base(parameters["username"], "Digest")
		{
			this._parameters = parameters;
		}

		public string Algorithm
		{
			get
			{
				return this._parameters["algorithm"];
			}
		}

		public string Cnonce
		{
			get
			{
				return this._parameters["cnonce"];
			}
		}

		public string Nc
		{
			get
			{
				return this._parameters["nc"];
			}
		}

		public string Nonce
		{
			get
			{
				return this._parameters["nonce"];
			}
		}

		public string Opaque
		{
			get
			{
				return this._parameters["opaque"];
			}
		}

		public string Qop
		{
			get
			{
				return this._parameters["qop"];
			}
		}

		public string Realm
		{
			get
			{
				return this._parameters["realm"];
			}
		}

		public string Response
		{
			get
			{
				return this._parameters["response"];
			}
		}

		public string Uri
		{
			get
			{
				return this._parameters["uri"];
			}
		}

		internal bool IsValid(string password, string realm, string method, string entity)
		{
			NameValueCollection nameValueCollection = new NameValueCollection(this._parameters);
			nameValueCollection["password"] = password;
			nameValueCollection["realm"] = realm;
			nameValueCollection["method"] = method;
			nameValueCollection["entity"] = entity;
			string b = AuthenticationResponse.CreateRequestDigest(nameValueCollection);
			return this._parameters["response"] == b;
		}

		private NameValueCollection _parameters;
	}
}
