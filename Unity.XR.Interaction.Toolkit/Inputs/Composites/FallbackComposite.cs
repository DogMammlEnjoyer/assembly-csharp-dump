using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.Scripting;

namespace UnityEngine.XR.Interaction.Toolkit.Inputs.Composites
{
	[Preserve]
	public abstract class FallbackComposite<TValue> : InputBindingComposite<TValue> where TValue : struct
	{
		internal struct QuaternionCompositeComparer : IComparer<Quaternion>
		{
			public int Compare(Quaternion x, Quaternion y)
			{
				if (!(x == Quaternion.identity))
				{
					return 1;
				}
				if (y == Quaternion.identity)
				{
					return 0;
				}
				return -1;
			}
		}
	}
}
