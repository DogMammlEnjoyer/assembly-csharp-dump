using System;
using System.Reflection;
using UnityEngine.Events;

namespace UnityEngine.Localization.Bridge
{
	internal static class UnityEventBridge
	{
		public static UnityEventCallState GetPersistentListenerState(this UnityEventBase unityEvent, int index)
		{
			return ((PersistentCallGroup)UnityEventBridge.k_PersistenCallGroup.GetValue(unityEvent)).GetListener(index).callState;
		}

		private static readonly FieldInfo k_PersistenCallGroup = typeof(UnityEventBase).GetField("m_PersistentCalls", BindingFlags.Instance | BindingFlags.NonPublic);
	}
}
