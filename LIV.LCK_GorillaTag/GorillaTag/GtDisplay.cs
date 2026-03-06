using System;
using UnityEngine;

namespace Liv.Lck.GorillaTag
{
	public class GtDisplay : MonoBehaviour
	{
		private void Awake()
		{
			this._initialMeshBodyPosition = this._meshBodyTransform.localPosition;
			this._initialMeshBodyScale = this._meshBodyTransform.localScale;
			Vector3 b = new Vector3(-0.264f, 0.108f, 0f);
			this._targetMeshBodyPosition = this._initialMeshBodyPosition + b;
			this._targetMeshBodyScale = new Vector3(this._initialMeshBodyScale.x * 1.3461539f, this._initialMeshBodyScale.y * 1.3461539f, this._initialMeshBodyScale.z);
			this._initialCanvasPosition = this._canvasTransform.localPosition;
			this._initialCanvasScale = this._canvasTransform.localScale;
			this._targetCanvasPosition = this._initialCanvasPosition + b;
			this._targetCanvasScale = new Vector3(this._initialCanvasScale.x * 1.3461539f, this._initialCanvasScale.y * 1.3461539f, this._initialCanvasScale.z);
		}

		public void Maximize()
		{
			this._meshBodyTransform.localPosition = this._targetMeshBodyPosition;
			this._meshBodyTransform.localScale = this._targetMeshBodyScale;
			this._canvasTransform.localPosition = this._targetCanvasPosition;
			this._canvasTransform.localScale = this._targetCanvasScale;
		}

		public void Minimize()
		{
			this._meshBodyTransform.localPosition = this._initialMeshBodyPosition;
			this._meshBodyTransform.localScale = this._initialMeshBodyScale;
			this._canvasTransform.localPosition = this._initialCanvasPosition;
			this._canvasTransform.localScale = this._initialCanvasScale;
		}

		[Header("Elements")]
		[SerializeField]
		private Transform _meshBodyTransform;

		[SerializeField]
		private RectTransform _canvasTransform;

		private Vector3 _initialMeshBodyPosition;

		private Vector3 _initialMeshBodyScale;

		private Vector3 _initialCanvasPosition;

		private Vector3 _initialCanvasScale;

		private Vector3 _targetMeshBodyPosition;

		private Vector3 _targetMeshBodyScale;

		private Vector3 _targetCanvasPosition;

		private Vector3 _targetCanvasScale;
	}
}
