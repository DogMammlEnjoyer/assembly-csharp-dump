using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	internal static class MonoBehaviourEndOfFrameExtensions
	{
		internal static void RegisterEndOfFrameCallback(this MonoBehaviour monoBehaviour, Action callback)
		{
			if (MonoBehaviourEndOfFrameExtensions._routines.ContainsKey(monoBehaviour))
			{
				throw new ArgumentException("This MonoBehaviour is already registered for the EndOfFrameCallback");
			}
			Coroutine value = monoBehaviour.StartCoroutine(MonoBehaviourEndOfFrameExtensions.EndOfFrameCoroutine(callback));
			MonoBehaviourEndOfFrameExtensions._routines.Add(monoBehaviour, value);
		}

		internal static void UnregisterEndOfFrameCallback(this MonoBehaviour monoBehaviour)
		{
			if (!MonoBehaviourEndOfFrameExtensions._routines.ContainsKey(monoBehaviour))
			{
				throw new ArgumentException("This MonoBehaviour is not registered for the EndOfFrameCallback");
			}
			monoBehaviour.StopCoroutine(MonoBehaviourEndOfFrameExtensions._routines[monoBehaviour]);
			MonoBehaviourEndOfFrameExtensions._routines.Remove(monoBehaviour);
		}

		private static IEnumerator EndOfFrameCoroutine(Action callback)
		{
			for (;;)
			{
				yield return MonoBehaviourEndOfFrameExtensions._endOfFrame;
				callback();
			}
			yield break;
		}

		private static YieldInstruction _endOfFrame = new WaitForEndOfFrame();

		private static Dictionary<MonoBehaviour, Coroutine> _routines = new Dictionary<MonoBehaviour, Coroutine>();
	}
}
