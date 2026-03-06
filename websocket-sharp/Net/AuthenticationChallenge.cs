using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
	internal class AuthenticationChallenge : AuthenticationBase
	{
		private AuthenticationChallenge(AuthenticationSchemes scheme, NameValueCollection parameters) : base(scheme, parameters)
		{
		}

		internal AuthenticationChallenge(AuthenticationSchemes scheme, string realm) : base(scheme, new NameValueCollection())
		{
			this.Parameters["realm"] = realm;
			bool flag = scheme == AuthenticationSchemes.Digest;
			if (flag)
			{
				this.Parameters["nonce"] = AuthenticationBase.CreateNonceValue();
				this.Parameters["algorithm"] = "MD5";
				this.Parameters["qop"] = "auth";
			}
		}

		public string Domain
		{
			get
			{
				return this.Parameters["domain"];
			}
		}

		public string Stale
		{
			get
			{
				return this.Parameters["stale"];
			}
		}

		internal static AuthenticationChallenge CreateBasicChallenge(string realm)
		{
			return new AuthenticationChallenge(AuthenticationSchemes.Basic, realm);
		}

		internal static AuthenticationChallenge CreateDigestChallenge(string realm)
		{
			return new AuthenticationChallenge(AuthenticationSchemes.Digest, realm);
		}

		internal static AuthenticationChallenge Parse(string value)
		{
			string[] array = value.Split(new char[]
			{
				' '
			}, 2);
			bool flag = array.Length != 2;
			AuthenticationChallenge result;
			if (flag)
			{
				result = null;
			}
			else
			{
				string a = array[0].ToLower();
				result = ((a == "basic") ? new AuthenticationChallenge(AuthenticationSchemes.Basic, AuthenticationBase.ParseParameters(array[1])) : ((a == "digest") ? new AuthenticationChallenge(AuthenticationSchemes.Digest, AuthenticationBase.ParseParameters(array[1])) : null));
			}
			return result;
		}

		internal override string ToBasicString()
		{
			return string.Format("Basic realm=\"{0}\"", this.Parameters["realm"]);
		}

		internal override string ToDigestString()
		{
			StringBuilder stringBuilder = new StringBuilder(128);
			string text = this.Parameters["domain"];
			bool flag = text != null;
			if (flag)
			{
				stringBuilder.AppendFormat("Digest realm=\"{0}\", domain=\"{1}\", nonce=\"{2}\"", this.Parameters["realm"], text, this.Parameters["nonce"]);
			}
			else
			{
				stringBuilder.AppendFormat("Digest realm=\"{0}\", nonce=\"{1}\"", this.Parameters["realm"], this.Parameters["nonce"]);
			}
			string text2 = this.Parameters["opaque"];
			bool flag2 = text2 != null;
			if (flag2)
			{
				stringBuilder.AppendFormat(", opaque=\"{0}\"", text2);
			}
			string text3 = this.Parameters["stale"];
			bool flag3 = text3 != null;
			if (flag3)
			{
				stringBuilder.AppendFormat(", stale={0}", text3);
			}
			string text4 = this.Parameters["algorithm"];
			bool flag4 = text4 != null;
			if (flag4)
			{
				stringBuilder.AppendFormat(", algorithm={0}", text4);
			}
			string text5 = this.Parameters["qop"];
			bool flag5 = text5 != null;
			if (flag5)
			{
				stringBuilder.AppendFormat(", qop=\"{0}\"", text5);
			}
			return stringBuilder.ToString();
		}
	}
}
