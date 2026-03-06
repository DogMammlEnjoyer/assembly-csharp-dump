using System;

namespace UnityEngine.Localization.SmartFormat.Extensions
{
	[Obsolete("Please use PersistentVariablesSource instead (UnityUpgradable) -> PersistentVariablesSource")]
	[Serializable]
	public class GlobalVariablesSource : PersistentVariablesSource
	{
		public GlobalVariablesSource(SmartFormatter formatter) : base(formatter)
		{
		}
	}
}
