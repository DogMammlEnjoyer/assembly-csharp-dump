using System;
using UnityEngine;

namespace Meta.WitAi.Data.Info
{
	[Serializable]
	public struct WitVersionTagInfo
	{
		public WitVersionTagInfo(string name, string createdAt, string updatedAt, string description)
		{
			this.name = name;
			this.created_at = createdAt;
			this.updated_at = updatedAt;
			this.desc = description;
		}

		[SerializeField]
		public string name;

		[SerializeField]
		public string created_at;

		[SerializeField]
		public string updated_at;

		[SerializeField]
		public string desc;
	}
}
