using System;
using System.Collections;
using System.Collections.Generic;
using Meta.WitAi.Composer.Interfaces;
using UnityEngine;

namespace Meta.WitAi.Composer.Handlers
{
	public class ComposerActionHandler : MonoBehaviour, IComposerActionHandler
	{
		public ComposerActionEventData[] ActionEvents
		{
			get
			{
				return this._actionEvents;
			}
		}

		protected virtual void Start()
		{
			this._highestIndex = Math.Max(0, this._actionEvents.Length - 1);
		}

		public void AddEvent(ComposerActionEventData actionEvent)
		{
			if (this._highestIndex >= this._actionEvents.Length - 1)
			{
				Array.Resize<ComposerActionEventData>(ref this._actionEvents, 1 + this._actionEvents.Length * 2);
			}
			ComposerActionEventData[] actionEvents = this._actionEvents;
			int highestIndex = this._highestIndex;
			this._highestIndex = highestIndex + 1;
			actionEvents[highestIndex] = actionEvent;
		}

		public void PerformAction(ComposerSessionData sessionData)
		{
			string actionID = sessionData.responseData.actionID;
			int actionEventIndex = this.GetActionEventIndex(actionID);
			if (actionEventIndex != -1)
			{
				ComposerActionEvent actionEvent = this._actionEvents[actionEventIndex].actionEvent;
				if (actionEvent != null)
				{
					actionEvent.Invoke(sessionData);
				}
			}
			if (this.HandleActionAsync != null)
			{
				base.StartCoroutine(this.PerformActionAsync(sessionData, this.HandleActionAsync));
			}
		}

		private IEnumerator PerformActionAsync(ComposerSessionData sessionData, Func<ComposerSessionData, IEnumerator> actionAsync)
		{
			this._actionCoroutines[sessionData] = true;
			foreach (Func<ComposerSessionData, IEnumerator> func in actionAsync.GetInvocationList())
			{
				yield return func(sessionData);
			}
			Delegate[] array = null;
			if (this._actionCoroutines.ContainsKey(sessionData))
			{
				this._actionCoroutines.Remove(sessionData);
			}
			yield break;
		}

		public bool IsPerformingAction(ComposerSessionData sessionData)
		{
			return this._actionCoroutines != null && this._actionCoroutines.ContainsKey(sessionData);
		}

		public int GetActionEventIndex(string actionID)
		{
			if (this._actionEvents != null)
			{
				return Array.FindIndex<ComposerActionEventData>(this._actionEvents, (ComposerActionEventData a) => string.Equals(a.actionID, actionID, StringComparison.CurrentCultureIgnoreCase));
			}
			return -1;
		}

		[SerializeField]
		private ComposerActionEventData[] _actionEvents;

		private int _highestIndex;

		public Func<ComposerSessionData, IEnumerator> HandleActionAsync;

		private Dictionary<ComposerSessionData, bool> _actionCoroutines = new Dictionary<ComposerSessionData, bool>();
	}
}
