using System;

namespace Fusion
{
	[Serializable]
	public struct SerializableType<BaseType> : IEquatable<SerializableType<BaseType>>
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
			this.AssemblyQualifiedName = type.AssemblyQualifiedName;
		}

		public SerializableType<BaseType> AsShort()
		{
			return new SerializableType<BaseType>
			{
				AssemblyQualifiedName = SerializableType.GetShortAssemblyQualifiedName(this.AssemblyQualifiedName)
			};
		}

		public Type Value
		{
			get
			{
				SerializableType serializableType = default(SerializableType);
				serializableType.AssemblyQualifiedName = this.AssemblyQualifiedName;
				Type value = serializableType.Value;
				Assert.Check(value != null);
				bool flag = !value.IsSubclassOf(typeof(BaseType));
				if (flag)
				{
					throw new Exception(string.Format("Type mismatch: {0} must inherit from {1}", value, typeof(BaseType)));
				}
				return value;
			}
		}

		public static implicit operator SerializableType<BaseType>(Type type)
		{
			return new SerializableType<BaseType>(type);
		}

		public static implicit operator Type(SerializableType<BaseType> serializableType)
		{
			return serializableType.Value;
		}

		public bool Equals(SerializableType<BaseType> other)
		{
			return this.AssemblyQualifiedName == other.AssemblyQualifiedName;
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is SerializableType<BaseType>)
			{
				SerializableType<BaseType> other = (SerializableType<BaseType>)obj;
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

		public string AssemblyQualifiedName;
	}
}
