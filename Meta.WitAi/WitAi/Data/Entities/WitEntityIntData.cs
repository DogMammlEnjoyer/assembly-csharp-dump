using System;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Data.Entities
{
	public class WitEntityIntData : WitEntityDataBase<int>
	{
		[Preserve]
		public WitEntityIntData()
		{
		}

		[Preserve]
		public WitEntityIntData(WitResponseNode node)
		{
			base.FromEntityWitResponseNode(node);
		}

		public static implicit operator bool(WitEntityIntData data)
		{
			return data != null && data.hasData;
		}

		public static bool operator ==(WitEntityIntData data, int value)
		{
			return data != null && data.value == value;
		}

		public static bool operator !=(WitEntityIntData data, int value)
		{
			return !(data == value);
		}

		public static bool operator ==(int value, WitEntityIntData data)
		{
			return data != null && data.value == value;
		}

		public static bool operator !=(int value, WitEntityIntData data)
		{
			return !(data == value);
		}

		public override bool Equals(object obj)
		{
			if (obj is int)
			{
				int num = (int)obj;
				return num == this.value;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
