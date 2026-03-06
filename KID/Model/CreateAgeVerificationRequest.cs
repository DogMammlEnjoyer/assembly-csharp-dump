using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CreateAgeVerificationRequest")]
	public class CreateAgeVerificationRequest
	{
		[JsonConstructor]
		protected CreateAgeVerificationRequest()
		{
		}

		public CreateAgeVerificationRequest(string jurisdiction = null, string locale = null, VerificationSubject subject = null, AgeCriteria criteria = null, VerificationOptions options = null)
		{
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CreateAgeVerificationRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			if (criteria == null)
			{
				throw new ArgumentNullException("criteria is a required property for CreateAgeVerificationRequest and cannot be null");
			}
			this.Criteria = criteria;
			this.Locale = locale;
			this.Subject = subject;
			this.Options = options;
		}

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		[DataMember(Name = "locale", EmitDefaultValue = false)]
		public string Locale { get; set; }

		[DataMember(Name = "subject", EmitDefaultValue = false)]
		public VerificationSubject Subject { get; set; }

		[DataMember(Name = "criteria", IsRequired = true, EmitDefaultValue = true)]
		public AgeCriteria Criteria { get; set; }

		[DataMember(Name = "options", EmitDefaultValue = false)]
		public VerificationOptions Options { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CreateAgeVerificationRequest {\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  Locale: ").Append(this.Locale).Append("\n");
			stringBuilder.Append("  Subject: ").Append(this.Subject).Append("\n");
			stringBuilder.Append("  Criteria: ").Append(this.Criteria).Append("\n");
			stringBuilder.Append("  Options: ").Append(this.Options).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
