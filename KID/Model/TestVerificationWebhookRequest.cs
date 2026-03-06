using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "TestVerificationWebhookRequest")]
	public class TestVerificationWebhookRequest
	{
		[DataMember(Name = "eventType", EmitDefaultValue = false)]
		public TestVerificationWebhookRequest.EventTypeEnum? EventType { get; set; }

		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public VerificationStatus Status { get; set; }

		[JsonConstructor]
		protected TestVerificationWebhookRequest()
		{
		}

		public TestVerificationWebhookRequest(TestVerificationWebhookRequest.EventTypeEnum? eventType = null, Guid id = default(Guid), AgeRange ageRange = null, VerificationStatus status = (VerificationStatus)0)
		{
			this.Id = id;
			this.Status = status;
			this.EventType = eventType;
			this.AgeRange = ageRange;
		}

		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
		public Guid Id { get; set; }

		[DataMember(Name = "ageRange", EmitDefaultValue = false)]
		public AgeRange AgeRange { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class TestVerificationWebhookRequest {\n");
			stringBuilder.Append("  EventType: ").Append(this.EventType).Append("\n");
			stringBuilder.Append("  Id: ").Append(this.Id).Append("\n");
			stringBuilder.Append("  AgeRange: ").Append(this.AgeRange).Append("\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum EventTypeEnum
		{
			[EnumMember(Value = "adultVerificationResult")]
			AdultVerificationResult = 1,
			[EnumMember(Value = "ageAssuranceVerificationResult")]
			AgeAssuranceVerificationResult
		}
	}
}
