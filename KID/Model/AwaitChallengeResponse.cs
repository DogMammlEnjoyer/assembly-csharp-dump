using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "AwaitChallengeResponse")]
	public class AwaitChallengeResponse
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public AwaitChallengeResponse.StatusEnum Status { get; set; }

		[JsonConstructor]
		protected AwaitChallengeResponse()
		{
		}

		public AwaitChallengeResponse(AwaitChallengeResponse.StatusEnum status = (AwaitChallengeResponse.StatusEnum)0, Guid sessionId = default(Guid), string approverEmail = null)
		{
			this.Status = status;
			this.SessionId = sessionId;
			this.ApproverEmail = approverEmail;
		}

		[DataMember(Name = "sessionId", EmitDefaultValue = false)]
		public Guid SessionId { get; set; }

		[DataMember(Name = "approverEmail", EmitDefaultValue = false)]
		public string ApproverEmail { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class AwaitChallengeResponse {\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("  SessionId: ").Append(this.SessionId).Append("\n");
			stringBuilder.Append("  ApproverEmail: ").Append(this.ApproverEmail).Append("\n");
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
			[EnumMember(Value = "FAIL")]
			FAIL,
			[EnumMember(Value = "POLL_TIMEOUT")]
			POLLTIMEOUT,
			[EnumMember(Value = "IN_PROGRESS")]
			INPROGRESS
		}
	}
}
