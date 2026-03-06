using System;
using System.Runtime.Serialization;
using System.Text;
using KID.Client;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "CheckAgeGateRequest")]
	public class CheckAgeGateRequest
	{
		[JsonConstructor]
		protected CheckAgeGateRequest()
		{
		}

		public CheckAgeGateRequest(string jurisdiction = null, DateTime dateOfBirth = default(DateTime), int age = 0)
		{
			if (jurisdiction == null)
			{
				throw new ArgumentNullException("jurisdiction is a required property for CheckAgeGateRequest and cannot be null");
			}
			this.Jurisdiction = jurisdiction;
			this.DateOfBirth = dateOfBirth;
			this.Age = age;
		}

		[DataMember(Name = "jurisdiction", IsRequired = true, EmitDefaultValue = true)]
		public string Jurisdiction { get; set; }

		[DataMember(Name = "dateOfBirth", EmitDefaultValue = false)]
		[JsonConverter(typeof(OpenAPIDateConverter))]
		public DateTime DateOfBirth { get; set; }

		[DataMember(Name = "age", EmitDefaultValue = false)]
		public int Age { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class CheckAgeGateRequest {\n");
			stringBuilder.Append("  Jurisdiction: ").Append(this.Jurisdiction).Append("\n");
			stringBuilder.Append("  DateOfBirth: ").Append(this.DateOfBirth).Append("\n");
			stringBuilder.Append("  Age: ").Append(this.Age).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
