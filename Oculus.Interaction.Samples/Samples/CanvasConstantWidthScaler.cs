using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class CanvasConstantWidthScaler : MonoBehaviour
	{
		private void Start()
		{
			this._initialLocalScaleY = base.transform.localScale.y;
			this._initialWidth = this._rect.sizeDelta.x;
			this._initialHeight = this._rect.sizeDelta.y;
		}

		private void Update()
		{
			base.transform.localScale = new Vector3(base.transform.localScale.x, this._initialLocalScaleY * base.transform.parent.lossyScale.x / base.transform.parent.lossyScale.y, base.transform.localScale.z);
			this._rect.sizeDelta = new Vector2(this._initialWidth, this._initialHeight * base.transform.localScale.x / base.transform.localScale.y);
		}

		[SerializeField]
		private RectTransform _rect;

		private float _initialLocalScaleY;

		private float _initialWidth;

		private float _initialHeight;
	}
}
