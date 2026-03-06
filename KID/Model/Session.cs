using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using KID.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace KID.Model
{
	[DataContract(Name = "Session")]
	public class Session
	{
		[DataMember(Name = "status", IsRequired = true, EmitDefaultValue = true)]
		public Session.StatusEnum Status { get; set; }

		[DataMember(Name = "ageStatus", IsRequired = true, EmitDefaultValue = true)]
		public AgeStatusType AgeStatus { get; set; }

		[DataMember(Name = "ageCategory", IsRequired = true, EmitDefaultValue = true)]
		public AgeCategoryV2 AgeCategory { get; set; }

		[DataMember(Name = "managedBy", IsRequired = true, EmitDefaultValue = true)]
		public Session.ManagedByEnum ManagedBy { get; set; }

		[JsonConstructor]
		protected Session()
		{
		}

		public Session(Guid sessionId = default(Guid), string kuid = null, string etag = null, Session.StatusEnum status = (Session.StatusEnum)0, List<Permission> permissions = null, AgeStatusType ageStatus = (AgeStatusType)0, AgeCategoryV2 ageCategory = (AgeCategoryV2)0, DateTime dateOfBirth = default(DateTime), string jurisdiction = null, Session.ManagedByEnum managedBy = (Session.ManagedByEnum)0)
		{
			this.SessionId = sessionId;
			if (etag == null)
			{
				throw new ArgumentNullException("etag is a required property for Session and cannot be null");
			}
			this.Etag = etag;
			this.Status = status;
			this.AgeStatus = ageStatus;
			this.AgeCategory = ageCategory;
			this.DateOfBirth = dateOfBirth;
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for Session and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			this.ManagedBy = managedBy;
			this.Kuid = kuid;
			this.Permissions = permissions;
		}

		[DataMember(Name = "sessionId", IsRequired = true, EmitDefaultValue = true)]
		public Guid SessionId { get; set; }

		[DataMember(Name = "kuid", EmitDefaultValue = false)]
		public string Kuid { get; set; }

		[DataMember(Name = "etag", IsRequired = true, EmitDefaultValue = true)]
		public string Etag { get; set; }

		[DataMember(Name = "permissions", EmitDefaultValue = false)]
		public List<Permission> Permissions { get; set; }

		[DataMember(Name = "dateOfBirth", IsRequired = true, EmitDefaultValue = true)]
		[JsonConverter(typeof(OpenAPIDateConverter))]
		public DateTime DateOfBirth { get; set; }

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class Session {\n");
			stringBuilder.Append("  SessionId: ").Append(this.SessionId).Append("\n");
			stringBuilder.Append("  Kuid: ").Append(this.Kuid).Append("\n");
			stringBuilder.Append("  Etag: ").Append(this.Etag).Append("\n");
			stringBuilder.Append("  Status: ").Append(this.Status).Append("\n");
			stringBuilder.Append("  Permissions: ").Append(this.Permissions).Append("\n");
			stringBuilder.Append("  AgeStatus: ").Append(this.AgeStatus).Append("\n");
			stringBuilder.Append("  AgeCategory: ").Append(this.AgeCategory).Append("\n");
			stringBuilder.Append("  DateOfBirth: ").Append(this.DateOfBirth).Append("\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  ManagedBy: ").Append(this.ManagedBy).Append("\n");
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
			[EnumMember(Value = "ACTIVE")]
			ACTIVE = 1,
			[EnumMember(Value = "HOLD")]
			HOLD
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public enum ManagedByEnum
		{
			[EnumMember(Value = "PLAYER")]
			PLAYER = 1,
			[EnumMember(Value = "GUARDIAN")]
			GUARDIAN
		}
	}
}
