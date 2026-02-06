using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class FlagEvents<T> where T : Enum
{
	public void InvokeAll(T test, bool isLocal = false)
	{
		int num = Convert.ToInt32(test);
		for (int i = 0; i < this.list.Length; i++)
		{
			if ((num & this.list[i].flagsAsInt) != 0 && (!this.list[i].runOnlyLocally || isLocal))
			{
				UnityEvent anyFlagTrue = this.list[i].anyFlagTrue;
				if (anyFlagTrue != null)
				{
					anyFlagTrue.Invoke();
				}
			}
		}
	}

	[SerializeField]
	private FlagEvents<T>.FlagEvent[] list;

	[Serializable]
	private class FlagEvent : ISerializationCallbackReceiver
	{
		private string FlagsLabel
		{
			get
			{
				return typeof(T).Name;
			}
		}

		public void OnBeforeSerialize()
		{
			this.flagsAsInt = Convert.ToInt32(this.flags);
		}

		public void OnAfterDeserialize()
		{
			this.flags = (T)((object)this.flagsAsInt);
		}

		public string debugName = "Any flag true";

		[Tooltip("Check this box if only the local player is supposed to run this event.")]
		public bool runOnlyLocally;

		private T flags;

		[HideInInspector]
		public int flagsAsInt;

		public UnityEvent anyFlagTrue;
	}
}
