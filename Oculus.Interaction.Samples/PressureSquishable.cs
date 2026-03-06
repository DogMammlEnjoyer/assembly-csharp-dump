using System;
using Oculus.Interaction.HandGrab;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PressureSquishable : MonoBehaviour, IHandGrabUseDelegate
	{
		protected virtual void Start()
		{
			this._initialScale = this._squishableObject.transform.localScale;
		}

		public void BeginUse()
		{
		}

		public void EndUse()
		{
			this._squishableObject.transform.localScale = this._initialScale;
		}

		public float ComputeUseStrength(float strength)
		{
			float num = Mathf.Lerp(1f, 1f - this._maxSquish, strength);
			float num2 = Mathf.Lerp(1f, 1f + this._maxStretch, strength);
			this._squishableObject.transform.localScale = new Vector3(this._initialScale.x * num2, this._initialScale.y * num, this._initialScale.z * num2);
			return strength;
		}

		[SerializeField]
		private GameObject _squishableObject;

		[SerializeField]
		[Range(0.01f, 1f)]
		private float _maxSquish = 0.25f;

		[SerializeField]
		[Range(0.01f, 1f)]
		private float _maxStretch = 0.15f;

		protected bool _started;

		private Vector3 _initialScale;
	}
}
