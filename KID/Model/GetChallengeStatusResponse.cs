using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "GetChallengeStatusResponse")]
	public class GetChallengeStatusResponse
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public GetChallengeStatusResponse.StatusEnum Status { get; set; }

		[JsonConstructor]
		protected GetChallengeStatusResponse()
		{
		}

		public GetChallengeStatusResponse(GetChallengeStatusResponse.StatusEnum status = (GetChallengeStatusResponse.StatusEnum)0, Guid sessionId = default(Guid), string approverEmail = null)
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
			stringBuilder.Append("class GetChallengeStatusResponse {\n");
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
			[EnumMember(Value = "PENDING")]
			PENDING,
			[EnumMember(Value = "IN_PROGRESS")]
			INPROGRESS
		}
	}
}
