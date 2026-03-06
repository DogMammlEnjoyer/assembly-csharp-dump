using System;
using UnityEngine.VFX.Utility;

namespace UnityEngine.VFX
{
	[Serializable]
	internal abstract class EventAttribute
	{
		public abstract bool ApplyToVFX(VFXEventAttribute eventAttribute);

		public ExposedProperty id;
	}
}
