using System;
using UnityEngine;

namespace Meta.WitAi.Data.ValueReferences
{
	[Serializable]
	public class StringReference<T> : IStringReference where T : ScriptableObject, IStringReference
	{
		public string Value
		{
			get
			{
				if (!this.stringObject)
				{
					return this.stringValue;
				}
				return this.stringObject.Value;
			}
			set
			{
				this.stringObject = default(T);
				this.stringValue = value;
			}
		}

		[SerializeField]
		private string stringValue;

		[SerializeField]
		private T stringObject;
	}
}
