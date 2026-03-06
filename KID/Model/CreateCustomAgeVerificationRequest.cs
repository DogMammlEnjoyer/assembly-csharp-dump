using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CreateCustomAgeVerificationRequest")]
	public class CreateCustomAgeVerificationRequest
	{
		[JsonConstructor]
		protected CreateCustomAgeVerificationRequest()
		{
		}

		public CreateCustomAgeVerificationRequest(Guid scenarioId = default(Guid), string jurisdiction = null, VerificationSubject subject = null, AgeCriteria criteria = null)
		{
			this.ScenarioId = scenarioId;
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CreateCustomAgeVerificationRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			if (criteria == null)
			{
				throw new ArgumentNullException("criteria is a required property for CreateCustomAgeVerificationRequest and cannot be null");
			}
			this.Criteria = criteria;
			this.Subject = subject;
		}

		[DataMember(Name = "scenarioId", IsRequired = true, EmitDefaultValue = true)]
		public Guid ScenarioId { get; set; }

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		[DataMember(Name = "subject", EmitDefaultValue = false)]
		public VerificationSubject Subject { get; set; }

		[DataMember(Name = "criteria", IsRequired = true, EmitDefaultValue = true)]
		public AgeCriteria Criteria { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CreateCustomAgeVerificationRequest {\n");
			stringBuilder.Append("  ScenarioId: ").Append(this.ScenarioId).Append("\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  Subject: ").Append(this.Subject).Append("\n");
			stringBuilder.Append("  Criteria: ").Append(this.Criteria).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
