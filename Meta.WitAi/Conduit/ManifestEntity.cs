using System;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.Data.Info;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	internal class ManifestEntity
	{
		[Preserve]
		public ManifestEntity()
		{
		}

		[Preserve]
		public string ID { get; set; }

		[Preserve]
		public string Namespace { get; set; }

		[Preserve]
		public string Type { get; set; }

		[Preserve]
		public string Name { get; set; }

		[Preserve]
		public List<WitKeyword> Values { get; set; } = new List<WitKeyword>();

		[Preserve]
		public string Assembly { get; set; }

		public WitEntityInfo GetAsInfo()
		{
			WitEntityKeywordInfo[] array = new WitEntityKeywordInfo[this.Values.Count];
			for (int i = 0; i < this.Values.Count; i++)
			{
				array[i] = this.Values[i].GetAsInfo();
			}
			return new WitEntityInfo
			{
				name = this.Name,
				keywords = array,
				roles = new WitEntityRoleInfo[0]
			};
		}

		public string GetQualifiedTypeName()
		{
			if (!string.IsNullOrEmpty(this.Namespace))
			{
				return this.Namespace + "." + this.ID;
			}
			return this.ID ?? "";
		}

		public override bool Equals(object obj)
		{
			ManifestEntity manifestEntity = obj as ManifestEntity;
			return manifestEntity != null && this.Equals(manifestEntity);
		}

		public override int GetHashCode()
		{
			return (((((17 * 31 + this.ID.GetHashCode()) * 31 + this.Type.GetHashCode()) * 31 + this.Name.GetHashCode()) * 31 + this.Values.GetHashCode()) * 31 + this.Namespace.GetHashCode()) * 31 + this.Assembly.GetHashCode();
		}

		private bool Equals(ManifestEntity other)
		{
			return this.ID == other.ID && this.Type == other.Type && this.Name == other.Name && this.Namespace == other.Namespace && this.Assembly == other.Assembly && this.Values.SequenceEqual(other.Values);
		}
	}
}
