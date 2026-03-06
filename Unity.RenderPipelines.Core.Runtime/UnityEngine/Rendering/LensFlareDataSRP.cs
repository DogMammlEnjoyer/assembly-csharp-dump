using System;

namespace UnityEngine.Rendering
{
	[Serializable]
	public sealed class LensFlareDataSRP : ScriptableObject
	{
		public LensFlareDataSRP()
		{
			this.elements = null;
		}

		public bool HasAModulateByLightColorElement()
		{
			if (this.elements != null)
			{
				LensFlareDataElementSRP[] array = this.elements;
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].modulateByLightColor)
					{
						return true;
					}
				}
			}
			return false;
		}

		public LensFlareDataElementSRP[] elements;
	}
}
