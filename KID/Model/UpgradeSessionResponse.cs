using System;
using System.Runtime.Serialization;
using System.Text;
using Newtonsoft.Json;

namespace KID.Model
{
	[DataContract(Name = "UpgradeSessionResponse")]
	public class UpgradeSessionResponse
	{
		[JsonConstructor]
		protected UpgradeSessionResponse()
		{
		}

		public UpgradeSessionResponse(Session session = null, Challenge challenge = null)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session is a required property for UpgradeSessionResponse and cannot be null");
			}
			this.Session = session;
			this.Challenge = challenge;
		}

		[DataMember(Name = "session", IsRequired = true, EmitDefaultValue = true)]
		public Session Session { get; set; }

		[DataMember(Name = "challenge", EmitDefaultValue = false)]
		public Challenge Challenge { get; set; }

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("class UpgradeSessionResponse {\n");
			stringBuilder.Append("  Session: ").Append(this.Session).Append("\n");
			stringBuilder.Append("  Challenge: ").Append(this.Challenge).Append("\n");
			stringBuilder.Append("}\n");
			return stringBuilder.ToString();
		}

		public virtual string ToJson()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}
