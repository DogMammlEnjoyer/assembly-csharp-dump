using System;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.WitAi.Data
{
	public class WitFloatValue : WitValue
	{
		public override object GetValue(WitResponseNode response)
		{
			return this.GetFloatValue(response);
		}

		public override bool Equals(WitResponseNode response, object value)
		{
			float num = 0f;
			if (value is float)
			{
				float num2 = (float)value;
				num = num2;
			}
			else if (value != null && !float.TryParse(((value != null) ? value.ToString() : null) ?? "", out num))
			{
				return false;
			}
			return Math.Abs(this.GetFloatValue(response) - num) < this.equalityTolerance;
		}

		public float GetFloatValue(WitResponseNode response)
		{
			return base.Reference.GetFloatValue(response);
		}

		[SerializeField]
		public float equalityTolerance = 0.0001f;
	}
}
