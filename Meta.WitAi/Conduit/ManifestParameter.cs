using System;
using System.Collections.Generic;
using System.Linq;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.Conduit
{
	internal class ManifestParameter
	{
		[Preserve]
		public ManifestParameter()
		{
		}

		[Preserve]
		public string Name
		{
			get
			{
				return this._name;
			}
			set
			{
				this._name = ConduitUtilities.DelimitWithUnderscores(value);
			}
		}

		[Preserve]
		public string InternalName { get; set; }

		[Preserve]
		public string QualifiedName { get; set; }

		[JsonIgnore]
		public string EntityType
		{
			get
			{
				int num = this.QualifiedTypeName.LastIndexOf('.');
				if (num < 0)
				{
					return this.QualifiedTypeName;
				}
				string text = this.QualifiedTypeName.Substring(num + 1);
				int num2 = text.LastIndexOf('+');
				if (num2 < 0)
				{
					return text;
				}
				return text.Substring(num2 + 1);
			}
		}

		[Preserve]
		public string TypeAssembly { get; set; }

		[Preserve]
		public string QualifiedTypeName { get; set; }

		[Preserve]
		public List<string> Aliases { get; set; }

		[Preserve]
		public List<string> Examples { get; set; }

		public override bool Equals(object obj)
		{
			ManifestParameter manifestParameter = obj as ManifestParameter;
			return manifestParameter != null && this.Equals(manifestParameter);
		}

		public override int GetHashCode()
		{
			return (((((17 * 31 + this._name.GetHashCode()) * 31 + this.InternalName.GetHashCode()) * 31 + this.QualifiedName.GetHashCode()) * 31 + this.TypeAssembly.GetHashCode()) * 31 + this.QualifiedTypeName.GetHashCode()) * 31 + this.Aliases.GetHashCode();
		}

		private bool Equals(ManifestParameter other)
		{
			return object.Equals(this.InternalName, other.InternalName) && object.Equals(this.QualifiedName, other.QualifiedName) && object.Equals(this.EntityType, other.EntityType) && this.Aliases.SequenceEqual(other.Aliases) && object.Equals(this.TypeAssembly, other.TypeAssembly) && object.Equals(this.QualifiedTypeName, other.QualifiedTypeName);
		}

		private string _name;
	}
}
