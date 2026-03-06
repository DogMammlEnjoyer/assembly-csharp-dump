using System;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class ColorChanger : MonoBehaviour
	{
		public void NextColor()
		{
			this._lastHue = (this._lastHue + 0.3f) % 1f;
			Color color = Color.HSVToRGB(this._lastHue, 0.8f, 0.8f);
			this._targetMaterial.color = color;
		}

		public void Save()
		{
			this._savedColor = this._targetMaterial.color;
		}

		public void Revert()
		{
			this._targetMaterial.color = this._savedColor;
		}

		protected virtual void Start()
		{
			this._targetMaterial = this._target.material;
			this._savedColor = this._targetMaterial.color;
		}

		private void OnDestroy()
		{
			Object.Destroy(this._targetMaterial);
		}

		[SerializeField]
		private Renderer _target;

		private Material _targetMaterial;

		private Color _savedColor;

		private float _lastHue;
	}
}
