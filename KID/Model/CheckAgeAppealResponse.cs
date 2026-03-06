using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "CheckAgeAppealResponse")]
	public class CheckAgeAppealResponse
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public CheckAgeAppealResponse.StatusEnum Status { get; set; }

		[JsonConstructor]
		protected CheckAgeAppealResponse()
		{
		}

		public CheckAgeAppealResponse(CheckAgeAppealResponse.StatusEnum status = (CheckAgeAppealResponse.StatusEnum)0, string url = null)
		{
			this.Status = status;
			this.Url = url;
		}

		[DataMember(Name = "url", EmitDefaultValue = false)]
		public string Url { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CheckAgeAppealResponse {\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("  Url: ").Append(this.Url).Append("\n");
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
			[EnumMember(Value = "CHALLENGE")]
			CHALLENGE
		}
	}
}
