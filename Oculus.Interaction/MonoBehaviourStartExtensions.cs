using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public static class MonoBehaviourStartExtensions
	{
		public static void BeginStart(this MonoBehaviour monoBehaviour, ref bool started, Action baseStart = null)
		{
			if (!started)
			{
				monoBehaviour.enabled = false;
				started = true;
				if (baseStart != null)
				{
					baseStart();
				}
				started = false;
				return;
			}
			if (baseStart != null)
			{
				baseStart();
			}
		}

		public static void EndStart(this MonoBehaviour monoBehaviour, ref bool started)
		{
			if (!started)
			{
				started = true;
				monoBehaviour.enabled = true;
			}
		}
	}
}
