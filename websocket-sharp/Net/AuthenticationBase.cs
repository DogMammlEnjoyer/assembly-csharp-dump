using System;
using System.Collections.Specialized;
using System.Text;

namespace WebSocketSharp.Net
{
	internal abstract class AuthenticationBase
	{
		protected AuthenticationBase(AuthenticationSchemes scheme, NameValueCollection parameters)
		{
			this._scheme = scheme;
			this.Parameters = parameters;
		}

		public string Algorithm
		{
			get
			{
				return this.Parameters["algorithm"];
			}
		}

		public string Nonce
		{
			get
			{
				return this.Parameters["nonce"];
			}
		}

		public string Opaque
		{
			get
			{
				return this.Parameters["opaque"];
			}
		}

		public string Qop
		{
			get
			{
				return this.Parameters["qop"];
			}
		}

		public string Realm
		{
			get
			{
				return this.Parameters["realm"];
			}
		}

		public AuthenticationSchemes Scheme
		{
			get
			{
				return this._scheme;
			}
		}

		internal static string CreateNonceValue()
		{
			byte[] array = new byte[16];
			Random random = new Random();
			random.NextBytes(array);
			StringBuilder stringBuilder = new StringBuilder(32);
			foreach (byte b in array)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		internal static NameValueCollection ParseParameters(string value)
		{
			NameValueCollection nameValueCollection = new NameValueCollection();
			foreach (string text in value.SplitHeaderValue(new char[]
			{
				','
			}))
			{
				int num = text.IndexOf('=');
				string name = (num > 0) ? text.Substring(0, num).Trim() : null;
				string value2 = (num < 0) ? text.Trim().Trim(new char[]
				{
					'"'
				}) : ((num < text.Length - 1) ? text.Substring(num + 1).Trim().Trim(new char[]
				{
					'"'
				}) : string.Empty);
				nameValueCollection.Add(name, value2);
			}
			return nameValueCollection;
		}

		internal abstract string ToBasicString();

		internal abstract string ToDigestString();

		public override string ToString()
		{
			return (this._scheme == AuthenticationSchemes.Basic) ? this.ToBasicString() : ((this._scheme == AuthenticationSchemes.Digest) ? this.ToDigestString() : string.Empty);
		}

		private AuthenticationSchemes _scheme;

		internal NameValueCollection Parameters;
	}
}
