using System;
using UnityEngine;

namespace DigitalOpus.MB.Core
{
	public class MB_BlendShape2CombinedMap : MonoBehaviour
	{
		public SerializableSourceBlendShape2Combined GetMap()
		{
			if (this.srcToCombinedMap == null)
			{
				this.srcToCombinedMap = new SerializableSourceBlendShape2Combined();
			}
			return this.srcToCombinedMap;
		}

		public SerializableSourceBlendShape2Combined srcToCombinedMap;
	}
}
