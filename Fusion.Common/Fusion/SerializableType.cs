using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Text.RegularExpressions;

namespace Fusion
{
	[Serializable]
	public struct SerializableType : IEquatable<SerializableType>
	{
		public bool IsValid
		{
			get
			{
				return !string.IsNullOrEmpty(this.AssemblyQualifiedName);
			}
		}

		public SerializableType(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				this.AssemblyQualifiedName = string.Empty;
			}
			else
			{
				this.AssemblyQualifiedName = type.AssemblyQualifiedName;
			}
		}

		public SerializableType(string type)
		{
			bool flag = string.IsNullOrEmpty(type);
			if (flag)
			{
				this.AssemblyQualifiedName = string.Empty;
			}
			else
			{
				this.AssemblyQualifiedName = type;
			}
		}

		public Type Value
		{
			get
			{
				bool flag = string.IsNullOrEmpty(this.AssemblyQualifiedName);
				Type result;
				if (flag)
				{
					result = null;
				}
				else
				{
					Dictionary<string, object> types = SerializableType.Cache.Types;
					object obj;
					lock (types)
					{
						bool flag3 = !SerializableType.Cache.Types.TryGetValue(this.AssemblyQualifiedName, out obj);
						if (flag3)
						{
							try
							{
								obj = Type.GetType(this.AssemblyQualifiedName, true);
							}
							catch (Exception source)
							{
								obj = ExceptionDispatchInfo.Capture(source);
							}
							SerializableType.Cache.Types.Add(this.AssemblyQualifiedName, obj);
						}
					}
					ExceptionDispatchInfo exceptionDispatchInfo = obj as ExceptionDispatchInfo;
					bool flag4 = exceptionDispatchInfo != null;
					if (flag4)
					{
						exceptionDispatchInfo.Throw();
					}
					Type type = (Type)obj;
					bool flag5 = type == null;
					if (flag5)
					{
						throw new Exception("Type " + this.AssemblyQualifiedName + " not found");
					}
					result = type;
				}
				return result;
			}
		}

		public SerializableType AsShort()
		{
			return new SerializableType
			{
				AssemblyQualifiedName = SerializableType.GetShortAssemblyQualifiedName(this.AssemblyQualifiedName)
			};
		}

		public static implicit operator SerializableType(Type type)
		{
			return new SerializableType(type);
		}

		public static implicit operator Type(SerializableType serializableType)
		{
			return serializableType.Value;
		}

		public bool Equals(SerializableType other)
		{
			return this.AssemblyQualifiedName == other.AssemblyQualifiedName;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is SerializableType)
			{
				SerializableType other = (SerializableType)obj;
				result = this.Equals(other);
			}
			else
			{
				result = false;
			}
			return result;
		}

		public override int GetHashCode()
		{
			return (this.AssemblyQualifiedName != null) ? this.AssemblyQualifiedName.GetHashCode() : 0;
		}

		public static string GetShortAssemblyQualifiedName(Type type)
		{
			bool flag = type == null;
			if (flag)
			{
				throw new ArgumentNullException("type");
			}
			string assemblyQualifiedName = type.AssemblyQualifiedName;
			bool flag2 = assemblyQualifiedName == null;
			if (flag2)
			{
				throw new InvalidOperationException(string.Format("Type {0} has no AssemblyQualifiedName", type));
			}
			return SerializableType.GetShortAssemblyQualifiedName(assemblyQualifiedName);
		}

		internal static string GetShortAssemblyQualifiedName(string assemblyQualifiedName)
		{
			return SerializableType.s_shortNameRegex.Replace(assemblyQualifiedName, string.Empty);
		}

		public string AssemblyQualifiedName;

		private static readonly Regex s_shortNameRegex = new Regex(", (Version|Culture|PublicKeyToken)=[^, \\]]+", RegexOptions.Compiled);

		private static class Cache
		{
			public static Dictionary<string, object> Types = new Dictionary<string, object>();
		}
	}
}
