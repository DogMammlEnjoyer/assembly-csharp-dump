using System;
using Oculus.Interaction.Surfaces;
using UnityEngine;

namespace Oculus.Interaction
{
	[RequireComponent(typeof(RectTransform))]
	[ExecuteInEditMode]
	public class RectTransformBoundsClipperDriver : MonoBehaviour
	{
		protected virtual void Awake()
		{
			this.Resize();
		}

		protected virtual void Start()
		{
		}

		private void OnRectTransformDimensionsChange()
		{
			this.Resize();
		}

		private void Resize()
		{
			if (this._boundsClipper == null)
			{
				return;
			}
			RectTransform rectTransform = base.transform as RectTransform;
			this._boundsClipper.Size = new Vector3(rectTransform.rect.width, rectTransform.rect.height, 0.01f);
		}

		[SerializeField]
		private BoundsClipper _boundsClipper;
	}
}
