using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
	public class FieldEditorButtonAttribute : DecoratingPropertyAttribute
	{
		public FieldEditorButtonAttribute(string label, string targetMethod)
		{
			this.Label = label;
			this.TargetMethod = targetMethod;
		}

		public string Label;

		public bool AllowMultipleTargets;

		public string TargetMethod;
	}
}
