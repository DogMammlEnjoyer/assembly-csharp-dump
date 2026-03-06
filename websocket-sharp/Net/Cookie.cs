using System;
using System.Globalization;
using System.Text;

namespace WebSocketSharp.Net
{
	[Serializable]
	public sealed class Cookie
	{
		internal Cookie()
		{
			this.init(string.Empty, string.Empty, string.Empty, string.Empty);
		}

		public Cookie(string name, string value) : this(name, value, string.Empty, string.Empty)
		{
		}

		public Cookie(string name, string value, string path) : this(name, value, path, string.Empty)
		{
		}

		public Cookie(string name, string value, string path, string domain)
		{
			bool flag = name == null;
			if (flag)
			{
				throw new ArgumentNullException("name");
			}
			bool flag2 = name.Length == 0;
			if (flag2)
			{
				throw new ArgumentException("An empty string.", "name");
			}
			bool flag3 = name[0] == '$';
			if (flag3)
			{
				string message = "It starts with a dollar sign.";
				throw new ArgumentException(message, "name");
			}
			bool flag4 = !name.IsToken();
			if (flag4)
			{
				string message2 = "It contains an invalid character.";
				throw new ArgumentException(message2, "name");
			}
			bool flag5 = value == null;
			if (flag5)
			{
				value = string.Empty;
			}
			bool flag6 = value.Contains(Cookie._reservedCharsForValue);
			if (flag6)
			{
				bool flag7 = !value.IsEnclosedIn('"');
				if (flag7)
				{
					string message3 = "A string not enclosed in double quotes.";
					throw new ArgumentException(message3, "value");
				}
			}
			this.init(name, value, path ?? string.Empty, domain ?? string.Empty);
		}

		internal bool ExactDomain
		{
			get
			{
				return this._domain.Length == 0 || this._domain[0] != '.';
			}
		}

		internal int MaxAge
		{
			get
			{
				bool flag = this._expires == DateTime.MinValue;
				int result;
				if (flag)
				{
					result = 0;
				}
				else
				{
					DateTime d = (this._expires.Kind != DateTimeKind.Local) ? this._expires.ToLocalTime() : this._expires;
					TimeSpan t = d - DateTime.Now;
					result = ((t > TimeSpan.Zero) ? ((int)t.TotalSeconds) : 0);
				}
				return result;
			}
			set
			{
				this._expires = ((value > 0) ? DateTime.Now.AddSeconds((double)value) : DateTime.Now);
			}
		}

		internal int[] Ports
		{
			get
			{
				return this._ports ?? Cookie._emptyPorts;
			}
		}

		internal string SameSite
		{
			get
			{
				return this._sameSite;
			}
			set
			{
				this._sameSite = value;
			}
		}

		public string Comment
		{
			get
			{
				return this._comment;
			}
			internal set
			{
				this._comment = value;
			}
		}

		public Uri CommentUri
		{
			get
			{
				return this._commentUri;
			}
			internal set
			{
				this._commentUri = value;
			}
		}

		public bool Discard
		{
			get
			{
				return this._discard;
			}
			internal set
			{
				this._discard = value;
			}
		}

		public string Domain
		{
			get
			{
				return this._domain;
			}
			set
			{
				this._domain = (value ?? string.Empty);
			}
		}

		public bool Expired
		{
			get
			{
				return this._expires != DateTime.MinValue && this._expires <= DateTime.Now;
			}
			set
			{
				this._expires = (value ? DateTime.Now : DateTime.MinValue);
			}
		}

		public DateTime Expires
		{
			get
			{
				return this._expires;
			}
			set
			{
				this._expires = value;
			}
		}

		public bool HttpOnly
		{
			get
			{
				return this._httpOnly;
			}
			set
			{
				this._httpOnly = value;
			}
		}

		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					throw new ArgumentNullException("value");
				}
				bool flag2 = value.Length == 0;
				if (flag2)
				{
					throw new ArgumentException("An empty string.", "value");
				}
				bool flag3 = value[0] == '$';
				if (flag3)
				{
					string message = "It starts with a dollar sign.";
					throw new ArgumentException(message, "value");
				}
				bool flag4 = !value.IsToken();
				if (flag4)
				{
					string message2 = "It contains an invalid character.";
					throw new ArgumentException(message2, "value");
				}
				this._name = value;
			}
		}

		public string Path
		{
			get
			{
				return this._path;
			}
			set
			{
				this._path = (value ?? string.Empty);
			}
		}

		public string Port
		{
			get
			{
				return this._port;
			}
			internal set
			{
				int[] ports;
				bool flag = !Cookie.tryCreatePorts(value, out ports);
				if (!flag)
				{
					this._port = value;
					this._ports = ports;
				}
			}
		}

		public bool Secure
		{
			get
			{
				return this._secure;
			}
			set
			{
				this._secure = value;
			}
		}

		public DateTime TimeStamp
		{
			get
			{
				return this._timeStamp;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				bool flag = value == null;
				if (flag)
				{
					value = string.Empty;
				}
				bool flag2 = value.Contains(Cookie._reservedCharsForValue);
				if (flag2)
				{
					bool flag3 = !value.IsEnclosedIn('"');
					if (flag3)
					{
						string message = "A string not enclosed in double quotes.";
						throw new ArgumentException(message, "value");
					}
				}
				this._value = value;
			}
		}

		public int Version
		{
			get
			{
				return this._version;
			}
			internal set
			{
				bool flag = value < 0 || value > 1;
				if (!flag)
				{
					this._version = value;
				}
			}
		}

		private static int hash(int i, int j, int k, int l, int m)
		{
			return i ^ (j << 13 | j >> 19) ^ (k << 26 | k >> 6) ^ (l << 7 | l >> 25) ^ (m << 20 | m >> 12);
		}

		private void init(string name, string value, string path, string domain)
		{
			this._name = name;
			this._value = value;
			this._path = path;
			this._domain = domain;
			this._expires = DateTime.MinValue;
			this._timeStamp = DateTime.Now;
		}

		private string toResponseStringVersion0()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}", this._name, this._value);
			bool flag = this._expires != DateTime.MinValue;
			if (flag)
			{
				stringBuilder.AppendFormat("; Expires={0}", this._expires.ToUniversalTime().ToString("ddd, dd'-'MMM'-'yyyy HH':'mm':'ss 'GMT'", CultureInfo.CreateSpecificCulture("en-US")));
			}
			bool flag2 = !this._path.IsNullOrEmpty();
			if (flag2)
			{
				stringBuilder.AppendFormat("; Path={0}", this._path);
			}
			bool flag3 = !this._domain.IsNullOrEmpty();
			if (flag3)
			{
				stringBuilder.AppendFormat("; Domain={0}", this._domain);
			}
			bool flag4 = !this._sameSite.IsNullOrEmpty();
			if (flag4)
			{
				stringBuilder.AppendFormat("; SameSite={0}", this._sameSite);
			}
			bool secure = this._secure;
			if (secure)
			{
				stringBuilder.Append("; Secure");
			}
			bool httpOnly = this._httpOnly;
			if (httpOnly)
			{
				stringBuilder.Append("; HttpOnly");
			}
			return stringBuilder.ToString();
		}

		private string toResponseStringVersion1()
		{
			StringBuilder stringBuilder = new StringBuilder(64);
			stringBuilder.AppendFormat("{0}={1}; Version={2}", this._name, this._value, this._version);
			bool flag = this._expires != DateTime.MinValue;
			if (flag)
			{
				stringBuilder.AppendFormat("; Max-Age={0}", this.MaxAge);
			}
			bool flag2 = !this._path.IsNullOrEmpty();
			if (flag2)
			{
				stringBuilder.AppendFormat("; Path={0}", this._path);
			}
			bool flag3 = !this._domain.IsNullOrEmpty();
			if (flag3)
			{
				stringBuilder.AppendFormat("; Domain={0}", this._domain);
			}
			bool flag4 = this._port != null;
			if (flag4)
			{
				bool flag5 = this._port != "\"\"";
				if (flag5)
				{
					stringBuilder.AppendFormat("; Port={0}", this._port);
				}
				else
				{
					stringBuilder.Append("; Port");
				}
			}
			bool flag6 = this._comment != null;
			if (flag6)
			{
				stringBuilder.AppendFormat("; Comment={0}", HttpUtility.UrlEncode(this._comment));
			}
			bool flag7 = this._commentUri != null;
			if (flag7)
			{
				string originalString = this._commentUri.OriginalString;
				stringBuilder.AppendFormat("; CommentURL={0}", (!originalString.IsToken()) ? originalString.Quote() : originalString);
			}
			bool discard = this._discard;
			if (discard)
			{
				stringBuilder.Append("; Discard");
			}
			bool secure = this._secure;
			if (secure)
			{
				stringBuilder.Append("; Secure");
			}
			return stringBuilder.ToString();
		}

		private static bool tryCreatePorts(string value, out int[] result)
		{
			result = null;
			string[] array = value.Trim(new char[]
			{
				'"'
			}).Split(new char[]
			{
				','
			});
			int num = array.Length;
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				string text = array[i].Trim();
				bool flag = text.Length == 0;
				if (flag)
				{
					array2[i] = int.MinValue;
				}
				else
				{
					bool flag2 = !int.TryParse(text, out array2[i]);
					if (flag2)
					{
						return false;
					}
				}
			}
			result = array2;
			return true;
		}

		internal bool EqualsWithoutValue(Cookie cookie)
		{
			StringComparison comparisonType = StringComparison.InvariantCulture;
			StringComparison comparisonType2 = StringComparison.InvariantCultureIgnoreCase;
			return this._name.Equals(cookie._name, comparisonType2) && this._path.Equals(cookie._path, comparisonType) && this._domain.Equals(cookie._domain, comparisonType2) && this._version == cookie._version;
		}

		internal bool EqualsWithoutValueAndVersion(Cookie cookie)
		{
			StringComparison comparisonType = StringComparison.InvariantCulture;
			StringComparison comparisonType2 = StringComparison.InvariantCultureIgnoreCase;
			return this._name.Equals(cookie._name, comparisonType2) && this._path.Equals(cookie._path, comparisonType) && this._domain.Equals(cookie._domain, comparisonType2);
		}

		internal string ToRequestString(Uri uri)
		{
			bool flag = this._name.Length == 0;
			string result;
			if (flag)
			{
				result = string.Empty;
			}
			else
			{
				bool flag2 = this._version == 0;
				if (flag2)
				{
					result = string.Format("{0}={1}", this._name, this._value);
				}
				else
				{
					StringBuilder stringBuilder = new StringBuilder(64);
					stringBuilder.AppendFormat("$Version={0}; {1}={2}", this._version, this._name, this._value);
					bool flag3 = !this._path.IsNullOrEmpty();
					if (flag3)
					{
						stringBuilder.AppendFormat("; $Path={0}", this._path);
					}
					else
					{
						bool flag4 = uri != null;
						if (flag4)
						{
							stringBuilder.AppendFormat("; $Path={0}", uri.GetAbsolutePath());
						}
						else
						{
							stringBuilder.Append("; $Path=/");
						}
					}
					bool flag5 = !this._domain.IsNullOrEmpty();
					if (flag5)
					{
						bool flag6 = uri == null || uri.Host != this._domain;
						if (flag6)
						{
							stringBuilder.AppendFormat("; $Domain={0}", this._domain);
						}
					}
					bool flag7 = this._port != null;
					if (flag7)
					{
						bool flag8 = this._port != "\"\"";
						if (flag8)
						{
							stringBuilder.AppendFormat("; $Port={0}", this._port);
						}
						else
						{
							stringBuilder.Append("; $Port");
						}
					}
					result = stringBuilder.ToString();
				}
			}
			return result;
		}

		internal string ToResponseString()
		{
			return (this._name.Length == 0) ? string.Empty : ((this._version == 0) ? this.toResponseStringVersion0() : this.toResponseStringVersion1());
		}

		internal static bool TryCreate(string name, string value, out Cookie result)
		{
			result = null;
			try
			{
				result = new Cookie(name, value);
			}
			catch
			{
				return false;
			}
			return true;
		}

		public override bool Equals(object comparand)
		{
			Cookie cookie = comparand as Cookie;
			bool flag = cookie == null;
			bool result;
			if (flag)
			{
				result = false;
			}
			else
			{
				StringComparison comparisonType = StringComparison.InvariantCulture;
				StringComparison comparisonType2 = StringComparison.InvariantCultureIgnoreCase;
				result = (this._name.Equals(cookie._name, comparisonType2) && this._value.Equals(cookie._value, comparisonType) && this._path.Equals(cookie._path, comparisonType) && this._domain.Equals(cookie._domain, comparisonType2) && this._version == cookie._version);
			}
			return result;
		}

		public override int GetHashCode()
		{
			return Cookie.hash(StringComparer.InvariantCultureIgnoreCase.GetHashCode(this._name), this._value.GetHashCode(), this._path.GetHashCode(), StringComparer.InvariantCultureIgnoreCase.GetHashCode(this._domain), this._version);
		}

		public override string ToString()
		{
			return this.ToRequestString(null);
		}

		private string _comment;

		private Uri _commentUri;

		private bool _discard;

		private string _domain;

		private static readonly int[] _emptyPorts = new int[0];

		private DateTime _expires;

		private bool _httpOnly;

		private string _name;

		private string _path;

		private string _port;

		private int[] _ports;

		private static readonly char[] _reservedCharsForValue = new char[]
		{
			';',
			','
		};

		private string _sameSite;

		private bool _secure;

		private DateTime _timeStamp;

		private string _value;

		private int _version;
	}
}
