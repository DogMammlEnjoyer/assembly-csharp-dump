using System;

namespace UnityEngine.Splines
{
	public class EmbeddedSplineDataFieldsAttribute : PropertyAttribute
	{
		public EmbeddedSplineDataFieldsAttribute(EmbeddedSplineDataField fields)
		{
			this.Fields = fields;
		}

		public readonly EmbeddedSplineDataField Fields;
	}
}
