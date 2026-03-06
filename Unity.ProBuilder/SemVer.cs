using System;
using System.Text;
using System.Text.RegularExpressions;

namespace UnityEngine.ProBuilder
{
	[Serializable]
	internal sealed class SemVer : IEquatable<SemVer>, IComparable<SemVer>, IComparable
	{
		public int major
		{
			get
			{
				return this.m_Major;
			}
		}

		public int minor
		{
			get
			{
				return this.m_Minor;
			}
		}

		public int patch
		{
			get
			{
				return this.m_Patch;
			}
		}

		public int build
		{
			get
			{
				return this.m_Build;
			}
		}

		public string type
		{
			get
			{
				if (this.m_Type == null)
				{
					return "";
				}
				return this.m_Type;
			}
		}

		public string metadata
		{
			get
			{
				if (this.m_Metadata == null)
				{
					return "";
				}
				return this.m_Metadata;
			}
		}

		public string date
		{
			get
			{
				if (this.m_Date == null)
				{
					return "";
				}
				return this.m_Date;
			}
		}

		public SemVer MajorMinorPatch
		{
			get
			{
				return new SemVer(this.major, this.minor, this.patch, -1, null, null, null);
			}
		}

		public SemVer()
		{
			this.m_Major = 0;
			this.m_Minor = 0;
			this.m_Patch = 0;
			this.m_Build = -1;
			this.m_Type = null;
			this.m_Date = null;
			this.m_Metadata = null;
		}

		public SemVer(string formatted, string date = null)
		{
			this.m_Metadata = formatted;
			this.m_Date = date;
			SemVer semVer;
			if (SemVer.TryGetVersionInfo(formatted, out semVer))
			{
				this.m_Major = semVer.m_Major;
				this.m_Minor = semVer.m_Minor;
				this.m_Patch = semVer.m_Patch;
				this.m_Build = semVer.m_Build;
				this.m_Type = semVer.m_Type;
				this.m_Metadata = semVer.metadata;
			}
		}

		public SemVer(int major, int minor, int patch, int build = -1, string type = null, string date = null, string metadata = null)
		{
			this.m_Major = major;
			this.m_Minor = minor;
			this.m_Patch = patch;
			this.m_Build = build;
			this.m_Type = type;
			this.m_Metadata = metadata;
			this.m_Date = date;
		}

		public bool IsValid()
		{
			return this.major != -1 && this.minor != -1 && this.patch != -1;
		}

		public override bool Equals(object o)
		{
			return o is SemVer && this.Equals((SemVer)o);
		}

		public override int GetHashCode()
		{
			int num = 13;
			if (this.IsValid())
			{
				num = num * 7 + this.major.GetHashCode();
				num = num * 7 + this.minor.GetHashCode();
				num = num * 7 + this.patch.GetHashCode();
				num = num * 7 + this.build.GetHashCode();
				return num * 7 + this.type.GetHashCode();
			}
			if (!string.IsNullOrEmpty(this.metadata))
			{
				return base.GetHashCode();
			}
			return this.metadata.GetHashCode();
		}

		public bool Equals(SemVer version)
		{
			if (version == null)
			{
				return false;
			}
			if (this.IsValid() != version.IsValid())
			{
				return false;
			}
			if (this.IsValid())
			{
				return this.major == version.major && this.minor == version.minor && this.patch == version.patch && this.type.Equals(version.type) && this.build.Equals(version.build);
			}
			return !string.IsNullOrEmpty(this.metadata) && !string.IsNullOrEmpty(version.metadata) && this.metadata.Equals(version.metadata);
		}

		public int CompareTo(object obj)
		{
			return this.CompareTo(obj as SemVer);
		}

		private static int WrapNoValue(int value)
		{
			if (value >= 0)
			{
				return value;
			}
			return int.MaxValue;
		}

		public int CompareTo(SemVer version)
		{
			if (version == null)
			{
				return 1;
			}
			if (this.Equals(version))
			{
				return 0;
			}
			if (this.major > version.major)
			{
				return 1;
			}
			if (this.major < version.major)
			{
				return -1;
			}
			if (this.minor > version.minor)
			{
				return 1;
			}
			if (this.minor < version.minor)
			{
				return -1;
			}
			if (SemVer.WrapNoValue(this.patch) > SemVer.WrapNoValue(version.patch))
			{
				return 1;
			}
			if (SemVer.WrapNoValue(this.patch) < SemVer.WrapNoValue(version.patch))
			{
				return -1;
			}
			if (string.IsNullOrEmpty(this.type) && !string.IsNullOrEmpty(version.type))
			{
				return 1;
			}
			if (!string.IsNullOrEmpty(this.type) && string.IsNullOrEmpty(version.type))
			{
				return -1;
			}
			if (SemVer.WrapNoValue(this.build) > SemVer.WrapNoValue(version.build))
			{
				return 1;
			}
			if (SemVer.WrapNoValue(this.build) < SemVer.WrapNoValue(version.build))
			{
				return -1;
			}
			return 0;
		}

		public static bool operator ==(SemVer left, SemVer right)
		{
			if (left == null)
			{
				return right == null;
			}
			return left.Equals(right);
		}

		public static bool operator !=(SemVer left, SemVer right)
		{
			return !(left == right);
		}

		public static bool operator <(SemVer left, SemVer right)
		{
			if (left == null)
			{
				return right != null;
			}
			return left.CompareTo(right) < 0;
		}

		public static bool operator >(SemVer left, SemVer right)
		{
			return left != null && left.CompareTo(right) > 0;
		}

		public static bool operator <=(SemVer left, SemVer right)
		{
			return left == right || left < right;
		}

		public static bool operator >=(SemVer left, SemVer right)
		{
			return left == right || left > right;
		}

		public string ToString(string format)
		{
			StringBuilder stringBuilder = new StringBuilder();
			bool flag = false;
			foreach (char c in format.ToCharArray())
			{
				if (flag)
				{
					stringBuilder.Append(c);
					flag = false;
				}
				else if (c == '\\')
				{
					flag = true;
				}
				else if (c == 'M')
				{
					stringBuilder.Append(this.major);
				}
				else if (c == 'm')
				{
					stringBuilder.Append(this.minor);
				}
				else if (c == 'p')
				{
					stringBuilder.Append(this.patch);
				}
				else if (c == 'b')
				{
					stringBuilder.Append(this.build);
				}
				else if (c == 'T' || c == 't')
				{
					stringBuilder.Append(this.type);
				}
				else if (c == 'd')
				{
					stringBuilder.Append(this.date);
				}
				else if (c == 'D')
				{
					stringBuilder.Append(this.metadata);
				}
				else
				{
					stringBuilder.Append(c);
				}
			}
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(this.ToString("M.m.p"));
			if (!string.IsNullOrEmpty(this.type))
			{
				stringBuilder.Append("-");
				stringBuilder.Append(this.type);
				if (this.build > -1)
				{
					stringBuilder.Append(".");
					stringBuilder.Append(this.build.ToString());
				}
			}
			if (!string.IsNullOrEmpty(this.date))
			{
				stringBuilder.Append(" ");
				stringBuilder.Append(this.date);
			}
			return stringBuilder.ToString();
		}

		public static bool TryGetVersionInfo(string input, out SemVer version)
		{
			version = new SemVer();
			bool result = false;
			try
			{
				Match match = Regex.Match(input, "^([0-9]+\\.[0-9]+\\.[0-9]+)");
				if (!match.Success)
				{
					return false;
				}
				string[] array = match.Value.Split('.', StringSplitOptions.None);
				int.TryParse(array[0], out version.m_Major);
				int.TryParse(array[1], out version.m_Minor);
				int.TryParse(array[2], out version.m_Patch);
				result = true;
				Match match2 = Regex.Match(input, "(?i)(?<=\\-)[a-z0-9\\-]+");
				if (match2.Success)
				{
					version.m_Type = match2.Value;
				}
				Match match3 = Regex.Match(input, "(?i)(?<=\\-[a-z0-9\\-]+\\.)[0-9]+");
				version.m_Build = (match3.Success ? SemVer.GetBuildNumber(match3.Value) : -1);
				Match match4 = Regex.Match(input, "(?<=\\+).+");
				if (match4.Success)
				{
					version.m_Metadata = match4.Value;
				}
			}
			catch
			{
				result = false;
			}
			return result;
		}

		private static int GetBuildNumber(string input)
		{
			Match match = Regex.Match(input, "[0-9]+");
			int result = 0;
			if (match.Success && int.TryParse(match.Value, out result))
			{
				return result;
			}
			return -1;
		}

		[SerializeField]
		private int m_Major = -1;

		[SerializeField]
		private int m_Minor = -1;

		[SerializeField]
		private int m_Patch = -1;

		[SerializeField]
		private int m_Build = -1;

		[SerializeField]
		private string m_Type;

		[SerializeField]
		private string m_Metadata;

		[SerializeField]
		private string m_Date;

		public const string DefaultStringFormat = "M.m.p-t.b";
	}
}
