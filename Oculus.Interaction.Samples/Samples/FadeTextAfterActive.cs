using System;
using TMPro;
using UnityEngine;

namespace Oculus.Interaction.Samples
{
	public class FadeTextAfterActive : MonoBehaviour
	{
		protected virtual void OnEnable()
		{
			this._timeLeft = this._fadeOutTime;
			this._text.fontMaterial.color = new Color(this._text.color.r, this._text.color.g, this._text.color.b, 255f);
		}

		protected virtual void Update()
		{
			if (this._timeLeft <= 0f)
			{
				return;
			}
			float t = 1f - this._timeLeft / this._fadeOutTime;
			float a = Mathf.SmoothStep(1f, 0f, t);
			this._text.color = new Color(this._text.color.r, this._text.color.g, this._text.color.b, a);
			this._timeLeft -= Time.deltaTime;
		}

		[SerializeField]
		private float _fadeOutTime;

		[SerializeField]
		private TextMeshPro _text;

		private float _timeLeft;
	}
}
