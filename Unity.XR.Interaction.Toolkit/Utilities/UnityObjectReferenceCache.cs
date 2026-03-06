using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class UnityObjectReferenceCache<T> where T : Object
	{
		public bool TryGet(T field, out T fieldOrNull)
		{
			if (this.m_CapturedField == field)
			{
				fieldOrNull = this.m_FieldOrNull;
				return this.m_FieldOrNull != null;
			}
			this.m_CapturedField = field;
			if (field != null)
			{
				this.m_FieldOrNull = field;
				fieldOrNull = field;
				return true;
			}
			this.m_FieldOrNull = default(T);
			fieldOrNull = default(T);
			return false;
		}

		private T m_CapturedField;

		private T m_FieldOrNull;
	}
}
