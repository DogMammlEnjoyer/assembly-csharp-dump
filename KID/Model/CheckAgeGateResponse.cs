using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "CheckAgeGateResponse")]
	public class CheckAgeGateResponse
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public CheckAgeGateResponse.StatusEnum Status { get; set; }

		[JsonConstructor]
		protected CheckAgeGateResponse()
		{
		}

		public CheckAgeGateResponse(CheckAgeGateResponse.StatusEnum status = (CheckAgeGateResponse.StatusEnum)0, Session session = null, Challenge challenge = null)
		{
			this.Status = status;
			this.Session = session;
			this.Challenge = challenge;
		}

		[DataMember(Name = "session", EmitDefaultValue = false)]
		public Session Session { get; set; }

		[DataMember(Name = "challenge", EmitDefaultValue = false)]
		public Challenge Challenge { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CheckAgeGateResponse {\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("  Session: ").Append(this.Session).Append("\n");
			stringBuilder.Append("  Challenge: ").Append(this.Challenge).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum StatusEnum
		{
			[EnumMember(Value = "PASS")]
			PASS = 1,
			[EnumMember(Value = "PROHIBITED")]
			PROHIBITED,
			[EnumMember(Value = "CHALLENGE")]
			CHALLENGE
		}
	}
}
