using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "GetDefaultPermissionsResponse")]
	public class GetDefaultPermissionsResponse
	{
		[DataMember(Name = "ageStatus", IsRequired = true, EmitDefaultValue = true)]
		public AgeStatusType AgeStatus { get; set; }

		[DataMember(Name = "ageCategory", IsRequired = true, EmitDefaultValue = true)]
		public AgeCategoryV2 AgeCategory { get; set; }

		[JsonConstructor]
		protected GetDefaultPermissionsResponse()
		{
		}

		public GetDefaultPermissionsResponse(bool requiresParentConsentForDataProcessing = false, List<Permission> permissions = null, AgeStatusType ageStatus = (AgeStatusType)0, AgeCategoryV2 ageCategory = (AgeCategoryV2)0)
		{
			this.RequiresParentConsentForDataProcessing = requiresParentConsentForDataProcessing;
			if (permissions == null)
			{
				throw new ArgumentNullException("permissions is a required property for GetDefaultPermissionsResponse and cannot be null");
			}
			this.Permissions = permissions;
			this.AgeStatus = ageStatus;
			this.AgeCategory = ageCategory;
		}

		[DataMember(Name = "requiresParentConsentForDataProcessing", IsRequired = true, EmitDefaultValue = true)]
		public bool RequiresParentConsentForDataProcessing { get; set; }

		[DataMember(Name = "permissions", IsRequired = true, EmitDefaultValue = true)]
		public List<Permission> Permissions { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class GetDefaultPermissionsResponse {\n");
			stringBuilder.Append("  RequiresParentConsentForDataProcessing: ").Append(this.RequiresParentConsentForDataProcessing).Append("\n");
			stringBuilder.Append("  Permissions: ").Append(this.Permissions).Append("\n");
			stringBuilder.Append("  AgeStatus: ").Append(this.AgeStatus).Append("\n");
			stringBuilder.Append("  AgeCategory: ").Append(this.AgeCategory).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
