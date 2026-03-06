using System;
using System.Collections.Generic;
using UnityEngine;

namespace Meta.WitAi.Events
{
	public class EventRegistry
	{
		public HashSet<string> OverriddenCallbacks
		{
			get
			{
				return this._overriddenCallbacks;
			}
		}

		public void RegisterOverriddenCallback(string callback)
		{
			this._overriddenCallbacks.Add(callback);
		}

		public void RemoveOverriddenCallback(string callback)
		{
			if (this._overriddenCallbacks.Contains(callback))
			{
				this._overriddenCallbacks.Remove(callback);
			}
		}

		public bool IsCallbackOverridden(string callback)
		{
			return this.OverriddenCallbacks.Contains(callback);
		}

		[SerializeField]
		private readonly HashSet<string> _overriddenCallbacks = new HashSet<string>();
	}
}
