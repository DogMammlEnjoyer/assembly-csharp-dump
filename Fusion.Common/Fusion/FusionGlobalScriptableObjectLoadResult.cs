using System;

namespace Fusion
{
	public readonly struct FusionGlobalScriptableObjectLoadResult
	{
		public FusionGlobalScriptableObjectLoadResult(FusionGlobalScriptableObject obj, FusionGlobalScriptableObjectUnloadDelegate unloader = null)
		{
			this.Object = obj;
			this.Unloader = unloader;
		}

		public static implicit operator FusionGlobalScriptableObjectLoadResult(FusionGlobalScriptableObject result)
		{
			return new FusionGlobalScriptableObjectLoadResult(result, null);
		}

		public readonly FusionGlobalScriptableObject Object;

		public readonly FusionGlobalScriptableObjectUnloadDelegate Unloader;
	}
}
