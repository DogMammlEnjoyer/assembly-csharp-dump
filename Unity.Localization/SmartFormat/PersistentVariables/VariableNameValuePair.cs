using System;

namespace UnityEngine.Localization.SmartFormat.PersistentVariables
{
	[Serializable]
	internal class VariableNameValuePair
	{
		public override string ToString()
		{
			string str = this.name;
			string str2 = " - ";
			IVariable variable = this.variable;
			return str + str2 + ((variable != null) ? variable.GetType().Name : null);
		}

		public string name;

		[SerializeReference]
		public IVariable variable;
	}
}
