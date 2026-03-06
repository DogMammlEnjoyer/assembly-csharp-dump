using System;

namespace UnityEngine.Rendering
{
	internal class LogarithmicAttribute : PropertyAttribute
	{
		public LogarithmicAttribute(int min, int max)
		{
			this.min = min;
			this.max = max;
		}

		public int min;

		public int max;
	}
}
