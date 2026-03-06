using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "GetAgeGateRequirementsResponse")]
	public class GetAgeGateRequirementsResponse
	{
		[JsonConstructor]
		protected GetAgeGateRequirementsResponse()
		{
		}

		public GetAgeGateRequirementsResponse(bool shouldDisplay = false, bool ageAssuranceRequired = false, int digitalConsentAge = 0, int civilAge = 0, int minimumAge = 0, List<string> approvedAgeCollectionMethods = null)
		{
			this.ShouldDisplay = shouldDisplay;
			this.AgeAssuranceRequired = ageAssuranceRequired;
			this.DigitalConsentAge = digitalConsentAge;
			this.CivilAge = civilAge;
			this.MinimumAge = minimumAge;
			if (approvedAgeCollectionMethods == null)
			{
				throw new ArgumentNullException("approvedAgeCollectionMethods is a required property for GetAgeGateRequirementsResponse and cannot be null");
			}
			this.ApprovedAgeCollectionMethods = approvedAgeCollectionMethods;
		}

		[DataMember(Name = "shouldDisplay", IsRequired = true, EmitDefaultValue = true)]
		public bool ShouldDisplay { get; set; }

		[DataMember(Name = "ageAssuranceRequired", IsRequired = true, EmitDefaultValue = true)]
		public bool AgeAssuranceRequired { get; set; }

		[DataMember(Name = "digitalConsentAge", IsRequired = true, EmitDefaultValue = true)]
		public int DigitalConsentAge { get; set; }

		[DataMember(Name = "civilAge", IsRequired = true, EmitDefaultValue = true)]
		public int CivilAge { get; set; }

		[DataMember(Name = "minimumAge", IsRequired = true, EmitDefaultValue = true)]
		public int MinimumAge { get; set; }

		[DataMember(Name = "approvedAgeCollectionMethods", IsRequired = true, EmitDefaultValue = true)]
		public List<string> ApprovedAgeCollectionMethods { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class GetAgeGateRequirementsResponse {\n");
			stringBuilder.Append("  ShouldDisplay: ").Append(this.ShouldDisplay).Append("\n");
			stringBuilder.Append("  AgeAssuranceRequired: ").Append(this.AgeAssuranceRequired).Append("\n");
			stringBuilder.Append("  DigitalConsentAge: ").Append(this.DigitalConsentAge).Append("\n");
			stringBuilder.Append("  CivilAge: ").Append(this.CivilAge).Append("\n");
			stringBuilder.Append("  MinimumAge: ").Append(this.MinimumAge).Append("\n");
			stringBuilder.Append("  ApprovedAgeCollectionMethods: ").Append(this.ApprovedAgeCollectionMethods).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
