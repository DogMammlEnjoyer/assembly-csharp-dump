using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Oculus.Interaction.Samples
{
	public class AnimatorOverrideLayerWeigth : MonoBehaviour
	{
		public float TransitionDuration
		{
			get
			{
				return this._transitionDuration;
			}
			set
			{
				this._transitionDuration = value;
			}
		}

		public AnimationCurve TransitionCurve
		{
			get
			{
				return this._transitionCurve;
			}
			set
			{
				this._transitionCurve = value;
			}
		}

		protected virtual void Reset()
		{
			this._animator = base.GetComponent<Animator>();
			this._toggle = base.GetComponent<Toggle>();
		}

		protected virtual void Start()
		{
			this.BeginStart(ref this._started, null);
			this._layerIndex = this._animator.GetLayerIndex(this._overrideLayer);
			this.EndStart(ref this._started);
		}

		protected virtual void OnEnable()
		{
			if (this._started)
			{
				if (this._layerIsActive)
				{
					this._animator.SetLayerWeight(this._layerIndex, 1f);
				}
				if (this._toggle != null)
				{
					this._toggle.onValueChanged.AddListener(new UnityAction<bool>(this.SetOverrideLayerActive));
					this.SetOverrideLayerActive(this._toggle.isOn);
				}
			}
		}

		protected virtual void OnDisable()
		{
			if (this._started && this._toggle != null)
			{
				this._toggle.onValueChanged.RemoveListener(new UnityAction<bool>(this.SetOverrideLayerActive));
			}
		}

		public void SetOverrideLayerActive(bool active)
		{
			this._layerIsActive = active;
			if ((double)this._transitionDuration > 0.0)
			{
				base.StopAllCoroutines();
				base.StartCoroutine(this.LayerTransition(this._layerIndex, active ? 1f : 0f));
				return;
			}
			this._animator.SetLayerWeight(this._layerIndex, active ? 1f : 0f);
		}

		private IEnumerator LayerTransition(int layerIndex, float targetWeight)
		{
			float startTime = Time.time;
			float startWeight = this._animator.GetLayerWeight(layerIndex);
			for (;;)
			{
				float num = (Time.time - startTime) / this._transitionDuration;
				float t = this._transitionCurve.Evaluate(Mathf.Clamp01(num));
				float weight = Mathf.Lerp(startWeight, targetWeight, t);
				this._animator.SetLayerWeight(layerIndex, weight);
				if ((double)num >= 1.0)
				{
					break;
				}
				yield return null;
			}
			yield break;
			yield break;
		}

		public void InjectAllAnimatorOverrideLayerWeigth(Animator animator, string overrideLayer)
		{
			this.InjectAnimator(animator);
			this.InjectOverrideLayer(overrideLayer);
		}

		public void InjectAnimator(Animator animator)
		{
			this._animator = animator;
		}

		public void InjectOverrideLayer(string overrideLayer)
		{
			this._overrideLayer = overrideLayer;
		}

		public void InjectOptionalToggle(Toggle toggle)
		{
			this._toggle = toggle;
		}

		[SerializeField]
		[FormerlySerializedAs("animator")]
		private Animator _animator;

		[SerializeField]
		[FormerlySerializedAs("overrideLayer")]
		private string _overrideLayer = "Selected Layer";

		[SerializeField]
		[FormerlySerializedAs("transitionDuration")]
		public float _transitionDuration = 0.2f;

		[SerializeField]
		[FormerlySerializedAs("transitionCurve")]
		public AnimationCurve _transitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		[Space]
		[SerializeField]
		[Optional(OptionalAttribute.Flag.DontHide)]
		[Tooltip("If provided, the animation layer will be syncronized with the isOn state of the toggle")]
		public Toggle _toggle;

		private bool _layerIsActive;

		private int _layerIndex = -1;

		protected bool _started;
	}
}
