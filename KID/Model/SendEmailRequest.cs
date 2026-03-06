using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "SendEmailRequest")]
	public class SendEmailRequest
	{
		[JsonConstructor]
		protected SendEmailRequest()
		{
		}

		public SendEmailRequest(Guid challengeId = default(Guid), string email = null, string locale = null)
		{
			this.ChallengeId = challengeId;
			this.Email = email;
			this.Locale = locale;
		}

		[DataMember(Name = "challengeId", IsRequired = true, EmitDefaultValue = true)]
		public Guid ChallengeId { get; set; }

		[DataMember(Name = "email", EmitDefaultValue = false)]
		public string Email { get; set; }

		[DataMember(Name = "locale", EmitDefaultValue = false)]
		public string Locale { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class SendEmailRequest {\n");
			stringBuilder.Append("  ChallengeId: ").Append(this.ChallengeId).Append("\n");
			stringBuilder.Append("  Email: ").Append(this.Email).Append("\n");
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
