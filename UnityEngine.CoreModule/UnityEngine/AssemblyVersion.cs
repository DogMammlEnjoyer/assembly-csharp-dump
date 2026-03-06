using System;
using UnityEngine.Bindings;
using UnityEngine.Scripting;

namespace UnityEngine
{
	[NativeHeader("Runtime/Mono/AssemblyFullName.h")]
	[RequiredByNativeCode(GenerateProxy = true)]
	internal struct AssemblyVersion
	{
		public AssemblyVersion(ushort major, ushort minor, ushort build, ushort revision)
		{
			this.major = major;
			this.minor = minor;
			this.build = build;
			this.revision = revision;
		}

		public static bool operator ==(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			return lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.build == rhs.build && lhs.revision == rhs.revision;
		}

		public static bool operator !=(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			return !(lhs == rhs);
		}

		public static bool operator <(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			bool flag = lhs.major != rhs.major;
			bool result;
			if (flag)
			{
				result = (lhs.major < rhs.major);
			}
			else
			{
				bool flag2 = lhs.minor != rhs.minor;
				if (flag2)
				{
					result = (lhs.minor < rhs.minor);
				}
				else
				{
					bool flag3 = lhs.build != rhs.build;
					if (flag3)
					{
						result = (lhs.build < rhs.build);
					}
					else
					{
						bool flag4 = lhs.revision != rhs.revision;
						result = (flag4 && lhs.revision < rhs.revision);
					}
				}
			}
			return result;
		}

		public static bool operator >(AssemblyVersion lhs, AssemblyVersion rhs)
		{
			bool flag = lhs.major != rhs.major;
			bool result;
			if (flag)
			{
				result = (lhs.major > rhs.major);
			}
			else
			{
				bool flag2 = lhs.minor != rhs.minor;
				if (flag2)
				{
					result = (lhs.minor > rhs.minor);
				}
				else
				{
					bool flag3 = lhs.build != rhs.build;
					if (flag3)
					{
						result = (lhs.build > rhs.build);
					}
					else
					{
						bool flag4 = lhs.revision != rhs.revision;
						result = (flag4 && lhs.revision > rhs.revision);
					}
				}
			}
			return result;
		}

		public override string ToString()
		{
			return string.Format("{0}.{1}.{2}.{3}", new object[]
			{
				this.major,
				this.minor,
				this.build,
				this.revision
			});
		}

		public override bool Equals(object other)
		{
			if (other is AssemblyVersion)
			{
				AssemblyVersion assemblyVersion = (AssemblyVersion)other;
				if (this.major == assemblyVersion.major && this.minor == assemblyVersion.minor && this.build == assemblyVersion.build)
				{
					return this.revision == assemblyVersion.revision;
				}
			}
			return false;
		}

		public override int GetHashCode()
		{
			return HashCode.Combine<ushort, ushort, ushort, ushort>(this.major, this.minor, this.build, this.revision);
		}

		public ushort major;

		public ushort minor;

		public ushort build;

		public ushort revision;
	}
}
