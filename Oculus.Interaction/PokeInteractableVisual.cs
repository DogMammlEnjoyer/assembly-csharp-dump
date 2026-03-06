using System;
using System.Collections.Generic;
using UnityEngine;

namespace Oculus.Interaction
{
	public class PokeInteractableVisual : MonoBehaviour
	{
		private Action _postProcessHandler
		{
			get
			{
				return new Action(this.UpdateComponentPosition);
			}
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._pokeInteractors = new HashSet<PokeInteractor>();
			this._maxOffsetAlongNormal = Vector3.Dot(base.transform.position - this._buttonBaseTransform.position, -1f * this._buttonBaseTransform.forward);
			Vector3 a = base.transform.position - this._maxOffsetAlongNormal * this._buttonBaseTransform.forward;
			this._planarOffset = new Vector2(Vector3.Dot(a - this._buttonBaseTransform.position, this._buttonBaseTransform.right), Vector3.Dot(a - this._buttonBaseTransform.position, this._buttonBaseTransform.up));
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				this._pokeInteractors.Clear();
				this._pokeInteractors.UnionWith(this._pokeInteractable.Interactors);
				this._pokeInteractable.WhenInteractorAdded.Action += this.HandleInteractorAdded;
				this._pokeInteractable.WhenInteractorRemoved.Action += this.HandleInteractorRemoved;
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started)
			{
				this._pokeInteractors.Clear();
				this._pokeInteractable.WhenInteractorAdded.Action -= this.HandleInteractorAdded;
				this._pokeInteractable.WhenInteractorRemoved.Action -= this.HandleInteractorRemoved;
				if (this._postProcessInteractor)
				{
					this._postProcessInteractor.WhenPostprocessed -= this._postProcessHandler;
					this._postProcessInteractor = null;
					this.UpdateComponentPosition();
				}
			}
		}

		private void HandleInteractorAdded(PokeInteractor pokeInteractor)
		{
			this._pokeInteractors.Add(pokeInteractor);
			if (this._postProcessInteractor == null)
			{
				this._postProcessInteractor = pokeInteractor;
				this._postProcessInteractor.WhenPostprocessed += this._postProcessHandler;
			}
		}

		private void HandleInteractorRemoved(PokeInteractor pokeInteractor)
		{
			this._pokeInteractors.Remove(pokeInteractor);
			if (pokeInteractor == this._postProcessInteractor)
			{
				this._postProcessInteractor.WhenPostprocessed -= this._postProcessHandler;
				using (HashSet<PokeInteractor>.Enumerator enumerator = this._pokeInteractors.GetEnumerator())
				{
					if (enumerator.MoveNext() && enumerator.Current != null)
					{
						this._postProcessInteractor = enumerator.Current;
						this._postProcessInteractor.WhenPostprocessed += this._postProcessHandler;
					}
					else
					{
						this._postProcessInteractor = null;
						this.UpdateComponentPosition();
					}
				}
			}
		}

		private void UpdateComponentPosition()
		{
			float num = this._maxOffsetAlongNormal;
			foreach (PokeInteractor pokeInteractor in this._pokeInteractors)
			{
				float num2 = Vector3.Dot(pokeInteractor.Origin - this._buttonBaseTransform.position, -1f * this._buttonBaseTransform.forward);
				num2 -= pokeInteractor.Radius;
				if (num2 < 0f)
				{
					num2 = 0f;
				}
				num = Math.Min(num2, num);
			}
			base.transform.position = this._buttonBaseTransform.position + this._buttonBaseTransform.forward * (-1f * num) + this._buttonBaseTransform.right * this._planarOffset.x + this._buttonBaseTransform.up * this._planarOffset.y;
		}

		public void InjectAllPokeInteractableVisual(PokeInteractable pokeInteractable, Transform buttonBaseTransform)
		{
			this.InjectPokeInteractable(pokeInteractable);
			this.InjectButtonBaseTransform(buttonBaseTransform);
		}

		public void InjectPokeInteractable(PokeInteractable pokeInteractable)
		{
			this._pokeInteractable = pokeInteractable;
		}

		public void InjectButtonBaseTransform(Transform buttonBaseTransform)
		{
			this._buttonBaseTransform = buttonBaseTransform;
		}

		[Tooltip("The Poke Interactable.")]
		[SerializeField]
		private PokeInteractable _pokeInteractable;

		[Tooltip("Acts as the limit of the button (the point where it's fully depressed).")]
		[SerializeField]
		private Transform _buttonBaseTransform;

		private float _maxOffsetAlongNormal;

		private Vector2 _planarOffset;

		private HashSet<PokeInteractor> _pokeInteractors;

		private PokeInteractor _postProcessInteractor;

		protected bool _started;
	}
}
