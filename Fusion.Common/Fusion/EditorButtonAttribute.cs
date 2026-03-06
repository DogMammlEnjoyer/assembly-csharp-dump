using System;

namespace Fusion
{
	[AttributeUsage(AttributeTargets.Method)]
	public class EditorButtonAttribute : Attribute
	{
		public EditorButtonAttribute(string label, EditorButtonVisibility visibility = EditorButtonVisibility.Always, int priority = 0, bool dirtyObject = false)
		{
			this.Label = label;
			this.Visibility = visibility;
			this.Priority = priority;
			this.DirtyObject = dirtyObject;
		}

		public EditorButtonAttribute(EditorButtonVisibility visibility = EditorButtonVisibility.Always, int priority = 0, bool dirtyObject = false) : this(null, visibility, priority, dirtyObject)
		{
		}

		public string Label;

		public EditorButtonVisibility Visibility;

		public int Priority;

		public bool AllowMultipleTargets;

		public bool DirtyObject;
	}
}
