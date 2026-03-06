using System;
using System.Runtime.Serialization;
using System.Text;
using KID.Client;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CreateVerificationRequest")]
	public class CreateVerificationRequest
	{
		[JsonConstructor]
		protected CreateVerificationRequest()
		{
		}

		public CreateVerificationRequest(Guid scenarioId = default(Guid), string jurisdiction = null, string email = null, AgeCriteria criteria = null, DateTime claimedDateOfBirth = default(DateTime), int claimedAge = 0)
		{
			this.ScenarioId = scenarioId;
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CreateVerificationRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			this.Email = email;
			this.Criteria = criteria;
			this.ClaimedDateOfBirth = claimedDateOfBirth;
			this.ClaimedAge = claimedAge;
		}

		[DataMember(Name = "scenarioId", IsRequired = true, EmitDefaultValue = true)]
		public Guid ScenarioId { get; set; }

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		[DataMember(Name = "email", EmitDefaultValue = false)]
		public string Email { get; set; }

		[DataMember(Name = "criteria", EmitDefaultValue = false)]
		public AgeCriteria Criteria { get; set; }

		[DataMember(Name = "claimedDateOfBirth", EmitDefaultValue = false)]
		[JsonConverter(typeof(OpenAPIDateConverter))]
		public DateTime ClaimedDateOfBirth { get; set; }

		[DataMember(Name = "claimedAge", EmitDefaultValue = false)]
		public int ClaimedAge { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CreateVerificationRequest {\n");
			stringBuilder.Append("  ScenarioId: ").Append(this.ScenarioId).Append("\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  Email: ").Append(this.Email).Append("\n");
			stringBuilder.Append("  Criteria: ").Append(this.Criteria).Append("\n");
			stringBuilder.Append("  ClaimedDateOfBirth: ").Append(this.ClaimedDateOfBirth).Append("\n");
			stringBuilder.Append("  ClaimedAge: ").Append(this.ClaimedAge).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
