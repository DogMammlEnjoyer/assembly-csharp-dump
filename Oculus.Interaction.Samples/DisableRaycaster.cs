using System;
using UnityEngine;
using UnityEngine.UI;

namespace Oculus.Interaction
{
	public class DisableRaycaster : MonoBehaviour
	{
		private void Update()
		{
			this.raycaster.enabled = (this.group.alpha > this.minAlpha);
		}

		public float minAlpha;

		public GraphicRaycaster raycaster;

		public CanvasGroup group;
	}
}
