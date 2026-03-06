using System;
using UnityEngine;

namespace Meta.XR.MultiplayerBlocks.Colocation
{
	internal class AnchorDebugVisual : MonoBehaviour
	{
		private static event Action _debugVisibilityChanged;

		public static bool DebugVisualsVisible
		{
			get
			{
				return AnchorDebugVisual._debugVisualsVisible;
			}
			set
			{
				if (value == AnchorDebugVisual._debugVisualsVisible)
				{
					return;
				}
				AnchorDebugVisual._debugVisualsVisible = value;
				AnchorDebugVisual._debugVisibilityChanged();
			}
		}

		private void Awake()
		{
			AnchorDebugVisual._debugVisibilityChanged += this.OnDebugVisibilityChanged;
			this.OnDebugVisibilityChanged();
		}

		private void OnDebugVisibilityChanged()
		{
			base.gameObject.SetActive(AnchorDebugVisual._debugVisualsVisible);
		}

		private static bool _debugVisualsVisible = true;
	}
}
