using System;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	internal class ManifestAction : IManifestMethod
	{
		[Preserve]
		public ManifestAction()
		{
		}

		[Preserve]
		public string ID { get; set; }

		[Preserve]
		public string Assembly { get; set; }

		[Preserve]
		public string Name { get; set; }

		[Preserve]
		public List<ManifestParameter> Parameters { get; set; } = new List<ManifestParameter>();

		[JsonIgnore]
		public string DeclaringTypeName
		{
			get
			{
				return this.ID.Substring(0, this.ID.LastIndexOf('.'));
			}
		}

		[Preserve]
		public List<string> Aliases { get; set; } = new List<string>();

		public override bool Equals(object obj)
		{
			ManifestAction manifestAction = obj as ManifestAction;
			return manifestAction != null && this.Equals(manifestAction);
		}

		public override int GetHashCode()
		{
			return ((((17 * 31 + this.ID.GetHashCode()) * 31 + this.Assembly.GetHashCode()) * 31 + this.Name.GetHashCode()) * 31 + this.Parameters.GetHashCode()) * 31 + this.Aliases.GetHashCode();
		}

		private bool Equals(ManifestAction other)
		{
			return this.ID == other.ID && this.Assembly == other.Assembly && this.Name == other.Name && this.Parameters.SequenceEqual(other.Parameters) && this.Aliases.SequenceEqual(other.Aliases);
		}
	}
}
