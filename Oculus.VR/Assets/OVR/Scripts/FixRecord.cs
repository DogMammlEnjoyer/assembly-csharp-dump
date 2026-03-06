using System;
using UnityEngine;

namespace Assets.OVR.Scripts
{
	internal class FixRecord : Record
	{
		public FixRecord(int order, string cat, string msg, FixMethodDelegate fix, Object target, bool editRequired, string[] buttons) : base(order, cat, msg)
		{
			this.buttonNames = buttons;
			this.fixMethod = fix;
			this.targetObject = target;
			this.editModeRequired = editRequired;
			this.complete = false;
		}

		public FixMethodDelegate fixMethod;

		public Object targetObject;

		public string[] buttonNames;

		public bool editModeRequired;

		public bool complete;
	}
}
