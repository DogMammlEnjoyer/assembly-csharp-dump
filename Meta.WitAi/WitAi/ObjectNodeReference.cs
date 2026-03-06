using System;
using Meta.WitAi.Json;

namespace Meta.WitAi
{
	public class ObjectNodeReference : WitResponseReference
	{
		public override string GetStringValue(WitResponseNode response)
		{
			if (this.child != null && null != ((response != null) ? response[this.key] : null))
			{
				return this.child.GetStringValue(response[this.key]);
			}
			if (response == null)
			{
				return null;
			}
			WitResponseNode witResponseNode = response[this.key];
			if (witResponseNode == null)
			{
				return null;
			}
			return witResponseNode.Value;
		}

		public override int GetIntValue(WitResponseNode response)
		{
			if (this.child != null)
			{
				return this.child.GetIntValue(response[this.key]);
			}
			return response[this.key].AsInt;
		}

		public override float GetFloatValue(WitResponseNode response)
		{
			if (this.child != null)
			{
				return this.child.GetFloatValue(response[this.key]);
			}
			return response[this.key].AsFloat;
		}

		public string key;
	}
}
