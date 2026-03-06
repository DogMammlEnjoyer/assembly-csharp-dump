using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "ShouldDisplayAgeGateResponse")]
	public class ShouldDisplayAgeGateResponse
	{
		[JsonConstructor]
		protected ShouldDisplayAgeGateResponse()
		{
		}

		public ShouldDisplayAgeGateResponse(bool shouldDisplay = false, bool ageAssuranceRequired = false, int digitalConsentAge = 0, int civilAge = 0)
		{
			this.ShouldDisplay = shouldDisplay;
			this.AgeAssuranceRequired = ageAssuranceRequired;
			this.DigitalConsentAge = digitalConsentAge;
			this.CivilAge = civilAge;
		}

		[DataMember(Name = "shouldDisplay", IsRequired = true, EmitDefaultValue = true)]
		public bool ShouldDisplay { get; set; }

		[DataMember(Name = "ageAssuranceRequired", IsRequired = true, EmitDefaultValue = true)]
		public bool AgeAssuranceRequired { get; set; }

		[DataMember(Name = "digitalConsentAge", IsRequired = true, EmitDefaultValue = true)]
		public int DigitalConsentAge { get; set; }

		[DataMember(Name = "civilAge", IsRequired = true, EmitDefaultValue = true)]
		public int CivilAge { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class ShouldDisplayAgeGateResponse {\n");
			stringBuilder.Append("  ShouldDisplay: ").Append(this.ShouldDisplay).Append("\n");
			stringBuilder.Append("  AgeAssuranceRequired: ").Append(this.AgeAssuranceRequired).Append("\n");
			stringBuilder.Append("  DigitalConsentAge: ").Append(this.DigitalConsentAge).Append("\n");
			stringBuilder.Append("  CivilAge: ").Append(this.CivilAge).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
