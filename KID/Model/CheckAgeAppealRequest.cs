using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CheckAgeAppealRequest")]
	public class CheckAgeAppealRequest
	{
		[JsonConstructor]
		protected CheckAgeAppealRequest()
		{
		}

		public CheckAgeAppealRequest(string email = null, Guid playerId = default(Guid), string jurisdiction = null)
		{
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CheckAgeAppealRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			this.Email = email;
			this.PlayerId = playerId;
		}

		[DataMember(Name = "email", EmitDefaultValue = false)]
		public string Email { get; set; }

		[DataMember(Name = "playerId", EmitDefaultValue = false)]
		public Guid PlayerId { get; set; }

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CheckAgeAppealRequest {\n");
			stringBuilder.Append("  Email: ").Append(this.Email).Append("\n");
			stringBuilder.Append("  PlayerId: ").Append(this.PlayerId).Append("\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
