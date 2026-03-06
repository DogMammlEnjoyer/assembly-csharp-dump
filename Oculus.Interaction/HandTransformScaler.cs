using System;
using Oculus.Interaction.Input;
using UnityEngine;

namespace Oculus.Interaction
{
	public class HandTransformScaler : MonoBehaviour
	{
		public IHand Hand { get; private set; }

		protected virtual void Awake()
		{
			this.Hand = (this._hand as IHand);
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._originalScale = base.transform.localScale;
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated += this.HandleHandUpdated;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this.Hand.WhenHandUpdated -= this.HandleHandUpdated;
			}
		}

		private void HandleHandUpdated()
		{
			float d = 1f;
			if (base.transform.parent != null)
			{
				d = base.transform.parent.lossyScale.x;
			}
			base.transform.localScale = this._originalScale * this.Hand.Scale / d;
		}

		[SerializeField]
		[Interface(typeof(IHand), new Type[]
		{

		})]
		private Object _hand;

		protected bool _started;

		private Vector3 _originalScale = Vector3.one;
	}
}
