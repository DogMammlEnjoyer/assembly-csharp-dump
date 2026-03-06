using System;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Data.Entities
{
	public abstract class WitEntityDataBase<T>
	{
		[Preserve]
		public WitEntityDataBase<T> FromEntityWitResponseNode(WitResponseNode node)
		{
			this.responseNode = node;
			return JsonConvert.DeserializeIntoObject<WitEntityDataBase<T>>(this, node, null, false);
		}

		public override string ToString()
		{
			return this.value.ToString();
		}

		public WitResponseNode responseNode;

		public string id;

		public string name;

		public string role;

		public int start;

		public int end;

		public string type;

		public string body;

		public T value;

		public float confidence;

		public bool hasData;

		public WitResponseArray entities;
	}
}
