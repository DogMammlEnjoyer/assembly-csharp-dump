using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "GetAgeAssuranceVerificationRequestStatusResponse")]
	public class GetAgeAssuranceVerificationRequestStatusResponse
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public VerificationStatus Status { get; set; }

		[JsonConstructor]
		protected GetAgeAssuranceVerificationRequestStatusResponse()
		{
		}

		public GetAgeAssuranceVerificationRequestStatusResponse(Guid id = default(Guid), VerificationStatus status = (VerificationStatus)0, AgeRange ageRange = null)
		{
			this.Id = id;
			this.Status = status;
			this.AgeRange = ageRange;
		}

		[DataMember(Name = "id", IsRequired = true, EmitDefaultValue = true)]
		public Guid Id { get; set; }

		[DataMember(Name = "ageRange", EmitDefaultValue = false)]
		public AgeRange AgeRange { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class GetAgeAssuranceVerificationRequestStatusResponse {\n");
			stringBuilder.Append("  Id: ").Append(this.Id).Append("\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("  AgeRange: ").Append(this.AgeRange).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
