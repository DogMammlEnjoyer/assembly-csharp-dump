using System;
using System.Collections.Generic;

namespace Sirenix.OdinInspector
{
	public class SelfMetaData : List<SelfValidationResult.ResultItemMetaData>
	{
		public void Add(string key, object value)
		{
			base.Add(new SelfValidationResult.ResultItemMetaData(key, value, Array.Empty<Attribute>()));
		}
	}
}
