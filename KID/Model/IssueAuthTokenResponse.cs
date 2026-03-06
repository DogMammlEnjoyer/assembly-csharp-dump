using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "IssueAuthTokenResponse")]
	public class IssueAuthTokenResponse
	{
		public IssueAuthTokenResponse(string accessToken = null, string refreshToken = null)
		{
			this.AccessToken = accessToken;
			this.RefreshToken = refreshToken;
		}

		[DataMember(Name = "accessToken", EmitDefaultValue = false)]
		public string AccessToken { get; set; }

		[DataMember(Name = "refreshToken", EmitDefaultValue = false)]
		public string RefreshToken { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class IssueAuthTokenResponse {\n");
			stringBuilder.Append("  AccessToken: ").Append(this.AccessToken).Append("\n");
			stringBuilder.Append("  RefreshToken: ").Append(this.RefreshToken).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
