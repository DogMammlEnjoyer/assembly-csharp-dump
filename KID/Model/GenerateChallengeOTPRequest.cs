using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "GenerateChallengeOTPRequest")]
	public class GenerateChallengeOTPRequest
	{
		[JsonConstructor]
		protected GenerateChallengeOTPRequest()
		{
		}

		public GenerateChallengeOTPRequest(Guid challengeId = default(Guid))
		{
			this.ChallengeId = challengeId;
		}

		[DataMember(Name = "challengeId", IsRequired = true, EmitDefaultValue = true)]
		public Guid ChallengeId { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class GenerateChallengeOTPRequest {\n");
			stringBuilder.Append("  ChallengeId: ").Append(this.ChallengeId).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
