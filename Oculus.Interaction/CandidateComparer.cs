using System;
using UnityEngine;

namespace Oculus.Interaction
{
	public abstract class CandidateComparer<T> : MonoBehaviour, ICandidateComparer where T : class
	{
		public int Compare(object a, object b)
		{
			T t = a as T;
			T t2 = b as T;
			if (t != null && t2 != null)
			{
				return this.Compare(t, t2);
			}
			return 0;
		}

		public abstract int Compare(T a, T b);
	}
}
