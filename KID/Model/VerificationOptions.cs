using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "VerificationOptions")]
	public class VerificationOptions
	{
		public VerificationOptions(bool sendEmail = false)
		{
			this.SendEmail = sendEmail;
		}

		[DataMember(Name = "sendEmail", EmitDefaultValue = true)]
		public bool SendEmail { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class VerificationOptions {\n");
			stringBuilder.Append("  SendEmail: ").Append(this.SendEmail).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
