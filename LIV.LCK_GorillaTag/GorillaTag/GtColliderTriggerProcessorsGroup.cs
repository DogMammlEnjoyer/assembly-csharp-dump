using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtColliderTriggerProcessorsGroup : MonoBehaviour
	{
		public void SetCurrentTriggerProcessor(GtColliderTriggerProcessor triggerProcessor)
		{
			this._currentTriggerProcessor = triggerProcessor;
		}

		public GtColliderTriggerProcessor GetCurrentTriggerProcessor()
		{
			return this._currentTriggerProcessor;
		}

		public void ClearAllTriggers()
		{
			this._currentTriggerProcessor = null;
		}

		private GtColliderTriggerProcessor _currentTriggerProcessor;
	}
}
