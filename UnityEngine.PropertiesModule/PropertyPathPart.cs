using System;
using System.Runtime.CompilerServices;

namespace Unity.Properties
{
	public readonly struct PropertyPathPart : IEquatable<PropertyPathPart>
	{
		public bool IsName
		{
			get
			{
				return this.Kind == PropertyPathPartKind.Name;
			}
		}

		public bool IsIndex
		{
			get
			{
				return this.Kind == PropertyPathPartKind.Index;
			}
		}

		public bool IsKey
		{
			get
			{
				return this.Kind == PropertyPathPartKind.Key;
			}
		}

		public PropertyPathPartKind Kind
		{
			get
			{
				return this.m_Kind;
			}
		}

		public string Name
		{
			get
			{
				this.CheckKind(PropertyPathPartKind.Name);
				return this.m_Name;
			}
		}

		public int Index
		{
			get
			{
				this.CheckKind(PropertyPathPartKind.Index);
				return this.m_Index;
			}
		}

		public object Key
		{
			get
			{
				this.CheckKind(PropertyPathPartKind.Key);
				return this.m_Key;
			}
		}

		public PropertyPathPart(string name)
		{
			this.m_Kind = PropertyPathPartKind.Name;
			this.m_Name = name;
			this.m_Index = -1;
			this.m_Key = null;
		}

		public PropertyPathPart(int index)
		{
			this.m_Kind = PropertyPathPartKind.Index;
			this.m_Name = string.Empty;
			this.m_Index = index;
			this.m_Key = null;
		}

		public PropertyPathPart(object key)
		{
			this.m_Kind = PropertyPathPartKind.Key;
			this.m_Name = string.Empty;
			this.m_Index = -1;
			this.m_Key = key;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void CheckKind(PropertyPathPartKind type)
		{
			bool flag = type != this.Kind;
			if (flag)
			{
				throw new InvalidOperationException();
			}
		}

		public override string ToString()
		{
			PropertyPathPartKind kind = this.Kind;
			if (!true)
			{
			}
			string result;
			switch (kind)
			{
			case PropertyPathPartKind.Name:
				result = this.m_Name;
				break;
			case PropertyPathPartKind.Index:
				result = "[" + this.m_Index.ToString() + "]";
				break;
			case PropertyPathPartKind.Key:
			{
				string str = "[\"";
				object key = this.m_Key;
				result = str + ((key != null) ? key.ToString() : null) + "\"]";
				break;
			}
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}

		public bool Equals(PropertyPathPart other)
		{
			return this.m_Kind == other.m_Kind && this.m_Name == other.m_Name && this.m_Index == other.m_Index && object.Equals(this.m_Key, other.m_Key);
		}

		public override bool Equals(object obj)
		{
			bool result;
			if (obj is PropertyPathPart)
			{
				PropertyPathPart other = (PropertyPathPart)obj;
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
			int kind = (int)this.m_Kind;
			PropertyPathPartKind kind2 = this.m_Kind;
			if (!true)
			{
			}
			int result;
			switch (kind2)
			{
			case PropertyPathPartKind.Name:
				result = (kind * 397 ^ ((this.m_Name != null) ? this.m_Name.GetHashCode() : 0));
				break;
			case PropertyPathPartKind.Index:
				result = (kind * 397 ^ this.m_Index);
				break;
			case PropertyPathPartKind.Key:
				result = (kind * 397 ^ ((this.m_Key != null) ? this.m_Key.GetHashCode() : 0));
				break;
			default:
				throw new ArgumentOutOfRangeException();
			}
			if (!true)
			{
			}
			return result;
		}

		private readonly PropertyPathPartKind m_Kind;

		private readonly string m_Name;

		private readonly int m_Index;

		private readonly object m_Key;
	}
}
