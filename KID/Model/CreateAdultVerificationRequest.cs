using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CreateAdultVerificationRequest")]
	public class CreateAdultVerificationRequest
	{
		[JsonConstructor]
		protected CreateAdultVerificationRequest()
		{
		}

		public CreateAdultVerificationRequest(string email = null, string jurisdiction = null, List<VerificationMethod> allowedMethods = null, string locale = null)
		{
			if (email == null)
			{
				throw new ArgumentNullException("email is a required property for CreateAdultVerificationRequest and cannot be null");
			}
			this.Email = email;
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CreateAdultVerificationRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			this.AllowedMethods = allowedMethods;
			this.Locale = locale;
		}

		[DataMember(Name = "email", IsRequired = true, EmitDefaultValue = true)]
		public string Email { get; set; }

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		[DataMember(Name = "allowedMethods", EmitDefaultValue = false)]
		public List<VerificationMethod> AllowedMethods { get; set; }

		[DataMember(Name = "locale", EmitDefaultValue = false)]
		public string Locale { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CreateAdultVerificationRequest {\n");
			stringBuilder.Append("  Email: ").Append(this.Email).Append("\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  AllowedMethods: ").Append(this.AllowedMethods).Append("\n");
			stringBuilder.Append("  Locale: ").Append(this.Locale).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
