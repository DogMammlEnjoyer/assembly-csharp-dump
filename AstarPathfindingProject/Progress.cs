using System;
using UnityEngine;

namespace Pathfinding
{
	public struct Progress
	{
		public Progress(float progress, string description)
		{
			this.progress = progress;
			this.description = description;
		}

		public Progress MapTo(float min, float max, string prefix = null)
		{
			return new Progress(Mathf.Lerp(min, max, this.progress), prefix + this.description);
		}

		public override string ToString()
		{
			return this.progress.ToString("0.0") + " " + this.description;
		}

		public readonly float progress;

		public readonly string description;
	}
}
