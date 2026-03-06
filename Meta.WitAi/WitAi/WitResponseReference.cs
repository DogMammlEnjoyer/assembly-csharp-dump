using System;
using Meta.WitAi.Json;

namespace Meta.WitAi
{
	public class WitResponseReference
	{
		public virtual string GetStringValue(WitResponseNode response)
		{
			return this.child.GetStringValue(response);
		}

		public virtual int GetIntValue(WitResponseNode response)
		{
			return this.child.GetIntValue(response);
		}

		public virtual float GetFloatValue(WitResponseNode response)
		{
			return this.child.GetFloatValue(response);
		}

		public WitResponseReference child;

		public string path;
	}
}
