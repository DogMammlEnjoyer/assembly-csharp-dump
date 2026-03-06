using System;
using System.Reflection;
using System.Text;
using Meta.Conduit;
using Meta.WitAi.Data;
using Meta.WitAi.Json;

namespace Meta.WitAi
{
	[Obsolete("Use ParameterProvider.SetSpecializedParameter() instead of this class")]
	internal class WitConduitParameterProvider : ParameterProvider
	{
		protected override object GetSpecializedParameter(ParameterInfo formalParameter)
		{
			if (formalParameter.ParameterType == typeof(WitResponseNode) && this.ActualParameters.ContainsKey("@WitResponseNode".ToLower()))
			{
				return this.ActualParameters["@WitResponseNode".ToLower()];
			}
			if (formalParameter.ParameterType == typeof(VoiceSession) && this.ActualParameters.ContainsKey("@VoiceSession".ToLower()))
			{
				return this.ActualParameters["@VoiceSession".ToLower()];
			}
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Specialized parameter not found");
			stringBuilder.AppendLine(string.Format("Parameter Type: {0}", formalParameter.ParameterType));
			stringBuilder.AppendLine("Parameter Name: " + formalParameter.Name);
			stringBuilder.AppendLine(string.Format("Actual Parameters: {0}", this.ActualParameters.Keys.Count));
			foreach (string text in this.ActualParameters.Keys)
			{
				string str = (this.ActualParameters[text] == null) ? "NULL" : this.ActualParameters[text].GetType().ToString();
				stringBuilder.AppendLine("\t" + text + ": " + str);
			}
			VLog.W(stringBuilder.ToString(), null);
			return null;
		}

		protected override bool SupportedSpecializedParameter(ParameterInfo formalParameter)
		{
			return formalParameter.ParameterType == typeof(WitResponseNode) || formalParameter.ParameterType == typeof(VoiceSession);
		}
	}
}
