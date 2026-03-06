using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ListSnapPoseDelegateRoundedBoxVisual : MonoBehaviour
	{
		protected virtual void LateUpdate()
		{
			float num = Mathf.Max(this._listSnapPoseDelegate.Size, this._minSize);
			if (num != this._targetWidth)
			{
				this._targetWidth = num;
				this._curve.Start();
				this._startWidth = this._properties.Width;
			}
			this._properties.Width = Mathf.Lerp(this._startWidth, this._targetWidth, this._curve.Progress());
			this._properties.BorderColor = ((this._snapInteractable.Interactors.Count != this._snapInteractable.SelectingInteractors.Count) ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, 0.5f));
		}

		[SerializeField]
		private ListSnapPoseDelegate _listSnapPoseDelegate;

		[SerializeField]
		private RoundedBoxProperties _properties;

		[SerializeField]
		private SnapInteractable _snapInteractable;

		[SerializeField]
		private float _minSize;

		[SerializeField]
		private ProgressCurve _curve;

		private float _targetWidth;

		private float _startWidth;
	}
}
