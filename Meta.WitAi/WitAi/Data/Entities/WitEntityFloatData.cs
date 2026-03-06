using System;
using Meta.WitAi.Json;
using UnityEngine.Scripting;

namespace Meta.WitAi.Data.Entities
{
	public class WitEntityFloatData : WitEntityDataBase<float>
	{
		[Preserve]
		public WitEntityFloatData()
		{
		}

		[Preserve]
		public WitEntityFloatData(WitResponseNode node)
		{
			base.FromEntityWitResponseNode(node);
		}

		public static implicit operator bool(WitEntityFloatData data)
		{
			return data != null && data.hasData;
		}

		public bool Approximately(float v, float tolerance = 0.001f)
		{
			return Math.Abs(v - this.value) < tolerance;
		}

		public static bool operator ==(WitEntityFloatData data, float value)
		{
			return data != null && data.value == value;
		}

		public static bool operator !=(WitEntityFloatData data, float value)
		{
			return !(data == value);
		}

		public static bool operator ==(WitEntityFloatData data, int value)
		{
			return data != null && data.value == (float)value;
		}

		public static bool operator !=(WitEntityFloatData data, int value)
		{
			return !(data == value);
		}

		public static bool operator ==(float value, WitEntityFloatData data)
		{
			return data != null && data.value == value;
		}

		public static bool operator !=(float value, WitEntityFloatData data)
		{
			return !(data == value);
		}

		public static bool operator ==(int value, WitEntityFloatData data)
		{
			return data != null && data.value == (float)value;
		}

		public static bool operator !=(int value, WitEntityFloatData data)
		{
			return !(data == value);
		}

		public override bool Equals(object obj)
		{
			if (obj is float)
			{
				float num = (float)obj;
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
