using System;

namespace UnityEngine.Rendering
{
	public abstract class RenderPipelineResources : ScriptableObject
	{
		protected virtual string packagePath
		{
			get
			{
				return null;
			}
		}

		internal string packagePath_Internal
		{
			get
			{
				return this.packagePath;
			}
		}
	}
}
