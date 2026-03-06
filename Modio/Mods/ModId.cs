using System;

namespace Modio.Mods
{
	public readonly struct ModId
	{
		public ModId(long id)
		{
			this._id = id;
		}

		public bool IsValid()
		{
			return this._id > 0L;
		}

		public static ModId Null
		{
			get
			{
				return -1L;
			}
		}

		internal long GetResourceId()
		{
			return this._id;
		}

		public static bool operator ==(ModId left, ModId right)
		{
			return left._id == right._id;
		}

		public static bool operator !=(ModId left, ModId right)
		{
			return left._id != right._id;
		}

		public override bool Equals(object obj)
		{
			if (obj is ModId)
			{
				ModId right = (ModId)obj;
				return this == right;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this._id.GetHashCode();
		}

		public static implicit operator long(ModId modId)
		{
			return modId._id;
		}

		public static implicit operator ModId(long id)
		{
			return new ModId(id);
		}

		public override string ToString()
		{
			return this._id.ToString();
		}

		public readonly long _id;
	}
}
