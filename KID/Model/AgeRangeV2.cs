using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "AgeRangeV2")]
	public class AgeRangeV2
	{
		public AgeRangeV2(int low = 0, int high = 0, [Optional] decimal confidence = 0m)
		{
			this.Low = low;
			this.High = high;
			this.Confidence = confidence;
		}

		[DataMember(Name = "low", EmitDefaultValue = false)]
		public int Low { get; set; }

		[DataMember(Name = "high", EmitDefaultValue = false)]
		public int High { get; set; }

		[DataMember(Name = "confidence", EmitDefaultValue = false)]
		public decimal Confidence { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class AgeRangeV2 {\n");
			stringBuilder.Append("  Low: ").Append(this.Low).Append("\n");
			stringBuilder.Append("  High: ").Append(this.High).Append("\n");
			stringBuilder.Append("  Confidence: ").Append(this.Confidence).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
