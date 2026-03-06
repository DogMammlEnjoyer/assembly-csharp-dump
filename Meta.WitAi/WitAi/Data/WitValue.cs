using System;
using Meta.WitAi.Json;
using UnityEngine;

namespace Meta.WitAi.Data
{
	public abstract class WitValue : ScriptableObject
	{
		public WitResponseReference Reference
		{
			get
			{
				if (this.reference == null)
				{
					this.reference = WitResultUtilities.GetWitResponseReference(this.path);
				}
				return this.reference;
			}
		}

		public abstract object GetValue(WitResponseNode response);

		public abstract bool Equals(WitResponseNode response, object value);

		public string ToString(WitResponseNode response)
		{
			return this.Reference.GetStringValue(response);
		}

		[SerializeField]
		public string path;

		private WitResponseReference reference;
	}
}
