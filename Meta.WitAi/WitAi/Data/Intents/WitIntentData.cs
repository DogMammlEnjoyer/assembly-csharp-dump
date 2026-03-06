using System;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Data.Intents
{
	public class WitIntentData
	{
		public WitIntentData()
		{
		}

		public WitIntentData(WitResponseNode node)
		{
			this.FromIntentWitResponseNode(node);
		}

		public WitIntentData FromIntentWitResponseNode(WitResponseNode node)
		{
			return JsonConvert.DeserializeIntoObject<WitIntentData>(this, node, null, false);
		}

		public WitResponseNode responseNode;

		[Preserve]
		public string id;

		public string name;

		public float confidence;
	}
}
