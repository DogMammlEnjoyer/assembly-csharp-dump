using System;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class AuthenticationResponse : AuthenticationBase
	{
		private AuthenticationResponse(AuthenticationSchemes scheme, NameValueCollection parameters) : base(scheme, parameters)
		{
		}

		internal AuthenticationResponse(NetworkCredential credentials) : this(AuthenticationSchemes.Basic, new NameValueCollection(), credentials, 0U)
		{
		}

		internal AuthenticationResponse(AuthenticationChallenge challenge, NetworkCredential credentials, uint nonceCount) : this(challenge.Scheme, challenge.Parameters, credentials, nonceCount)
		{
		}

		internal AuthenticationResponse(AuthenticationSchemes scheme, NameValueCollection parameters, NetworkCredential credentials, uint nonceCount) : base(scheme, parameters)
		{
			this.Parameters["username"] = credentials.Username;
			this.Parameters["password"] = credentials.Password;
			this.Parameters["uri"] = credentials.Domain;
			this._nonceCount = nonceCount;
			bool flag = scheme == AuthenticationSchemes.Digest;
			if (flag)
			{
				this.initAsDigest();
			}
		}

		internal uint NonceCount
		{
			get
			{
				return (this._nonceCount < uint.MaxValue) ? this._nonceCount : 0U;
			}
		}

		public string Cnonce
		{
			get
			{
				return this.Parameters["cnonce"];
			}
		}

		public string Nc
		{
			get
			{
				return this.Parameters["nc"];
			}
		}

		public string Password
		{
			get
			{
				return this.Parameters["password"];
			}
		}

		public string Response
		{
			get
			{
				return this.Parameters["response"];
			}
		}

		public string Uri
		{
			get
			{
				return this.Parameters["uri"];
			}
		}

		public string UserName
		{
			get
			{
				return this.Parameters["username"];
			}
		}

		private static string createA1(string username, string password, string realm)
		{
			return string.Format("{0}:{1}:{2}", username, realm, password);
		}

		private static string createA1(string username, string password, string realm, string nonce, string cnonce)
		{
			return string.Format("{0}:{1}:{2}", AuthenticationResponse.hash(AuthenticationResponse.createA1(username, password, realm)), nonce, cnonce);
		}

		private static string createA2(string method, string uri)
		{
			return string.Format("{0}:{1}", method, uri);
		}

		private static string createA2(string method, string uri, string entity)
		{
			return string.Format("{0}:{1}:{2}", method, uri, AuthenticationResponse.hash(entity));
		}

		private static string hash(string value)
		{
			byte[] bytes = Encoding.UTF8.GetBytes(value);
			MD5 md = MD5.Create();
			byte[] array = md.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder(64);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private void initAsDigest()
		{
			string text = this.Parameters["qop"];
			bool flag = text != null;
			if (flag)
			{
				bool flag2 = text.Split(new char[]
				{
					','
				}).Contains((string qop) => qop.Trim().ToLower() == "auth");
				if (flag2)
				{
					this.Parameters["qop"] = "auth";
					this.Parameters["cnonce"] = AuthenticationBase.CreateNonceValue();
					NameValueCollection parameters = this.Parameters;
					string name = "nc";
					string format = "{0:x8}";
					uint num = this._nonceCount + 1U;
					this._nonceCount = num;
					parameters[name] = string.Format(format, num);
				}
				else
				{
					this.Parameters["qop"] = null;
				}
			}
			this.Parameters["method"] = "GET";
			this.Parameters["response"] = AuthenticationResponse.CreateRequestDigest(this.Parameters);
		}

		internal static string CreateRequestDigest(NameValueCollection parameters)
		{
			string username = parameters["username"];
			string password = parameters["password"];
			string realm = parameters["realm"];
			string text = parameters["nonce"];
			string uri = parameters["uri"];
			string text2 = parameters["algorithm"];
			string text3 = parameters["qop"];
			string text4 = parameters["cnonce"];
			string text5 = parameters["nc"];
			string method = parameters["method"];
			string value = (text2 != null && text2.ToLower() == "md5-sess") ? AuthenticationResponse.createA1(username, password, realm, text, text4) : AuthenticationResponse.createA1(username, password, realm);
			string value2 = (text3 != null && text3.ToLower() == "auth-int") ? AuthenticationResponse.createA2(method, uri, parameters["entity"]) : AuthenticationResponse.createA2(method, uri);
			string arg = AuthenticationResponse.hash(value);
			string arg2 = (text3 != null) ? string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				text,
				text5,
				text4,
				text3,
				AuthenticationResponse.hash(value2)
			}) : string.Format("{0}:{1}", text, AuthenticationResponse.hash(value2));
			return AuthenticationResponse.hash(string.Format("{0}:{1}", arg, arg2));
		}

		internal static AuthenticationResponse Parse(string value)
		{
			try
			{
				string[] array = value.Split(new char[]
				{
					' '
				}, 2);
				bool flag = array.Length != 2;
				if (flag)
				{
					return null;
				}
				string a = array[0].ToLower();
				return (a == "basic") ? new AuthenticationResponse(AuthenticationSchemes.Basic, AuthenticationResponse.ParseBasicCredentials(array[1])) : ((a == "digest") ? new AuthenticationResponse(AuthenticationSchemes.Digest, AuthenticationBase.ParseParameters(array[1])) : null);
			}
			catch
			{
			}
			return null;
		}

		internal static NameValueCollection ParseBasicCredentials(string value)
		{
			string @string = Encoding.Default.GetString(Convert.FromBase64String(value));
			int num = @string.IndexOf(':');
			string text = @string.Substring(0, num);
			string value2 = (num < @string.Length - 1) ? @string.Substring(num + 1) : string.Empty;
			num = text.IndexOf('\\');
			bool flag = num > -1;
			if (flag)
			{
				text = text.Substring(num + 1);
			}
			NameValueCollection nameValueCollection = new NameValueCollection();
			nameValueCollection["username"] = text;
			nameValueCollection["password"] = value2;
			return nameValueCollection;
		}

		internal override string ToBasicString()
		{
			string s = string.Format("{0}:{1}", this.Parameters["username"], this.Parameters["password"]);
			string str = Convert.ToBase64String(Encoding.UTF8.GetBytes(s));
			return "Basic " + str;
		}

		internal override string ToDigestString()
		{
			StringBuilder stringBuilder = new StringBuilder(256);
			stringBuilder.AppendFormat("Digest username=\"{0}\", realm=\"{1}\", nonce=\"{2}\", uri=\"{3}\", response=\"{4}\"", new object[]
			{
				this.Parameters["username"],
				this.Parameters["realm"],
				this.Parameters["nonce"],
				this.Parameters["uri"],
				this.Parameters["response"]
			});
			string text = this.Parameters["opaque"];
			bool flag = text != null;
			if (flag)
			{
				stringBuilder.AppendFormat(", opaque=\"{0}\"", text);
			}
			string text2 = this.Parameters["algorithm"];
			bool flag2 = text2 != null;
			if (flag2)
			{
				stringBuilder.AppendFormat(", algorithm={0}", text2);
			}
			string text3 = this.Parameters["qop"];
			bool flag3 = text3 != null;
			if (flag3)
			{
				stringBuilder.AppendFormat(", qop={0}, cnonce=\"{1}\", nc={2}", text3, this.Parameters["cnonce"], this.Parameters["nc"]);
			}
			return stringBuilder.ToString();
		}

		public IIdentity ToIdentity()
		{
			AuthenticationSchemes scheme = base.Scheme;
			IIdentity result;
			if (scheme != AuthenticationSchemes.Basic)
			{
				IIdentity identity = (scheme == AuthenticationSchemes.Digest) ? new HttpDigestIdentity(this.Parameters) : null;
				result = identity;
			}
			else
			{
				IIdentity identity = new HttpBasicIdentity(this.Parameters["username"], this.Parameters["password"]);
				result = identity;
			}
			return result;
		}

		private uint _nonceCount;
	}
}
