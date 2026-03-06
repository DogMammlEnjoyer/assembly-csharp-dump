using System;

namespace UnityEngine.XR.Interaction.Toolkit.Utilities
{
	internal class UnityObjectReferenceCache<TInterface, TObject> where TInterface : class where TObject : Object
	{
		public TInterface Get(TObject field)
		{
			if (this.m_CapturedObject == field)
			{
				return this.m_Interface;
			}
			this.m_CapturedObject = field;
			this.m_Interface = (field as TInterface);
			return this.m_Interface;
		}

		public void Set(ref TObject field, TInterface value)
		{
			field = (value as TObject);
			this.m_CapturedObject = field;
			this.m_Interface = value;
		}

		private TObject m_CapturedObject;

		private TInterface m_Interface;
	}
}
