using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "Permission")]
	public class Permission
	{
		[DataMember(Name = "managedBy", IsRequired = true, EmitDefaultValue = true)]
		public Permission.ManagedByEnum ManagedBy { get; set; }

		[JsonConstructor]
		protected Permission()
		{
		}

		public Permission(string name = null, bool enabled = false, Permission.ManagedByEnum managedBy = (Permission.ManagedByEnum)0)
		{
			if (name == null)
			{
				throw new ArgumentNullException("name is a required property for Permission and cannot be null");
			}
			this.Name = name;
			this.Enabled = enabled;
			this.ManagedBy = managedBy;
		}

		[DataMember(Name = "name", IsRequired = true, EmitDefaultValue = true)]
		public string Name { get; set; }

		[DataMember(Name = "enabled", IsRequired = true, EmitDefaultValue = true)]
		public bool Enabled { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class Permission {\n");
			stringBuilder.Append("  Name: ").Append(this.Name).Append("\n");
			stringBuilder.Append("  Enabled: ").Append(this.Enabled).Append("\n");
			stringBuilder.Append("  ManagedBy: ").Append(this.ManagedBy).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ManagedByEnum
		{
			[EnumMember(Value = "PLAYER")]
			PLAYER = 1,
			[EnumMember(Value = "GUARDIAN")]
			GUARDIAN,
			[EnumMember(Value = "PROHIBITED")]
			PROHIBITED
		}
	}
}
