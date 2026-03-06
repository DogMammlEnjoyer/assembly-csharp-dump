using System;
using UnityEngine;

namespace Unity.Cinemachine
{
	public sealed class CinemachineEmbeddedAssetPropertyAttribute : PropertyAttribute
	{
		public CinemachineEmbeddedAssetPropertyAttribute(bool warnIfNull = false)
		{
			this.WarnIfNull = warnIfNull;
		}

		public bool WarnIfNull;
	}
}
