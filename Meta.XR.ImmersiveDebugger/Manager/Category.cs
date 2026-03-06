using System;
using Meta.XR.ImmersiveDebugger.Hierarchy;

namespace Meta.XR.ImmersiveDebugger.Manager
{
	internal struct Category : IEquatable<Category>
	{
		public string Label
		{
			get
			{
				Item item = this.Item;
				string result;
				if ((result = ((item != null) ? item.Label : null)) == null)
				{
					if (!string.IsNullOrEmpty(this.Id))
					{
						return this.Id;
					}
					result = "Uncategorized";
				}
				return result;
			}
		}

		private string Uid
		{
			get
			{
				Item item = this.Item;
				return (((item != null) ? item.Id.ToString() : null) ?? this.Id) ?? string.Empty;
			}
		}

		public bool Equals(Category other)
		{
			return this.Uid == other.Uid;
		}

		public override bool Equals(object obj)
		{
			if (obj is Category)
			{
				Category other = (Category)obj;
				return this.Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.Uid.GetHashCode();
		}

		private const string DefaultCategoryName = "Uncategorized";

		public static Category Default;

		public string Id;

		public Item Item;
	}
}
