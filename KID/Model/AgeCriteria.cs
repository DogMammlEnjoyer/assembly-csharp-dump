using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "AgeCriteria")]
	public class AgeCriteria
	{
		[DataMember(Name = "ageCategory", EmitDefaultValue = false)]
		public AgeCategory? AgeCategory { get; set; }

		public AgeCriteria(int age = 0, AgeCategory? ageCategory = null)
		{
			this.Age = age;
			this.AgeCategory = ageCategory;
		}

		[DataMember(Name = "age", EmitDefaultValue = false)]
		public int Age { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class AgeCriteria {\n");
			stringBuilder.Append("  Age: ").Append(this.Age).Append("\n");
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
