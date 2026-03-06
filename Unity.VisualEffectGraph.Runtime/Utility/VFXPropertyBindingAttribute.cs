using System;

namespace UnityEngine.VFX.Utility
{
	[AttributeUsage(AttributeTargets.Field)]
	public class VFXPropertyBindingAttribute : PropertyAttribute
	{
		public VFXPropertyBindingAttribute(params string[] editorTypes)
		{
			this.EditorTypes = editorTypes;
		}

		public string[] EditorTypes;
	}
}
