using System;
using UnityEngine;

namespace Meta.XR.ImmersiveDebugger
{
	[Serializable]
	internal class InspectedItemBase
	{
		public bool Valid { get; protected set; }

		public bool Visible
		{
			get
			{
				return this.Valid && this.enabled;
			}
		}

		[SerializeField]
		public bool enabled;

		[SerializeField]
		protected string typeName;
	}
}
