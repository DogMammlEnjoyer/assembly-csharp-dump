using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CreateVerificationResponse")]
	public class CreateVerificationResponse
	{
		[JsonConstructor]
		protected CreateVerificationResponse()
		{
		}

		public CreateVerificationResponse(Guid id = default(Guid), string url = null)
		{
			this.Id = id;
			if (url == null)
			{
				throw new ArgumentNullException("url is a required property for CreateVerificationResponse and cannot be null");
			}
			this.Url = url;
		}

		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
		public Guid Id { get; set; }

		[DataMember(Name = "url", IsRequired = true, EmitDefaultValue = true)]
		public string Url { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CreateVerificationResponse {\n");
			stringBuilder.Append("  Id: ").Append(this.Id).Append("\n");
			stringBuilder.Append("  Url: ").Append(this.Url).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
