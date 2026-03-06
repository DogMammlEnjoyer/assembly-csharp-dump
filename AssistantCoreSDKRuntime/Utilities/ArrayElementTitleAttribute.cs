using System;
using UnityEngine;

namespace Oculus.Voice.Core.Utilities
{
	public class ArrayElementTitleAttribute : PropertyAttribute
	{
		public ArrayElementTitleAttribute(string elementTitleVar = null, string fallbackName = null)
		{
			this.varname = elementTitleVar;
			this.fallbackName = fallbackName;
		}

		public string varname;

		public string fallbackName;
	}
}
