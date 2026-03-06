using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "ClientErrorResponse")]
	public class ClientErrorResponse
	{
		[JsonConstructor]
		protected ClientErrorResponse()
		{
		}

		public ClientErrorResponse(string error = null, string errorMessage = null)
		{
			if (error == null)
			{
				throw new ArgumentNullException("error is a required property for ClientErrorResponse and cannot be null");
			}
			this.Error = error;
			this.ErrorMessage = errorMessage;
		}

		[DataMember(Name = "error", IsRequired = true, EmitDefaultValue = true)]
		public string Error { get; set; }

		[DataMember(Name = "errorMessage", EmitDefaultValue = false)]
		public string ErrorMessage { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class ClientErrorResponse {\n");
			stringBuilder.Append("  Error: ").Append(this.Error).Append("\n");
			stringBuilder.Append("  ErrorMessage: ").Append(this.ErrorMessage).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
