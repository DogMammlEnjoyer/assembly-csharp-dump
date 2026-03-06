using System;
using UnityEngine;

namespace Meta.WitAi.Data.Info
{
	[Serializable]
	public struct WitEntityInfo
	{
		public override bool Equals(object obj)
		{
			if (obj is WitEntityInfo)
			{
				WitEntityInfo other = (WitEntityInfo)obj;
				return this.Equals(other);
			}
			return false;
		}

		public bool Equals(WitEntityInfo other)
		{
			return this.name == other.name && this.id == other.id && this.lookups.Equivalent(other.lookups) && this.roles.Equivalent(other.roles) && this.keywords.Equivalent(other.keywords);
		}

		public override int GetHashCode()
		{
			return ((((17 * 31 + this.name.GetHashCode()) * 31 + this.id.GetHashCode()) * 31 + this.lookups.GetHashCode()) * 31 + this.roles.GetHashCode()) * 31 + this.keywords.GetHashCode();
		}

		[SerializeField]
		public string name;

		[SerializeField]
		public string id;

		[SerializeField]
		public string[] lookups;

		[SerializeField]
		public WitEntityRoleInfo[] roles;

		[SerializeField]
		public WitEntityKeywordInfo[] keywords;
	}
}
