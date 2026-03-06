using System;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Data.Entities
{
	public class WitEntityData : WitEntityDataBase<string>
	{
		[Preserve]
		public WitEntityData()
		{
		}

		[Preserve]
		public WitEntityData(WitResponseNode node)
		{
			base.FromEntityWitResponseNode(node);
		}

		public static implicit operator bool(WitEntityData data)
		{
			return null != data && !string.IsNullOrEmpty(data.value);
		}

		public static implicit operator string(WitEntityData data)
		{
			return data.value;
		}

		public static bool operator ==(WitEntityData data, object value)
		{
			return object.Equals((data != null) ? data.value : null, value);
		}

		public static bool operator !=(WitEntityData data, object value)
		{
			return !object.Equals((data != null) ? data.value : null, value);
		}

		public override bool Equals(object obj)
		{
			string text = obj as string;
			if (text != null)
			{
				return text == this.value;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
