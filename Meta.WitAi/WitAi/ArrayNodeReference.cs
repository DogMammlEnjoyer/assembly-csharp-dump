using System;
using Meta.WitAi.Json;

namespace Meta.WitAi
{
	public class ArrayNodeReference : WitResponseReference
	{
		public override string GetStringValue(WitResponseNode response)
		{
			if (this.child != null)
			{
				return this.child.GetStringValue(response[this.index]);
			}
			return response[this.index].Value;
		}

		public override int GetIntValue(WitResponseNode response)
		{
			if (this.child != null)
			{
				return this.child.GetIntValue(response[this.index]);
			}
			return response[this.index].AsInt;
		}

		public override float GetFloatValue(WitResponseNode response)
		{
			if (this.child != null)
			{
				return this.child.GetFloatValue(response[this.index]);
			}
			return (float)response[this.index].AsInt;
		}

		public int index;
	}
}
